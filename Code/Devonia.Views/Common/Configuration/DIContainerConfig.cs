/// Written by: Yulia Danilova
/// Creation Date: 11th of June, 2021
/// Purpose: Registers services in the Autofac Dependency Injection container
#region ========================================================================= USING =====================================================================================
using Autofac;
using Autofac.Core;
using Autofac.Extras.DynamicProxy;
using Devonia.Infrastructure.Configuration;
using Devonia.Infrastructure.Dialog;
using Devonia.Infrastructure.Logging;
using Devonia.Infrastructure.Notification;
using Devonia.Models.Core.Options;
using Devonia.Models.Core.Security;
using Devonia.ViewModels.Common.Clipboard;
using Devonia.ViewModels.Common.Dialogs.MessageBox;
using Devonia.ViewModels.Common.Dispatcher;
using Devonia.ViewModels.Common.ViewFactory;
using Devonia.ViewModels.Main;
using Devonia.ViewModels.Register;
using Devonia.ViewModels.Startup;
using Devonia.Views.Common.Clipboard;
using Devonia.Views.Common.Dialogs;
using Devonia.Views.Common.Dialogs.MessageBox;
using Devonia.Views.Common.Dispatcher;
using Devonia.Views.Common.UIFactory;
using Devonia.Views.Main;
using Devonia.Views.Register;
using Devonia.Views.Startup;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
#endregion

namespace Devonia.Views.Common.Configuration
{
    public class DIContainerConfig
    {
        public static IContainer Configure()
        {
            // TODO: use reflection and pattern matching for all repetitive registrations
            ContainerBuilder builder = new ContainerBuilder();

            #region models            

            builder.RegisterType<Authentication>()
                   .As<IAuthentication>()
//#if !DEBUG
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(LoggerInterceptor))
//#endif
                   .SingleInstance();

            builder.RegisterType<AppOptions>()
                   .As<IAppOptions>()
//#if !DEBUG
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(LoggerInterceptor))
//#endif
                   .SingleInstance();

            builder.RegisterType<OptionsInterface>()
                   .As<IOptionsInterface>()
//#if !DEBUG
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(LoggerInterceptor))
//#endif
                   .SingleInstance();
            #endregion

            #region view models 
            builder.RegisterType<StartupVM>().As<IStartupVM>().InstancePerDependency();
            builder.RegisterType<MsgBoxVM>().As<IMsgBoxVM>().InstancePerDependency();
            builder.RegisterType<RegisterVM>().As<IRegisterVM>().InstancePerDependency();
            builder.RegisterType<RecoverPasswordVM>().As<IRecoverPasswordVM>().InstancePerDependency();
            builder.RegisterType<ChangePasswordVM>().As<IChangePasswordVM>().InstancePerDependency();
            builder.RegisterType<MainWindowVM>().As<IMainWindowVM>().InstancePerDependency();
            //builder.RegisterType<SystemVM>().As<ISystemVM>().InstancePerDependency();
            #endregion

            #region infrastructure
            builder.RegisterType<LoggerManager>().As<ILoggerManager>().SingleInstance();
            builder.RegisterType<LoggerInterceptor>();
            builder.RegisterType<AsyncLoggerInterceptor>();
            #endregion

            #region dialogs
            builder.RegisterType<MessageBoxService>()
                .WithParameter(new ResolvedParameter(
                    (propertyInfo, context) => propertyInfo.ParameterType == typeof(IDispatcher),
                    (propertyInfo, context) => context.Resolve<IDispatcher>()))
                .As<INotificationService>().InstancePerDependency();
            #endregion

            #region views
            builder.RegisterType<StartupV>().OnActivating(e => e.Instance.DataContext = e.Context.Resolve<IStartupVM>()).As<IStartupView>().SingleInstance();
            builder.RegisterType<RegisterV>().OnActivating(e => e.Instance.DataContext = e.Context.Resolve<IRegisterVM>()).As<IRegisterView>().InstancePerDependency();
            builder.RegisterType<MainWindowV>().OnActivating(e => e.Instance.DataContext = e.Context.Resolve<IMainWindowVM>()).As<IMainWindowView>().InstancePerDependency();
            builder.RegisterType<RecoverPasswordV>().OnActivating(e => e.Instance.DataContext = e.Context.Resolve<IRecoverPasswordVM>()).As<IRecoverPasswordView>().InstancePerDependency();
            builder.RegisterType<ChangePasswordV>().OnActivating(e => e.Instance.DataContext = e.Context.Resolve<IChangePasswordVM>()).As<IChangePasswordView>().InstancePerDependency();
            #endregion

            if (File.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "appsettings.json"))
                builder.Register(context => JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "appsettings.json")))
                       .OnActivating(e => e.Instance.ConfigurationFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "appsettings.json")
                       .As<IAppConfig>()
                       .SingleInstance();
#if !DEBUG
            else
                throw new FileNotFoundException("Configuration file not found!\nPath: " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)  + Path.DirectorySeparatorChar + "appsettings.json");
#endif

            builder.RegisterType<MsgBoxV>().As<IMsgBoxView>().InstancePerDependency();
            builder.RegisterType<ApplicationDispatcher>().As<IDispatcher>().InstancePerDependency();
            builder.RegisterType<WindowsClipboard>().As<IClipboard>().SingleInstance();

            builder.RegisterType<ViewFactory>().As<IViewFactory>().InstancePerDependency();

            return builder.Build();
        }
    }
}
