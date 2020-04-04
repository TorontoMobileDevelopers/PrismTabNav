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
using System.Collections.Generic;
using System.Linq;

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
            containerRegistry.RegisterForNavigation<ItemDetailPage, ItemDetailViewModel>();
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

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            try
            {
                var url = uri.ToString();
                if (url.Contains(KnownNavigationParameters.SelectedTab))
                {
                    var deeplinkDetails = ResolveDeepLink(uri.PathAndQuery);
                    NavigateAsync(deeplinkDetails.PageNavigationUrl, deeplinkDetails.Parameters).FireAndForgetSafeAsync();
                }
                else
                    NavigateAsync(url).FireAndForgetSafeAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static PageNavigationDetails ResolveDeepLink(string deepLinkUrl)
        {
            string pageName;
            var parameters = new NavigationParameters();

            var deepLinkPathParts = deepLinkUrl.Trim('/').Split('/');

            string tabbedPageUrl = deepLinkPathParts.First();
            var tabbedPageAndSelectedTab = tabbedPageUrl.Split('?'); // /MainPage?selectedTab=ItemsPage/
            string tabbedPage = tabbedPageAndSelectedTab.First();
            string selectedTab = tabbedPageAndSelectedTab.Last()
                .Replace(KnownNavigationParameters.SelectedTab, "")
                .Replace("=", "");

            string deepLinkPagePath = deepLinkUrl.Replace($"/{tabbedPageUrl}/", "");

            pageName = $"/{tabbedPage}?{Prism.Navigation.KnownNavigationParameters.SelectedTab}={selectedTab}";
            Debug.WriteLine($"PrimaryNavigationPath: {pageName}");

            parameters.Add(CustomNavigationParameters.SubPagePath, deepLinkPagePath);
            Debug.WriteLine($"SecondaryNavigationPath: {deepLinkPagePath}");

            return new PageNavigationDetails(pageName, parameters);
        }
    }

    internal class CustomNavigationParameters
    {
        public static string SubPagePath { get; } = nameof(SubPagePath);
        public static string Id { get; } = nameof(Id);
    }

    public class PageNavigationDetails
    {
        public string PageNavigationUrl { get; }
        public NavigationParameters Parameters { get; }

        public PageNavigationDetails(string pageNavigationUrl, NavigationParameters parameters)
        {
            PageNavigationUrl = pageNavigationUrl;
            Parameters = parameters;
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
