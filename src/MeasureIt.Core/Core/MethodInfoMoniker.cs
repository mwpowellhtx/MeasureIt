using System.Reflection;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    internal class MethodInfoMoniker : MonikerBase
    {
        private MethodInfo _method;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        public MethodInfoMoniker(MethodInfo method)
        {
            _method = method;
        }

        private MethodInfoMoniker(MethodInfoMoniker other)
        {
            Copy(other);
        }

        public override string ToString()
        {
            return _method.Name;
        }

        private void Copy(MethodInfoMoniker other)
        {
            _method = other._method;
        }

        public override object Clone()
        {
            return new MethodInfoMoniker(this);
        }
    }
}
