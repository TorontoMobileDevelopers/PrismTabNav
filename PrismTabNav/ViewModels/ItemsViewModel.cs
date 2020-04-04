using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Prism.Navigation.Xaml;
using Xamarin.Forms;

using PrismTabNav.Models;
using PrismTabNav.Services;
using PrismTabNav.Views;
using NavigationParameters = Prism.Navigation.NavigationParameters;

namespace PrismTabNav.ViewModels
{
    public class ItemsViewModel : BaseViewModel, INavigationAware
    {
        private readonly INavigationService _navigationService;
        private readonly MockDataStore _datastore;
        public ObservableCollection<Item> Items { get; }= new ObservableCollection<Item>();
        public DelegateCommand LoadItemsCommand { get; }
        public DelegateCommand AddItemCommand { get; }
        public DelegateCommand<Item> ItemSelectedCommand { get; }
        public ItemsViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            
            Title = "Browse";
            LoadItemsCommand = new DelegateCommand(OnLoadItems);
            AddItemCommand = new DelegateCommand(OnAddItem);
            ItemSelectedCommand = new DelegateCommand<Item>(OnItemSelected);
            _datastore = new MockDataStore();

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Item;
                Items.Add(newItem);
                await DataStore.AddItemAsync(newItem);
            });
        }

        private async void OnLoadItems()
        {
            try
            {
                IsBusy = true;
                Items.Clear();
                var items = await _datastore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnItemSelected(Item item) =>
            _navigationService.NavigateAsync(nameof(ItemDetailPage), new NavigationParameters {{nameof(Item), item}})
                .FireAndForgetSafeAsync();

        private void OnAddItem() =>
            _navigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(NewItemPage)}")
                .FireAndForgetSafeAsync();

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            OnLoadItems();
        }
    }
}