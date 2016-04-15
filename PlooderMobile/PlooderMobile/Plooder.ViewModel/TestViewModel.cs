
using System.Collections.ObjectModel;

namespace Plooder.ViewModel
{
    public class TestViewModel
    {
        public ObservableCollection<Item> Items { get; set; }

        public TestViewModel()
        {
            Items = new ObservableCollection<Item>();
            Items.Add(new Item("item1"));
        }
    }
}
