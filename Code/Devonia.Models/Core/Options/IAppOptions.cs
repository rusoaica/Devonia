/// Written by: Yulia Danilova
/// Creation Date: 28th of August, 2021
/// Purpose: Interface business model for application options

namespace Devonia.Models.Core.Options
{
    public interface IAppOptions
    {
        #region ================================================================ PROPERTIES =================================================================================
        IOptionsInterface OptionsInterface { get; }
        #endregion
    }
}
