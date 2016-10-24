using System;
using System.Collections.Generic;

namespace MeasureIt
{
    //using IRequestContextBag = IDictionary<string, object>;

    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceCounterAdapterDescriptor : IDescriptor
    {
        /// <summary>
        /// Gets or sets the <see cref="CounterName"/>.
        /// </summary>
        string CounterName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CounterHelp"/>.
        /// </summary>
        string CounterHelp { get; set; }

        /// <summary>
        /// Gets or sets the AdapterType.
        /// </summary>
        Type AdapterType { get; set; }

        /// <summary>
        /// Gets the CreationDataDescriptors.
        /// </summary>
        IEnumerable<ICounterCreationDataDescriptor> CreationDataDescriptors { get; }

        // TODO: TBD: the above is fine for now; remember at this level this is METADATA ONLY, once removed from Attributes
        // TODO: TBD: the below starts to lend itself to a more active, possibly disposable, context

        ///// <summary>
        ///// Gets the CreationData.
        ///// </summary>
        //IReadOnlyCollection<CounterCreationData> CreationData { get; }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="contextBag"></param>
        //void OnRequestStarting(IRequestContextBag contextBag);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="contextBag"></param>
        //void OnRequestEnding(IRequestContextBag contextBag);
    }
}
