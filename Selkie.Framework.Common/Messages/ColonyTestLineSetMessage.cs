using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Selkie.Framework.Common.Messages
{
    [ExcludeFromCodeCoverage]
    public class ColonyTestLineSetMessage
    {
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

        private string m_Type = string.Empty;
    }
}