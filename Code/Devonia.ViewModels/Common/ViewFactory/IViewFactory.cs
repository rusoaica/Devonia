/// Written by: Yulia Danilova
/// Creation Date: 11th of June, 2021
/// Purpose: Abstract factory for creating views
#region ========================================================================= USING =====================================================================================
using Devonia.ViewModels.Common.MVVM;
#endregion

namespace Devonia.ViewModels.Common.ViewFactory
{
    public interface IViewFactory
    {
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Creates a view of type <typeparamref name="TResult"/>
        /// </summary>
        /// <typeparam name="TResult">The type of view to create</typeparam>
        /// <returns>A view of type <typeparamref name="TResult"/></returns>
        TResult CreateView<TResult>() where TResult : IView;

        /// <summary>
        /// Creates a view of type <typeparamref name="TResult"/> with an additional parameter passed to its DataContext
        /// </summary>
        /// <typeparam name="TResult">The type of view to create</typeparam>
        /// <param name="param">Additional parameter to be passed to the DataContext of the created view</param>
        /// <returns>A view of type <typeparamref name="TResult"/>, with an assigned property</returns>
        TResult CreateView<TResult>(string param) where TResult : IView;

        /// <summary>
        /// Creates a view of type <typeparamref name="TResult"/> with an assigned DataContext of type <typeparamref name="TParam"/>
        /// </summary>
        /// <typeparam name="TResult">The type of view to create</typeparam>
        /// <typeparam name="TParam">The type of the view model to assign to the DataContext of the view</typeparam>
        /// <param name="param">The view model to assign to the DataContext of the view</param>
        /// <returns>A view of type <typeparamref name="TResult"/>, with a DataContext of type <typeparamref name="TParam"/></returns>
        TResult CreateView<TResult, TParam>(TParam param) where TResult : IView
                                                          where TParam : IBaseModel;
        #endregion
    }
}
