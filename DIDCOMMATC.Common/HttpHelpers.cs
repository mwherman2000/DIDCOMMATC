using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIDCOMMATC.Common
{
    public static class HttpHelpers
    {
        // https://www.thecodebuzz.com/using-httpclient-best-practices-and-anti-patterns/
        static readonly HttpClient httpClient = new HttpClient(); 

        public static string SendHttpMessage(string url, string jsonRequest)
        {
            string jsonResponse = "{ }";

            Console.WriteLine(">>>Agent Url:" + url);
            using (var requestMessage = new HttpRequestMessage(new HttpMethod("POST"), url))
            {
                requestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
                Console.WriteLine(">>>Request:" + jsonRequest);
                requestMessage.Content = new StringContent(jsonRequest);
                var task = httpClient.SendAsync(requestMessage);
                task.Wait();
                var result = task.Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;   
                Console.WriteLine(">>>Response:" + jsonResponse);
            }

            return jsonResponse;
        }
    }
}
