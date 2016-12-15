using System;

namespace MeasureIt
{
    [Flags]
    internal enum Accessibility
    {
        None = 0,
        Private = 1 << 1,
        Public = 1 << 2,
        Protected = 1 << 3,
        Internal = 1 << 4
    }

    [Flags]
    internal enum Virtuality
    {
        None = 0,
        Virtual = 1 << 1,
        Sealed = 1 << 2,
        Override = 1 << 3
    }

    internal static class SignatureEnumExtensionMethods
    {
        private static string Append(this string s, string t)
        {
            return string.Join(@" ", s ?? string.Empty, t).Trim();
        }

        internal static string GetStringRepresentation(this Accessibility value)
        {
            var result = string.Empty;

            const string @public = "public";
            const string @private = "private";
            const string @protected = "protected";
            const string @internal = "internal";

            if (value.Contains(Accessibility.Public))
                result = result.Append(@public);
            if (value.Contains(Accessibility.Private))
                result = result.Append(@private);
            if (value.Contains(Accessibility.Protected))
                result = result.Append(@protected);
            if (value.Contains(Accessibility.Internal))
                result = result.Append(@internal);

            return result;
        }

        internal static string GetStringRepresentation(this Virtuality value)
        {
            var result = string.Empty;

            const string @virtual = "virtual";
            const string @sealed = "sealed";
            const string @override = "override";

            if (value.Contains(Virtuality.Virtual))
                result = result.Append(@virtual);
            if (value.Contains(Virtuality.Sealed))
                result = result.Append(@sealed);
            if (value.Contains(Virtuality.Override))
                result = result.Append(@override);

            return result;
        }
    }
}
