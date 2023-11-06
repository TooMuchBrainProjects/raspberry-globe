const apiUrl = 'http://localhost:7071/api/status';

async function fetchData() {
    try {
        const response = await fetch(apiUrl);
        const data = await response.json();

        document.getElementById('ip').textContent = data.ipInfo.ip;
        document.getElementById('city').textContent = data.ipInfo.city;
        document.getElementById('region').textContent = data.ipInfo.region;
        document.getElementById('country').textContent = data.ipInfo.country;
        document.getElementById('timezone').textContent = data.ipInfo.timezone;
        document.getElementById('temperature').textContent = data.wheaterData.current.temperature_2m;
        document.getElementById('humidity').textContent = data.wheaterData.current.relative_humidity_2m;
        document.getElementById('apparent-temperature').textContent = data.wheaterData.current.apparent_temperature;
        document.getElementById('weather-description').textContent = data.wheaterData.current.weather_description;
    } catch (error) {
        console.error('Error fetching data:', error);
    }
}

fetchData();
