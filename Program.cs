using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text;
using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramChatBot;

namespace TelegramChatBot
{
    class Program
    {
        static void Main(string[] args)
        {
            TelegramGPTBots bots = new TelegramGPTBots();
            while (true)
            {
                var prompt = Console.ReadLine();
                if(prompt == null)
                {
                    Console.WriteLine("Enter something...");
                }
                else if (prompt.StartsWith("BotInteraction"))
                {
                    bots.BotInteraction(new List<int> { 0, 1 });
                }
                else if (prompt.StartsWith("Exit"))
                {
                    Console.WriteLine("Good bye!");
                    break;
                }
            }
        }
    }
}