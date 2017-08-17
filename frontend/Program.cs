﻿using System;
using RestSharp;

namespace frontend
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient("http://localhost:5000");
            ListAllHardware(client);
            ListPlatformFilteredHardware(client, "PC");
        }

        static void ListAllHardware(RestClient client) {
            var request = new RestRequest("todo/api/v1.0/hardware_list", Method.GET);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            Console.WriteLine(content);
        }

        static void ListPlatformFilteredHardware(RestClient client, string platform) {
            Console.WriteLine(platform);
            var request = new RestRequest("todo/api/v1.0/hardware_list", Method.GET);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            Console.WriteLine(content);
        }
    }
}
