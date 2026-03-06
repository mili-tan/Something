using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;
using System.Threading;

namespace SniProxy
{
    public class SniProxyConfig
    {
        public Dictionary<string, string> RouteMap { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public HashSet<IPAddress> AllowedClientIPs { get; set; } = new HashSet<IPAddress>();

        public static SniProxyConfig LoadDefault()
        {
            return new SniProxyConfig
            {
                RouteMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["example.com"] = "192.168.1.100:443",
                    ["www.example.org"] = "10.0.0.5:8443",
                    ["test.local"] = "[::1]:443"
                },
                AllowedClientIPs = new HashSet<IPAddress>
                {
                    IPAddress.Loopback,
                    IPAddress.IPv6Loopback,
                    IPAddress.Parse("192.168.1.10")
                }
            };
        }
    }

    public static class SniParser
    {
        public static bool TryExtractSni(ReadOnlySpan<byte> buffer, out string sni)
        {
            sni = null;
            if (buffer.Length < 5)
                return false;
            if (buffer[0] != 0x16)
                return false;
            int tlsLength = (buffer[3] << 8) | buffer[4];
            if (buffer.Length < 5 + tlsLength)
                return false;
            int pos = 5;
            if (buffer[pos] != 0x01)
                return false;
            uint handshakeLen = (uint)((buffer[pos + 1] << 16) | (buffer[pos + 2] << 8) | buffer[pos + 3]);
            pos += 4;
            pos += 2 + 32;
            if (pos >= buffer.Length) return false;
            int sessionIdLen = buffer[pos];
            pos += 1 + sessionIdLen;
            if (pos + 1 >= buffer.Length) return false;
            int cipherLen = (buffer[pos] << 8) | buffer[pos + 1];
            pos += 2 + cipherLen;
            if (pos >= buffer.Length) return false;
            int compLen = buffer[pos];
            pos += 1 + compLen;
            if (pos + 1 >= buffer.Length) return false;
            int extLen = (buffer[pos] << 8) | buffer[pos + 1];
            pos += 2;
            int endExt = pos + extLen;
            while (pos + 4 <= endExt)
            {
                ushort extType = (ushort)((buffer[pos] << 8) | buffer[pos + 1]);
                ushort extDataLen = (ushort)((buffer[pos + 2] << 8) | buffer[pos + 3]);
                pos += 4;
                if (extType == 0x00)
                {
                    if (pos + 1 >= endExt) break;
                    int sniListLen = (buffer[pos] << 8) | buffer[pos + 1];
                    pos += 2;
                    if (pos + 3 <= endExt && buffer[pos] == 0x00)
                    {
                        int nameLen = (buffer[pos + 1] << 8) | buffer[pos + 2];
                        pos += 3;
                        if (pos + nameLen <= endExt)
                        {
                            sni = Encoding.UTF8.GetString(buffer.Slice(pos, nameLen));
                            return true;
                        }
                    }
                    break;
                }
                pos += extDataLen;
            }
            return false;
        }
    }

    public static class ConnectionRelay
    {
        public static async Task RelayAsync(Socket client, Socket server)
        {
            client.NoDelay = true;
            server.NoDelay = true;
            var task1 = CopyAsync(client, server);
            var task2 = CopyAsync(server, client);
            await Task.WhenAny(task1, task2).ConfigureAwait(false);
            try { client.Shutdown(SocketShutdown.Both); } catch { }
            try { server.Shutdown(SocketShutdown.Both); } catch { }
            client.Close();
            server.Close();
            await Task.WhenAll(task1, task2).ConfigureAwait(false);
        }

        private static async Task CopyAsync(Socket from, Socket to)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(8192);
            try
            {
                while (true)
                {
                    int bytesRead = await from.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None).ConfigureAwait(false);
                    if (bytesRead == 0)
                        break;
                    await to.SendAsync(new ArraySegment<byte>(buffer, 0, bytesRead), SocketFlags.None).ConfigureAwait(false);
                }
            }
            catch
            {
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }

    public class SniProxyHost
    {
        private readonly Socket _listenSocket;
        private readonly SniProxyConfig _config;
        private readonly int _initialReceiveTimeout = 5000;
        private readonly int _maxInitialRead = 4096;

        public SniProxyHost(int listenPort, SniProxyConfig config)
        {
            _config = config;
            _listenSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            _listenSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, listenPort));
            _listenSocket.Listen(1024);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"SNI Proxy listening on port {((IPEndPoint)_listenSocket.LocalEndPoint).Port}...");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Socket client = await _listenSocket.AcceptAsync(cancellationToken).ConfigureAwait(false);
                    _ = ProcessClientAsync(client, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Accept error: {ex.Message}");
                }
            }
        }

        private async Task ProcessClientAsync(Socket client, CancellationToken cancellationToken)
        {
            IPAddress clientIp = ((IPEndPoint)client.RemoteEndPoint).Address;
            if (clientIp.IsIPv4MappedToIPv6)
                clientIp = clientIp.MapToIPv4();
            if (_config.AllowedClientIPs.Count > 0 && !_config.AllowedClientIPs.Contains(clientIp))
            {
                Console.WriteLine($"Connection from {((IPEndPoint)client.RemoteEndPoint).Address} rejected (not in whitelist)");
                return;
            }

            client.ReceiveTimeout = _initialReceiveTimeout;
            byte[] initialBuffer = ArrayPool<byte>.Shared.Rent(_maxInitialRead);
            try
            {
                int received = 0;
                while (received < _maxInitialRead)
                {
                    try
                    {
                        int read = await client.ReceiveAsync(new ArraySegment<byte>(initialBuffer, received, _maxInitialRead - received), SocketFlags.None)
                                                .ConfigureAwait(false);
                        if (read == 0)
                        {
                            return;
                        }
                        received += read;
                        if (SniParser.TryExtractSni(initialBuffer.AsSpan(0, received), out string sni))
                        {
                            if (!_config.RouteMap.TryGetValue(sni, out string target))
                            {
                                Console.WriteLine($"No route for SNI: {sni}");
                                return;
                            }
                            Console.WriteLine($"Routing {sni} -> {target}");
                            if (!TryParseEndpoint(target, out IPEndPoint backendEp))
                            {
                                Console.WriteLine($"Invalid backend address: {target}");
                                return;
                            }
                            using Socket backend = new Socket(backendEp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                            try
                            {
                                await backend.ConnectAsync(backendEp).ConfigureAwait(false);
                                await backend.SendAsync(new ArraySegment<byte>(initialBuffer, 0, received), SocketFlags.None).ConfigureAwait(false);
                                await ConnectionRelay.RelayAsync(client, backend).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Backend connection error: {ex.Message}");
                            }
                            return;
                        }
                    }
                    catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                    {
                        Console.WriteLine("Timeout reading initial data");
                        return;
                    }
                }
                Console.WriteLine("Failed to extract SNI from initial data");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client handling error: {ex.Message}");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(initialBuffer);
                try { client.Close(); } catch { }
            }
        }

        private bool TryParseEndpoint(string address, out IPEndPoint endpoint)
        {
            endpoint = null;
            if (string.IsNullOrEmpty(address)) return false;
            UriHostNameType type = Uri.CheckHostName(address.Split(':')[0]);
            if (type == UriHostNameType.IPv4 || type == UriHostNameType.IPv6)
            {
                if (IPEndPoint.TryParse(address, out endpoint))
                    return true;
            }
            string[] parts = address.Split(':');
            if (parts.Length != 2) return false;
            string host = parts[0];
            if (!int.TryParse(parts[1], out int port)) return false;
            try
            {
                IPAddress[] ips = Dns.GetHostAddressesAsync(host).GetAwaiter().GetResult();
                if (ips.Length > 0)
                {
                    endpoint = new IPEndPoint(ips[0], port);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            int port = 443;
            if (args.Length > 0 && int.TryParse(args[0], out int customPort))
                port = customPort;
            var config = SniProxyConfig.LoadDefault();
            var host = new SniProxyHost(port, config);
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("Shutting down...");
                cts.Cancel();
                e.Cancel = true;
            };
            try
            {
                await host.StartAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex}");
            }
        }
    }
}
