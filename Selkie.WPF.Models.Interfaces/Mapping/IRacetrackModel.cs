using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;

namespace Selkie.WPF.Models.Interfaces.Mapping
{
    public interface IRacetrackModel : IModel
    {
        [NotNull]
        IEnumerable <IPath> Paths { get; }
    }
}