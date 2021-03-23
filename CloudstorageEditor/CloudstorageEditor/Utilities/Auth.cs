using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Diagnostics;
using System.Threading;

namespace CloudstorageEditorLauncher
{
    class Auth
    {
        public static string GetDevicecodetoken()
        {
            var client = new RestClient("https://account-public-service-prod03.ol.epicgames.com/account/api/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Basic NTIyOWRjZDNhYzM4NDUyMDhiNDk2NjQ5MDkyZjI1MWI6ZTNiZDJkM2UtYmY4Yy00ODU3LTllN2QtZjNkOTQ3ZDIyMGM3");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("grant_type", "client_credentials");

            string reply = client.Execute(request).Content;
            string[] array = reply.Split(new char[] { ':' }, 26);
            try
            {
                string[] token1 = array[1].ToString().Split(new char[] { ',' }, 2)[0].ToString().Split(new char[] { '"' }, 2)[1].ToString().Split(new char[] { '"' }, 2);
                var token = token1[0].ToString();
                return token;
            }
            catch
            {
                Console.WriteLine("Cannot run the application offline.\nLauncher will now close.");
                Process.GetCurrentProcess().Kill();
                return "error";
            }
        }
            public static string GetDevicecode(string auth)
            {
                var client = new RestClient("https://account-public-service-prod03.ol.epicgames.com/account/api/oauth/deviceAuthorization");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", $"Bearer {auth}");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                string reply = client.Execute(request).Content;
                string[] array = reply.Split(new char[] { ',' }, 8);
                //array[3] = "verification_uri_complete": "https://www.epicgames.com/activate?userCode={code}"
                string[] deviecodeurlarray = array[3].ToString().Split(new char[] { '"' }, 4);
                string[] url = deviecodeurlarray[3].ToString().Split(new char[] { '"' }, 2);
                string[] deviecodearray = array[1].ToString().Split(new char[] { '"' }, 4);
                string[] devicecode = deviecodearray[3].ToString().Split(new char[] { '"' }, 2);
                Process.Start(url[0]);

                while (true)
                {
                    var checkurl = new RestClient("https://account-public-service-prod03.ol.epicgames.com/account/api/oauth/token");
                    var checkrequest = new RestRequest(Method.POST);
                    checkrequest.AddHeader("Authorization", "Basic NTIyOWRjZDNhYzM4NDUyMDhiNDk2NjQ5MDkyZjI1MWI6ZTNiZDJkM2UtYmY4Yy00ODU3LTllN2QtZjNkOTQ3ZDIyMGM3");
                    checkrequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                    checkrequest.AddParameter("grant_type", "device_code");
                    checkrequest.AddParameter("device_code", devicecode[0].ToString());
                    string checkreply = checkurl.Execute(checkrequest).Content;
                    if (checkreply.Contains("access_token"))
                    {
                        string[] array2 = checkreply.Split(new char[] { ':' }, 26);
                        string[] token1 = array2[1].ToString().Split(new char[] { ',' }, 2)[0].ToString().Split(new char[] { '"' }, 2)[1].ToString().Split(new char[] { '"' }, 2);
                        var token = token1[0].ToString();
                    string[] username1 = array2[16].ToString().Split(new char[] { ',' }, 2);
                        var username = username1[0];

                        return JsonConvert.SerializeObject(new { 
                            access_token = token,
                            displayName = username,
                            account_id = JObject.Parse(checkreply)["account_id"]
                        });
                        //break;

                    }
                    else if (checkreply.Contains("errors.com.epicgames.not_found"))
                    {
                    }
                    Thread.Sleep(150);
                }
            }

        public static string GetToken(string authCode)
        {
            Console.WriteLine("Requesting access token...");
            var client = new RestClient("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/token");
            var request = new RestRequest(Method.POST);

            request.AddHeader("Authorization", "basic ZWM2ODRiOGM2ODdmNDc5ZmFkZWEzY2IyYWQ4M2Y1YzY6ZTFmMzFjMjExZjI4NDEzMTg2MjYyZDM3YTEzZmM4NGQ=");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("code", authCode);

            string reply = client.Execute(request).Content;
            var token = "error";
            var username = "error";
            if (reply.Contains("access_token"))
            {

                string[] array = reply.Split(new char[] { ':' }, 26);
                string[] username1 = array[17].ToString().Split(new char[] { ',' }, 2);
                username = username1[0];
                string[] token1 = array[1].ToString().Split(new char[] { ',' }, 2)[0].ToString().Split(new char[] { '"' }, 2)[1].ToString().Split(new char[] { '"' }, 2);
                token = token1[0].ToString();

                return token + ',' + username;
            }
            else if (reply.Contains("It is possible that it was no longer valid"))
            {
                Console.WriteLine("There Was An ERROR Please Get New Token To Proceed");
                {
                    Process.Start("https://www.epicgames.com/id/logout?redirectUrl=https%3A//www.epicgames.com/id/login%3FredirectUrl%3Dhttps%253A%252F%252Fwww.epicgames.com%252Fid%252Fapi%252Fredirect%253FclientId%253D3446cd72694c4a4485d81b77adbb2141%2526responseType%253Dcode");
                }
                return "error";
            }
            else
            {
                Console.WriteLine(reply);
                return "error";
            }
        }

        public static string GetExchange(string token)
        {
            Console.WriteLine("Requesting exchange code...");
            var client = new RestClient("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/exchange");
            var request = new RestRequest(Method.GET);

            request.AddHeader("Authorization", $"bearer {token}");

            string reply = client.Execute(request).Content;
            //Console.WriteLine(reply);
            var exenge = "error";
            if (!reply.Contains("errors.com.epicgames.common.oauth.invalid_token"))
            {
                string[] array = reply.Split(new char[] { ',' }, 4);
                string[] exenge1 = array[1].ToString().Split(new char[] { ',' }, 2)[0].ToString().Split(new char[] { '"' }, 2)[1].ToString().Split(new char[] { '"' }, 2);
                string[] exenge2 = exenge1[1].ToString().Split(new char[] { '"' }, 2)[1].ToString().Split(new char[] { '"' }, 2);
                exenge = exenge2[0].ToString();

                return exenge;
            }
            else
            {
                return "error";
            }
        }
    }
}