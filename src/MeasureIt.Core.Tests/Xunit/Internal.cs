using System;

// ReSharper disable once CheckNamespace
namespace Xunit
{
    public delegate bool TryParseDelegate<in T, TResult>(T value, out TResult result);

    public static class Internal
    {
        public static Type VerifySubclassOf(this Type derivedType, Type baseType)
        {
            Assert.NotNull(derivedType);
            Assert.NotNull(baseType);
            Assert.True(derivedType.IsSubclassOf(baseType));
            return derivedType;
        }

        public static Type VerifyUnrelatedType(this Type firstType, Type secondType)
        {
            Assert.NotNull(firstType);
            Assert.NotNull(secondType);
            Assert.False(firstType.IsSubclassOf(secondType));
            Assert.False(secondType.IsSubclassOf(firstType));
            return firstType;
        }

        public static bool Parse<T, TResult>(this T value, TryParseDelegate<T, TResult> tryParse)
        {
            TResult result;
            return tryParse(value, out result);
        }

        public static void CanParse<T, TResult>(this T value, TryParseDelegate<T, TResult> tryParse)
        {
            Assert.True(Parse(value, tryParse));
        }

        public static void DoesNotParse<T, TResult>(this T value, TryParseDelegate<T, TResult> tryParse)
        {
            Assert.False(Parse(value, tryParse));
        }

        public static void Confirm<T>(this Type type)
        {
            type.Confirm(typeof(T));
        }

        public static void Confirm(this Type type, Type expectedType)
        {
            Assert.NotNull(expectedType);
            Assert.NotNull(type);
            Assert.Equal(expectedType, type);
        }
    }
}
