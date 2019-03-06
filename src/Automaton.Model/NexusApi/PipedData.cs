namespace Automaton.Model.NexusApi
{
    public class PipedData
    {
        public PipedData(string nxmFullText)
        {
            var splitString = nxmFullText.Split('/');

            NxmFullText = nxmFullText;
            Game = splitString[2];
            ModId = splitString[4];
            FileId = splitString[6].Split('?')[0];

            AuthenticationParams = "?" + splitString[6].Split('?')[1];
        }

        public string NxmFullText { get; set; }
        public string Game { get; set; }
        public string ModId { get; set; }
        public string FileId { get; set; }

        public string AuthenticationParams { get; set; }
    }
}