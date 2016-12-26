using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    using Xunit;

    internal static class BoilerplateExtensiomMethods
    {
        internal static void VerifyCreationData(IEnumerable<CounterCreationData> data,
            params Action<CounterCreationData>[] verification)
        {
            verification = verification.Select(v => v ?? (d => { })).ToArray();
            Assert.NotNull(data);
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.Collection(data, verification);
        }
    }
}
