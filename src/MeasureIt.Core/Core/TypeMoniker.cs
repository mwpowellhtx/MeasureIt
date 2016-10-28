using System;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    internal class TypeMoniker : MonikerBase, ITypeMoniker
    {
        private readonly Type _type;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public TypeMoniker(Type type)
        {
            _type = type;
        }

        public override string ToString()
        {
            return _type.FullName;
        }
    }
}
