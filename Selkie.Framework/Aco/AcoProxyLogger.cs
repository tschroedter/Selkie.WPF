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
        public AcoProxyLogger([NotNull] ISelkieLogger logger)
        {
            m_Logger = logger;
        }

        private readonly ISelkieLogger m_Logger;

        public void Error(string text)
        {
            m_Logger.Error(text);
        }

        public void Info(string text)
        {
            m_Logger.Info(text);
        }

        public void LogCostPerFeature(int[] costPerFeature)
        {
            string text = string.Join(",",
                                      costPerFeature);

            string message = "CostPerFeature: {0}".Inject(text);

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