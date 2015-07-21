using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;
using Selkie.Framework.Interfaces.Converters;
using Selkie.Geometry.Shapes;
using Selkie.Windsor;

namespace Selkie.Framework
{
    [ProjectComponent(Lifestyle.Transient)]
    public class LinesSource : ILinesSource
    {
        public static ILinesSource Unknown = new LinesSource();
        private readonly IEnumerable <int> m_CostPerLine = new int[0];
        private readonly IEnumerable <ILine> m_Lines = new ILine[0];

        private LinesSource()
        {
        }

        public LinesSource([NotNull] ILinesToCostPerLineConverter converter,
                           [NotNull] IEnumerable <ILine> lines)
        {
            m_Lines = lines.ToArray();

            converter.Lines = m_Lines; // todo doing to much in the converter?
            converter.Convert();

            m_CostPerLine = converter.CostPerLine;
        }

        public IEnumerable <ILine> Lines
        {
            get
            {
                return m_Lines;
            }
        }

        public IEnumerable <int> CostPerLine
        {
            get
            {
                return m_CostPerLine;
            }
        }
    }
}