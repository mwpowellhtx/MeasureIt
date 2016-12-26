using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReflectionExtensionMethods
    {
        /// <summary>
        /// Returns whether <paramref name="type"/> and <paramref name="otherType"/> are Related.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public static bool IsRelatedTo(this Type type, Type otherType)
        {
            return !(type == null || otherType == null)
                   && (type == otherType
                       || type.IsSubclassOf(otherType)
                       || otherType.IsSubclassOf(type));
        }

        /// <summary>
        /// Returns whether <paramref name="type"/> Has <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            return type.GetCustomAttribute<TAttribute>() != null;
        }

        /// <summary>
        /// Returns whether <paramref name="type"/> Has <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static bool HasAttribute<TAttribute>(this Type type, bool inherit)
        {
            return Attribute.GetCustomAttribute(type, typeof(TAttribute), inherit) != null;
        }

        /// <summary>
        /// Returns whether <paramref name="member"/> Has <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <remarks>May apply solely for <see cref="MethodInfo"/>, or maybe
        /// <see cref="PropertyInfo"/>, or even <see cref="FieldInfo"/>.</remarks>
        public static bool HasAttribute<TAttribute>(this MemberInfo member)
            where TAttribute : Attribute
        {
            return member.GetCustomAttribute<TAttribute>() != null;
        }

        /// <summary>
        /// Returns whether <paramref name="member"/> Has <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <remarks>May apply solely for <see cref="MethodInfo"/>, or maybe
        /// <see cref="PropertyInfo"/>, or even <see cref="FieldInfo"/>.</remarks>
        public static bool HasAttributes<TAttribute>(this MemberInfo member)
            where TAttribute : Attribute
        {
            return member.GetCustomAttributes<TAttribute>().Any();
        }

        /// <summary>
        /// Returns whether <paramref name="member"/> Has <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        /// <remarks>May apply solely for <see cref="MethodInfo"/>, or maybe
        /// <see cref="PropertyInfo"/>, or even <see cref="FieldInfo"/>.</remarks>
        public static bool HasAttributes<TAttribute>(this MemberInfo member, bool inherit)
            where TAttribute : Attribute
        {
            return Attribute.GetCustomAttributes(member, typeof(TAttribute), inherit).Any();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo member, bool inherit)
            where TAttribute : Attribute
        {
            return Attribute.GetCustomAttributes(member, typeof(TAttribute), inherit).OfType<TAttribute>();
        }

        /// <summary>
        /// Returns a <typeparamref name="TResult"/> based on the <paramref name="type"/>,
        /// <paramref name="getter"/>, and <paramref name="defaultValue"/>.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="type"></param>
        /// <param name="getter"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TResult GetAttributeValue<TAttribute, TResult>(this Type type,
            Func<TAttribute, TResult> getter, TResult defaultValue = default(TResult))
            where TAttribute : Attribute
        {
            if (type == null) return defaultValue;
            var attr = type.GetCustomAttribute<TAttribute>(true);
            return attr == null ? defaultValue : getter(attr);
        }

        /// <summary>
        /// Returns a <typeparamref name="TResult"/> based on the <paramref name="obj"/>,
        /// <paramref name="getter"/>, and <paramref name="defaultValue"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="obj"></param>
        /// <param name="getter"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TResult GetAttributeValue<T, TAttribute, TResult>(this T obj,
            Func<TAttribute, TResult> getter, TResult defaultValue = default(TResult))
            where TAttribute : Attribute
        {
            var type = typeof(T);
            var attr = type.GetCustomAttribute<TAttribute>();
            return attr == null ? defaultValue : getter(attr);
        }

        /// <summary>
        /// Returns a <typeparamref name="TResult"/> based on the <paramref name="type"/>,
        /// <paramref name="getter"/>.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="type"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> GetAttributeValues<TAttribute, TResult>(this Type type,
            Func<TAttribute, TResult> getter)
            where TAttribute : Attribute
        {
            if (type == null) yield break;
            var attrs = type.GetCustomAttributes<TAttribute>(true);
            foreach (var result in attrs.Select(getter))
                yield return result;
        }

        /// <summary>
        /// Returns the <typeparamref name="TResult"/> based on the <paramref name="member"/> and
        /// <paramref name="getter"/>.
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="member"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> GetAttributeValues<TMember, TAttribute, TResult>(
            this TMember member, Func<TAttribute, TResult> getter)
            where TMember : MemberInfo
            where TAttribute : Attribute
        {
            if (member == null) yield break;
            var attrs = member.GetCustomAttributes<TAttribute>(false);
            foreach (var result in attrs.Select(getter))
                yield return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="excluded"></param>
        /// <returns></returns>
        public static IEnumerable<T> Filter<T>(this ICollection<T> items,
            Func<IEnumerable<T>, T, bool> excluded)
        {
            var readOnlyItems = new ReadOnlyCollection<T>(items.ToList());
            return items.Where(x => !excluded(readOnlyItems.Where(y => !ReferenceEquals(y, x)), x));
        }

        internal static PropertyInfo GetProperty<T, TValue>(this T instance, Expression<Func<T, TValue>> property)
        {
            Expression body = property;

            if (body is LambdaExpression)
                body = ((LambdaExpression) body).Body;

            if (body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException("property invalid expression function", "property");

            return (PropertyInfo) ((MemberExpression) body).Member;
        }

        private const BindingFlags DefaultCreateInstanceBindingAttr = BindingFlags.Public | BindingFlags.Instance;

        internal static TResult CreateInstance<TResult>(this Type type
            , BindingFlags bindingAttr = DefaultCreateInstanceBindingAttr
            , params object[] args)
        {
            var resultType = typeof(TResult);

            if (!resultType.IsAssignableFrom(type))
            {
                var message = string.Format(@"Type '{0}' does not implement '{1}'.", type, resultType);
                throw new ArgumentException(message, "type");
            }

            var argTypes = args.Select(arg => arg.GetType()).ToArray();

            var ctor = type.GetConstructor(bindingAttr, Type.DefaultBinder, argTypes, null);

            return (TResult) ctor.Invoke(args);
        }
    }
}
