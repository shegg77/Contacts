using Buddy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;

namespace ContactProject
{
    internal static class ServiceManager
    {
        public const string BUDDY_APP_NAME = "Contacts";
        public const string BUDDY_APP_KEY = "307B372A-23E4-49E5-BB71-F2C761A30FAC";

        private static BuddyClient m_client;
        public static BuddyClient Client
        {
            get
            {
                if (m_client == null)
                    m_client = new BuddyClient(BUDDY_APP_NAME, BUDDY_APP_KEY);
                return m_client;
                
            }
        }

        public static void EncryptPasscode(string password = null, string token = null)
        {
            byte[] passByteArray = Encoding.UTF8.GetBytes(token);
            

            
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            using (var stream = new IsolatedStorageFileStream("myteryFile.txt",
                                        FileMode.Create, FileAccess.Write, store))
            {
                stream.Write(passByteArray, 0, passByteArray.Length);
            }

            if (password != null)
            {
                byte[] passwordArray = Encoding.UTF8.GetBytes(password);
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                using (var stream = new IsolatedStorageFileStream("AnotherMysteriousFile.txt",
                                            FileMode.Create, FileAccess.Write, store))
                {
                    stream.Write(passwordArray, 0, passwordArray.Length);
                }
            }
        }

        public static string DecryptPasscode(string need)
        {
            int passINT;
            string passSTRING = null;
            byte[] passByteArray;
            byte[] passwordArray;
            string secret; 

            if (need == "Bread")
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists("myteryFile.txt"))
                    {
                        using (IsolatedStorageFileStream stream = store.OpenFile("myteryFile.txt", System.IO.FileMode.Open))
                        {
                            int fileLength = (int)stream.Length;
                            passwordArray = new byte[fileLength];

                            stream.Read(passwordArray, 0, fileLength);
                            secret = Encoding.UTF8.GetString(passwordArray, 0, passwordArray.Length);
                        }
                    }
                    else
                        return null;
                };

                return secret;
            }
            else
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists("AnotherMysteriousFile.txt"))
                    {
                        using (IsolatedStorageFileStream stream = store.OpenFile("AnotherMysteriousFile.txt", System.IO.FileMode.Open))
                        {
                            int fileLength = (int)stream.Length;
                            passByteArray = new byte[fileLength];

                            stream.Read(passByteArray, 0, fileLength);
                            passSTRING = Encoding.UTF8.GetString(passByteArray, 0, passByteArray.Length);
                        }
                    }
                    else
                        return null;
                };

                return passSTRING;
            }
        }

       

        //Buddy Authenticated user
        public static AuthenticatedUser User;

    }
}
