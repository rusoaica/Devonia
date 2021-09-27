/// Written by: Yulia Danilova
/// Creation Date: 12th of December, 2019
/// Purpose: Base class for MVVM pattern
#region ========================================================================= USING =====================================================================================
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

namespace Devonia.ViewModels.Common.MVVM
{
    public class LightBaseModel : INotifyPropertyChanged
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Notifies subscribers about a property's value being changed
        /// </summary>
        /// <param name="propName">The property that had the value changed</param>
        public virtual void Notify([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}
