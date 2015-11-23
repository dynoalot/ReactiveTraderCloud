﻿using System;
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Messaging;
using Common.Logging;

namespace Adaptive.ReactiveTrader.Server.Pricing
{
    public class PriceServiceLauncher
    {
        public static async Task<IDisposable> Run(IBroker broker)
        {
            var cache = new PriceGenerator();

            var service = new PricingService(cache.GetPriceStream);
            var serviceHost = new PricingServiceHost(service, broker);
            try
            {
                Console.WriteLine("Pricing Data Service starting...");
                await serviceHost.Start();
            }
            catch (MessagingException e)
            {
                throw new Exception("Can't start service", e);
            }

            Console.WriteLine("Service Started.");

            return serviceHost;
        }
    }


    public class Program
    {
        protected static readonly ILog Log = LogManager.GetLogger<Program>();

        public static void Main(string[] args)
        {
            var uri = "ws://127.0.0.1:8080/ws";
            var realm = "com.weareadaptive.reactivetrader";

            if (args.Length > 0)
                uri = args[0];
            if (args.Length > 1)
                realm = args[1];

            try
            {
                var broker = BrokerFactory.Create(uri, realm);

                using (PriceServiceLauncher.Run(broker.Result).Result)
                {
                    Console.WriteLine("Press Any Key To Stop...");
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}