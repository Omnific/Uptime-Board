using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UptimeBoard.Node.Models;

namespace UptimeBoard.Node
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(Process).Wait();
        }

        static async Task Process()
        {
            var configJson = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<Config>(configJson);

            bool continueRequests = true;
            while (continueRequests)
            {
                var deviceConfig = await DownloadDeviceConfig(config.RequestApi);

                if (deviceConfig == null)
                {
                    Console.WriteLine($"Failed to load device configurations from {config.RequestApi}");
                }
                else
                {
                    var responses = deviceConfig.AsParallel()
                                        .Select(async d => (await RequestDevice(d)))
                                        .Select(d => d.Result)
                                        .Select(async r => (await SendResult(config.ResultApi, r)))
                                        .Select(r => r.Result)
                                        .AsEnumerable();

                    continueRequests = responses.All(r => r);

                    Console.WriteLine("Requests Complete");

                    if (continueRequests)
                    {
                        Thread.Sleep(config.RequestInterval);
                    }
                }
            }
        }

        public async static Task<bool> SendResult(string responseUrl, DeviceResponse response)
        {
            var success = false;
            using (var client = new HttpClient())
            {
                try
                {
					Console.WriteLine($"Sending To: {responseUrl}");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var request = $"plex_info,host={response.Name},address={response.Address} pings={response.Total},ms={response.TotalMs},up={(response.Up ? "1" : "0")}";
                    
					Console.WriteLine($"Sending: {request}");

                    var content = new StringContent(request);
                    var requestResponse = await client.PostAsync(responseUrl, content);
                    success = requestResponse.EnsureSuccessStatusCode().IsSuccessStatusCode;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine(ex.ToString());
                    success = false;
                }
            }

            return success;
        }

        public async static Task<DeviceResponse> RequestDevice(DeviceConfig config)
        {
            if (config.Type == RequestType.Ping)
            {
                List<PingReply> pingReplies = new List<PingReply>();

                using (Ping pinger = new Ping())
                {
                    var pingOptions = new PingOptions();
                    byte[] buffer = new byte[32];

                    for(var i=0; i<config.Total; i++)
                    {
                        pingReplies.Add(await pinger.SendPingAsync(config.Address, config.RequestTimeout, buffer));
					    Console.WriteLine($"Pinged: {config.Address}");
                    }
                }

                return new DeviceResponse
                {
                    Name = config.Name,
                    Address = config.Address,
                    Total = config.Total,
                    Success = pingReplies.All(r => r.Status == IPStatus.Success),
                    TotalMs = Convert.ToInt64(pingReplies.Average(r => r.RoundtripTime)),
                    Up = !pingReplies.All(r => r.Status != IPStatus.Success)
                };
            }

            return null;
        }

        public async static Task<List<DeviceConfig>> DownloadDeviceConfig(string apiUrl)
        {
            List<DeviceConfig> result = null;

            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
    
                    var stringResult = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<List<DeviceConfig>>(stringResult);
                }
                catch (HttpRequestException)
                {
                }
            }

            return result;
        }
    }
}
