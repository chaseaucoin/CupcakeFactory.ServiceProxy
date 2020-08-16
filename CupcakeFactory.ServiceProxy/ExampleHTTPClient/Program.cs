using ExampleContracts;
using System;
using System.Net.Http;
using CupcakeFactory.ServiceProxy;

namespace ExampleHTTPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:5024");            

            ISubtractionService subtractionService = httpClient.GetProxy<ISubtractionService>();
            IAdditionService additionService = httpClient.GetProxy<IAdditionService>();

            var addResult = additionService.Add(2, 3);
            var subtractResult = subtractionService.Subtract(2, 3);

            Console.WriteLine($"2 + 3 = {addResult}");
            Console.WriteLine($"2 - 3 = {subtractResult}");
        }
    }
}
