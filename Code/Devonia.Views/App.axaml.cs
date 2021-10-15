/// Written by: Avalonia, Yulia Danilova
/// Creation Date: 10th of June, 2021
/// Purpose: Application's entry point class
#region ========================================================================= USING =====================================================================================
using Autofac;
using Avalonia;
using Avalonia.Markup.Xaml;
using Devonia.Infrastructure.Configuration;
using Devonia.Models.Core.Options;
using Devonia.Views.Common.Styles;
using Devonia.Views.Common.Configuration;
using Devonia.ViewModels.Common.ViewFactory;
using Devonia.ViewModels.Main;
using Devonia.ViewModels.Startup;
#endregion

namespace Devonia.Views
{
    public class App : Application
    {
        #region ============================================================== FIELD MEMBERS ================================================================================
        public static StyleManager styles;
        #endregion
        
        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Initializes the application by loading XAML etc.
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Framework initialization code
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            styles = new StyleManager(this);
            OptionsInterface.ThemeChanged += styles.SwitchThemeByIndex;

            // configure the dependency injection services
            IContainer container = DIContainerConfig.Configure();
            // begin the application's lifetime score
            using (ILifetimeScope scope = container.BeginLifetimeScope())
            {
                // get a view factory from the DI container and display the startup view from it, as modal dialog
                IViewFactory factory = container.Resolve<IViewFactory>();
                // if the user set authentication in app settinggs, display the login view, otherwise, the main view
                IView main = container.Resolve<IAppConfig>().Miscellaneous.UsesAuthentication ? factory.CreateView<IStartupView>() : factory.CreateView<IMainWindowView>();
                main.Show();
            }
            base.OnFrameworkInitializationCompleted();
        }
        #endregion
    }
}
