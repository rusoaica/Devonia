/// Written by: Yulia Danilova
/// Creation Date: 24th of October, 2019
/// Purpose: Task extension method to handle exceptions (avoid littering whole project with try...catch'es)
#region ========================================================================= USING =====================================================================================
using System;
using System.Diagnostics;
using System.Threading.Tasks;
#endregion

namespace Devonia.ViewModels.Common.Extensions
{
    public static class TaskUtilities
    {
        #region ================================================================ METHODS ====================================================================================
        /// <summary>
        /// Extends Task by providing exception handling when a task is invoked on a void method (not awaited)
        /// </summary>
        /// <param name="task">The task to be awaited</param>
        public static async void FireAndForgetSafeAsync(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            { 
                Debug.WriteLine(ex); 
            }
        }
        #endregion
    }
}