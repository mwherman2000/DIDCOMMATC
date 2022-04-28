using System;
using System.Text;
using System.IO;
using CsvHelper;
using System.Globalization;
using Trinity;
using System.Linq;
using DIDCOMMATC.Common;
using System.Text.Json;

namespace DIDCOMMATC.FlightPlanAgent
{
    public class FlightPlanAgentImplementation : FlightPlanAgentBase
    {
        public override void GetFlightPlanEndpointHandler(GetFlightPlanRequest request, out GetFlightPlanResponse response)
        {
            string requestedId = request.airportInfoId;
            var result = Global.LocalStorage.FlightPlan_Cell_Accessor_Selector()
            .Where(node => node.airportInfo.Ident == requestedId)
            .Select(node => node.airportInfo);
            FlightPlan airportInfo = result.FirstOrDefault();

            response.airportInfo = airportInfo;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            //Global.LocalStorage.ResetStorage();
            Global.LocalStorage.LoadStorage();
            Console.WriteLine("Existing cell count: " + Global.LocalStorage.CellCount.ToString() + " flightplans");
            if (Global.LocalStorage.CellCount == 0)
            {
                string filename = "C:\\Users\\mwher\\OneDrive\\Documents\\FlightPlan.csv";
                using (var reader = new StreamReader(filename))
                {
                    // https://joshclose.github.io/CsvHelper/
                    using (var csvreader = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var airportInfoRecords = csvreader.GetRecords<FlightPlanCsv>();
                        foreach (var flightplan in airportInfoRecords)
                        {
                            FlightPlan flightplanrecord = new FlightPlan(
                                flightplan.Ident,
                                flightplan.Type,
                                flightplan.Origin, flightplan.Destination, flightplan.Departure, flightplan.EstimatedArrival,
                                flightplan.OriginId, flightplan.OriginName, flightplan.DestinationId,   flightplan.DestinationName,
                                flightplan.AircraftId, flightplan.DepartureTime, flightplan.EstimatedArrivalTime,
                                flightplan.StartBoarding, flightplan.BoardingComplete,
                                flightplan.StartTaxiOut, flightplan.TaxiOutComplete,
                                flightplan.StartTakeOff, flightplan.TakeoffComplete,
                                flightplan.StartLanding, flightplan.LandingComplete,
                                flightplan.StartTaxiIn, flightplan.TaxiInComplete,
                                flightplan.StartDisembark, flightplan.DisembarkComplete
                                );
                            FlightPlan_Cell airportInfo_cell = new FlightPlan_Cell(flightplanrecord);
                            Global.LocalStorage.SaveFlightPlan_Cell(airportInfo_cell);
                        }
                    }
                }
                Global.LocalStorage.SaveStorage();
            }

            Trinity.TrinityConfig.HttpPort = 8001;
            string agentUrl = "http://localhost:" + Trinity.TrinityConfig.HttpPort.ToString() + "/GetFlightPlanEndpoint/";
            FlightPlanAgentImplementation agent = new FlightPlanAgentImplementation();
            agent.Start();
            Console.WriteLine("Server started with " + Global.LocalStorage.CellCount.ToString() + " flightplans");

            GetFlightPlanRequest request = new GetFlightPlanRequest("UAL529");
            string jsonResponse = HttpHelpers.SendHttpMessage(agentUrl, request.ToString());
            GetFlightPlanResponse response = JsonSerializer.Deserialize<GetFlightPlanResponse>(jsonResponse);
            FlightPlan flightplanresult = response.airportInfo;
            Console.WriteLine("Result: " + flightplanresult.ToString());

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();

            agent.Stop();
        }
    }
}