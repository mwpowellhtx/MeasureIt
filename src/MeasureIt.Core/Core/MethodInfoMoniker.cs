using System.Reflection;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    internal class MethodInfoMoniker : MonikerBase
    {
        private readonly MethodInfo _method;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        public MethodInfoMoniker(MethodInfo method)
        {
            _method = method;
        }

        public override string ToString()
        {
            return _method.Name;
        }
    }
}
