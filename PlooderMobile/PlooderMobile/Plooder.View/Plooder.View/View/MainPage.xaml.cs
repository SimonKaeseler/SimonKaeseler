using System;
using System.Collections.Generic;
using Plooder.View.ViewModel;
using Xamarin.Forms;

namespace Plooder.View.View
{
   
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            List<TestViewModel> model = new List<TestViewModel>
            {
                new TestViewModel()
            };

            ItemView.ItemsSource = model;

        }
    }


}
