using Microsoft.Phone.UserData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace ContactProject
{
    public class ModelView
    {
        public ModelView()
        {
            this.ContactItems = new ObservableCollection<ContactModel>();
        }

        public ObservableCollection<ContactModel> ContactItems { get; private set; }
        public ObservableCollection<AlphaKeyGroup<ContactModel>> DataSource { get; set; }
        public AppSettings settings = new AppSettings();

        public void LoadFavorites(string saver)
        {            
            if (saver == "Contacts")
            {
                App.ViewModel.ContactItems.Clear();
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists("ContactList.xml"))
                    {
                        using (IsolatedStorageFileStream stream = store.OpenFile("ContactList.xml", System.IO.FileMode.Open))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<ContactModel>));
                            App.ViewModel.ContactItems = (ObservableCollection<ContactModel>)serializer.Deserialize(stream);
                        }
                    }
                }
            }
            if (saver == "Favorites")
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists("Favorites.xml"))
                    {
                        using (IsolatedStorageFileStream stream = store.OpenFile("Favorites.xml", System.IO.FileMode.Open))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<ContactModel>));
                            App.FavoritesList = (ObservableCollection<ContactModel>)serializer.Deserialize(stream);
                        }
                    }
                    App.FAVORITE_LENGTH = App.FavoritesList.Count();
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData(bool refresh)
        {
            ContactItems.Clear();
            settings.IsLoadingContentSetting = true;
            Contacts cons = new Contacts();

            LoadFavorites("Favorites");
            cons.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(ContactsSearch_Completed);
            cons.SearchAsync(String.Empty, FilterKind.None," Test" );
            
        }

        public void SaveFavorites(string saver)
        {
            if (saver == "Contacts")
            {
                if (App.FavoritesList != null)
                {
                    using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream stream = store.CreateFile("ContactList.xml"))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<ContactModel>));
                            serializer.Serialize(stream, App.ViewModel.ContactItems);
                        }
                    }
                }
                
            }
            if (saver == "Favorites")
            {
                if (App.FAVORITE_LENGTH == App.FavoritesList.Count()) return;

                if (App.FavoritesList != null)
                {
                    using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream stream = store.CreateFile("Favorites.xml"))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<ContactModel>));
                            serializer.Serialize(stream, App.FavoritesList);
                        }
                    }
                }
            }
        }

        private void ContactsSearch_Completed(object sender, ContactsSearchEventArgs e)
        {            
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            string fullName = null;
            string PhoneNumber = null;
            string firstName = null;
            string lastName = null;
            string title = null;
            string location = null;
            string email = null;

            bool favorite = false;
            VCard card = new VCard();

            foreach (Contact con in e.Results)                
            {                
                if (con.CompleteName == null)
                {
                    firstName = con.DisplayName;
                    lastName = String.Empty;
                }
                else if (con.CompleteName.FirstName != null && con.CompleteName.LastName != null)
                {
                    firstName = con.CompleteName.FirstName;
                    lastName = con.CompleteName.LastName;
                    if (con.CompleteName.Title != null)
                    {
                        title = con.CompleteName.Title;
                    }
                }
                else if (con.CompleteName.FirstName != null)
                {
                    firstName = con.CompleteName.FirstName;
                    lastName = String.Empty;
                    if (con.CompleteName.Title != null)
                    {
                        title = con.CompleteName.Title;
                    }
                    
                }
                else if (con.CompleteName.LastName != null)
                {
                    firstName = String.Empty;
                    lastName = con.CompleteName.LastName;
                    if (con.CompleteName.Title != null)
                    {
                        title = con.CompleteName.Title;
                    }
                }

                if (title == null)
                {
                    title = String.Empty;
                }
                fullName = card.CleanName(firstName, lastName);

                var userResult = App.FavoritesList.Where(f => f.FullName == fullName);
                if (userResult != null && userResult.Count() > 0)
                {
                    favorite = true;
                }

                if (con.PhoneNumbers.Count() != 0)
                {
                    PhoneNumber = con.PhoneNumbers.First().ToString();
                    
                    PhoneNumber = card.CleanPhoneNumber(PhoneNumber);
                }
                else
                {
                    PhoneNumber = String.Empty;
                }

                List<String> tmpEmails = new List<String>();

                if (con.EmailAddresses.Count() > 0)
                {
                    foreach (ContactEmailAddress userEmail in con.EmailAddresses)
                    {
                        if (userEmail.EmailAddress != null)
                        {
                            
                            tmpEmails.Add(userEmail.EmailAddress);
                        }
                    }
                }

                if (con.Accounts.First().Kind.ToString() == "Facebook")
                {
                    if (!settings.UsesFacebookSetting)
                        continue;
                    PhoneNumber = "Facebook";
                }

                if (con.Accounts.First().Kind.ToString() == "Skype")
                {
                    if (!settings.UsesSkypeSetting)
                        continue;

                    PhoneNumber = "Skype";
                }

                BitmapImage bitPic = new BitmapImage();
                
                if (con.GetPicture() == null)
                {                   
                    MemoryStream ms = new MemoryStream();
                    ms.Write(Resources.AppResources.EmptyPic, 0, Resources.AppResources.EmptyPic.Length);
                    bitPic.SetSource(ms);

                    ContactItems.Add(new ContactModel 
                    {   PhoneNumber = PhoneNumber,
                        FirstName = firstName,
                        LastName = lastName,
                        Key = fullName[0],
                        Email = tmpEmails,
                        FullName = fullName,
                        Image = bitPic,
                        Favorite = favorite,
                        Title = title
                        });
                }
                else
                {
                    bitPic.SetSource(con.GetPicture());
                    ContactItems.Add(new ContactModel
                    {
                        Key = fullName[0],
                        FirstName = firstName,
                        LastName = lastName,
                        Email = tmpEmails,
                        PhoneNumber = PhoneNumber,
                        FullName = fullName,
                        Image = bitPic,
                        Favorite = favorite,
                        Title = title
                    });
                }
                favorite = false;
            }
            IsDataLoaded = true;
            App.ViewModel.DataSource = AlphaKeyGroup<ContactModel>.CreateGroups(App.ViewModel.ContactItems,
            System.Threading.Thread.CurrentThread.CurrentUICulture, s => s.FullName, true);
            
        }
    }
}
