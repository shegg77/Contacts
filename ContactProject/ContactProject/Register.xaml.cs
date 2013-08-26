using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Buddy;
using System.Windows.Media;
using System.Threading.Tasks;

namespace ContactProject
{
    public partial class Register : PhoneApplicationPage
    {
        ProgressIndicator prog;
        public Register()
        {
            InitializeComponent();
            prog = new ProgressIndicator();
            prog.IsVisible = true;
            SystemTray.SetProgressIndicator(this, prog);
            SystemTray.SetBackgroundColor(this, Color.FromArgb(225, 220, 84, 82));
            SystemTray.SetForegroundColor(this, Colors.White);
        }

        public async Task RegisterUser(string userName, string password)
        {
            try
            {
                prog.Text = "Signing you up...";
                App.User = await ServiceManager.Client.CreateUserAsync
                    (
                        userName,
                        password,
                        UserGender.Any,
                        0,
                        userName,
                        UserStatus.Any,
                        false,
                        false,
                        String.Empty
                    );

            }
            catch (BuddyServiceException ex)
            {
                if (ex.Error == "SecurityFailedBadUserName" || ex.Error == "SecurityFailedBadUserNameOrPassword")
                {
                    MessageBox.Show("Unknown username or password.");
                    txtEmailAddress.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 220, 84, 82));
                }
                else if (ex.Error == "UserNameAlreadyInUse")
                {
                    MessageBox.Show("Username already in use");
                    txtEmailAddress.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 220, 84, 82));
                    txtEmailAddress.Focus();
                    txtEmailAddress.SelectAll();
                }
                else
                {
                    MessageBox.Show("Error: " + ex.Error);
                }
                prog.Text = "Contacts";
            }
            finally
            {
                ServiceManager.EncryptPasscode(password, App.User.Token);
                prog.Text = "Contacts";
                NavigationService.Navigate(new Uri("/ComposeEmail.xaml", UriKind.Relative));
            }
        }

        private void txtEmailAddress_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtEmailAddress.Text.Contains("@") && !txtEmailAddress.Text.Contains(".com"))
            {
                txtEmailAddress.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 220, 84, 82));
                EmailError.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                txtEmailAddress.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 52, 152, 219));
                EmailError.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void txtEmailPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtEmailPassword.Password.Count() == 0)
            {
                txtEmailPassword.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 220, 84, 82));
                PasswordError.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                txtEmailPassword.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 52, 152, 219));
                PasswordError.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (!txtEmailAddress.Text.Contains("@") && !txtEmailAddress.Text.Contains(".com"))
            {
                txtEmailAddress.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 220, 84, 82));
                EmailError.Visibility = System.Windows.Visibility.Visible;
            }
            if (txtEmailPassword.Password.Count() == 0)
            {
                txtEmailPassword.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 220, 84, 82));
                PasswordError.Visibility = System.Windows.Visibility.Visible;
                return;
            }

            RegisterUser(txtEmailAddress.Text, txtEmailPassword.Password);
        }
    }
}