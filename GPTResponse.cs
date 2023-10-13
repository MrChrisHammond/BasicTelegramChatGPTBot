using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramChatBot
{
    public class GPTResponse
    {
        public string response { get; set; }
        public bool hasError { get; set; }
        public string errorMessage { get; set; }
        public string responseStatusCode { get; set; }
        public string querySent { get; set; }
    }
}
