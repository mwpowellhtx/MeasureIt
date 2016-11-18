using System;

namespace MeasureIt
{
    internal class TypeEqualityComparer : MemberInfoEqualityComparer<Type>
    {
        protected override bool TryEquals(Type x, Type y, out bool? result)
        {
            if (base.TryEquals(x, y, out result) && result == false)
                return true;

            result = x == y;

            return true;
        }

        public override int GetHashCode(Type obj)
        {
            return obj == null
                ? 0
                : obj.GetHashCode();
        }
    }
}
