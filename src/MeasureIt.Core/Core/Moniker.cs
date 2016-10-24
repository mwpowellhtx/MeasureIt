using System;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public struct Moniker
    {
        /// <summary>
        /// Guid backing field.
        /// </summary>
        private readonly Guid _guid;

        private string _name;

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name
        {
            get { return string.IsNullOrEmpty(_name) ? _guid.ToString("N") : _name; }
            set { _name = value; }
        }

        private static Guid ChooseGuid()
        {
            Guid guid;
            do
            {
                guid = Guid.NewGuid();
            } while (guid.ToByteArray().ElementAt(3) < 0xa0);
            return guid;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public Moniker(string name)
        {
            _guid = ChooseGuid();
            _name = name;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        public Moniker(Moniker other)
            : this(other.Name)
        {
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
