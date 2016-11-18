using System.Linq;
using System.Reflection;

namespace MeasureIt
{
    /// <summary>
    /// Provides a general use <see cref="MethodInfo"/> comparison visitor.
    /// </summary>
    /// <remarks>This comparer is general use enough that it could be re-factored into a shared
    /// assembly, or even assembly external to the project. But for now, will leave it as an
    /// internal resource.</remarks>
    internal class MethodInfoEqualityComparer : MemberInfoEqualityComparer<MethodInfo>
    {
        protected override bool TryEquals(MethodInfo x, MethodInfo y, out bool? result)
        {
            if (base.TryEquals(x, y, out result) && result != false)
                return true;

            var xParams = x.GetParameters();
            var yParams = y.GetParameters();

            result = x.ReturnType == y.ReturnType
                     && xParams.Length == yParams.Length
                     && xParams.Zip(yParams, (a, b) => new {a, b})
                         .All(z => z.a.ParameterType == z.b.ParameterType);

            return true;
        }

        public override int GetHashCode(MethodInfo obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
        }
    }
}
