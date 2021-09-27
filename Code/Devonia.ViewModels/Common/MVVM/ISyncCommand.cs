/// Written by: Yulia Danilova
/// Creation Date: 24th of October, 2019
/// Purpose: Asynchronous non-generic and generic implementation of ICommand interface
#region ========================================================================= USING =====================================================================================
using System.Windows.Input;
#endregion

namespace Devonia.ViewModels.Common.MVVM
{
    public interface ISyncCommand : ICommand
    {
        #region ================================================================ METHODS ====================================================================================
        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        void ExecuteSync();

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state
        /// </summary>
        /// <returns>True if this command can be executed; otherwise, False.</returns>
        bool CanExecute();

        /// <summary>
        /// Executes the delegate that signals changes in permissions of execution of the command
        /// </summary>
        void RaiseCanExecuteChanged();
        #endregion
    }

    public interface ISyncCommand<T> : ICommand
    {
        #region ================================================================ METHODS ====================================================================================
        /// <summary>
        /// Defines the generic method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Generic parameter passed to the command</param>
        void ExecuteSync(T parameter);

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state
        /// </summary>
        /// <param name="parameter">Generic parameter passed to the command</param>
        /// <returns>True if this command can be executed; otherwise, False.</returns>
        bool CanExecute(T parameter);
        #endregion
    }
}
