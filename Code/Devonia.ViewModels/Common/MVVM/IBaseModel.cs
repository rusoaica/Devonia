/// Written by: Yulia Danilova
/// Creation Date: 10th of June, 2021
/// Purpose: Interface for the common functionality of all view models
#region ========================================================================= USING =====================================================================================
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

namespace Devonia.ViewModels.Common.MVVM
{
    public interface IBaseModel : INotifyPropertyChanged
    {
        #region ================================================================ PROPERTIES =================================================================================
        string WindowTitle { get; set; }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Notifies the UI about a binded property's value being changed
        /// </summary>
        /// <param name="propertyName">The property that had the value changed</param>
        void Notify([CallerMemberName] string propertyName = null);
        #endregion
    }
}
