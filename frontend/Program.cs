﻿using System;
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
        public int time_left_on_lease;
    }

    class Program
    {
        static void Main(string[] args)
        {
            RunClient();
        }

        static void RunClient() {
            var client = new RestClient("http://localhost:5000");
            bool applicationIsRunning = true;
            do { 
                Console.Clear();
                ShowMenu();
                string input = Console.ReadLine();
                switch(input) {
                    case "1":
                        ListAllHardware(client);
                        Console.WriteLine("\nHit 'Return' to continue.");
                        Console.ReadLine();
                        break;
                    case "2":
                        try {
                            Console.Write("Would you kindly type the platform name: ");
                            string platformInput = Console.ReadLine();
                            ListPlatformFilteredHardware(client, platformInput);
                            
                        } catch (FormatException e) {
                            Console.WriteLine("The input you specified is not valid. Would you kindly try again?");
                        }
                        Console.WriteLine("\nHit 'Return' to continue.");
                        Console.ReadLine();
                        break;
                    case "3":
                        ListActiveLeases(client);
                        Console.WriteLine("\nHit 'Return' to continue.");
                        Console.ReadLine();
                        break;
                    case "4":
                        try {
                            Console.Write("\nWould you kindly type the hardware name: ");
                            string newNameInput = Console.ReadLine();
                            Console.Write("\n\n... and platform: ");
                            string newPlatformInput = Console.ReadLine();
                            Console.Write("\n\n... and finally the IP: ");
                            string newIpInput = Console.ReadLine();
                            AddHardware(client, newNameInput, newPlatformInput, newIpInput); 
                        } catch (FormatException e) {
                            Console.WriteLine("The input you specified is not valid. Would you kindly try again?");
                        }
                        Console.WriteLine("\nNew hardware was added.");
                        Console.WriteLine("\nHit 'Return' to continue.");
                        Console.ReadLine();
                        break;
                    case "5":
                        try {
                            Console.Write("\nWould you kindly enter the id for the hardware you would like to lease: ");
                            int leaseID = Int32.Parse(Console.ReadLine());
                            Console.Write("\nDuration of lease: ");
                            int duration = Int32.Parse(Console.ReadLine());
                            Lease(client, leaseID, duration);
                        } catch (FormatException e) {
                            Console.WriteLine("The input you specified is not valid. Would you kindly try again?");
                        } catch (System.OverflowException e) {
                            Console.WriteLine("Something went horribly wrong. Would you kindly try again?");
                        }
                        Console.WriteLine("\nHit 'Return' to continue.");
                        Console.ReadLine();
                        break;
                    case "6":
                        Console.Clear();
                        applicationIsRunning = false;
                        break;
                    default:
                        Console.WriteLine("Would you kindly select a valid option?");
                        Console.WriteLine("\nHit 'Return' key to continue.");
                        Console.ReadLine();
                        break;
                }
            } while(applicationIsRunning);
        }

        static void ShowMenu() {
            Console.WriteLine("\n\nWelcome to the Hardware Store!");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("1. List all hardware.");
            Console.WriteLine("2. List all hardware filtered on platform.");
            Console.WriteLine("3. Show active leases.");
            Console.WriteLine("4. Add new hardware.");
            Console.WriteLine("5. Lease hardware.");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("6. Exit application.\n\n");
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

        static void Lease(RestClient client, int passedId, int passedTimeLeftOnLease)
        {
            var request = new RestRequest("todo/api/v1.0/lease/" + passedId, Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddBody( new Hardware
            {
                leased = true,
                time_left_on_lease = passedTimeLeftOnLease
            });
            client.Execute(request);
        }
    }
}
