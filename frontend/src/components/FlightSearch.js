import React, { useState } from 'react';
import axios from 'axios';
import './FlightSearch.css';

const FlightSearch = () => {
    const [origin, setOrigin] = useState('');
    const [destination, setDestination] = useState('');
    const [departDate, setDepartureDate] = useState('');
    const [returnDate, setReturnDate] = useState('');
    const [adults, setAdults] = useState(1); 
    const [currency, setCurrency] = useState('USD'); 
    const [flights, setFlights] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const handleSearch = async () => {
        const cacheKey = `${origin}-${destination}-${departDate}-${returnDate}-${adults}-${currency}`;
        const cachedData = localStorage.getItem(cacheKey);

        if (cachedData) {
            setFlights(JSON.parse(cachedData));
            setError(null);
        } else {
            setLoading(true);
            setError('');
            try {
                const response = await axios.get('http://localhost:5198/api/flights', {
                    params: {
                        origin,
                        destination,
                        departDate,
                        returnDate,
                        adults,
                        currency
                    }
                });
                if (response.data.length > 0) {
                    setFlights(response.data);
                    localStorage.setItem(cacheKey, JSON.stringify(response.data));
                } else {
                    setFlights([]);
                }
                setError(null);
            } catch (error) {
                setError("Nisu pronađeni letovi na temelju navedenih parametara.");
            } finally {
                setLoading(false);
            }
        }
    };

    return (
        <div className="flight-search">
            <h1>Pretraži letove:</h1>
            <div className="search-fields">
                <input type="text" value={origin} onChange={(e) => setOrigin(e.target.value)} placeholder="Polazište" />
                <input type="text" value={destination} onChange={(e) => setDestination(e.target.value)} placeholder="Dolazište" />
                <input type="date" value={departDate} onChange={(e) => setDepartureDate(e.target.value)} placeholder="Datum polaska" />
                <input type="date" value={returnDate} onChange={(e) => setReturnDate(e.target.value)} placeholder="Datum povratka" />
                <input type="number" value={adults} onChange={(e) => setAdults(e.target.value)} placeholder="Broj putnika" min="1" />
                <select value={currency} onChange={(e) => setCurrency(e.target.value)}>
                    <option value="USD">USD</option>
                    <option value="EUR">EUR</option>
                    <option value="HRK">HRK</option>
                </select>
                <button onClick={handleSearch}>Pretraži</button>
            </div>
            {loading && <div className="spinner"></div>}
            {error && <div className="error">{error}</div>}
            {flights.length > 0 && (
                <table>
                    <thead>
                        <tr>
                            <th>Polazni Aerodrom</th>
                            <th>Odredišni Aerodrom</th>
                            <th>Datum Polaska</th>
                            <th>Datum Povratka</th>
                            <th>Broj Presjedanja (Odlazak)</th>
                            <th>Broj Presjedanja (Povratak)</th>
                            <th>Broj Putnika</th>
                            <th>Valuta</th>
                            <th>Ukupna Cijena</th>
                        </tr>
                    </thead>
                    <tbody>
                        {flights.map((flight, index) => (
                            <tr key={index}>
                                <td>{flight.originAirport}</td>
                                <td>{flight.destinationAirport}</td>
                                <td>{flight.departureDate}</td>
                                <td>{flight.returnDate}</td>
                                <td>{flight.numStopsOutbound}</td>
                                <td>{flight.numStopsInbound}</td>
                                <td>{flight.numPassengers}</td>
                                <td>{flight.currency}</td>
                                <td>{flight.totalPrice}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};

export default FlightSearch;
