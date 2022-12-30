using System.Text;
using ClientAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace ClientAPI;

internal class Program
{
    static readonly string _inputTextFile = @"C:\in.txt";
    static readonly string _outputTextFile = @"C:\out.txt";
    static readonly string _url = @"https://localhost:7013/api/Reverse";

    static async Task Main(string[] args)
    {
        var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHttpClient();
                services.AddScoped<FileReader>();
            }).UseConsoleLifetime();

        var host = builder.Build();

        try
        {
            var fileReaderService = host.Services.GetRequiredService<FileReader>(); // adding custom service
            var data = fileReaderService.GetData(_inputTextFile);
            var lines = new List<string>();
            var clientFactory = host.Services.GetRequiredService<IHttpClientFactory>(); // adding http service for API request

            foreach (var entry in data)
            {
                var request = new { Word = entry.Word }; // generating request data
                var jsonRequest = JsonConvert.SerializeObject(request);

                using var client = clientFactory.CreateClient(); // creating client for sending request to API
                var httpResponse = await client.PostAsync(_url,
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                var response = await httpResponse.Content.ReadAsStringAsync(); // get response
                lines.Add(response);
            }
            File.WriteAllLines(_outputTextFile, lines.ToArray());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}