using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using Buddy;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ContactProject
{
    public partial class Login : PhoneApplicationPage
    {
        ProgressIndicator prog;
        bool SignedIn = false;

        public Login()
        {
            prog = new ProgressIndicator();
            prog.IsVisible = true;

            InitializeComponent();
            if (App.User == null)
            {
                //string result = ServiceManager.DecryptPasscode("Bread");
               // if (result != null)
                //{
                //    prog.Text = "Signing In...";
                //    SignUserIn(null, null, result);
                //    SignedIn = true;
                //}
            }

            #region SystemTray
            
            SystemTray.SetForegroundColor(this, Colors.White);
            SystemTray.SetBackgroundColor(this, Color.FromArgb(255, 49, 134, 179));
            SystemTray.SetProgressIndicator(this, prog);
            #endregion

        }

        private void txtEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtEmail.Text.Contains("@") && !txtEmail.Text.Contains(".com"))
            {
                txtEmail.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 220, 84, 82));
                EmailError.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                txtEmail.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 52, 152, 219));
                EmailError.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ContactModel item = new ContactModel();
            string msg = string.Empty;

            #region Query
            if (NavigationContext.QueryString.TryGetValue("parameter", out msg))
            {

                var emailUser = App.ViewModel.ContactItems.Where(f => f.FullName == msg);
                if (emailUser.Count() != 0)
                {
                    item = emailUser.First();

                    //vcard.EmailAddress = item.Email[0];                  
                }
                if (msg != "batch")
                {
                    App.vCard = new VCard();
                    App.vCard.FirstName = item.FirstName;
                    App.vCard.LastName = item.LastName;
                    App.vCard.PhoneNumber = item.PhoneNumber;
                }

            }

        }
            #endregion

        
        public async void SignUserIn(string userName = null, string password = null, string token = null)
        {
            try
            {
                if (userName != null)
                {
                    prog.Text = "Logging you in...";
                    App.User = await ServiceManager.Client.LoginAsync(userName, password);
                    ServiceManager.EncryptPasscode(password, App.User.Token);
                }
                else
                {
                    App.User = await ServiceManager.Client.LoginAsync(token);
                    ServiceManager.EncryptPasscode(null, App.User.Token);
                    
                }
            }
            catch (BuddyServiceException ex)
            {
                if (ex.Error == "SecurityFailedBadUserName" || ex.Error == "SecurityFailedBadUserNameOrPassword")
                {
                    MessageBox.Show("Unknown username or password.");
                    txtEmail.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 220, 84, 82));
                    txtEmail.Focus();
                    txtEmail.SelectAll();
                }
                else if (ex.Error == "UserNameAlreadyInUse")
                {
                    MessageBox.Show("Username already in use");
                    txtEmail.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 220, 84, 82));
                    txtEmail.Focus();
                    txtEmail.SelectAll();
                }
                else
                {
                    MessageBox.Show("Error: " + ex.Error);
                    txtEmail.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 220, 84, 82));
                    txtEmail.Focus();
                    txtEmail.SelectAll();
                }
            }
            finally
            {
                ServiceManager.EncryptPasscode(password, App.User.Token);
                NavigationService.Navigate(new Uri("/ComposeEmail.xaml", UriKind.Relative));
            }

            
        }

        private void txtEmailPass_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtEmailPass.Password.Count() == 0)
            {
                txtEmailPass.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 220, 84, 82));
                PasswordError.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                txtEmailPass.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 52, 152, 219));
                PasswordError.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!txtEmail.Text.Contains("@") && !txtEmail.Text.Contains(".com"))
            {
                txtEmail.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 220, 84, 82));
                EmailError.Visibility = System.Windows.Visibility.Visible;
            }
            if (txtEmailPass.Password.Count() == 0)
            {
                txtEmailPass.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 220, 84, 82));
                PasswordError.Visibility = System.Windows.Visibility.Visible;
                return;
            }

            SignUserIn(txtEmail.Text, txtEmailPass.Password);
        }
    }
}