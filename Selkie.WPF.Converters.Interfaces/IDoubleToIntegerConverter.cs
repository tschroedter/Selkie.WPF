namespace Selkie.WPF.Converters.Interfaces
{
    public interface IDoubleToIntegerConverter : IConverter
    {
        int NumberOfPossibleValues { get; set; }
        double Minimum { get; set; }
        double Interval { get; set; }
        double Value { get; set; }
        int Integer { get; }
    }
}