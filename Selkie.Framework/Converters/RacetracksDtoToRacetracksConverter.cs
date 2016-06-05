using System.Linq;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;
using Selkie.Services.Common.Dto;
using Selkie.Windsor;

namespace Selkie.Framework.Converters
{
    [ProjectComponent(Lifestyle.Transient)]
    public class RacetracksDtoToRacetracksConverter : IRacetracksDtoToRacetracksConverter
    {
        public RacetracksDtoToRacetracksConverter(IPathDtoToPath pathDtoToPath)
        {
            m_PathDtoToPath = pathDtoToPath;
            Dto = CreateUnknownRacetracksDto();
            Racetracks = Framework.Racetracks.Unknown;
        }

        private readonly IPathDtoToPath m_PathDtoToPath;

        public RacetracksDto Dto { get; set; }
        public IRacetracks Racetracks { get; private set; }

        public void Convert()
        {
            IPath[][] forwardToForward = ConvertPathDtos(Dto.ForwardToForward);
            IPath[][] forwardToReverse = ConvertPathDtos(Dto.ForwardToReverse);
            IPath[][] reverseToForward = ConvertPathDtos(Dto.ReverseToForward);
            IPath[][] reverseToReverse = ConvertPathDtos(Dto.ReverseToReverse);

            Racetracks = new Racetracks
                         {
                             ForwardToForward = forwardToForward,
                             ForwardToReverse = forwardToReverse,
                             ReverseToForward = reverseToForward,
                             ReverseToReverse = reverseToReverse
                         };
        }

        internal IPath ConvertPathDto([NotNull] PathDto pathDto)
        {
            m_PathDtoToPath.Dto = pathDto;
            m_PathDtoToPath.Convert();

            return m_PathDtoToPath.Path;
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

        private static RacetracksDto CreateUnknownRacetracksDto()
        {
            return new RacetracksDto
                   {
                       ForwardToForward = new PathDto[0][],
                       ForwardToReverse = new PathDto[0][],
                       ReverseToForward = new PathDto[0][],
                       ReverseToReverse = new PathDto[0][],
                       IsUnknown = true
                   };
        }
    }
}