using System;
using System.Linq;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ContactProject
{


    public partial class UserPage : PhoneApplicationPage
    {
        ContactModel item;
        ContactModel favItem;

        public UserPage()
        {
            InitializeComponent();

            prog = new ProgressIndicator();
            prog.IsVisible = true;
            SystemTray.SetProgressIndicator(this, prog);
            SystemTray.SetForegroundColor(this, Colors.White);
            SystemTray.SetBackgroundColor(this, Color.FromArgb(255, 50, 58, 69));
            item = new ContactModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string msg = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("msg", out msg))
            {
                SearchForUser(msg);
                var favoriteUser = App.FavoritesList.Where(f => f.FullName == item.FullName);
                if (favoriteUser.Count() != 0)
                {
                    favItem = favoriteUser.First();
                    favButton.Source = new BitmapImage(new Uri("/Assets/AppBar1/FavoriteUserFilled.png", UriKind.Relative));
                }
                else
                {
                }

            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            App.ViewModel.SaveFavorites("Favorites");
        }

        private void SearchForUser(string userName)
        {
            var userResult = App.ViewModel.ContactItems.Where(f => f.FullName == userName);
            if (userResult != null && userResult.Count() > 0)
            {
                item = userResult.First();
                UserName.Text = item.FullName;
                UserPhoneNumber.Text = item.PhoneNumber;
                profilePicture.Source = item.Image;
                txtLocation.Text = item.Location;

                if (item.PhoneNumber == "Facebook")
                {
                    UserPhoneNumber.Text = "No phone number present";
                    txtCall.Visibility = System.Windows.Visibility.Collapsed;
                    txtText.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (item.Email.Count() != 0)
                {
                    EmailAddress.Text = item.Email[0];
                }
                else
                {
                    TapEmail.Visibility = System.Windows.Visibility.Collapsed;
                }
                return;
            }
            UserPhoneNumber.Text = "No phone number present";
            txtCall.Visibility = System.Windows.Visibility.Collapsed;
            txtText.Visibility = System.Windows.Visibility.Collapsed;
            TapEmail.Visibility = System.Windows.Visibility.Collapsed;
            txtLocation.Text = "No location";

        }

        private void PhoneTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneCallTask call = new PhoneCallTask();
            call.PhoneNumber = item.PhoneNumber;
            call.DisplayName = item.FullName;
            string Name = null;
            Name = item.FullName + "(Outgoing)";

            App.RecentList.Add
                (new RecentContact
                {
                    ContactName = item,
                    PhoneNumber = item.PhoneNumber,
                    FullName = Name,
                    Placed = "Outgoing",
                    Type = "Call",
                    Time = DateTime.Now
                });
            call.Show();
        }

        private void PhoneTile_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string Name = null;

            SmsComposeTask Compose = new SmsComposeTask();
            Compose.To = item.PhoneNumber;
            Name = item.FullName + "(SMS)";
            Compose.Show();
            App.RecentList.Add
                (new RecentContact
                {
                    ContactName = item,
                    PhoneNumber = item.PhoneNumber,
                    FullName = Name,
                    Type = "SMS",
                    Time = DateTime.Now
                });
        }


        public ProgressIndicator prog { get; set; }

        private void profilePicture_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            App.vCard = new VCard();
            this.NavigationService.Navigate(new Uri("/RegisterOrSignIn.xaml?msg=" + item.FullName, UriKind.Relative));
        }

        private void locationTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            AddressChooserTask addressTask = new AddressChooserTask();
            addressTask.Completed += addressTask_Completed;
            addressTask.Show();

        }

        void addressTask_Completed(object sender, AddressResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                item.Location += e.Address;
                txtLocation.Text = e.Address;
                App.ViewModel.SaveFavorites("Contacts");
            }
        }

        private void favButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (item.Favorite == true)
            {
                favButton.Source = new BitmapImage(new Uri("/Assets/AppBar1/FavoriteUser.png", UriKind.Relative));
                App.FavoritesList.Remove(favItem);
                item.Favorite = false;
                return;
            }
            favButton.Source = new BitmapImage(new Uri("/Assets/AppBar1/FavoriteUserFilled.png", UriKind.Relative));
            App.FavoritesList.Add(item);
            item.Favorite = true;
        }
    }
}