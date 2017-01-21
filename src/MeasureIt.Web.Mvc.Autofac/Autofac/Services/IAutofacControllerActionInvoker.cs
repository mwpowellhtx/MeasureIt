using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace MeasureIt.Web.Mvc.Autofac
{
    using Kingdom.Web.Mvc;
    using global::Autofac;
    using static BindingFlags;

    /// <summary>
    /// <see cref="IActionInvoker"/> for use with <see cref="IContainer"/>.
    /// </summary>
    public interface IAutofacControllerActionInvoker : IActionInvoker
    {
    }

    internal static class InternalExtensionMethods
    {
        internal static bool IsAssignableTo<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        /// <summary>
        /// <see cref="Public"/>, <see cref="NonPublic"/>, <see cref="Instance"/>
        /// </summary>
        private const BindingFlags PublicNonPublicInstance = Public | NonPublic | Instance;

        /// <summary>
        /// Injects the <paramref name="obj"/> corresponding with the <see cref="Attribute"/>.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        /// <param name="baseFlags"></param>
        /// <returns></returns>
        internal static TContext InjectObject<TContext>(this TContext context, object obj,
            BindingFlags baseFlags = PublicNonPublicInstance)
            where TContext : IComponentContext
        {
            // TODO: should be InjectAttribute... or derivation of IInjectionAttribute ...
            return context.InjectObject<TContext, InjectAttribute>(obj, baseFlags);
        }

        /// <summary>
        /// Injects the <paramref name="obj"/> corresponding with the
        /// <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        /// <param name="baseFlags"></param>
        /// <returns></returns>
        internal static TContext InjectObject<TContext, TAttribute>(this TContext context, object obj,
            BindingFlags baseFlags = PublicNonPublicInstance)
            where TContext : IComponentContext
            where TAttribute : Attribute, IInjectionAttribute
        {
            if (obj == null) return context;

            var objType = obj.GetType();

            {
                var properties = objType.GetProperties(baseFlags | SetProperty);

                foreach (var property in properties.Where(p => p.HasAttribute<TAttribute>()))
                {
                    property.SetValue(obj, context.Resolve(property.PropertyType));
                }
            }

            {
                var fields = objType.GetFields(baseFlags | SetField);

                foreach (var field in fields.Where(f => f.HasAttribute<TAttribute>()))
                {
                    field.SetValue(obj, context.Resolve(field.FieldType));
                }
            }

            return context;
        }
    }
}
