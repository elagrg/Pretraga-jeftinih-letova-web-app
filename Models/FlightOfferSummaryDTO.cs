namespace FlightSearch.Models
{
    public class FlightOfferSummaryDTO
    {
        public string OriginAirport { get; set; }
        public string DestinationAirport { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int NumStopsOutbound { get; set; }
        public int NumStopsInbound { get; set; }
        public int NumPassengers { get; set; }
        public string Currency { get; set; }
        public decimal TotalPrice { get; set; }
    }
}