using System.Collections.Generic;
using System.Linq;
using Selkie.Windsor;
using Selkie.WPF.ViewModels.Interfaces;

namespace Selkie.WPF.ViewModels.TrailHistory
{
    [ProjectComponent(Lifestyle.Transient)]
    public class DisplayHistoryRow : IDisplayHistoryRow
    {
        public DisplayHistoryRow(int interation,
                                 IEnumerable <int> trail,
                                 double length,
                                 double lengthDelta,
                                 double lengthDeltaInPercent,
                                 double alpha,
                                 double beta,
                                 double gamma,
                                 string type)
        {
            m_Interation = interation;
            m_Trail = trail.ToArray();
            m_TrailRaw = TrailToString(m_Trail);
            m_Length = length;
            m_LengthDelta = lengthDelta;
            m_LengthDeltaInPercent = lengthDeltaInPercent;
            m_Alpha = alpha;
            m_Beta = beta;
            m_Gamma = gamma;
            m_Type = type;
        }

        private readonly double m_Alpha;
        private readonly double m_Beta;
        private readonly double m_Gamma;
        private readonly int m_Interation;
        private readonly double m_Length;
        private readonly double m_LengthDelta;
        private readonly double m_LengthDeltaInPercent;
        private readonly IEnumerable <int> m_Trail;
        private readonly string m_TrailRaw;
        private readonly string m_Type;

        public double LengthDelta
        {
            get
            {
                return m_LengthDelta;
            }
        }

        public double LengthDeltaInPercent
        {
            get
            {
                return m_LengthDeltaInPercent;
            }
        }

        public int Interation
        {
            get
            {
                return m_Interation;
            }
        }

        public double Length
        {
            get
            {
                return m_Length;
            }
        }

        public IEnumerable <int> Trail
        {
            get
            {
                return m_Trail;
            }
        }

        public string TrailRaw
        {
            get
            {
                return m_TrailRaw;
            }
        }

        public double Alpha
        {
            get
            {
                return m_Alpha;
            }
        }

        public double Beta
        {
            get
            {
                return m_Beta;
            }
        }

        public double Gamma
        {
            get
            {
                return m_Gamma;
            }
        }

        public string Type
        {
            get
            {
                return m_Type;
            }
        }

        private string TrailToString(IEnumerable <int> trail)
        {
            string text = string.Join(",",
                                      trail);

            return text;
        }
    }
}