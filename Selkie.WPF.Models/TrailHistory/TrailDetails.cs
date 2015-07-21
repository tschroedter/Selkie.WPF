using System.Collections.Generic;
using Selkie.Windsor;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models.TrailHistory
{
    [ProjectComponent(Lifestyle.Transient)]
    public class TrailDetails : ITrailDetails
    {
        public static ITrailDetails Unknown = new TrailDetails(int.MaxValue,
                                                               new int[]
                                                               {
                                                               },
                                                               double.PositiveInfinity,
                                                               double.PositiveInfinity,
                                                               double.PositiveInfinity,
                                                               "Unknown",
                                                               double.PositiveInfinity,
                                                               double.PositiveInfinity,
                                                               double.PositiveInfinity);

        private readonly double m_Alpha;
        private readonly double m_Beta;
        private readonly double m_Gamma;
        private readonly int m_Interation;
        private readonly double m_Length;
        private readonly double m_LengthDelta;
        private readonly double m_LengthDeltaInPercent;
        private readonly IEnumerable <int> m_Trail;
        private readonly string m_Type;

        public TrailDetails(int interation,
                            IEnumerable <int> trail,
                            double length,
                            double lengthDelta,
                            double lengthDeltaInPercent,
                            string type,
                            double alpha,
                            double beta,
                            double gamma)
        {
            m_Interation = interation;
            m_Trail = trail;
            m_Length = length;
            m_LengthDelta = lengthDelta;
            m_LengthDeltaInPercent = lengthDeltaInPercent;
            m_Type = type;
            m_Alpha = alpha;
            m_Beta = beta;
            m_Gamma = gamma;
        }

        public int Interation
        {
            get
            {
                return m_Interation;
            }
        }

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

        public bool IsUnknown
        {
            get
            {
                return double.IsInfinity(m_Length);
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

        public string Type
        {
            get
            {
                return m_Type;
            }
        }
    }
}