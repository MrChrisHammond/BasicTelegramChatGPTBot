using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TelegramChatBot
{
    internal class ChatGPT
    {
        private string apiKey;
        public ChatGPT(string apiKey = "insert your chatgpt key here")
        {
            this.apiKey = apiKey;
        }
        public static HttpClient client = new HttpClient();
        public async Task<GPTResponse> GenerateResponse(string prompt)
        {
            GPTResponse gptResponse = new GPTResponse();
            prompt = prompt.Trim() + "\n"; //ChatGPT seems to be picky about this, without new line you may not receive a response.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var request = new
            {
                prompt = prompt,
                model = "text-davinci-003",
                suffix = "",
                max_tokens = 1024,
                temperature = 0.99,
                top_p = 0.86,
                n = 1,
                presence_penalty = 0.2,
                frequency_penalty = 0.2,
                // stream =  ,  uncomment if you wish to use these parameters 
                // logprobs =  ,
                // echo =  ,
                // stop =  ,
                // best_of =  ,
                // logit_bias = 0 // logit_bias["50256"] = -100; if you want to use all tokens, but may result in gibberish output at the end
                // user =  
            };
            var jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            gptResponse.querySent = jsonRequest;
            var response = await client.PostAsync("https://api.openai.com/v1/completions", content);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                gptResponse.errorMessage = $"{response.StatusCode}\n\n request:{jsonRequest}";
                gptResponse.hasError = true;
            }
            else
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(jsonResponse);
                if (data != null)
                {
                    gptResponse.response = data.choices[0].text;
                    if (gptResponse.response.Length <= 0)
                    {
                        gptResponse.hasError = true;
                        gptResponse.responseStatusCode = "EMPTY";
                    }
                    else
                    {
                        gptResponse.hasError = false;
                        gptResponse.responseStatusCode = "OK";
                    }
                }
                else
                {
                    gptResponse.hasError = true;
                    gptResponse.responseStatusCode = "EMPTY";
                }
            }
            return gptResponse;
        }
    }
}
