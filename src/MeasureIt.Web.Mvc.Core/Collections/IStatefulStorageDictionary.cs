using System.Collections.Generic;

namespace MeasureIt.Web.Mvc.Collections
{
    /// <summary>
    /// There were optimizations that took place with Mvc that consequently require some extra
    /// effort in order to do some sort of reasonable stateful caching.
    /// </summary>
    /// <see cref="!:http://channel9.msdn.com/series/mvcconf/mvcconf-2011-brad-wilson-advanced-mvc-3">
    /// Mr. Wilson purports to have "invented" stateful storage, and I certainly thank him for the
    /// inspiration. I did some refinements in order to better meet the needs of this project.</see>
    /// <see cref="!:http://bradwilson.typepad.com/blog/2010/07/aspnet-mvc-filters-and-statefulness.html"/>
    internal interface IStatefulStorageDictionary : IDictionary<string, object>
    {
    }
}
