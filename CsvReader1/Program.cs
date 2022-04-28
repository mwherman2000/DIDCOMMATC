using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Globalization;
using System.Text.Json;

namespace CsvReader1 // Note: actual namespace depends on the project name.
{
    // Ident,Type,Origin,Destination,Departure,Estimated Arrival
    public class BaseFlightInfo
    {
        public string Ident { get; set; }
        public string Type { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Departure { get; set; }
        public string EstimatedArrival { get; set; }
    }
    public class AirportATC : BaseFlightInfo
    {
        public string OriginId { get; set; }    
        public string OriginName { get; set; }
        public string DestinationId { get; set; }
        public string DestinationName { get; set; }
        public string AircraftId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime EstimatedArrivalTime { get; set; }
        public DateTime StartBoarding { get; set; }
        public DateTime BoardingComplete { get; set; }
        public DateTime StartTaxiOut { get; set; }
        public DateTime TaxiOutComplete { get; set; }
        public DateTime StartTakeOff { get; set; }
        public DateTime TakeoffComplete { get; set; }
        public DateTime StartLanding { get; set; }
        public DateTime LandingComplete { get; set; }
        public DateTime StartTaxiIn { get; set; }
        public DateTime TaxiInComplete { get; set; }
        public DateTime StartDisembark { get; set; }
        public DateTime DisembarkComplete { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            List<AirportATC> flightplans = new List<AirportATC>();

            string filename = "C:\\Users\\mwher\\OneDrive\\Documents\\UAL4.csv";
            using (var reader = new StreamReader(filename))
            {
                // https://joshclose.github.io/CsvHelper/
                using (var csvreader = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var baseinfoRecords = csvreader.GetRecords<BaseFlightInfo>();
                    foreach (var baseinfo in baseinfoRecords)
                    {
                        int pos = baseinfo.Departure.IndexOf("M ") + 2;
                        string tz = baseinfo.Departure.Substring(pos);
                        string newtz = TimeZoneMap[tz];
                        baseinfo.Departure = baseinfo.Departure.Replace(tz, newtz);
                        pos = baseinfo.EstimatedArrival.IndexOf("M ") + 2;
                        tz = baseinfo.EstimatedArrival.Substring(pos);
                        newtz = TimeZoneMap[tz];
                        baseinfo.EstimatedArrival = baseinfo.EstimatedArrival.Replace(tz, newtz);
                        string jsonBaseInfo = JsonSerializer.Serialize(baseinfo);
                        Console.WriteLine(jsonBaseInfo);

                        var flightplan = JsonSerializer.Deserialize<AirportATC>(jsonBaseInfo);
                        pos = baseinfo.Origin.IndexOf(" (");
                        flightplan.OriginName = baseinfo.Origin.Substring(0, pos);
                        flightplan.OriginId = baseinfo.Origin.Substring(pos + 2).Replace(")", "");
                        pos = flightplan.OriginId.IndexOf(" /");
                        if (pos != -1) flightplan.OriginId = flightplan.OriginId.Substring(0, pos); 
                        pos = baseinfo.Destination.IndexOf(" (");
                        flightplan.DestinationName = baseinfo.Destination.Substring(0, pos);
                        flightplan.DestinationId = baseinfo.Destination.Substring(pos + 2).Replace(")", "");
                        pos = flightplan.DestinationId.IndexOf(" /");
                        if (pos != -1) flightplan.DestinationId = flightplan.DestinationId.Substring(0, pos);
                        flightplan.AircraftId = "N" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
                        flightplan.DepartureTime = DateTime.Parse(baseinfo.Departure);
                        flightplan.EstimatedArrivalTime = DateTime.Parse(baseinfo.EstimatedArrival);
                        flightplan.StartBoarding = flightplan.DepartureTime.AddMinutes(-30);
                        flightplan.BoardingComplete = flightplan.DepartureTime.AddMinutes(-2);
                        flightplan.StartTaxiOut = flightplan.DepartureTime.AddMinutes(1);
                        flightplan.TaxiOutComplete = flightplan.DepartureTime.AddMinutes(6);
                        flightplan.StartTakeOff = flightplan.DepartureTime.AddMinutes(7);
                        flightplan.TakeoffComplete = flightplan.DepartureTime.AddMinutes(10);
                        flightplan.StartLanding = flightplan.EstimatedArrivalTime.AddMinutes(-30);
                        flightplan.LandingComplete = flightplan.EstimatedArrivalTime;
                        flightplan.StartTaxiIn = flightplan.EstimatedArrivalTime.AddMinutes(1);
                        flightplan.TaxiInComplete = flightplan.EstimatedArrivalTime.AddMinutes(6);
                        flightplan.StartDisembark = flightplan.EstimatedArrivalTime.AddMinutes(7);
                        flightplan.DisembarkComplete = flightplan.EstimatedArrivalTime.AddMinutes(17);
                        string jsonAirportATC = JsonSerializer.Serialize(flightplan);
                        Console.WriteLine(jsonAirportATC);

                        flightplans.Add(flightplan);
                    }
                }

                using (var writer = new StreamWriter("C:\\Users\\mwher\\OneDrive\\Documents\\UAL4-out.csv"))
                using (var csvwriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csvwriter.WriteRecords(flightplans);
                }
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        // https://stackoverflow.com/questions/241789/parse-datetime-with-time-zone-of-form-pst-cest-utc-etc
        public static Dictionary<string, string> TimeZoneMap = new Dictionary<string, string>() {
            {"ACDT", "+1030"},
            {"ACST", "+0930"},
            {"ADT", "-0300"},
            {"AEDT", "+1100"},
            {"AEST", "+1000"},
            {"AHDT", "-0900"},
            {"AKDT", "-0900"},
            {"AHST", "-1000"},
            {"AST", "-0400"},
            {"AT", "-0200"},
            {"AWDT", "+0900"},
            {"AWST", "+0800"},
            {"BAT", "+0300"},
            {"BDST", "+0200"},
            {"BET", "-1100"},
            {"BST", "-0300"},
            {"BT", "+0300"},
            {"BZT2", "-0300"},
            {"CADT", "+1030"},
            {"CAST", "+0930"},
            {"CAT", "-1000"},
            {"CCT", "+0800"},
            {"CDT", "-0500"},
            {"CED", "+0200"},
            {"CET", "+0100"},
            {"CEST", "+0200"},
            {"CST", "-0600"},
            {"ChST", "+1000"}, // added
            {"EAST", "+1000"},
            {"EDT", "-0400"},
            {"EED", "+0300"},
            {"EET", "+0200"},
            {"EEST", "+0300"},
            {"EST", "-0500"},
            {"FST", "+0200"},
            {"FWT", "+0100"},
            {"GMT", "GMT"},
            {"GST", "+1000"},
            {"HDT", "-0900"},
            {"HST", "-1000"},
            {"IDLE", "+1200"},
            {"IDLW", "-1200"},
            {"IDT", "+0300"}, // added
            {"IST", "+0530"},
            {"IT", "+0330"},
            {"JST", "+0900"},
            {"JT", "+0700"},
            {"KST", "+0900"}, // added
            {"MDT", "-0600"},
            {"MED", "+0200"},
            {"MET", "+0100"},
            {"MEST", "+0200"},
            {"MEWT", "+0100"},
            {"MST", "-0700"},
            {"MT", "+0800"},
            {"NDT", "-0230"},
            {"NFT", "-0330"},
            {"NT", "-1100"},
            {"NST", "+0630"},
            {"NZ", "+1100"},
            {"NZST", "+1200"},
            {"NZDT", "+1300"},
            {"NZT", "+1200"},
            {"PDT", "-0700"},
            {"PET", "-0500"}, // added
            {"PST", "-0800"},
            {"ROK", "+0900"},
            {"SAD", "+1000"},
            {"SAST", "+0900"},
            {"SAT", "+0900"},
            {"SDT", "+1000"},
            {"SST", "+0200"},
            {"SWT", "+0100"},
            {"USZ3", "+0400"},
            {"USZ4", "+0500"},
            {"USZ5", "+0600"},
            {"USZ6", "+0700"},
            {"UT", "-0000"},
            {"UTC", "-0000"},
            {"UZ10", "+1100"},
            {"WAT", "-0100"},
            {"WET", "-0000"},
            {"WST", "+0800"},
            {"YDT", "-0800"},
            {"YST", "-0900"},
            {"ZP4", "+0400"},
            {"ZP5", "+0500"},
            {"ZP6", "+0600"}
        };
    }
}