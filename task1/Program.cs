using Microsoft.VisualBasic.FileIO;
class Program
{
    object locker = new();
    public async Task<string> Get(string url)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new HttpRequestException($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException($"Request error: {e.Message}");
            }
        }
    }

    public async Task ParseCSV(string CSVString, string ticket)
    {
        int days = 0;
        double value = 0;
        using (StringReader stringReader = new StringReader(CSVString)){

            using (TextFieldParser textFieldParser = new TextFieldParser(stringReader))
            {
                textFieldParser.TextFieldType = FieldType.Delimited;
                textFieldParser.SetDelimiters(",");

                textFieldParser.ReadFields();

                while (!textFieldParser.EndOfData)
                {
                    string[] rows = textFieldParser.ReadFields();
                    double highValue = 0;
                    double lowValue = 0;

                    try
                    {
                        highValue = Convert.ToDouble(rows[2]);
                        lowValue = Convert.ToDouble(rows[3]);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"Wrong data in file for ticket={ticket}");
                        continue;
                    }

                    value += (highValue + lowValue)/2;
                    days++;
                }
            }

        }
        lock( locker ){
            
            using (StreamWriter output = File.AppendText("result.txt"))
            {
                output.WriteLine($"{ticket}:{value / days}");
            };

        }
    }

    static async Task Main()
    {
        Program program = new Program();

        DateTime currentTimestamp = DateTime.Now;
        DateTime oneYearAgo = currentTimestamp.AddYears(-1);

        long currentUnixTimestamp = ((DateTimeOffset)currentTimestamp).ToUnixTimeSeconds();
        long oneYearAgoUnixTimestamp = ((DateTimeOffset)oneYearAgo).ToUnixTimeSeconds();

        foreach(string ticket in File.ReadLines("ticket.txt"))
        {
            string url =    $"https://query1.finance.yahoo.com/v7/finance/download/"    +
                            $"{ticket}?period1={oneYearAgoUnixTimestamp}&period2={currentUnixTimestamp}" +
                            "&interval=1d&events=history&includeAdjustedClose=true";
            
            string response;
            try{
                response = await program.Get(url);
            }
            catch(Exception e) {
                Console.WriteLine($"url with ticket={ticket} not found. Error 404");
                continue;
            }

            program.ParseCSV(response, ticket);

        };

    }

}