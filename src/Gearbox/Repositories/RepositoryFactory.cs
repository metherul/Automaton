using LanguageExt;
using Newtonsoft.Json.Linq;

using static LanguageExt.Prelude;

namespace Gearbox.Repositories
{
    public class RepositoryFactory
    {
        public static Option<IRepository> FromJson(JObject json)
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
                _ => None
            };
        }
    }
}