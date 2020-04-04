using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Prism;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PrismTabNav.Services;
using PrismTabNav.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Navigation;
using PrismTabNav.ViewModels;

namespace PrismTabNav
{
    public partial class App : PrismApplication
    {

        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<AboutPage>();
            containerRegistry.RegisterForNavigation<ItemDetailPage>();
            containerRegistry.RegisterForNavigation<ItemsPage, ItemsViewModel>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<NewItemPage>();
        }

        protected override void OnInitialized()
        {
            NavigateAsync(nameof(MainPage)).FireAndForgetSafeAsync();
        }

        protected override void OnResume()
        {
        }

        private async Task NavigateAsync(string page, INavigationParameters parameters = null, bool useModalNavigation = false)
        {
            var result = await NavigationService.NavigateAsync(page, parameters, useModalNavigation);
            if (result.Success) return;
            throw result.Exception;
        }
    }
    
    public static class TaskUtilities
    {
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public static async void FireAndForgetSafeAsync(this Task task, Action<Exception> handleErrorAction = null)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                await task.ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                if (handleErrorAction is null)
                    Console.WriteLine(ex);
                else
                    handleErrorAction.Invoke(ex);
            }
        }
    }
}
