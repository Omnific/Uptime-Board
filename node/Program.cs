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
            var deviceConfig = await DownloadDeviceConfig(config.RequestApi);

            if (deviceConfig == null)
            {
                Console.WriteLine($"Failed to load device configurations from {config.RequestApi}");
            }

            bool continueRequests = true;
            while (continueRequests)
            {
                var responses = deviceConfig.AsParallel()
                                    .Select(async d => (await RequestDevice(d)))
                                    .Select(d => d.Result)
                                    .Select(async r => (await SendResult(config.ResultApi, r)))
                                    .Select(r => r.Result)
                                    .AsEnumerable();

                continueRequests = responses.All(r => r);

                Thread.Sleep(config.RequestInterval);
            }
        }

        public async static Task<bool> SendResult(string responseUrl, DeviceResponse response)
        {
            var success = false;
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var httpContent = new StringContent("plex_info,host=raxxy transcodes=$numtranscodes");
                    var request = await client.PostAsync(responseUrl, httpContent);
                    success = request.EnsureSuccessStatusCode().IsSuccessStatusCode;
                }
                catch (HttpRequestException)
                {
                    success = false;
                }
            }

            return success;
        }

        public async static Task<DeviceResponse> RequestDevice(DeviceConfig config)
        {
            if (config.Type == RequestType.Ping)
            {
                using (Ping pinger = new Ping())
                {
                    var pingOptions = new PingOptions();
                    byte[] buffer = new byte[32];
                    PingReply reply = await pinger.SendPingAsync(config.Address, 1000, buffer);
                    return new DeviceResponse
                    {
                        Name = config.Name,
                        Success = reply.Status == IPStatus.Success,
                        TotalMs = reply.RoundtripTime
                    };
                }
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
