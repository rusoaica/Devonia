/// Written by: Yulia Danilova
/// Creation Date: 27th of November, 2019
/// Purpose: Explicit implementation of abstract Dispatcher interface, used during unit testing synchronization contexts
#region ========================================================================= USING =====================================================================================
using System;
using System.Threading.Tasks;
#endregion

namespace Devonia.ViewModels.Common.Dispatcher
{
    public class MockDispatcher : IDispatcher
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Executes the specified delegate
        /// </summary>
        /// <param name="method">A delegate to a method that takes one argument.</param>
        /// <param name="args">An object to pass as an argument to the given method.</param>
        public async Task DispatchAsync(Action method, params object[] args)
        {
            method.DynamicInvoke((object)args);
        }

        /// <summary>
        /// Executes the specified delegate
        /// </summary>
        /// <param name="callback">A func delegate to a method that takes one argument and returns a result.</param>
        /// <exception cref="NotImplementedException">Always thrown, this method violates interface segregation principle (it is only used for the ApplicationDispatcher, not the mocked one) :(</exception>
        public async Task<TResult> DispatchAsync<TResult>(Func<TResult> callback)
        {
            // TODO: implement a possible working solution
            throw new NotImplementedException();
        }
        #endregion
    }
}
