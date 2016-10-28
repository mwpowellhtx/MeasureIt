namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    internal class NameMoniker : MonikerBase, INameMoniker
    {
        private readonly string _name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public NameMoniker(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
