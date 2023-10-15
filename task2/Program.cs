using Models.WeatherClasses;
using Models.CountryClass;

using Utils.Requests;

using Newtonsoft.Json;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                // You can configure CORS policies here.
                // By default, this allows all origins, methods, and headers.
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();


app.MapGet("/get-weather", async (HttpContext context) =>
{
    
    string lat = context.Request.Query["lat"];
    string lon = context.Request.Query["lon"];
    
    const string API_KEY = "9b7551f0eb91104dcd252bd651ec2d08";

    string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={API_KEY}";

    string content = await HttpGet.Get(url);

    WeatherResponse response = JsonConvert.DeserializeObject<WeatherResponse>(content);

    Weather result = new Weather(response);

    return result;

})
.WithName("GetWeather");

app.MapGet("get-countries", () =>
{
    List<Country> result = new List<Country>();

    if (File.Exists("city.txt"))
    {
        

        using (StreamReader reader = new StreamReader("city.txt"))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] data = line.Split(',' , '\t');
                result.Add(new Country(data[0], Convert.ToDouble(data[1]), Convert.ToDouble(data[2])));
            }
        } 
        
    }
    
    return result;
});




app.Run();

