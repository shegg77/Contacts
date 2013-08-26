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
using Microsoft.Phone.Tasks;

namespace ContactProject
{
    public partial class ComposeEmail : PhoneApplicationPage
    {

        public ComposeEmail()
        {
            InitializeComponent();

            prog = new ProgressIndicator();
            prog.IsVisible = true;
            prog.Text = "Contacts";
            SystemTray.SetForegroundColor(this, App.Foreground_Color);
            SystemTray.SetBackgroundColor(this, Color.FromArgb(255,62,186,210));
            SystemTray.SetProgressIndicator(this, prog);           
        }

        private void addEmail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            AddressChooserTask addressTask = new AddressChooserTask();
            addressTask.Completed += addressTask_Completed;
            addressTask.Show();
        }

        void addressTask_Completed(object sender, AddressResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                txtEmails.Text += e.Address + ";";
            }
        }

        private void chkDefaultSubject_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkBox = sender as CheckBox;
            if (chkBox.IsChecked == true)
            {
                txtSubject.IsEnabled = false;
                return;
            }
            txtSubject.IsEnabled = true;
            
        }

        private void txtEmails_LostFocus(object sender, RoutedEventArgs e)
        {
            ApplicationBarIconButton btn = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            TextBox tb = sender as TextBox;
            if (!tb.Text.Contains("@"))
            {
                tb.Background = new SolidColorBrush(Colors.Red);
                tb.Foreground = new SolidColorBrush(Color.FromArgb(68, 0, 0, 0));
                btn.IsEnabled = false;
                return;
            }
            else if (!tb.Text.Contains(".com"))
            {
                tb.Background = new SolidColorBrush(Colors.Red);
                tb.Foreground = new SolidColorBrush(Color.FromArgb(68, 0, 0, 0));
                btn.IsEnabled = false;
                return;
            }

            tb.Background = new SolidColorBrush(Colors.Cyan);
            tb.Foreground = new SolidColorBrush(Color.FromArgb(68, 0, 0, 0));
            btn.IsEnabled = true;
        }

        private void sendEmail_Click(object sender, EventArgs e)
        {
            txtEmails.Text.Replace(" ", "");

            if (App.vcards == null)
            {
                App.vCard.EmailSubject = txtSubject.Text;
                App.vCard.EmailBody = txtCustomMessage.Text;
                App.vCard.EmailAddress = txtEmails.Text;
                App.vCard.SendViaEmail(false);
            }
            else if (App.vCard == null)
            {
                VCard tmp = new VCard();
                tmp.SendViaEmail(true, txtEmails.Text, txtSubject.Text, txtCustomMessage.Text);

                this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }

            
        }

        private void txtEmails_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void TextBlock_KeyDown_1(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space)
            {
                txtEmails.Text += ";";
                
            }
        }

        public ProgressIndicator prog { get; set; }

        private void txtEmails_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                txtEmails.Text += ";" + "\n";
                txtEmails.Select(txtEmails.Text.Length, 0);
            }
        }

    }
}