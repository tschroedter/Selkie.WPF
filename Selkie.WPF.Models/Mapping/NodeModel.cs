using Selkie.Geometry.Primitives;
using Selkie.WPF.Common.Interfaces;

namespace Selkie.WPF.Models.Mapping
{
    public class NodeModel : INodeModel
    {
        internal const int UnknownId = -1;

        internal static INodeModel Unknown = new NodeModel(UnknownId,
                                                           0.0,
                                                           0.0,
                                                           Angle.ForZeroDegrees);

        private readonly Angle m_DirectionAngle;
        private readonly int m_Id;
        private readonly double m_X;
        private readonly double m_Y;

        public NodeModel(int id,
                         double x,
                         double y,
                         Angle directionAngle)
        {
            m_Id = id;
            m_X = x;
            m_Y = y;
            m_DirectionAngle = directionAngle;
        }

        public int Id
        {
            get
            {
                return m_Id;
            }
        }

        public double X
        {
            get
            {
                return m_X;
            }
        }

        public double Y
        {
            get
            {
                return m_Y;
            }
        }

        public Angle DirectionAngle
        {
            get
            {
                return m_DirectionAngle;
            }
        }

        public bool IsUnknown
        {
            get
            {
                return Id == UnknownId;
            }
        }
    }
}