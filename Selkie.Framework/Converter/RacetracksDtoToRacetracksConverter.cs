using System.Linq;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;
using Selkie.Services.Racetracks.Common.Dto;
using Selkie.Windsor;

namespace Selkie.Framework.Converter
{
    [ProjectComponent(Lifestyle.Transient)]
    public class RacetracksDtoToRacetracksConverter : IRacetracksDtoToRacetracksConverter
    {
        private readonly IPathDtoToPath m_PathDtoToPath;
        private RacetracksDto m_Dto = new RacetracksDto(); // todo Unknown
        private IRacetracks m_Racetracks = new Racetracks(); // todo Unknown

        public RacetracksDtoToRacetracksConverter(IPathDtoToPath pathDtoToPath)
        {
            m_PathDtoToPath = pathDtoToPath;
        }

        public RacetracksDto Dto
        {
            get
            {
                return m_Dto;
            }
            set
            {
                m_Dto = value;
            }
        }

        public IRacetracks Racetracks
        {
            get
            {
                return m_Racetracks;
            }
        }

        public void Convert()
        {
            IPath[][] forwardToForward = ConvertPathDtos(m_Dto.ForwardToForward);
            IPath[][] forwardToReverse = ConvertPathDtos(m_Dto.ForwardToReverse);
            IPath[][] reverseToForward = ConvertPathDtos(m_Dto.ReverseToForward);
            IPath[][] reverseToReverse = ConvertPathDtos(m_Dto.ReverseToReverse);

            m_Racetracks = new Racetracks
                           {
                               ForwardToForward = forwardToForward,
                               ForwardToReverse = forwardToReverse,
                               ReverseToForward = reverseToForward,
                               ReverseToReverse = reverseToReverse
                           };
        }

        internal IPath[][] ConvertPathDtos(PathDto[][] pathDtos)
        {
            var paths = new IPath[pathDtos.Length][];
            var i = 0;

            foreach ( PathDto[] dtos in pathDtos )
            {
                paths [ i++ ] = dtos.Select(ConvertPathDto).ToArray();
            }

            return paths;
        }

        internal IPath ConvertPathDto([NotNull] PathDto pathDto)
        {
            m_PathDtoToPath.Dto = pathDto;
            m_PathDtoToPath.Convert();

            return m_PathDtoToPath.Path;
        }
    }
}