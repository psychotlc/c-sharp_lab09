
namespace CountryClass;

public class Country{
    public string Name {get; set;}
    public double Lat  {get; set;}
    public double Lon  {get; set;}

    public Country(string name, double lat, double lon){
        this.Name = name;
        this.Lat = lat;
        this.Lon = lon;
    }
}