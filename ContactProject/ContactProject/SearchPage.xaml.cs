using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace ContactProject
{
    public partial class SearchPage : PhoneApplicationPage
    {
        public ObservableCollection<ContactModel> searchCollection;
        public List<ContactModel> tmpCollection;
        AppSettings settings;
        
        public SearchPage()
        {
            InitializeComponent();
            ContactList.DataContext = App.ViewModel.ContactItems;
            settings = new AppSettings();
            settings.SearchItem = "Full Name";
            txtNarrower.WatermarkText = "Narrow your search for: " + settings.SearchItem;

            tmpCollection = new List<ContactModel>();
            SystemTray.SetForegroundColor(this, App.Foreground_Color);
            SystemTray.SetBackgroundColor(this, App.Background_Color);
            
            
        }

        private void Refresh()
        {
            searchCollection = App.ViewModel.ContactItems;
        }

        private void about_Completed(object sender, PopUpEventArgs<object, PopUpResult> e)
        {
            txtNarrower.WatermarkText = "Narrow your search for: " + settings.SearchItem;
        }

        private void txtNarrower_TextChanged(object sender, TextChangedEventArgs e)
        {         
            if (settings.SearchItem == "First Name")
            {
                SearchByFirstName(txtNarrower.Text);
            }
            else if (settings.SearchItem == "Last Name")
            {
                SearchByLastName(txtNarrower.Text);
            }
            else if (settings.SearchItem == "Full Name")
            {
                SearchByFullName(txtNarrower.Text);
            }
            else if (settings.SearchItem == "Phone Number")
            {
                SearchByPhoneNumber(txtNarrower.Text);
            }
            else if (settings.SearchItem == "Address Name")
            {
                SearchByAddress(txtNarrower.Text);
            }    
        }

        private void SearchByAddress(string p)
        {
            List<ContactModel> modelList = new List<ContactModel>();
            foreach (ContactModel items in App.ViewModel.ContactItems)
            {
                if (items.Location.ToLower().StartsWith(p))
                {
                    modelList.Add(items);
                }
            }

            //App.ViewModel.ContactItems.Clear();
            ContactList.ItemsSource = modelList;
        }

        private void SearchByPhoneNumber(string p)
        {
            List<ContactModel> modelList = new List<ContactModel>();
            foreach (ContactModel items in App.ViewModel.ContactItems)
            {  
                if (items.PhoneNumber == null) { continue; }

                if (items.PhoneNumber.StartsWith("(" + p))
                {
                    modelList.Add(items);
                }
                if (items.PhoneNumber.StartsWith(p))
                {
                    modelList.Add(items);
                }
            }

            //App.ViewModel.ContactItems.Clear();
            ContactList.ItemsSource = modelList;
        }

        private void SearchByLastName(string p)
        {
            List<ContactModel> modelList = new List<ContactModel>();
            foreach (ContactModel items in App.ViewModel.ContactItems)
            {
                if (items.LastName == null) continue;
                if (items.LastName.ToLower().StartsWith(p))
                {
                    modelList.Add(items);
                }
            }

            //App.ViewModel.ContactItems.Clear();
            ContactList.ItemsSource = modelList;
        }

        private void SearchByFirstName(string p)
        {
            List<ContactModel> modelList = new List<ContactModel>();
            foreach (ContactModel items in App.ViewModel.ContactItems)
            {
                if (items.FirstName.ToLower().StartsWith(p))
                {
                    modelList.Add(items);
                }
            }

            //App.ViewModel.ContactItems.Clear();
            ContactList.ItemsSource = modelList;
        }
        private void SearchPrefButton_Click(object sender, EventArgs e)
        {
            AboutPrompt about = new AboutPrompt { Body = new SearchControl(), Title = "Define Search" };
            about.Completed += about_Completed;
            about.Show();
        }

        public void SearchByFullName(string name)
        {
            List<ContactModel> modelList = new List<ContactModel>();
            foreach(ContactModel items in App.ViewModel.ContactItems)
            {
                if (items.FullName.ToLower().StartsWith(name))
                {
                    modelList.Add(items);
                }
            }

            //App.ViewModel.ContactItems.Clear();
            ContactList.ItemsSource = modelList;
        }

        private void ContactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector list = sender as LongListSelector;
            ContactModel item = list.SelectedItem as ContactModel;

            this.NavigationService.Navigate(new Uri("/UserPage.xaml?msg=" + item.FullName, UriKind.Relative));
        }

        private void txtNarrower_Loaded_1(object sender, RoutedEventArgs e)
        {
            txtNarrower.Focus();
        }



        
       

        
    }
}