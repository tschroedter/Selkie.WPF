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
            CostPerFeature = new int[0]; // todo use default cost per line
        }

        public int[][] CostMatrix { get; set; }
        public int[] CostPerFeature { get; set; }
        public bool IsFixedStartNode { get; set; }
        public int FixedStartNode { get; set; }
    }
}