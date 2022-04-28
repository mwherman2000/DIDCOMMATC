using System;
public class FlightPlanCsv
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
    public string Ident { get; set; }
    public string Type { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public string Departure { get; set; }
    public string EstimatedArrival { get; set; }
}