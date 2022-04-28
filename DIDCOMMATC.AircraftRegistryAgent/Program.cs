using System;
using System.Text;
using System.IO;
using CsvHelper;
using System.Globalization;
using Trinity;
using System.Linq;
using DIDCOMMATC.Common;
using System.Text.Json;

namespace DIDCOMMATC.AircraftRegistryAgent
{
    public class AircraftRegistryAgentImplementation : AircraftRegistryAgentBase
    {
        public override void GetAircraftEndpointHandler(GetAircraftRequest request, out GetAircraftResponse response)
        {
            string requestedId = request.aircraftId;
            var result = Global.LocalStorage.AircraftInfo_Cell_Accessor_Selector()
            .Where(node => node.aircraftinfo.aircraftId == requestedId)
            .Select(node => node.aircraftinfo);
            AircraftInfo aircraft = result.FirstOrDefault();

            response.aircraft = aircraft;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            Global.LocalStorage.LoadStorage();
            Console.WriteLine("Existing cell count: " + Global.LocalStorage.CellCount.ToString() + " aircraft");
            if (Global.LocalStorage.CellCount == 0)
            {
                string filename = "C:\\Users\\mwher\\OneDrive\\Documents\\Aircraft.csv";
                using (var reader = new StreamReader(filename))
                {
                    // https://joshclose.github.io/CsvHelper/
                    using (var csvreader = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var aircraftinfoRecords = csvreader.GetRecords<AircraftInfoCsv>();
                        foreach (var aircraftinfo in aircraftinfoRecords)
                        {
                            AircraftInfo aircraftrecord = new AircraftInfo(aircraftinfo.Airline, aircraftinfo.AircraftId, aircraftinfo.Type);
                            AircraftInfo_Cell aircraft_cell = new AircraftInfo_Cell(aircraftrecord);
                            Global.LocalStorage.SaveAircraftInfo_Cell(aircraft_cell);
                        }
                    }
                }
                Global.LocalStorage.SaveStorage();
            }

            Trinity.TrinityConfig.HttpPort = 8000;
            string agentUrl = "http://localhost:" + Trinity.TrinityConfig.HttpPort.ToString() + "/GetAircraftEndpoint/";
            AircraftRegistryAgentImplementation agent = new AircraftRegistryAgentImplementation();
            agent.Start();
            Console.WriteLine("Server started with " + Global.LocalStorage.CellCount.ToString() + " aircraft");

            GetAircraftRequest request = new GetAircraftRequest("NE11AF");
            string jsonResponse = HttpHelpers.SendHttpMessage(agentUrl, request.ToString());
            GetAircraftResponse response = JsonSerializer.Deserialize<GetAircraftResponse>(jsonResponse);
            AircraftInfo aircraft = response.aircraft;
            Console.WriteLine("Result: " + aircraft.ToString());

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();

            agent.Stop();
        }
    }
}