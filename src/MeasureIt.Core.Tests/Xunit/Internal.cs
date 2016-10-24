using System;

// ReSharper disable once CheckNamespace
namespace Xunit
{
    public delegate bool TryParseDelegate<in T, TResult>(T value, out TResult result);

    public static class Internal
    {
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
