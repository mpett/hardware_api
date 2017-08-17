using System;
using RestSharp;

namespace frontend
{
    class Hardware
    {
        public int id;
        public string name;
        public string platform;
        public string ip;
        public bool leased;
    }
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient("http://localhost:5000");
            ListAllHardware(client);
            ListPlatformFilteredHardware(client, "PC");
            ListActiveLeases(client);
            AddHardware(client, "Mechanical Keyboard", "PC", "Cherry"); 
        }

        static void AddHardware(RestClient client, string passedName, string passedPlatform, string passedIp)
        {
            var request = new RestRequest("todo/api/v1.0/hardware_list", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody( new Hardware
            {
                name = passedName,
                platform = passedPlatform,
                ip = passedIp
            });
            client.Execute(request);
        }

        static void Lease(RestClient client, int passedId)
        {
            var request = new RestRequest("todo/api/v1.0/hardware_list/" + passedId, Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody( new Hardware
            {
                leased = true
            });
            client.Execute(request);
        }

        static void ListAllHardware(RestClient client) {
            var request = new RestRequest("todo/api/v1.0/hardware_list", Method.GET);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            Console.WriteLine(content);
        }

        static void ListPlatformFilteredHardware(RestClient client, string platform) {
            var request = new RestRequest("todo/api/v1.0/hardware_list/" + platform, Method.GET);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            Console.WriteLine(content);
        }

        static void ListActiveLeases(RestClient client) {
            var request = new RestRequest("todo/api/v1.0/active_leases", Method.GET);
            IRestResponse response = client.Execute(request);
            var content = response.Content;
            Console.WriteLine(content);
        }
    }
}
