/// Written by: Yulia Danilova
/// Creation Date: 27th of November, 2019
/// Purpose: Interface for interacting with Dispatcher objects
#region ========================================================================= USING =====================================================================================
using System;
using System.Threading.Tasks;
#endregion

namespace Devonia.ViewModels.Common.Dispatcher
{
    public interface IDispatcher
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Executes the specified <paramref name="callback"/> asynchronously on the thread the Dispatcher is associated with
        /// </summary>
        /// <typeparam name="TResult">The return type of the method to be executed</typeparam>
        /// <param name="callback">The method to be executed</param>
        /// <returns>The result of executing <paramref name="callback"/></returns>
        Task<TResult> DispatchAsync<TResult>(Func<TResult> callback);

        /// <summary>
        /// Executes <paramref name="method"/> with the <paramref name="args"/> arguments, synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="method">The method to be executed</param>
        /// <param name="args">The arguments to pass to the method to be executed</param>
        //void Dispatch(Delegate method, params object[] args);
        Task DispatchAsync(Action method, params object[] args);
        #endregion
    }
}