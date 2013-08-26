using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace ContactProject
{
    public class ContactModel : INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private string _fullName;
        private string _phoneNumber;
        private string _location;
        private string _title;
        private bool _favorite;
        private BitmapImage _favoriteIcon;
        private char _key;
        private List<String> _email;
        
        [XmlIgnore]
        public BitmapImage Image { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("Image")]
        public byte[] ImageSerialized
        {
            get
            {
                if (Image == null) return null;
                using (MemoryStream ms = new MemoryStream())
                {
                    
                    
                    WriteableBitmap btmMap = new WriteableBitmap
                    (Image);
                    

                    btmMap.SaveJpeg(ms, Image.PixelWidth, Image.PixelHeight,0, 100);
                    return ms.ToArray();
                }
            }
            

            set
            {
                if (value == null)
                {
                    Image = null;
                }
                else
                {
                    Image = new BitmapImage();
                    

                    MemoryStream ms = new MemoryStream();
                    ms.Write(value, 0, value.Length);
                    Image.SetSource(ms);
                    OnPropertyChanged("Image");
                }
            }
        }


        public BitmapImage FavoriteIcon
        {
            get
            {
                if ((Visibility)App.Current.Resources["PhoneDarkThemeVisibility"]
                    == Visibility.Visible)
                {
                    _favoriteIcon = new BitmapImage(new Uri("/Assets/AppBar1/add.png", UriKind.Relative));
                    return _favoriteIcon;
                }
                else
                {
                    _favoriteIcon = new BitmapImage(new Uri("/Assets/AppBar1/add_dark.png", UriKind.Relative));
                    return _favoriteIcon;
                }
            }
        }
        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (value == null) { return; }
                _fullName = value;
                OnPropertyChanged("FullName");
            }
        }

        public char Key
        {
            get { return _key; }
            set
            {
                if (value == null) { return; }
                _key = value;
                OnPropertyChanged("Key");
            }
        }
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (value == null) { return; }
                _firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (value == null) { return; }
                _lastName = value;
                OnPropertyChanged("LastName");
            }
        }
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                if (value == null) { return; }
                _phoneNumber = value;
                OnPropertyChanged("PhoneNumber");
            }
        }
        public string Location
        {
            get { return _location; }
            set
            {
                if (value == null) { return; }
                _location = value;
                OnPropertyChanged("Location");
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == null) { return; }
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public bool Favorite
        {
            get { return _favorite; }
            set
            {
                if (value == null) { return; }
                _favorite = value;
                OnPropertyChanged("Favorite");
            }

        }
        public List<String> Email
        {
            get { return _email; }
            set
            {
                if (value == null) { return; }
                _email = value;
                OnPropertyChanged("Email");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
