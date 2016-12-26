using System.Threading.Tasks;

namespace MeasureIt.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInstallerContext : IContext
    {
        /// <summary>
        /// 
        /// </summary>
        void Install();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task InstallAsync();

        /// <summary>
        /// 
        /// </summary>
        void Uninstall();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task UninstallAsync();
    }
}
