using Newtonsoft.Json.Linq;

namespace Gearbox.Repositories
{
    public class RepositoryFactory
    {
        public static IRepository FromJson(JObject json)
        {
            if (json == null || !json.HasValues)
            {
                return null;
            }

            var repoType = json["RepositoryType"].ToString();

            return repoType switch
            {
                "NexusMods" => json.ToObject<NexusMods.Repository>(),
                "Direct" => json.ToObject<Direct.Repository>(),
                "Github" => json.ToObject<Github.Repository>(),
                "MEGA" => json.ToObject<MEGA.Repository>(),
                _ => null
            };
        }
    }
}