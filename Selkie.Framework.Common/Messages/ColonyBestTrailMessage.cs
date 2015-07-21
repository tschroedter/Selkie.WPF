using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Selkie.Framework.Common.Messages
{
    [ExcludeFromCodeCoverage]
    public class ColonyBestTrailMessage
    {
        private IEnumerable <int> m_Trail = new int[0];
        private string m_Type = string.Empty;
        public double Alpha { get; set; }
        public double Beta { get; set; }
        public double Gamma { get; set; }
        public int Iteration { get; set; }
        public double Length { get; set; }

        [NotNull]
        public IEnumerable <int> Trail
        {
            get
            {
                return m_Trail;
            }
            set
            {
                m_Trail = value;
            }
        }

        [NotNull]
        public string Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
        }
    }
}