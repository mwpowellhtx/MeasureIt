namespace MeasureIt
{
    /// <summary>
    /// Named Disposable class.
    /// </summary>
    public abstract class NamedDisposable : Disposable, INamedDisposable
    {

        private IMoniker _moniker;

        /// <summary>
        /// Sets the Moniker.
        /// </summary>
        protected virtual IMoniker Moniker
        {
            private get { return _moniker; }
            set { _moniker = value ?? DefaultMoniker.New(); }
        }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public virtual string Name
        {
            get { return _moniker.ToString(); }
            set { Moniker = string.IsNullOrEmpty(value) ? null : new NameMoniker(value); }
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        protected NamedDisposable()
            : this(null)
        {
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="name"></param>
        protected NamedDisposable(string name)
        {
            Initialize(name);
        }

        private void Initialize(string name)
        {
            Name = name;
        }
    }
}
