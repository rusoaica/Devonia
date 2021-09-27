/// Written by: Yulia Danilova
/// Creation Date: 20th of June, 2021
/// Purpose: Class for providing asynchronous support for the dynamic proxy interceptor 
#region ========================================================================= USING =====================================================================================
using System;
using System.Linq;
using System.Diagnostics;
using Castle.DynamicProxy;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
#endregion

namespace Devonia.Infrastructure.Logging
{
    public class AsyncLoggerInterceptor : AsyncInterceptorBase
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly ILoggerManager logger;
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="logger">The injected logger to be used in interception</param>
        public AsyncLoggerInterceptor(ILoggerManager logger)
        {
            this.logger = logger;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Intercepts method invocations, both synchronous and asynchronous, that have no return type, and adds logging functionality to them
        /// </summary>
        /// <param name="invocation">The method that will be invoked</param>
        /// <param name="proceedInfo"> Describes the <see cref="IInvocation.Proceed"/> operation for <paramref name="invocation"/> at a specific point during interception</param>
        /// <param name="proceed">The function to proceed the <paramref name="proceedInfo"/></param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            try
            {
                // method that returns a task has no return value
                await proceed(invocation, invocation.CaptureProceedInfo()).ConfigureAwait(false);
            }
            catch (Exception innerException)
            {
                // wrap the original exception in a new exception that contains information about the intercepted method
                Exception outterException =
                       new Exception(
                           $"{invocation.Method.DeclaringType}.{invocation.Method.Name}" +
                           Environment.NewLine +
                           string.Join(", ", invocation.Arguments.Select(a => (a ?? string.Empty).ToString())),
                       innerException);
                logger.LogError(outterException);
                Debug.WriteLine(outterException);
                Trace.WriteLine(outterException);
                // preserve the original exception type and its stacktrace and re-throw it
                ExceptionDispatchInfo.Capture(innerException).Throw();
            }
        }

        /// <summary>
        /// Intercepts method invocations, both synchronous and asynchronous, that have a return type, and adds logging functionality to them
        /// </summary>
        /// <typeparam name="TResult">The type of the result to return</typeparam>
        /// <param name="invocation">The method that will be invoked</param>
        /// <param name="proceedInfo"> Describes the <see cref="IInvocation.Proceed"/> operation for <paramref name="invocation"/> at a specific point during interception</param>
        /// <param name="proceed">The function to proceed the <paramref name="proceedInfo"/></param>
        /// <returns>A Task object that represents the result of awaiting the asynchronous operation</returns>
        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            try
            {
                // intercepted method is invoked here, return its awaited result
                return await proceed(invocation, invocation.CaptureProceedInfo()).ConfigureAwait(false); 
            }
            catch (Exception innerException)
            {
                // wrap the original exception in a new exception that contains information about the intercepted method
                Exception outterException =
                    new Exception(
                        $"{invocation.Method.DeclaringType}.{invocation.Method.Name}" +
                        Environment.NewLine +
                        string.Join(", ", invocation.Arguments.Select(a => (a ?? string.Empty).ToString())),
                    innerException);
                logger.LogError(outterException);
                Debug.WriteLine(outterException);
                // a return value MUST be set as the return value of the intercepted method, or the proxy class will throw an InvalidOperationException!
                invocation.ReturnValue = default;
                // preserve the original exception type and its stacktrace and re-throw it
                ExceptionDispatchInfo.Capture(innerException).Throw();
                throw;
            }
        }
        #endregion
    }
}
