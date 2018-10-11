using System;
using Newtonsoft.Json;
using Automaton.Model.ModpackBase.Interfaces;
using Automaton.Model.Utility.Interfaces;

namespace Automaton.Model.Utility
{
    public class JsonUtilities : IJsonUtilities
    {
        public IHeader DeserializeHeader(string headerContents, out string parseError)
        {
            parseError = "";

            try
            {
                return JsonConvert.DeserializeObject<IHeader>(headerContents);
            }

            catch (Exception e)
            {
                parseError = e.Message;
                return null;
            }
        }

        public IMod DeserializeMod(string modContents, out string parseError)
        {
            parseError = "";

            try
            {
                return JsonConvert.DeserializeObject<IMod>(modContents);
            }

            catch (Exception e)
            {
                parseError = e.Message;
                return null;
            }
        }
    }
}