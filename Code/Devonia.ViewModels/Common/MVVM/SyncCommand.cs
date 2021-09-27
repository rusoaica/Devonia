/// Written by: Yulia Danilova
/// Creation Date: 24th of October, 2019
/// Purpose: Implementation of ICommand and generic ISyncCommand interfaces
#region ========================================================================= USING =====================================================================================
using System;
using System.Windows.Input;
#endregion

namespace Devonia.ViewModels.Common.MVVM
{
    public class SyncCommand : ISyncCommand
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        public event EventHandler CanExecuteChanged;

        private bool isExecuting;
        private readonly Action executeSync;
        private readonly Func<bool> canExecute;
        #endregion

        #region ================================================================== CTOR =====================================================================================  
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="execute">The async Task method to be executed</param>
        /// <param name="canExecute">The method indicating whether the <paramref name="execute"/>can be executed</param>
        public SyncCommand(Action execute, Func<bool> canExecute = null)
        {
            executeSync = execute;
            this.canExecute = canExecute;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Indicates whether the command can be executed
        /// </summary>
        /// <returns>True if the command can be executed; False otherwise.</returns>
        public bool CanExecute()
        {
            return !isExecuting && (canExecute?.Invoke() ?? true);
        }

        /// <summary>
        /// Executes a delegate synchronously
        /// </summary>
        public void ExecuteSync()
        {
            if (CanExecute())
            {
                try
                {
                    isExecuting = true;
                    executeSync();
                }
                finally
                {
                    isExecuting = false;
                }
            }
            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Executes the delegate that signals changes in permissions of execution of the command
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.
        /// Also, see the parameter version <seealso cref="AsyncCommand{T}.CanExecute(T)">AsyncCommand{T}.CanExecute(T)</seealso></param>
        /// <returns>True if this command can be executed; otherwise, False.</returns>
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.
        /// Also, see the parameter version <seealso cref="AsyncCommand{T}.CanExecute(T)">AsyncCommand{T}.CanExecute(T)</seealso></param>
        void ICommand.Execute(object parameter)
        {
            try
            {
                ExecuteSync();
            }
            catch { }
        }
        #endregion
        #endregion
    }

    public class SyncCommand<T> : ISyncCommand<T>
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        public event EventHandler CanExecuteChanged;

        private bool isExecuting;
        private readonly Func<T, bool> canExecute;
        private readonly Action<T> executeSync;
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="execute">The Action to be executed</param>
        /// <param name="canExecute">The method indicating whether the <paramref name="execute"/>can be executed</param>
        public SyncCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            executeSync = execute;
            this.canExecute = canExecute;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Executes a delegate synchronously, with an object parameter
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null</param>
        public void ExecuteSync(object parameter)
        {
            if (CanExecute((T)parameter))
            {
                try
                {
                    isExecuting = true;
                    executeSync((T)parameter);
                }
                finally
                {
                    isExecuting = false;
                }
            }
            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Executes the delegate that signals changes in permissions of execution of the command
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Executes a delegate synchronously, with a strong typed parameter
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null</param>
        public void ExecuteSync(T parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    isExecuting = true;
                    executeSync(parameter);
                }
                finally
                {
                    isExecuting = false;
                }
            }
            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Indicates whether the command can be executed
        /// </summary>
        /// <param name="parameter">Generic parameter passed to the command to be executed</param>
        /// <returns>True if the command can be executed; False otherwise.</returns>
        public bool CanExecute(T parameter)
        {
            return !isExecuting && (canExecute?.Invoke(parameter) ?? true);
        }

        #region Explicit implementations
        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.
        void ICommand.Execute(object parameter)
        {
            try
            {
                ExecuteSync((T)parameter);
            }
            catch { }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.
        /// <returns>True if this command can be executed; otherwise, False.</returns>
        bool ICommand.CanExecute(object parameter)
        {
            return parameter == null || CanExecute((T)parameter);
        }
        #endregion
        #endregion
    }
}
