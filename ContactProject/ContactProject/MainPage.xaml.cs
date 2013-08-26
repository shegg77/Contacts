using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Shapes;
using Microsoft.Live;

namespace ContactProject
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush accent = new SolidColorBrush(Color.FromArgb(255, 220, 84, 82));
            if ((bool)value)
            {
                return accent;
            }
            return new SolidColorBrush(Colors.Transparent);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }

    }

    public class DateTimeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;                      
            return (date.ToString("MM/dd"));
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }

    }
    public partial class MainPage : PhoneApplicationPage
    {        
        AppSettings settings;
        bool SAVING_FAVORITE = false;
        ProgressIndicator prog;
        ApplicationBar mainAppBar;
        ApplicationBar favAppBar;
        ApplicationBar recentAppBar;
        ApplicationBarIconButton selectItems;
        ApplicationBarIconButton deleteItems;
        ApplicationBarMenuItem selectAll;

        string CURRENT_PIVOTITEM = null;

        public MainPage()
        {
            
            InitializeComponent();
            App.FirstVisit = true;
            SetGlobalColors();
            InitAppBars();
            recentList.ItemsSource = App.RecentList;
            
            

            prog = new ProgressIndicator();
            prog.IsVisible = true;
            SystemTray.SetForegroundColor(this, App.Foreground_Color);
            SystemTray.SetBackgroundColor(this, App.Background_Color);
            SystemTray.SetProgressIndicator(this, prog);
            settings = new AppSettings();
            
            ModelView model = new ModelView();
            App.ViewModel = model;           
        }

        private void InitAppBars()
        {
            mainAppBar = new ApplicationBar();
            mainAppBar.BackgroundColor = App.AppBarColor;
            mainAppBar.ForegroundColor = Colors.White;
            mainAppBar.Mode = ApplicationBarMode.Default;
            mainAppBar.Opacity = 1.0;
            mainAppBar.IsVisible = true;
            mainAppBar.IsMenuEnabled = true;

            ApplicationBarMenuItem shareAll = new ApplicationBarMenuItem();
            shareAll.Text = "share all contacts";
            shareAll.Click += shareAll_Click;
            mainAppBar.MenuItems.Add(shareAll);

            ApplicationBarIconButton AddNew = new ApplicationBarIconButton();
            AddNew.IconUri = new Uri("/Assets/AppBar1/add.png", UriKind.Relative);
            AddNew.Click += AddNewContactButton_Click;
            ApplicationBarIconButton searchbutton = new ApplicationBarIconButton();
            searchbutton.IconUri = new Uri("/Assets/AppBar1/feature.search.png", UriKind.Relative);
            searchbutton.Click += SearchForContactButton_Click;
            ApplicationBarIconButton prefButton = new ApplicationBarIconButton();
            prefButton.IconUri = new Uri("/Assets/AppBar1/feature.settings.png", UriKind.Relative);
            prefButton.Click += PrefButton_Click;
            AddNew.Text = "add";
            searchbutton.Text = "search";
            prefButton.Text = "options";
            mainAppBar.Buttons.Add(AddNew);
            mainAppBar.Buttons.Add(searchbutton);
            mainAppBar.Buttons.Add(prefButton);

            selectAll = new ApplicationBarMenuItem();
            selectAll.Text = "select all";
            selectAll.Click += selectAll_Click;

            favAppBar = new ApplicationBar();
            favAppBar.BackgroundColor = App.AppBarColor;
            favAppBar.ForegroundColor = Colors.White;
            favAppBar.Mode = ApplicationBarMode.Default;
            favAppBar.Opacity = 1.0;
            favAppBar.IsVisible = true;
            favAppBar.IsMenuEnabled = true;

            ApplicationBarMenuItem deleteList = new ApplicationBarMenuItem();
            deleteList.Text = "delete entire list";
            deleteList.Click += deleteList_Click;
            favAppBar.MenuItems.Add(deleteList);

            recentAppBar = new ApplicationBar();
            recentAppBar.BackgroundColor = App.AppBarColor;
            recentAppBar.ForegroundColor = Colors.White;
            recentAppBar.Mode = ApplicationBarMode.Default;
            recentAppBar.IsMenuEnabled = true;
            recentAppBar.MenuItems.Add(deleteList);
            

            selectItems = new ApplicationBarIconButton();
            selectItems.IconUri = new Uri("/Assets/AppBar1/tasks.png", UriKind.Relative);
            selectItems.Text = "select";
            selectItems.Click += selectItems_Click;
            recentAppBar.Buttons.Add(selectItems);
            favAppBar.Buttons.Add(selectItems);

            deleteItems = new ApplicationBarIconButton();
            deleteItems.IconUri = new Uri("/Assets/AppBar1/delete.png", UriKind.Relative);
            deleteItems.Text = "delete";
            deleteItems.IsEnabled = false;
            deleteItems.Click += deleteItems_Click;

            deleteItems.IconUri = new Uri("/Assets/AppBar1/delete.png", UriKind.Relative);
            deleteItems.Text = "delete";
            deleteItems.IsEnabled = false;
            deleteItems.Click += deleteItems_Click;
        }

        void shareAll_Click(object sender, EventArgs e)
        {
            App.vcards = new List<VCard>();
            foreach (ContactModel user in App.ViewModel.ContactItems)
            {
                App.vcards.Add(
                    new VCard
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        Title = user.Title                                               
                    });
            }
            this.NavigationService.Navigate(new Uri("/ComposeEmail.xaml?parameter=" + "batch", UriKind.Relative));
        }

        void selectItems_Click(object sender, EventArgs e)
        {
            if (CURRENT_PIVOTITEM == "recent")
            {
                recentList.EnforceIsSelectionEnabled = true;
            }
            else
            {
                FavoritesList.EnforceIsSelectionEnabled = true;
            }
            
        }

        void selectAll_Click(object sender, EventArgs e)
        {
            if (recentList.EnforceIsSelectionEnabled)
            {
                List<RecentContact> users = new List<RecentContact>();

                foreach (RecentContact user in recentList.ItemsSource)
                {
                    users.Add(user);
                }

                foreach (RecentContact user in users)
                {
                    recentList.SelectedItems.Add(user);
                }
            }
            else
            {
                List<ContactModel> users = new List<ContactModel>();

                foreach (ContactModel user in FavoritesList.ItemsSource)
                {
                    users.Add(user);
                }

                foreach (ContactModel user in users)
                {
                    FavoritesList.SelectedItems.Add(user);
                }
            }
        }

        void deleteItems_Click(object sender, EventArgs e)
        {
            if (recentList.EnforceIsSelectionEnabled)
            {
                List<RecentContact> users = new List<RecentContact>();

                foreach (RecentContact user in recentList.ItemsSource)
                {
                    users.Add(user);
                }

                foreach (RecentContact user in users)
                {
                    App.RecentList.Remove(user);
                }
                recentList.EnforceIsSelectionEnabled = false;
            }
            else
            {
                List<ContactModel> users = new List<ContactModel>();

                foreach (ContactModel user in FavoritesList.SelectedItems)
                {
                    if (App.FavoritesList.Contains(user))
                    {
                        users.Add(user);
                    }
                }

                foreach (ContactModel user in users)
                {
                    App.FavoritesList.Remove(user);
                }
                FavoritesList.EnforceIsSelectionEnabled = false;
            }
           
        }

        void deleteList_Click(object sender, EventArgs e)
        {
            var userResult = App.ViewModel.ContactItems.Where(f => f.Favorite == true);
            foreach (ContactModel user in userResult)
            {
                user.Favorite = false;
            }
            App.FavoritesList.Clear();

        }

        private void AddNewContactButton_Click(object sender, EventArgs e)
        {
            SaveContactTask sct = new SaveContactTask();
            sct.Completed += saveContactTask_Completed;
            sct.Show();
        }

        private void PrefButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void SearchForContactButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        private void saveContactTask_Completed(object sender, SaveContactResult e)
        {
            switch (e.TaskResult)
            {
                case TaskResult.OK:
                    MessageBox.Show("Contact save successfully.");
                    App.ViewModel.LoadData(true);
                    break;
                case TaskResult.Cancel:
                    MessageBox.Show("Save Cancelled.");
                    break;
                case TaskResult.None:
                    MessageBox.Show("Contact could not be saved.");
                    break;
            }
        }

        private void DrawLongListSelector()
        {
           
            // Uses Headers or not
            if (settings.UsesGridSetting)
            {
                ContactList.LayoutMode = LongListSelectorLayoutMode.Grid;
                ContactList.GridCellSize = new Size(150, 150);
                ContactList.ItemTemplate = ContactListGrid;
                // Header
                ContactList.ItemsSource = App.ViewModel.DataSource;
                ContactList.GroupHeaderTemplate = Resources["AddrBookHeaderTemplate"] as DataTemplate;
                ContactList.JumpListStyle = Resources["AddrBookJumpListStyle"] as Style;
                ContactList.IsGroupingEnabled = true;

                if (!settings.UsesHeaderSetting)
                {
                    ContactList.GroupHeaderTemplate = null;
                    ContactList.JumpListStyle = null;
                    ContactList.IsGroupingEnabled = false;
                    ContactList.ItemsSource = App.ViewModel.ContactItems;
                }
            }
            else if (!settings.UsesGridSetting)
            {
                ContactList.LayoutMode = LongListSelectorLayoutMode.List;
                ContactList.ItemTemplate = Resources["AddrBookItemTemplate"] as DataTemplate;
                ContactList.GroupHeaderTemplate = Resources["AddrBookHeaderTemplate"] as DataTemplate;
                ContactList.JumpListStyle = Resources["AddrBookJumpListStyle"] as Style;
                ContactList.ItemsSource = App.ViewModel.DataSource;
                ContactList.IsGroupingEnabled = true;

                if (!settings.UsesHeaderSetting)
                {
                    ContactList.GroupHeaderTemplate = null;
                    ContactList.JumpListStyle = null;
                    ContactList.IsGroupingEnabled = false;
                    ContactList.ItemsSource = App.ViewModel.ContactItems;
                }
            }
        }

        private void SetGlobalColors()
        {
            App.Background_Color = Color.FromArgb(255,220,84,82);
            App.Foreground_Color = Colors.White;
            App.AppBarColor = Color.FromArgb(255, 53, 75, 96);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.ViewModel.LoadData(true);

            base.OnNavigatedTo(e);
            SlideTransition transition = new SlideTransition();
            transition.Mode = SlideTransitionMode.SlideRightFadeIn;
            PhoneApplicationPage page = (PhoneApplicationPage)((PhoneApplicationFrame)Application.Current.RootVisual).Content;
            ITransition trans = transition.GetTransition(page);
            trans.Completed += delegate
            {

                RefreshList();
                DrawLongListSelector();
                trans.Stop();
            };
            trans.Begin();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {            
            App.ViewModel.SaveFavorites("Favorites");
            if (App.FAVORITE_LENGTH < App.FavoritesList.Count())
            {
                //App.ViewModel.SaveFavorites("Contacts");
            }
            if (App.FirstVisit == true)
            {

                //App.ViewModel.SaveFavorites("Contacts");
            }
            App.FirstVisit = false;
        }
        private void RefreshList()
        {
            App.ViewModel.LoadFavorites("Favorites");
            FavoritesList.ItemsSource = App.FavoritesList;            
            App.FAVORITE_LENGTH = App.FavoritesList.Count();
        }

        private void ContactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContactList.SelectedItem == null) return;
            if (SAVING_FAVORITE)
            {
                SAVING_FAVORITE = false;
                return;
            }
            LongListSelector list = sender as LongListSelector;
            ContactModel item = list.SelectedItem as ContactModel;

            this.NavigationService.Navigate(new Uri("/UserPage.xaml?msg=" + item.FullName, UriKind.Relative));
        }
       
        private void MainPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot main = sender as Pivot;
            PivotItem item = main.SelectedItem as PivotItem;

            switch (item.Header.ToString())
            {
                case "contact list":
                    ApplicationBar = mainAppBar;
                    CURRENT_PIVOTITEM = "contact list";
                    break;

                case "favorites":
                    ApplicationBar = favAppBar;
                    CURRENT_PIVOTITEM = "favorites";
                    break;
                case "recent":
                    ApplicationBar = recentAppBar;
                    CURRENT_PIVOTITEM = "recent";
                    break;
            }
        }

        private void ContactList_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ContextMenu menu = new ContextMenu();

        }

        private void ContactItemFav_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContactModel itemModel = item.DataContext as ContactModel;
            App.FavoritesList.Add(itemModel);
        }

        private void ContactItemDelete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContactModel itemModel = item.DataContext as ContactModel;
            App.FavoritesList.Remove(itemModel);
        }

        private void ContactItemName_Click(object sender, RoutedEventArgs e)
        {

        }

         private void ContactItemNumber_Click(object sender, RoutedEventArgs e)
         {

         }

         private void ContactItemBlack_Click(object sender, RoutedEventArgs e)
         {

         }

        private void ListPinch_PinchCompleted_1(object sender, PinchGestureEventArgs e)
        {
            //MessageBox.Show("Pinch COMPLETED");
            ContactList.LayoutMode = LongListSelectorLayoutMode.Grid;
            ContactList.GridCellSize = new Size(150, 150);
            ContactList.ItemTemplate = ContactListGrid;

            // Uses Headers or not


        }

        private void FavoritesItem_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void ContactList_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {

        }

        private void ContactList_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                var transform = (CompositeTransform)ContactList.RenderTransform;
            }
        }

        private void favIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
            Grid myGrid = sender as Grid;
            ContactModel itemModel = myGrid.DataContext as ContactModel;
            Ellipse favEl = myGrid.FindName("elipse") as Ellipse;

            foreach (ContactModel fav in App.FavoritesList)
            {
                if (fav.FullName == itemModel.FullName)
                {
                    SAVING_FAVORITE = true;
                    itemModel.Favorite = false;
                    App.FavoritesList.Remove(fav);
                    return;
                }
            }

            itemModel.Favorite = true;
            App.FavoritesList.Add(itemModel);                        
            SAVING_FAVORITE = true;
        }

        private void FavoriteQuickAdd_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid myGrid = sender as Grid;
            ContactModel itemModel = myGrid.DataContext as ContactModel;
            Ellipse favEl = myGrid.FindName("elipse") as Ellipse;

            var search = App.FavoritesList.Where(f => f.FullName == itemModel.FullName);
            if (search.Count() > 0)
            {
                SAVING_FAVORITE = true;
                return;
            }

            itemModel.Favorite = true;
            App.FavoritesList.Add(itemModel);
            favEl.Fill = new SolidColorBrush(Color.FromArgb(100, 0, 102, 153));
            SAVING_FAVORITE = true;

        }

        private void recentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (recentList.SelectedItems.Count > 0)
            {
                deleteItems.IsEnabled = true;
            }
            
            else
            {
                deleteItems.IsEnabled = false;
            }
        }

        private void FavoritesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 0)
            {
                deleteItems.IsEnabled = true;
            }

            else if (FavoritesList.SelectedItems.Count == 0)
            {
                deleteItems.IsEnabled = false;
                FavoritesList.EnforceIsSelectionEnabled = false;
            }
        }

        private void FavoritesList_IsSelectionEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.Equals(true))
            {
                favAppBar.Buttons.Add(deleteItems);
                favAppBar.MenuItems.Add(selectAll);
                favAppBar.Buttons.Remove(selectItems);
            }
            else
            {
                
                favAppBar.Buttons.Remove(deleteItems);
                favAppBar.Buttons.Add(selectItems);
                favAppBar.MenuItems.Remove(selectAll);
            }


        }

        private void recentList_IsSelectionEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.Equals(true))
            {
                recentAppBar.Buttons.Add(deleteItems);
                recentAppBar.MenuItems.Add(selectAll);
                recentAppBar.Buttons.Remove(selectItems);
            }
            else
            {
                recentList.EnforceIsSelectionEnabled = false;
                recentAppBar.Buttons.Remove(deleteItems);
                recentAppBar.Buttons.Add(selectItems);
                recentAppBar.MenuItems.Remove(selectAll);
            }
        }

        private void phoneCallIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid myGrid = sender as Grid;
            ContactModel itemModel = myGrid.DataContext as ContactModel;

            PhoneCallTask task = new PhoneCallTask();
            task.DisplayName = itemModel.FullName;
            task.PhoneNumber = itemModel.PhoneNumber;
            task.Show();

            App.RecentList.Add
                (new RecentContact
                {
                    PhoneNumber = itemModel.PhoneNumber,
                    FullName = itemModel.FullName,
                    Type = "Phone",
                    Time = DateTime.Now
                });   
        }

        private void phoneCallRecent_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid myGrid = sender as Grid;
            RecentContact itemModel = myGrid.DataContext as RecentContact;

            PhoneCallTask task = new PhoneCallTask();
            task.DisplayName = itemModel.FullName;
            task.PhoneNumber = itemModel.PhoneNumber;
            task.Show();

            App.RecentList.Add
                (new RecentContact
                {
                    PhoneNumber = itemModel.PhoneNumber,
                    FullName = itemModel.FullName,
                    Type = "Phone",
                    Time = DateTime.Now
                });
        }


    }
}