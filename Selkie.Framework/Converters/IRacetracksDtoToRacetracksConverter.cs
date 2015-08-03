﻿using JetBrains.Annotations;
using Selkie.Framework.Interfaces;
using Selkie.Services.Racetracks.Common.Dto;

namespace Selkie.Framework.Converters
{
    public interface IRacetracksDtoToRacetracksConverter
    {
        [NotNull]
        RacetracksDto Dto { get; set; }

        [NotNull]
        IRacetracks Racetracks { get; }

        void Convert();
    }
}