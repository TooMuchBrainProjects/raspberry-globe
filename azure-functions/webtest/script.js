const apiUrl = 'https://raspberry-globe-1.azurewebsites.net/api/status';
const apiKey = 'xi6eyMI1ddu6-RliD-tl7V4aty0EsXbg1E5eYOFwBo9vAzFuL-8RSQ==';


async function fetchData() {
    try {
        const response = await fetch(
            apiUrl,
            headers: {
                'x-functions-key': apiKey,
            }
        );
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
