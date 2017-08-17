using System;
using RestSharp;

namespace frontend
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient("http://localhost:5000");
            var request = new RestRequest("todo/api/v1.0/hardware_list", Method.GET);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            Console.WriteLine(content);
        }
    }
}
