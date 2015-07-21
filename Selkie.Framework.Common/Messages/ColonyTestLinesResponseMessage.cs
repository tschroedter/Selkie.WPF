using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Selkie.Framework.Common.Messages
{
    [ExcludeFromCodeCoverage]
    public class ColonyTestLinesResponseMessage
    {
        private IEnumerable <string> m_Types = new string[0];

        [NotNull]
        public IEnumerable <string> Types
        {
            get
            {
                return m_Types;
            }
            set
            {
                m_Types = value;
            }
        }
    }
}