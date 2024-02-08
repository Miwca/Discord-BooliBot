using BooliBot.Bully.Services.Abstractions;
using BooliBot.Bully.Settings;

namespace BooliBot.Bully.Services
{
    public class BullyService : IBullyService
    {
        private readonly BullyConfig _bullySettings;

        public BullyService(BullyConfig bullySettings)
        {
            _bullySettings = bullySettings;
        }

        public string GetRandomBurn()
        {
            var random = new Random();
            var index = random.Next(_bullySettings.Burns!.Length);
            
            return _bullySettings.Burns[index];
        }
    }
}
