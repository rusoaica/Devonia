/// Written by: Yulia Danilova
/// Creation Date: 20th of June, 2021
/// Purpose: Proxy class providing logging functionality for intercepted interfaces
#region ========================================================================= USING =====================================================================================
using Castle.DynamicProxy;
#endregion

namespace Devonia.Infrastructure.Logging
{
    public class LoggerInterceptor : IInterceptor
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly AsyncLoggerInterceptor asyncLoggerInterceptor;
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="asyncLoggerInterceptor">Injected class that provides asynchronous dynamic proxy interception support</param>
        public LoggerInterceptor(AsyncLoggerInterceptor asyncLoggerInterceptor)
        {
            this.asyncLoggerInterceptor = asyncLoggerInterceptor;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Intercepts a method and forwards it to an asynchronous dynamic proxy interceptor
        /// </summary>
        /// <param name="invocation">Encapsulates an invocation of a proxied method</param>
        public void Intercept(IInvocation invocation)
        {
            asyncLoggerInterceptor.ToInterceptor().Intercept(invocation);
        }
        #endregion
    }
}
