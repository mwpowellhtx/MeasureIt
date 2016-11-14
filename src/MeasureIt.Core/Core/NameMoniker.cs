namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    internal class NameMoniker : MonikerBase, INameMoniker
    {
        private string _name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public NameMoniker(string name)
        {
            Initialize(name);
        }

        private NameMoniker(NameMoniker other)
        {
            Copy(other);
        }

        private void Initialize(string name)
        {
            _name = name;
        }

        private void Copy(NameMoniker other)
        {
            _name = other._name;
        }

        public override string ToString()
        {
            return _name;
        }

        public override object Clone()
        {
            return new NameMoniker(this);
        }
    }
}
