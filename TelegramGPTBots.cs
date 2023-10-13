using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;

namespace TelegramChatBot
{
    internal class TelegramGPTBots
    {
        private List<ChatBot> botClientList = new List<ChatBot>();
        public TelegramGPTBots()
        {
            botClientList.Add(new ChatBot("insertTelegramBotKeyHere", "yourTelegramBotName", "Full Name", "You are Susie Green from Curb Your Enthusiasm. Your personality is filled with sarcasm, you are blunt, outspoken, strong-willed, and foul-mouthed."));
            botClientList.Add(new ChatBot("insertTelegramBotKeyHere", "yourTelegramBotName", "Full Name", "You are Larry David from Curb Your Enthusiasm. Your personality is filled with Sardonic Humor, you are blunt, candid, and self-centered."));
        }
        /// <summary>
        /// This function orchestrates a chatbot interaction using a predefined list of chatbots, creating a conversation
        /// with a specific structure. It is designed to engage in a whimsical and humorous conversation by generating messages 
        /// in a playful and entertaining manner. 
        /// Telegram bots cannot reply to each other due to Telegram API restrictions to prevent an endless chat, therefore,
        /// replies should be tracked.
        /// </summary>
        /// <param name="botsToUse"> A list of integers representing the indices of the chatbots to be used in the conversation.</param>
        public async void BotInteraction(List<int> botsToUse)
        {
            string lastMsg = "";

            List<string> previousMessages = new List<string>();
            List<string> humourWords = new List<string> { "Whimsical", "Frivolous", "Cynical", "Sarcastic", "Witty", "Silly", "Goofy", "Mirthful", "Insane", "Absurd", "Playful", "Raucous", "Jocular", "Amusing", "Grotesque" };
            List<string> foodFruitNames = new List<string> { "Jackfruit", "Durian", "Miracle Fruit", "Kumquat", "Physalis", "African Cucumber", "Cherimoya", "Pitaya", "Chempedak", "Soursop", "Langsat", "Dragonfruit", "Custard Apple", "Rambutan", "Horned Melon" };
            int whoIsTalkingIndex = 1;
            Random rand = new Random();
            int lengthOfConvo = 5; //5 message conversation
            ChatGPT chatGPT = new ChatGPT();
            for (int i = 0; i < lengthOfConvo; i++)
            {
                string query = "";
                if (i == 0)
                {
                    query = $"Genereate a {humourWords[rand.Next(0, humourWords.Count - 1)]} monologue as if {botClientList[botsToUse[whoIsTalkingIndex]].personality}. " +
                            $"The monologue should be no longer than 30 seconds and should be really silly or strange. " +
                            $"Then conclude with a love of {foodFruitNames[rand.Next(0, foodFruitNames.Count - 1)]} dancing.";
                }
                else
                {
                    query = $"Reply to the following message as if you are {botClientList[botsToUse[whoIsTalkingIndex]].personality}: {lastMsg}";
                }
                GPTResponse response = await chatGPT.GenerateResponse(query);
                if (response != null && response.response.Length > 0)
                {
                    previousMessages.Add(response.response);
                    lastMsg = response.response;
                    var chatId = 1234567890; //Replace with your telegram channel id
                    CancellationToken cancellationToken = new CancellationToken();

                    Telegram.Bot.Types.Message message = await botClientList[botsToUse[whoIsTalkingIndex]].client.SendTextMessageAsync(
                        chatId: chatId,
                        text: response.response,
                        cancellationToken: cancellationToken
                        );
                    whoIsTalkingIndex++;
                    if(whoIsTalkingIndex >= botsToUse.Count)
                    {
                        whoIsTalkingIndex = 0;
                    }
                }
                else
                {
                    Console.WriteLine("No response for query: " + query);
                }
                await Task.Delay(8000);
            }
        }
    }
}
