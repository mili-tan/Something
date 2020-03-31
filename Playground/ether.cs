using System;
using System.Numerics;
using System.Threading;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace ether
{
    class Program
    {
        static void Main(string[] args)
        {
            var w3 = new Web3("https://cloudflare-eth.com");
            var balance = w3.Eth.GetBalance.SendRequestAsync("0xd3cc440b8adba63f2d25fea900fc16f434a6700c").Result;
            Console.WriteLine($"Balance in Wei: {balance.Value}");
            var etherAmount = Web3.Convert.FromWei(balance.Value);
            Console.WriteLine($"Balance in Ether: {etherAmount}");
            var transaction = w3.Eth.Transactions.GetTransactionByHash.SendRequestAsync("0x1a9292a1d3c96f741d2729d7b6032f28bf65d462b7fe9a2ee6c5ab5da4640675").Result;
            Console.WriteLine("BlockNumber " + transaction.BlockNumber.Value);
            Console.WriteLine("BlockHash " + transaction.BlockHash);
            Console.WriteLine("From " + transaction.From);
            Console.WriteLine("To " + transaction.To);
            Console.WriteLine("Val " + Web3.Convert.FromWei(transaction.Value.Value));
            Console.WriteLine("InputText " + transaction.Input);

            BigInteger integerBlockNumber = BigInteger.Zero; 
            while (true)
            {
                if (w3.Eth.Blocks.GetBlockNumber.SendRequestAsync().Result.Value != integerBlockNumber)
                {
                    var blockWithTransactions = w3.Eth.Blocks.GetBlockWithTransactionsByNumber
                        .SendRequestAsync(BlockParameter.CreateLatest()).Result;
                    integerBlockNumber = blockWithTransactions.Number.Value;
                    foreach (var itemTransaction in blockWithTransactions.Transactions)
                    {
                        Console.WriteLine(blockWithTransactions.Number + " - " + itemTransaction.TransactionIndex +
                                          " / " + blockWithTransactions.Transactions.Length);
                        Console.WriteLine("-----------------------------");
                        Console.WriteLine("TransactionHash " + itemTransaction.TransactionHash);
                        Console.WriteLine("From " + itemTransaction.From);
                        Console.WriteLine("To " + itemTransaction.To);
                        Console.WriteLine("Val " + Web3.Convert.FromWei(itemTransaction.Value.Value));
                        //Console.WriteLine("InputText " + itemTransaction.Input);
                        Console.WriteLine("-----------------------------");
                    }

                    Thread.Sleep(100);
                }
            }

            //var iRequest = w3.Eth.Filters.NewBlockFilter.SendRequestAsync(new NewFilterInput()
            //{
            //    Address = new[] { "0xd3cc440b8adba63f2d25fea900fc16f434a6700c" },
            //    FromBlock = BlockParameter.CreateEarliest(),
            //    ToBlock = BlockParameter.CreateLatest()
            //}).Result;
            //Console.WriteLine(iRequest.HexValue.Length);
            Console.ReadKey();
        }
    }
}
