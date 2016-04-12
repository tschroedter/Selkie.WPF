namespace Selkie.Framework.Messages
{
    public class CostMatrixCalculatedMessage
    {
        public int[][] Matrix { get; set; }
        public int[] CostPerLine { get; set; }
    }
}