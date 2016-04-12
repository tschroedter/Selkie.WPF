using JetBrains.Annotations;
using Selkie.Windsor;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models.Settings
{
    [ProjectComponent(Lifestyle.Transient)]
    public class AntSettingsNode : IAntSettingsNode
    {
        // todo testing
        public AntSettingsNode(int id,
                               [NotNull] string description)
        {
            Id = id;
            Description = description;
        }

        public int Id { get; private set; }

        public string Description { get; private set; }
    }
}