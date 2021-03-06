﻿using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;

namespace Selkie.Framework.Common.Messages
{
    [ExcludeFromCodeCoverage]
    public class ColonyRacetracksResponseMessage
    {
        [CanBeNull]
        public IRacetracks Racetracks { get; set; }
    }
}