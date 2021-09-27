/// Written by: Yulia Danilova
/// Creation Date: 27th of November, 2019
/// Purpose: Explicit implementation of abstract Dispatcher interface, used in UI environments
#region ========================================================================= USING =====================================================================================
using System;
using Avalonia;
using Avalonia.Threading;
using System.Threading.Tasks;
#endregion

namespace Devonia.Views.Common.Dispatcher
{
    public class ApplicationDispatcher : ViewModels.Common.Dispatcher.IDispatcher
    {
        #region =============================================================== PROPERTIES ==================================================================================
        Avalonia.Threading.Dispatcher UnderlyingDispatcher
        {
            get
            {
                if (Application.Current == null)
                    throw new InvalidOperationException("You must call this method from within a running Avalonia application!");
                if (Avalonia.Threading.Dispatcher.UIThread == null)
                    throw new InvalidOperationException("You must call this method from within a running Avalonia application with an active dispatcher!");
                return Avalonia.Threading.Dispatcher.UIThread;
            }
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Executes the specified delegate with the specified argument, synchronously
        /// </summary>
        /// <param name="method">A delegate to a method that takes one argument, which is pushed onto the System.Windows.Threading.Dispatcher event queue.</param>
        /// <param name="args">An object to pass as an argument to the given method.</param>
        public async Task DispatchAsync(Action method, params object[] args)
        {
            await UnderlyingDispatcher.InvokeAsync(method, DispatcherPriority.Background);
        }

        /// <summary>
        /// Executes the specified delegate synchronously
        /// </summary>
        /// <typeparam name="TResult">The type of result returned by <paramref name="callback"/></typeparam>
        /// <param name="callback">A func returning a result of type <typeparamref name="TResult"/></param>
        /// <returns>A Func callback of type <typeparamref name="TResult"/></returns>
        public async Task<TResult> DispatchAsync<TResult>(Func<TResult> callback)
        {
            return await UnderlyingDispatcher.InvokeAsync(callback);
        }
        #endregion
    }
}
