using System;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    internal class TypeMoniker : MonikerBase, ITypeMoniker
    {
        private Type _type;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public TypeMoniker(Type type)
        {
            Initialize(type);
        }

        private TypeMoniker(TypeMoniker other)
        {
            Copy(this);
        }

        private void Initialize(Type type)
        {
            _type = type;
        }

        private void Copy(TypeMoniker other)
        {
            _type = other._type;
        }

        public override string ToString()
        {
            return _type.FullName;
        }

        public override object Clone()
        {
            return new TypeMoniker(this);
        }
    }
}
