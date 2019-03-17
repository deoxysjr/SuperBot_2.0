using Newtonsoft.Json;
using StrawPollNET.Enums;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SuperBot_2_0.Services
{
    public class Strawpoll
    {
        private const string endpointURL = "https://www.strawpoll.me/api/v2/polls";

        public async Task<Poll> CreatePollAsync(string title, List<string> options, bool multi, DupCheck dupcheck, bool capcha)
        {
            HttpResponseMessage resultJson;
            var jsondata = Request.CreateRequest(title, options, multi, dupcheck, capcha);

            using (var client = new HttpClient())
            {
                var content = new StringContent(jsondata, Encoding.UTF8, "application/json");
                resultJson = await client.PostAsync(endpointURL, content);
            }

            return JsonConvert.DeserializeObject<Poll>(await resultJson.Content.ReadAsStringAsync());
        }

        public async Task<Poll> CreatePollAsync(PollRequest poll)
        {
            HttpResponseMessage resultJson;
            var jsondata = Request.CreateRequest(poll);

            using (var client = new HttpClient())
            {
                var content = new StringContent(jsondata, Encoding.UTF8, "application/json");
                resultJson = await client.PostAsync(endpointURL, content);
            }

            return JsonConvert.DeserializeObject<Poll>(await resultJson.Content.ReadAsStringAsync());
        }

        public async Task<Poll> GetPollAsync(int id)
        {
            HttpResponseMessage resultJson;

            using (var client = new HttpClient())
                resultJson = await client.GetAsync(endpointURL + "/" + id);

            return JsonConvert.DeserializeObject<Poll>(await resultJson.Content.ReadAsStringAsync());
        }
    }
}