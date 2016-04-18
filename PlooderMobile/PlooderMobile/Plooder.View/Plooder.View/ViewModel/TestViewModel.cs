using System.Collections.ObjectModel;

namespace Plooder.View.ViewModel
{
    public class TestViewModel
    {
        public static ObservableCollection<Item> Items;

        public TestViewModel()
        {
            Items = new ObservableCollection<Item>
            {
                new Item("item1"),
                new Item("item2"),
                new Item("item3")
            };
        }
    }

    public class Item
    {
        private string _name;

        public Item(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
            set { value = _name; }
        }
    }
}