using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
namespace ContactProject
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        private ProgressIndicator prog;
        public SettingsPage()
        {
            InitializeComponent();
            prog = new ProgressIndicator();
            prog.IsVisible = true;
            SystemTray.SetProgressIndicator(this, prog);
            SystemTray.SetForegroundColor(this, App.Foreground_Color);
            SystemTray.SetBackgroundColor(this, Color.FromArgb(255,101,159,130));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            
        }

        private void Toggle_Checked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch switches = sender as ToggleSwitch;
            switches.Content = "On";
        }

        private void Toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch switches = sender as ToggleSwitch;
            switches.Content = "Off";
        }

       
    }
}