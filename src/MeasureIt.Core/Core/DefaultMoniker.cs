using System;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    internal class DefaultMoniker : MonikerBase, IDefaultMoniker
    {
        public static IMoniker New()
        {
            return new DefaultMoniker();
        }

        private Guid _guid;

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
                _guid = Guid.NewGuid();
            } while (_guid.ToByteArray().ElementAt(3) < 0xa0);
        }

        public override string ToString()
        {
            return _guid.ToString("N");
        }
    }
}
