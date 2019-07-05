using Automaton.Model.Interfaces;
using Pathoschild.FluentNexus;
using System.Threading.Tasks;

namespace Automaton.Model
{
    public class NexusApi : INexusApi
    {
        private NexusClient _nexusClient;

        private bool _isPremium;

        public void Init(string key)
        {
            _nexusClient = new NexusClient(key, "Automaton", "indev");
        }

        public async Task<bool> IsPremiumUser()
        {
            var validateResult = await _nexusClient.Users.ValidateAsync();
            var test = _nexusClient.GetRateLimits();

            _isPremium = validateResult.IsPremium;

            return validateResult.IsPremium;
        }
    }
}
