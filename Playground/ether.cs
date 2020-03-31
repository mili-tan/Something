using System;
using Nethereum.Web3;

namespace ether
{
    class Program
    {
        static void Main(string[] args)
        {
            var w3 = new Web3("https://mainnet.infura.io/v3/d05f5fc29e5543a1bce5d336c0815c6e");
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
            //var iRequest = w3.Eth.Filters.NewFilter.SendRequestAsync(new NewFilterInput()
            //{
            //    Address = new[] {"0xd3cc440b8adba63f2d25fea900fc16f434a6700c"},
            //    FromBlock = BlockParameter.CreateEarliest(),
            //    ToBlock = BlockParameter.CreateLatest()
            //}).Result;
            //Console.WriteLine(iRequest.HexValue.Length);
            Console.ReadKey();
        }
    }
}
