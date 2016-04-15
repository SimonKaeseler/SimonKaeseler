using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Plooder.View
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Item> Items { get; set; }
        public MainPage()
        {
            InitializeComponent();

            this.BindingContext = Items;
            ItemView.ItemsSource = Items;

            Items.Add(new Item("item1"));
        }
        

    }

    public class Item
    {
        string name;

        public Item(string name)
        {
            this.name = name;
        }
    }
}
