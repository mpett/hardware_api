using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static System.Console;
using RestSharp;
using Newtonsoft.Json;

public class Hardware
{
    public int id {get;set;}
    public string name {get;set;}
    public string platform {get;set;}
    public string ip {get;set;}
    public bool leased {get;set;}
    public int time_left_on_lease {get;set;}
}

public static class Frontend
{
    public static void Main(string[] args) 
    {
        string ip = "localhost";
        if (args.Length != 0)
        {
            if (args[0].Length != 0)
                ip = args[0];
        }

        try
        {
            RunClient(ip);
        } catch (Exception e)
        {
            WriteLine("Something went wrong. Make sure that there is connection with the server.");
        }
    }

   private static void RunClient(string ip) 
   {
        var client = new RestClient("http://" + ip + ":2204/");
        
        bool applicationIsRunning = true;
        do 
        { 
            Clear();
            WriteLine("Running client with http://" + ip + ":2204/");
            ShowMenu();
            string input = ReadLine();
            switch(input) 
            {
                case "1":
                    ListAllHardware(client);
                    WriteLine("\nHit 'Return' to continue.");
                    ReadLine();
                    break;
                case "2":
                    WriteLine("\nWould you kindly choose a platform: ");
                    WriteLine("1. PC");
                    WriteLine("2. PS4");
                    WriteLine("3. XBOX");
                    String platformAlternative = ReadLine();
                    String platformInput = "PC";
                    switch (platformAlternative)
                    {
                        case "1":
                            break;
                        case "2":
                            platformInput = "PS4";
                            break;
                        case "3":
                            platformInput = "XBOX";
                            break;
                        default:
                            WriteLine("You have not chosen an alternative from the list. Choosing default platform: PC");
                            break;
                    }
                    ListPlatformFilteredHardware(client, platformInput);
                    WriteLine("\nHit 'Return' to continue.");
                    ReadLine();
                    break;
                case "3":
                    ListActiveLeases(client);
                    WriteLine("\nHit 'Return' to continue.");
                    ReadLine();
                    break;
                case "4":
                    WriteLine("\nWould you kindly choose a platform: ");
                    WriteLine("1. PC");
                    WriteLine("2. PS4");
                    WriteLine("3. XBOX");
                    platformAlternative = ReadLine();
                    String newPlatformInput = "PC";
                    switch (platformAlternative)
                    {
                        case "1":
                            break;
                        case "2":
                            newPlatformInput = "PS4";
                            break;
                        case "3":
                            newPlatformInput = "XBOX";
                            break;
                        default:
                            WriteLine("You have not chosen an alternative from the list. Choosing default platform: PC");
                            break;
                    }
                    try 
                    {
                        Write("\nWould you kindly type the hardware name: ");
                        string newNameInput = ReadLine();
                        Write("\n\n... and finally the IP: ");
                        string newIpInput = ReadLine();
                        AddHardware(client, newNameInput, newPlatformInput, newIpInput); 
                    } catch (FormatException e) {
                        WriteLine("The input you specified is not valid. Would you kindly try again?");
                    }
                    WriteLine("\nNew hardware was added.");
                    WriteLine("\nHit 'Return' to continue.");
                    ReadLine();
                    break;
                case "5":
                    try 
                    {
                        Write("\nWould you kindly enter the name for the hardware you would like to lease: ");
                        string inputName = ReadLine();
                        Write("\nWould you kindly enter the platform for the hardware you would like to lease: ");
                        string inputPlatform = ReadLine();
                        Write("\nDuration of lease in minutes: ");
                        int duration = Int32.Parse(ReadLine());
                        Lease(client, duration, inputName, inputPlatform);
                    } catch (FormatException e) 
                    {
                        WriteLine("The input you specified is not valid. Would you kindly try again?");
                    } catch (System.OverflowException e)
                    {
                        WriteLine("Something went horribly wrong. Would you kindly try again?");
                    }
                    WriteLine("\nHit 'Return' to continue.");
                    ReadLine();
                    break;
                case "6":
                    Clear();
                    applicationIsRunning = false;
                    break;
                default:
                    WriteLine("Would you kindly select a valid option?");
                    WriteLine("\nHit 'Return' key to continue.");
                    ReadLine();
                    break;
            }
        } while(applicationIsRunning);
    }

    private static void ShowMenu() 
    {
        WriteLine("\n\nWelcome to the Hardware Store!");
        WriteLine("-----------------------------------");
        WriteLine("1. List all hardware.");
        WriteLine("2. List all hardware filtered on platform.");
        WriteLine("3. Show active leases.");
        WriteLine("4. Add new hardware.");
        WriteLine("5. Lease hardware.");
        WriteLine("-----------------------------------");
        WriteLine("6. Exit application.\n\n");
    }

    private static void ListAllHardware(RestClient client) 
    {
        var request = new RestRequest("hardware/api/1.0/hardware_list", Method.GET);
        IRestResponse response = client.Execute(request);
        var content = response.Content;
        if (content.Equals("Not found."))
            WriteLine(content);
        else 
            DeserializeAndDisplayResponse(content);
    }

    private static void ListPlatformFilteredHardware(RestClient client, string platform) 
    {
        var request = new RestRequest("hardware/api/1.0/hardware_list/" + platform, Method.GET);
        IRestResponse response = client.Execute(request);
        var content = response.Content;
        if (content.Equals("Not found."))
            WriteLine(content);
        else 
            DeserializeAndDisplayResponse(content);
    }

    private static void ListActiveLeases(RestClient client) 
    {
        client.AddHandler("hardware_list", new RestSharp.Deserializers.JsonDeserializer());
        var request = new RestRequest("hardware/api/1.0/active_leases", Method.GET);
        IRestResponse response = client.Execute(request);
        var content = response.Content;
        if (content.Equals("Not found."))
            WriteLine(content);
        else 
            DeserializeAndDisplayResponse(content);
    }

    private static void AddHardware(RestClient client, string passedName, string passedPlatform, string passedIp)
    {
        var request = new RestRequest("hardware/api/1.0/hardware_list", Method.POST);
        request.RequestFormat = DataFormat.Json;
        request.AddBody( new Hardware
        {
            name = passedName,
            platform = passedPlatform,
            ip = passedIp
        });
        client.Execute(request);
    }

    private static void Lease(RestClient client, int passedTimeLeftOnLease, string passedName, string passedPlatform)
    {
        var request = new RestRequest("hardware/api/1.0/lease/", Method.PUT);
        request.RequestFormat = DataFormat.Json;
        request.AddBody( new Hardware
        {
            name = passedName,
            platform = passedPlatform,
            time_left_on_lease = passedTimeLeftOnLease
        });
        client.ExecuteAsync(request, response => {
            WriteLine(response.Content);
        });
    }

    private static void DeserializeAndDisplayResponse(string content) 
    {
        try {
            List<Hardware> response = JsonConvert.DeserializeObject<List<Hardware>>(content);
            foreach(Hardware hardware in response) 
            {
                WriteLine("\n\n------------------------------------------");
                WriteLine("ID: " + hardware.id);
                WriteLine("NAME: " + hardware.name);
                WriteLine("PLATFORM: " + hardware.platform);
                WriteLine("IP: " + hardware.ip);
                WriteLine("LEASED: " + hardware.leased);
                WriteLine("TIME LEFT ON LEASE: " + hardware.time_left_on_lease);
                WriteLine("------------------------------------------");
            }
        } catch (Newtonsoft.Json.JsonSerializationException e) 
        {
            WriteLine("\nEither there are no results or something went wrong.");
        }
    }
}
