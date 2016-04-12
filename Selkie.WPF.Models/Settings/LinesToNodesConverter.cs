using System.Collections.Generic;
using Selkie.Geometry.Shapes;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models.Settings
{
    [ProjectComponent(Lifestyle.Transient)]
    public class LinesToNodesConverter : ILinesToNodesConverter
    {
        public LinesToNodesConverter()
        {
            Nodes = new IAntSettingsNode[0];
        }

        public void Convert(IAntSettingsNodeFactory antSettingsNodeFactory,
                            IEnumerable <ILine> lines)
        {
            var nodes = new List <IAntSettingsNode>();

            foreach ( ILine line in lines )
            {
                int forwardId = line.Id * 2;

                IAntSettingsNode nodeForward = antSettingsNodeFactory.Create(forwardId,
                                                                             "Line {0}".Inject(line.Id));
                nodes.Add(nodeForward);

                int reverseId = line.Id * 2 + 1;

                IAntSettingsNode nodeReverse = antSettingsNodeFactory.Create(reverseId,
                                                                             "Line {0} (Reverse)".Inject(line.Id));
                nodes.Add(nodeReverse);
            }

            Nodes = nodes;
        }

        public IEnumerable <IAntSettingsNode> Nodes { get; private set; }
    }
}