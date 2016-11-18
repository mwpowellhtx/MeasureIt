using System.Collections.Generic;
using System.Reflection;

namespace MeasureIt
{
    /// <summary>
    /// Provides a general use <see cref="MemberInfo"/> based <typeparamref name="TMember"/>
    /// comparison visitor.
    /// </summary>
    /// <remarks>This comparer is general use enough that it could be re-factored into a shared
    /// assembly, or even assembly external to the project. But for now, will leave it as an
    /// internal resource.</remarks>
    internal abstract class MemberInfoEqualityComparer<TMember> : EqualityComparer<TMember>
        where TMember : MemberInfo
    {
        protected virtual bool TryEquals(TMember x, TMember y, out bool? result)
        {
            result = null;

            if (x == null || y == null) result = false;
            else if (ReferenceEquals(x, y)) result = true;

            return result != null;
        }

        public sealed override bool Equals(TMember x, TMember y)
        {
            bool? result;

            if (TryEquals(x, y, out result)) return result ?? false;

            return true;
        }
    }
}
