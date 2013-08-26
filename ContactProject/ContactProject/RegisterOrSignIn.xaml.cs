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
    public partial class RegisterOrSignIn : PhoneApplicationPage
    {
        string parameter = string.Empty;
        public RegisterOrSignIn()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            if (NavigationContext.QueryString.TryGetValue("parameter", out parameter))
            {
            }
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/login.xaml?parameter=" + parameter, UriKind.Relative));
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Register.xaml?parameter=" + parameter, UriKind.Relative));
        }
    }
}