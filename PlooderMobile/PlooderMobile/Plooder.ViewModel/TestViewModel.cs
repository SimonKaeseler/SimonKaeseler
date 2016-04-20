using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Plooder.ViewModel.Annotations;

namespace Plooder.ViewModel
{
    public class TestViewModel : INotifyPropertyChanged
    {
        public static ObservableCollection<Item> Items { get; set; }

        public TestViewModel()
        {
            Items = new ObservableCollection<Item>
            {
                new Item("item1"),
                new Item("item2"),
                new Item("item3"),
                new Item("item4")
            };

            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
