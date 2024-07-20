import React from 'react';
import FlightSearch from './components/FlightSearch';
import './App.css'; 

function App() {
    return (
        <div className="App">
            <header className="App-header">
                <h1 style={{ fontSize: '3rem' }}>Pretraživač jeftinih letova</h1>
                <FlightSearch />
            </header>
        </div>
    );
}

export default App;
