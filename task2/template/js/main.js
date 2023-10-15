
let selectCountryObject = document.getElementById("select-country-object")

let countries;

let abc = "abc";

getAllCountries();

async function getAllCountries(){
    

    var response = await fetch(
        "http://localhost:7111/get-countries",   
    )

    var response = await response.json();
    countries = response;
    for (var item of response){
        var optionObj = document.createElement("option")
        optionObj.innerHTML = item.name
        selectCountryObject.appendChild(optionObj)
    }
}

async function loadCurrentWeather(){
    var country= countries[selectCountryObject.selectedIndex]
    
    var response = await fetch(
        `http://localhost:7111/get-weather?lat=${country.lat}&lon=${country.lon}`
    )

    response = await response.json();

    var WeatherInfromationContentObj = document.getElementById("weather-information-content");

    WeatherInfromationContentObj.innerHTML = 
        `
            <p>country : ${response.country}</p>
            <p>name : ${response.name}</p>
            <p>temp : ${response.temp}</p>
            <p>description : ${response.description}</p>
        `
}
