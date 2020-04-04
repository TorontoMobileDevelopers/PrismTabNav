using System;
using Prism.Navigation;
using PrismTabNav.Models;
using PrismTabNav.Services;

namespace PrismTabNav.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel, INavigationAware
    {
        private Item _item;
        private readonly MockDataStore _datastore;

        public Item Item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        public ItemDetailViewModel()
        {
            _datastore = new MockDataStore();
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (parameters.TryGetValue(nameof(Item), out Item item) && item != null)
                {
                    Item = item;
                    Title = item?.Text;
                }
                else if (parameters.TryGetValue(CustomNavigationParameters.Id, out string itemId))
                {
                    var selectedItem = await _datastore.GetItemAsync(itemId);
                    Item = selectedItem;
                    Title = selectedItem?.Text;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
