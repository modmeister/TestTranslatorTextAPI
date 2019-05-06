using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace TestTranslatorTextAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            GetSupportedLanguages();
            TranslateText("Chicken");
            TransliterateText("チキン");
            Console.ReadKey();
        }

        static void GetSupportedLanguages()
        {
            var route = "/languages?api-version=3.0";
            var result = MakeWebCall(route, "", HttpMethod.Get, "");
            Console.WriteLine(result);
        }

        static void TranslateText(string textToTranslate)
        {
            var route = "/translate?api-version=3.0";
            var parameters = "&to=de&to=es&to=fr&to=it";
            var result = MakeWebCall(route, parameters, HttpMethod.Post, textToTranslate);
            Console.WriteLine(result);
        }

        static void TransliterateText(string textToTransliterate)
        {
            var route = "/transliterate?api-version=3.0";
            var parameters = "&language=ja&fromScript=Jpan&toScript=Latn";
            var result = MakeWebCall(route, parameters, HttpMethod.Post, textToTransliterate);
            Console.WriteLine(result);
        }

        private static string MakeWebCall(string route, string parameters, HttpMethod httpMethod, string textToProcess)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                var body = new object[] { new { Text = textToProcess } };
                var requestBody = JsonConvert.SerializeObject(body);
                request.Method = httpMethod;
                request.RequestUri = new Uri(Properties.Settings.Default.baseUrl + route + parameters);
                request.Headers.Add("Ocp-Apim-Subscription-Key", Properties.Settings.Default.subscriptionKey);
                request.Content = httpMethod == HttpMethod.Post ? new StringContent(requestBody, Encoding.UTF8, "application/json") : null;

                var response = client.SendAsync(request).Result;
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var jsonresponseContent = FormatStringAsJson(responseContent);

                return jsonresponseContent;
            }
        }

        static string FormatStringAsJson(string stringToFormat)
        {
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(stringToFormat), Formatting.Indented);
        }
    }
}
