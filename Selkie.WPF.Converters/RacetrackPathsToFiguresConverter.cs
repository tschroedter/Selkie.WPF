using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters
{
    public class RacetrackPathsToFiguresConverter : IRacetrackPathsToFiguresConverter
    {
        public RacetrackPathsToFiguresConverter([NotNull] IRacetrackPathToFiguresConverter converter)
        {
            m_Converter = converter;
        }

        private readonly IRacetrackPathToFiguresConverter m_Converter;
        private List <PathFigureCollection> m_Figures = new List <PathFigureCollection>();
        private IEnumerable <IPath> m_Paths = new IPath[0];

        public IEnumerable <PathFigureCollection> Figures
        {
            get
            {
                return m_Figures;
            }
        }

        public IEnumerable <IPath> Paths
        {
            get
            {
                return m_Paths;
            }
            set
            {
                m_Paths = value;
            }
        }

        public void Convert()
        {
            var list = new List <PathFigureCollection>();

            IPath[] paths = Paths.ToArray();

            foreach ( IPath racetrack in paths )
            {
                m_Converter.Path = racetrack;
                m_Converter.Convert();
                m_Converter.FiguresCollection.Freeze();

                list.Add(m_Converter.FiguresCollection);
            }

            m_Figures = list;
        }
    }
}