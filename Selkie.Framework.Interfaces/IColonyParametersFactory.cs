using JetBrains.Annotations;
using Selkie.Windsor;

namespace Selkie.Framework.Interfaces
{
    public interface IColonyParametersFactory : ITypedFactory
    {
        IColonyParameters Create();

        IColonyParameters Create([NotNull] int[][] costMatrix,
                                 [NotNull] int[] costPerLine,
                                 bool isFixedStartNode,
                                 int fixedStartNode);

        void Release([NotNull] IColonyParameters colonyParameters);
    }
}