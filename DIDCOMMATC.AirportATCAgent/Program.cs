using CsvHelper;
using DIDCOMMATC.Common;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Trinity;
using System.Threading;

namespace DIDCOMMATC.AirportATCAgent
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string airportAgentExe = 
                @"C:\TDW\repos\DIDCOMMATC\DIDCOMMATC.AirportAgent\bin\Debug\net6.0\DIDCOMMATC.AirportAgent.exe";
            //Global.LocalStorage.LoadStorage();
            Global.LocalStorage.ResetStorage();
            Console.WriteLine("Existing cell count: " + Global.LocalStorage.CellCount.ToString() + " airports");
            if (Global.LocalStorage.CellCount == 0)
            {
                string filename = "C:\\Users\\mwher\\OneDrive\\Documents\\Airports.csv";
                using (var reader = new StreamReader(filename))
                {
                    // https://joshclose.github.io/CsvHelper/
                    using (var csvreader = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var AirportInfoRecords = csvreader.GetRecords<AirportInfoCsv>();
                        foreach (var airportinfo in AirportInfoRecords)
                        {
                            AirportInfo airportInfo = new AirportInfo(
                                airportinfo.AirportCode,
                                airportinfo.AirportName,
                                airportinfo.HttpPort
                                );
                            AirportInfo_Cell AirportInfo_cell = new AirportInfo_Cell(airportInfo);
                            Global.LocalStorage.SaveAirportInfo_Cell(AirportInfo_cell);
                        }
                    }
                }
                Console.WriteLine("New cells: " + Global.LocalStorage.CellCount.ToString() + " airports");
                Global.LocalStorage.SaveStorage();
            }

            string requestedId = "AMS";
            var result = Global.LocalStorage.AirportInfo_Cell_Accessor_Selector()
            .Where(node => node.airportInfo.AirportCode == requestedId)
            .Select(node => node.airportInfo);
            AirportInfo airport = result.FirstOrDefault();
            Console.WriteLine(airport.ToString());

            List<Process> processes = new List<Process>();
            var airports = Global.LocalStorage.AirportInfo_Cell_Accessor_Selector().Select(node => node.airportInfo);
            foreach (AirportInfo a in airports)
            {
                Console.WriteLine(a.ToString());
                var p = Process.Start(airportAgentExe, a.HttpPort.ToString());
                processes.Add(p);
                Thread.Sleep(1000);
            }

            int count = 0;
            foreach (AirportInfo a in airports)
            {
                count++;
                Console.WriteLine(count.ToString() + ": " + a.ToString());
                string airportAgentUrl = "http://localhost:" + a.HttpPort + "/Initialize/";
                HttpHelpers.SendHttpMessage(airportAgentUrl, a.ToString());
                Thread.Sleep(1000);
            }

            foreach (Process p in processes)
            {
                p.Kill();
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}