using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using FlightSearch.Models;
using FlightSearch.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace FlightSearch.Controllers
{
    [ApiController]
    [Route("api/flights")]
    public class FlightsController : ControllerBase
    {
        private readonly AmadeusService _amadeusService;

        public FlightsController(AmadeusService amadeusService)
        {
            _amadeusService = amadeusService;
        }


        [HttpGet]
        public async Task<ActionResult<List<FlightOfferSummaryDTO>>> GetFlights(
            [FromQuery(Name = "origin")] string originLocationCode,
            [FromQuery(Name = "destination")] string destinationLocationCode,
            [FromQuery(Name = "departDate")] string departureDate,
            [FromQuery(Name = "returnDate")] string returnDate,
            [FromQuery(Name = "adults")] int adults,
            [FromQuery(Name = "currency")] string currencyCode)
        {
            try
            {
                var request = new FlightOfferSearch
                {
                    OriginLocationCode = originLocationCode,
                    DestinationLocationCode = destinationLocationCode,
                    DepartureDate = departureDate,
                    ReturnDate = returnDate,
                    Adults = adults,
                    Currency = currencyCode
                };                       
        
                var flights = await _amadeusService.GetFlightOffersAsync(request);

                if (flights.Data == null || !flights.Data.Any())
                {
                    return NotFound("Nisu pronađeni letovi na temelju navedenih parametara.");
                }

                Console.WriteLine($"Broj dobivenih letova: {flights.Data.Count}");

                var flightCount = Math.Min(50, flights.Data.Count);
                var filteredFlights = flights.Data.Take(flightCount).ToList();

                var flightSummaries = filteredFlights.Select(flight =>
                {
                    var firstItinerary = flight.Itineraries.First();
                    var lastItinerary = flight.Itineraries.Last();

                    var firstSegmentOutbound = firstItinerary.Segments.First();
                    var lastSegmentOutbound = firstItinerary.Segments.Last();

                    var firstSegmentInbound = lastItinerary.Segments.First();
                    var lastSegmentInbound = lastItinerary.Segments.Last();

                    decimal totalPrice;
                    if (!decimal.TryParse(flight.Price.Total, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out totalPrice))
                    {
                        totalPrice = 0; 
                    }

                    return new FlightOfferSummaryDTO
                    {
                        OriginAirport = firstSegmentOutbound.Departure.IataCode,
                        DestinationAirport = lastSegmentOutbound.Arrival.IataCode,
                        DepartureDate = flight.Itineraries.First().Segments.First().Departure.At,
                        ReturnDate = flight.Itineraries.Last().Segments.Last().Arrival.At,
                        NumStopsOutbound = firstItinerary.Segments.Count - 1,
                        NumStopsInbound = lastItinerary.Segments.Count - 1,
                        NumPassengers = request.Adults,
                        Currency = flight.Price.Currency,
                        TotalPrice = totalPrice      
                    };
                }).ToList();

                return Ok(flightSummaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching flight offers.");
            }
        }
    }
}
