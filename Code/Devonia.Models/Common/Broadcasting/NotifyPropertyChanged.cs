/// Written by: Yulia Danilova
/// Creation Date: 13th of July, 2021
/// Purpose: Broadcasts an event about a property value being changed
#region ========================================================================= USING =====================================================================================
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Devonia.Models.Common.Broadcasting
{
    public class NotifyPropertyChanged
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        public event Action<string> PropertyChanged;
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Notifies the UI about a binded property's value being changed
        /// </summary>
        /// <param name="propertyName">The property that had the value changed</param>
        public void Notify([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(propertyName);
        }
        #endregion
    }
}
