using Selkie.Framework.Interfaces;
using Selkie.Windsor;

namespace Selkie.Framework.Aco
{
    [ProjectComponent(Lifestyle.Transient)]
    public class ColonyParameters : IColonyParameters
    {
        public ColonyParameters()
        {
            CostMatrix = new int[0][]; // todo use default matrix
            CostPerLine = new int[0]; // todo use default cost per line
        }

        public int[][] CostMatrix { get; set; }
        public int[] CostPerLine { get; set; }
        public bool IsFixedStartNode { get; set; }
        public int FixedStartNode { get; set; }
    }
}