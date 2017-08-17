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
            
            bool applicationIsRunning = true;
            
            do { 
                
                Console.WriteLine("Welcome to the Hardware Store!");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("1. List all hardware.");
                Console.WriteLine("2. List all hardware filtered on platform.");
                Console.WriteLine("3. Show active leases.");
                Console.WriteLine("4. Add new hardware.");
                Console.WriteLine("5. Lease hardware.");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("6. Exit application.");

                string input = Console.ReadLine();
                
                switch(input) {
                    case "1":
                        ListAllHardware(client);
                        break;
                    case "2":
                        ListPlatformFilteredHardware(client, "PC");
                        break;
                    case "3":
                        ListActiveLeases(client);
                        break;
                    case "4":
                        AddHardware(client, "Mechanical Keyboard", "PC", "Cherry"); 
                        break;
                    case "5":
                        Lease(client, 5);
                        break;
                    case "6":
                        applicationIsRunning = false;
                        break;
                    default:
                        Console.WriteLine("Would you kindly select a valid option?");
                        break;
                }
            } while(applicationIsRunning);
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
