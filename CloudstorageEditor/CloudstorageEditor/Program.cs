using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using RestSharp;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace CloudstorageEditorLauncher
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Cloudstorage Editor - @Ender#0001\n");
            if (DeviceAuth.Default.ProfileSaved == true)
            {
                Console.Write("Enter Custom Username >");
                File.WriteAllText("config.json", JsonConvert.SerializeObject(new
                {
                    displayName = Console.ReadLine()
                }));
                Console.Write("\n\nFound a saved profile wanna use it. Y/N > ");
                string awnser = Console.ReadLine().ToLower();
                if (awnser == "y")
                {
                    var url = "https://account-public-service-prod03.ol.epicgames.com/account/api/oauth/token";

                    var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpRequest.Method = "POST";
                    httpRequest.UserAgent = "Fortnite/++Fortnite+Release-15.10-CL-14937640 Windows/10.0.19042.1.768.64bit";
                    httpRequest.Headers["Authorization"] = "Basic NTIyOWRjZDNhYzM4NDUyMDhiNDk2NjQ5MDkyZjI1MWI6ZTNiZDJkM2UtYmY4Yy00ODU3LTllN2QtZjNkOTQ3ZDIyMGM3";
                    httpRequest.ContentType = "application/x-www-form-urlencoded";

                    var data = $"grant_type=device_auth&account_id={DeviceAuth.Default.AccountID}&device_id={DeviceAuth.Default.DeviceID}&secret={DeviceAuth.Default.Secret}";

                    using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                    {
                        streamWriter.Write(data);
                    }

                    var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    var result = new StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                    string exchange = Auth.GetExchange(JObject.Parse(result)["access_token"].ToString());
                    new Helper().Launch($"-AUTH_LOGIN=unused -AUTH_PASSWORD={exchange} -AUTH_TYPE=exchangecode").GetAwaiter().GetResult();

                }
                else if (awnser == "n")
                {
                    var devicecode = Auth.GetDevicecode(Auth.GetDevicecodetoken());
                    string exchange = Auth.GetExchange(JObject.Parse(devicecode)["access_token"].ToString());
                    var client = new RestClient("https://account-public-service-prod.ol.epicgames.com/account/api/public/account/" + JObject.Parse(devicecode)["account_id"].ToString() + "/deviceAuth");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", $"bearer {JObject.Parse(devicecode)["access_token"]}");
                    dynamic deviceresponse = JsonConvert.DeserializeObject(client.Execute(request).Content);
                    DeviceAuth.Default.AccountID = deviceresponse.accountId;
                    DeviceAuth.Default.DeviceID = deviceresponse.deviceId;
                    DeviceAuth.Default.Secret = deviceresponse.secret;
                    DeviceAuth.Default.Save();
                    Console.WriteLine("Im sorry WHAT");
                    Console.ReadLine();
                    new Helper().Launch($"-AUTH_LOGIN=unused -AUTH_PASSWORD={exchange} -AUTH_TYPE=exchangecode").GetAwaiter().GetResult();

                }
                else
                {
                    Environment.Exit(0);
                }
                
            }
            else
            {
                var devicecode = Auth.GetDevicecode(Auth.GetDevicecodetoken());
                string exchange = Auth.GetExchange(JObject.Parse(devicecode)["access_token"].ToString());
                var client = new RestClient("https://account-public-service-prod.ol.epicgames.com/account/api/public/account/" + JObject.Parse(devicecode)["account_id"].ToString() + "/deviceAuth");
                var request = new RestRequest(Method.POST);
                request.AddHeader("User-Agent", "Fortnite/++Fortnite+Release-15.10-CL-14937640 Windows/10.0.19042.1.768.64bit");
                request.AddHeader("Authorization", $"bearer {JObject.Parse(devicecode)["access_token"]}");
                dynamic deviceresponse = JsonConvert.DeserializeObject(client.Execute(request).Content);
                DeviceAuth.Default.AccountID = deviceresponse.accountId;
                DeviceAuth.Default.DeviceID = deviceresponse.deviceId;
                DeviceAuth.Default.Secret = deviceresponse.secret;
                DeviceAuth.Default.ProfileSaved = true;
                DeviceAuth.Default.Save();
                new Helper().Launch($"-AUTH_LOGIN=unused -AUTH_PASSWORD={exchange} -AUTH_TYPE=exchangecode").GetAwaiter().GetResult();

            }
        }
    }
}