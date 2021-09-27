/// Written by: Yulia Danilova
/// Creation Date: 10th of June, 2021
/// Purpose: Base interface for all views
#region ========================================================================= USING =====================================================================================
using System.Threading.Tasks;
#endregion

namespace Devonia.ViewModels.Common.ViewFactory
{
    public interface IView
    {
        #region ================================================================ PROPERTIES =================================================================================
        dynamic DataContext { get; set; }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Opens a new view and returns without waiting for the newly opened view to close
        /// </summary>
        void Show();

        /// <summary>
        /// Opens a view and returns only when the newly opened view is closed.
        /// </summary>
        /// <returns>A nullable bool that specifies whether the activity was accepted (true) or canceled (false). 
        /// The return value is the value of the DialogResult property before a window closes</returns>
        Task<bool?> ShowDialogAsync();
        #endregion
    }
}
