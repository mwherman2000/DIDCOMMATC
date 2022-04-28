using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Trinity;

namespace DIDCOMMATC.AirportAgent
{
    public class AirportAgentImplementation : AirportAgentBase
    {
        public override void InitializeHandler(AirportInfo request, out InitializeResponse response)
        {
            Program.airportCode = request.AirportCode;
            Program.airportName = request.AirportName;

            Console.WriteLine(Program.airportCode + ": " + Program.airportName);

            response.rc = 200;
        }
    }

    public class Program
    {
        public static string airportCode = "";
        public static string airportName = "";

        static void Main(string[] args)
        {
            int httpPort = 8080;

            if (args.Length == 0)
            {
                //throw new ArgumentException();
                httpPort = 8081;
            }
            else
            {
                httpPort = int.Parse(args[0]);
            }

            Trinity.TrinityConfig.HttpPort = httpPort;
            Trinity.TrinityConfig.ServerPort = httpPort + 200;
            Trinity.TrinityConfig.ProxyPort = httpPort + 400;
            Trinity.TrinityConfig.StorageRoot = @"c:/temp/atc/" + httpPort + @"/storage/"; 
            AirportAgentImplementation airportAgent = new AirportAgentImplementation();
            airportAgent.Start();

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();

            airportAgent.Stop();
        }
    }
}