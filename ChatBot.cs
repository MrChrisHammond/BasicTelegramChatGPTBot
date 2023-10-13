using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
namespace TelegramChatBot
{
    internal class ChatBot
    {
        public TelegramBotClient client { get; set; }
        public string botName { get; set; }
        public string fullName { get; set; }
        public string personality { get; set; }
        private string apiToken { get; set; }
        public ChatBot(string apiToken, string botName = "SallysRasin", string fullName = "Sally's Rasin", string personality = "A helpful and friendly raisin.")
        {
            this.apiToken = apiToken;
            this.botName = botName;
            this.fullName = fullName;
            this.personality = personality;
            StartTelegramBotClient();
        }
        public async void StartTelegramBotClient()
        {
            client = new TelegramBotClient(apiToken);

            using CancellationTokenSource cts = new();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            client.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingError,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );
            var me = await client.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;
            DateTime targetDate = update.Message.Date;
            TimeSpan timeDiff = DateTime.UtcNow - targetDate;

            //no stale messages
            if (Math.Abs(timeDiff.TotalSeconds) >= 30)
                return;

            ChatGPT chatGPT = new ChatGPT();
            GPTResponse gptResponse = new GPTResponse();
            if (((message.Text.Contains("@"+ botName)) ||
                (message.ReplyToMessage != null && message.ReplyToMessage.From.Username == botName)) &&
                botClient.BotId == client.BotId)
            {
                string query = messageText.Replace("@"+ botName, "");

                gptResponse = await chatGPT.GenerateResponse($"{personality}. Reply to the following message: {query}");
                var chatId = message.Chat.Id;
                Telegram.Bot.Types.Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                gptResponse.response,
                cancellationToken: cancellationToken,
                replyToMessageId: message.MessageId);        
            }
        }
        private Task HandlePollingError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
