using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ContactProject
{
    public partial class SearchControl : UserControl
    {
        public SearchControl()
        {
            InitializeComponent();
            
        }

        private void ListBoxItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            AppSettings settings = new AppSettings();

            ListBoxItem item = sender as ListBoxItem;
            settings.SearchItem = (item.Content.ToString());
        }
    }
}
