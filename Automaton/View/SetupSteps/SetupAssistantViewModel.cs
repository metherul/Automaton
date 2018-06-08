using Automaton.Model.Instances;

namespace Automaton.View.SetupSteps
{
    public class SetupAssistantViewModel
    {
        public string ImagePath { get; set; }
        public string Description { get; set; }

        public SetupAssistantViewModel()
        {
            ModpackInstance.ModpackHeaderChangedEvent += ModpackHeaderInstanceUpdate;
        }

        private void ModpackHeaderInstanceUpdate()
        {
            if (ModpackInstance.ModpackHeader.ContainsSetupAssistant == false) return;

            var setupAssistant = ModpackInstance.ModpackHeader.SetupAssistant;

            ImagePath = setupAssistant.DefaultImage;
            Description = setupAssistant.DefaultDescription;
        }
    }
}
