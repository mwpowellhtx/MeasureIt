using System.Collections.Generic;

namespace MeasureIt.Collections.Generic
{
    /// <summary>
    /// 
    /// </summary>
    internal interface IBidirectionalList<T> : IList<T>
    {
    }

    internal static class GenericExtensionMethods
    {
        internal static IBidirectionalList<T> ToBidirectionalList<T>(
            this IList<T> list
            , AfterOperationDelegate<T> onBeforeAdded = null
            , AfterOperationDelegate<T> onBeforeRemoved = null
            , BeforeOperationDelegate<T> onAfterAdded = null
            , BeforeOperationDelegate<T> onAfterRemoved = null
            )
        {
            return new BidirectionalList<T>(list, onBeforeAdded, onBeforeRemoved, onAfterAdded, onAfterRemoved);
        }
    }
}
