using System;

namespace MeasureIt
{
    internal class ParameterDescriptor
    {
        public ParameterDescriptor(Type parameterType, string name)
        {
            ParameterType = parameterType;
            Name = name;
        }

        public Type ParameterType { get; private set; }

        public string Name { get; private set; }

        public static ParameterDescriptor Create(Type parameterType, string name)
        {
            return new ParameterDescriptor(parameterType, name);
        }

        public static ParameterDescriptor Create<TParameter>(string name)
        {
            return Create(typeof(TParameter), name);
        }
    }
}
