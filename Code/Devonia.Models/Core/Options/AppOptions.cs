/// Written by: Yulia Danilova
/// Creation Date: 28th of August, 2021
/// Purpose: Business model for application options

namespace Devonia.Models.Core.Options
{
    public class AppOptions : IAppOptions
    {
        #region ================================================================ PROPERTIES =================================================================================
        public IOptionsInterface OptionsInterface { get; }
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="optionsInterface">The injected user interface options</param>
        public AppOptions(IOptionsInterface optionsInterface)
        {
            OptionsInterface = optionsInterface;
        }
        #endregion
    }
}