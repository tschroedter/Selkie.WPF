namespace Selkie.WPF.Models.Common.Messages
{
    public class ControlModelChangedMessage
    {
        public bool IsRunning { get; set; }
        public bool IsFinished { get; set; }
        public bool IsApplying { get; set; }
    }
}