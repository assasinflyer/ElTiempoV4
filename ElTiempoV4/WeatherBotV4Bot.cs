using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Linq;
using WeatherBotV4Bot.Services;
using WeatherBotV4Bot.Helpers;

namespace WeatherBotV4Bot
{
    public class WeatherBotV4Bot : IBot
    {
        public static readonly string LuisKey = "WeatherBotV4Bot";
        private readonly BotService _services;

        public WeatherBotV4Bot(BotService services)
        {
            _services = services ?? throw new System.ArgumentNullException(nameof(services));

            if (!_services.LuisServices.ContainsKey(LuisKey))
                throw new System.ArgumentException($"Invalid configuration....");
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var recognizer = await _services.LuisServices[LuisKey].RecognizeAsync(turnContext, cancellationToken);
                var topIntent = recognizer?.GetTopScoringIntent();

                if (topIntent != null && topIntent.HasValue && topIntent.Value.intent != "None")
                {
                    var location = LuisParser.GetEntityValue(recognizer);

                    var ro = await WeatherService.GetWeather(location);

                    if(ro == null)
                    {
                        location = "";
                    }

                    if (location.ToString() != string.Empty)
                    {
                        var weather = $"{ro.weather.First().main} ({ro.main.temp.ToString("N2")} °C)";

                        var typing = Activity.CreateTypingActivity();
                        var delay = new Activity { Type = "delay", Value = 5000 };

                        var activities = new IActivity[] {
                            typing,
                            delay,
                            MessageFactory.Text($"Weather of {location} is: {weather}"),
                            MessageFactory.Text("Thanks for using our service!")
                        };

                        await turnContext.SendActivitiesAsync(activities);
                    }
                    else if (ro == null)
                    {
                        await turnContext.SendActivityAsync($"==>Location not found!");
                    }
                    else
                        await turnContext.SendActivityAsync($"==>Can't understand you, sorry!");
                }
                else
                {
                    var msg = @"No LUIS intents were found.
                    This sample is about identifying a city and an intent:
                    'Find the current weather in a city'
                    Try typing 'What's the weather in Prague'";

                    await turnContext.SendActivityAsync(msg);
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
                await SendWelcomeMessageAsync(turnContext, cancellationToken);
            else
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
        }

        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome to WeatherBotv4 {member.Name}!",
                        cancellationToken: cancellationToken);
                }
            }
        }
    }
}
