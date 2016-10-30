using System;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    internal class DefaultMoniker : MonikerBase, IDefaultMoniker
    {
        public Guid Id { get; private set; }

        public static IMoniker New()
        {
            return new DefaultMoniker();
        }

        /// <summary>
        /// 
        /// </summary>
        public DefaultMoniker()
        {
            Initialize();
        }

        private void Initialize()
        {
            do
            {
                Id = Guid.NewGuid();
            } while (Id.ToByteArray().ElementAt(3) < 0xa0);
        }

        public override string ToString()
        {
            return Id.ToString("N");
        }
    }
}
