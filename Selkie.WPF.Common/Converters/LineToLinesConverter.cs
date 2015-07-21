using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces;
using Selkie.Geometry.Shapes;
using Selkie.WPF.Common.Interfaces.Converters;
using WPF.Common.Converters;

namespace Selkie.WPF.Common.Converters
{
    public class LineToLinesConverter : ILineToLinesConverter
    {
        private readonly IBaseCostCalculator[] m_CostCalculators;
        private readonly ICostEndToEndCalculator m_CostEndToEndCalculator;
        private readonly ICostEndToStartCalculator m_CostEndToStartCalculator;
        private readonly ICostStartToEndCalculator m_CostStartToEndCalculator;
        private readonly ICostStartToStartCalculator m_CostStartToStartCalculator;
        private ILine m_Line = Geometry.Shapes.Line.Unknown;
        private IEnumerable<ILine> m_Lines = new ILine[] {};
        private IRacetracks m_Racetracks;

        public LineToLinesConverter([NotNull] ICostStartToStartCalculator costStartToStartCalculator,
                                    [NotNull] ICostStartToEndCalculator costStartToEndCalculator,
                                    [NotNull] ICostEndToStartCalculator costEndToStartCalculator,
                                    [NotNull] ICostEndToEndCalculator costEndToEndCalculator)
        {
            m_CostStartToStartCalculator = costStartToStartCalculator;
            m_CostStartToEndCalculator = costStartToEndCalculator;
            m_CostEndToStartCalculator = costEndToStartCalculator;
            m_CostEndToEndCalculator = costEndToEndCalculator;

            m_CostCalculators = new IBaseCostCalculator[]
                                {
                                    m_CostStartToStartCalculator,
                                    m_CostStartToEndCalculator,
                                    m_CostEndToStartCalculator,
                                    m_CostEndToEndCalculator
                                };
        }

        #region ILineToLinesConverter Members

        public ILine Line
        {
            get { return m_Line; }
            set { m_Line = value; }
        }

        public IEnumerable<ILine> Lines
        {
            get { return m_Lines; }
            set { m_Lines = value; }
        }

        public IRacetracks Racetracks
        {
            get { return m_Racetracks; }
            set { m_Racetracks = value; }
        }

        public double BaseCost
        {
            get { return m_Line.Length; }
        }

        public double CostForwardForward(ILine other)
        {
            return CostEndToStart(other);
        }

        public double CostForwardReverse(ILine other)
        {
            return CostEndToEnd(other);
        }

        public double CostReverseForward(ILine to)
        {
            return CostStartToStart(to);
        }

        public double CostReverseReverse(ILine to)
        {
            return CostStartToEnd(to);
        }

        public double CostStartToStart(ILine to)
        {
            double costToOther = m_CostStartToStartCalculator.Costs[to.Id];

            return CalculateTotalCost(m_Line.Length,
                                      costToOther);
        }

        public double CostStartToEnd(ILine to)
        {
            double costToOther = m_CostStartToEndCalculator.Costs[to.Id];

            return CalculateTotalCost(m_Line.Length,
                                      costToOther);
        }

        public double CostEndToStart(ILine other)
        {
            double costToOther = m_CostEndToStartCalculator.Costs[other.Id];

            return CalculateTotalCost(m_Line.Length,
                                      costToOther);
        }

        public double CostEndToEnd(ILine other)
        {
            double costToOther = m_CostEndToEndCalculator.Costs[other.Id];

            return CalculateTotalCost(m_Line.Length,
                                      costToOther);
        }

        public void Convert()
        {
            foreach (IBaseCostCalculator baseCostCalculator in m_CostCalculators)
            {
                baseCostCalculator.Line = m_Line;
                baseCostCalculator.Lines = m_Lines;
                baseCostCalculator.Racetracks = m_Racetracks;
                baseCostCalculator.Calculate();
            }
        }

        internal double CalculateTotalCost(double lineLength,
                                           double costToOther)
        {
            if (!IsCostValid(costToOther))
            {
                return double.MaxValue;
            }

            double cost = lineLength + costToOther;

            return cost;
        }

        internal bool IsCostValid(double costToOther)
        {
            return costToOther >= 0.1 &&
                   !IsCostToMySelf(costToOther);
        }

        internal bool IsCostToMySelf(double costToOther)
        {
            return Math.Abs(costToOther - CostMatrix.CostToMyself) < 0.1;
        }

        #endregion
    }
}