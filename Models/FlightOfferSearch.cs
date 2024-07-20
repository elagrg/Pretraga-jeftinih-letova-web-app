namespace FlightSearch.Models
{
    public class FlightOfferSearch
    {
        public string OriginLocationCode { get; set; }
        public string DestinationLocationCode { get; set; }
        public string DepartureDate { get; set; }
        public string ReturnDate { get; set; } 
        public int Adults { get; set; }
        public string Currency { get; set; } 
    }
}