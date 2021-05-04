using System.Collections.Generic;
using Gehtsoft.PDFFlowLib.Xml.Actions;

namespace Gehtsoft.PDFFlowLib.Xml.Test
{
    internal static class CallActionExtension
    {
        internal static CallActionAssertions Should(this CallAction action) => new CallActionAssertions(action);
        internal static CallActionQueueAssertions Should(this IReadOnlyCollection<CallAction> action) => new CallActionQueueAssertions(action);
    }
}
