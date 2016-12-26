using System;
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
        private static bool Equals(ParameterInfo a, ParameterInfo b)
        {
            var paramArrayType = typeof(ParamArrayAttribute);

            return !(a == null || b == null)
                   && a.ParameterType == b.ParameterType
                   && a.ParameterType.IsByRef == b.ParameterType.IsByRef
                   && a.IsIn == b.IsIn
                   && a.IsOut == b.IsOut
                   && a.IsOptional == b.IsOptional
                   && a.HasDefaultValue == b.HasDefaultValue
                   && a.DefaultValue == b.DefaultValue
                   && Attribute.IsDefined(a, paramArrayType)
                   == Attribute.IsDefined(b, paramArrayType);
        }

        protected override bool TryEquals(MethodInfo x, MethodInfo y, out bool? result)
        {
            if (base.TryEquals(x, y, out result) && result != false)
                return true;

            var xParams = x.GetParameters();
            var yParams = y.GetParameters();

            // Must do a little bit more in depth comparison besides just the Type itself.
            result = x.ReturnType == y.ReturnType
                     && xParams.Length == yParams.Length
                     && xParams.Zip(yParams, Equals).All(zEq => zEq);

            return true;
        }

        public override int GetHashCode(MethodInfo obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
        }
    }
}
