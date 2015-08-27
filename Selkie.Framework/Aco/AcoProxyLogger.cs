using System.Text;
using JetBrains.Annotations;
using Selkie.Framework.Interfaces.Aco;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;

namespace Selkie.Framework.Aco
{
    [ProjectComponent(Lifestyle.Transient)]
    public class AcoProxyLogger : IAcoProxyLogger
    {
        private readonly ISelkieLogger m_Logger;

        public AcoProxyLogger([NotNull] ISelkieLogger logger)
        {
            m_Logger = logger;
        }

        public void Error(string text)
        {
            m_Logger.Error(text);
        }

        public void Info(string text)
        {
            m_Logger.Info(text);
        }

        public void LogCostPerLine(int[] costPerLine)
        {
            string text = string.Join(",",
                                      costPerLine);

            string message = "CostPerLine: {0}".Inject(text);

            m_Logger.Info(message);
        }

        public void LogCostMatrix(int[][] matrix)
        {
            if ( matrix.Length == 0 )
            {
                m_Logger.Info("CostMatrix: { }");

                return;
            }

            var builder = new StringBuilder();

            builder.AppendLine("CostMatrix");

            var i = 0;

            foreach ( int[] costArray in matrix )
            {
                string text = string.Join(",",
                                          costArray);

                builder.AppendLine("[{0}] {1}".Inject(i++,
                                                      text));
            }

            m_Logger.Info(builder.ToString());
        }
    }
}