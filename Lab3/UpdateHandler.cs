using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Lab3
{
    public class UpdateHandler
    {
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, 
            Update update, 
            CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            try
            {

                CommandHander.HandleCommand(chatId, messageText, botClient);
            }
            catch (ArgumentException e)
            {
                botClient.SendTextMessageAsync(chatId, e.Message);
            }
            catch (Exception e)
            {
                botClient.SendTextMessageAsync(chatId, "Возникла ошибка! Повторите попытку.");
            }
            

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
        }

        public static Task HandlePollingErrorAsync(
            ITelegramBotClient botClient, 
            Exception exception, 
            CancellationToken cancellationToken)
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
