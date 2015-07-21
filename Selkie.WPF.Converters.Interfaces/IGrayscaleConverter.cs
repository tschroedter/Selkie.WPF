using System.Collections.Generic;
using JetBrains.Annotations;

namespace Selkie.WPF.Converters.Interfaces
{
    public interface IGrayscaleConverter : IConverter
    {
        double Minimum { get; set; }
        double Maximum { get; set; }

        [NotNull]
        double[][] Pheromones { get; set; }

        List <List <int>> Grayscale { get; }
        int NumberOfPossibleValues { get; set; }
    }
}