/// Written by: Yulia Danilova
/// Creation Date: 11th of June, 2021
/// Purpose: Factory for creating views
#region ========================================================================= USING =====================================================================================
using System;
using Autofac;
using System.Linq;
using Devonia.ViewModels.Common.MVVM;
using Devonia.ViewModels.Common.ViewFactory;
#endregion

namespace Devonia.Views.Common.UIFactory
{
    public class ViewFactory : IViewFactory
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        private readonly ILifetimeScope scope;
        #endregion

        #region ================================================================== CTOR =====================================================================================
        /// <summary>
        /// Overload C-tor
        /// </summary>
        /// <param name="scope">The injected scope to be used</param>
        public ViewFactory(ILifetimeScope scope)
        {
            this.scope = scope;
        }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Creates a view of type <typeparamref name="TResult"/>
        /// </summary>
        /// <typeparam name="TResult">The type of view to create</typeparam>
        /// <returns>A view of type <typeparamref name="TResult"/></returns>
        public TResult CreateView<TResult>() where TResult : IView
        {
            return scope.Resolve<TResult>();
        }

        /// <summary>
        /// Creates a view of type <typeparamref name="TResult"/> with an additional parameter passed to its DataContext
        /// </summary>
        /// <typeparam name="TResult">The type of view to create</typeparam>
        /// <param name="param">Additional parameter to be passed to the DataContext of the created view</param>
        /// <returns>A view of type <typeparamref name="TResult"/>, with an assigned property</returns>
        public TResult CreateView<TResult>(string param) where TResult : IView
        {
            TResult view = scope.Resolve<TResult>();
            // check if the class assigned as DataContext of the view contains a property named Id, and if so, assign it
            Type t = view.DataContext.GetType();
            if (t.GetProperties().Any(p => p.Name.Equals("Id")))
                view.DataContext.Id = param;
            return view;
        }

        /// <summary>
        /// Creates a view of type <typeparamref name="TResult"/> with an assigned DataContext of type <typeparamref name="TParam"/>
        /// </summary>
        /// <typeparam name="TResult">The type of view to create</typeparam>
        /// <typeparam name="TParam">The type of the view model to assign to the DataContext of the view</typeparam>
        /// <param name="param">The view model to assign to the DataContext of the view</param>
        /// <returns>A view of type <typeparamref name="TResult"/>, with a DataContext of type <typeparamref name="TParam"/></returns>
        public TResult CreateView<TResult, TParam>(TParam param) where TResult : IView
                                                                 where TParam : IBaseModel
        {
            TResult view = scope.Resolve<TResult>();
            view.DataContext = param;
            return view;
        }
        #endregion
    }
}
