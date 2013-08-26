using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using Venetasoft.WP.Net;

namespace ContactProject
{
    public class VCard
    {
        public string EmailAddress { get; set; }
        public string EmailPassword { get; set; }
        public string EmailSender { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailType { get; set; }

        public string FirstName { get; set; }
        public string Title { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        private string vCard { get; set; }
        private string ContactEmail { get; set; }
        public byte[] Image { get; set; }


        public string CleanPhoneNumber(string number)
        {
            string FinalNumber = number;
            string seriouslyfinalNumber = null;
            
            FinalNumber = new string(number.Where(Char.IsDigit).ToArray());            
            if (FinalNumber.Count() == 11)
            {
                var tmpNumber = FinalNumber.Substring(1);
                FinalNumber = tmpNumber;
            }
            if (FinalNumber.StartsWith("*") || FinalNumber.Length < 5)
            {
                return FinalNumber;
            }
            
            seriouslyfinalNumber = String.Format(
                "({0}{1}{2}) {3}{4}{5}-{6}{7}{8}{9}",
                FinalNumber[0], FinalNumber[1], FinalNumber[2], FinalNumber[3],
                FinalNumber[4], FinalNumber[5], FinalNumber[6],
                FinalNumber[7], FinalNumber[8], FinalNumber[9]);            
            return seriouslyfinalNumber;
        }

        public string CleanName(string firstName = null, string lastName = null)
        {
            string fullName = null;

            if (firstName != String.Empty)
            {
                char[] a = firstName.ToCharArray();
                a[0] = char.ToUpper(a[0]);

                if (lastName != String.Empty)
                {
                    char[] s = lastName.ToCharArray();
                    s[0] = char.ToUpper(s[0]);
                    fullName = (new String(a) + " " + new string(s));
                    return fullName;
                }

                return new string(a);
                
            }
            else
            {
                char[] a = lastName.ToCharArray();
                a[0] = char.ToUpper(a[0]);
                return new string(a);
            }
        }

        public void FormCard(bool BatchOrNo)
        {
            StringBuilder vCardBuilder = new StringBuilder();

            if (BatchOrNo)
            {
                foreach (VCard card in App.vcards)
                {
                    vCardBuilder.AppendLine("BEGIN:VCARD");
                    vCardBuilder.AppendLine("VERSION:4.0");
                    vCardBuilder.AppendLine("N:" + card.LastName + ";" + card.FirstName + ";;;");
                    vCardBuilder.AppendLine("FN:" + card.FirstName + " " + card.LastName);
                    vCardBuilder.AppendLine("TITLE:" + card.Title);
                    vCardBuilder.AppendLine("TEL;TYPE=CELL:" + card.PhoneNumber);
                    vCardBuilder.AppendLine("END:VCARD");

                }
            }
            else
            {
                vCardBuilder.AppendLine("BEGIN:VCARD");
                vCardBuilder.AppendLine("VERSION:4.0");
                vCardBuilder.AppendLine("N:" + App.vCard.LastName + ";" + App.vCard.FirstName + ";;;");
                vCardBuilder.AppendLine("FN:" + App.vCard.FirstName + " " + App.vCard.LastName);
                vCardBuilder.AppendLine("TITLE:" + App.vCard.Title);
                vCardBuilder.AppendLine("TEL;TYPE=CELL:" + App.vCard.PhoneNumber);
                vCardBuilder.AppendLine("END:VCARD");
            }
            vCard = vCardBuilder.ToString();                       
        }

        public void SendViaEmail(bool BatchOrNo, string to = null, string Subject = null, string Body = null)
        {
            FormCard(BatchOrNo);

            MailMessage emailMessage = new MailMessage();

            emailMessage.UserName = "shegg77@hotmail.com";
            emailMessage.Password = ServiceManager.DecryptPasscode("secret");
            emailMessage.AccountType = MailMessage.accountType.MicrosoftAccount;

            if ((emailMessage.UserName.ToLower()).Contains("gmail"))
            {
                emailMessage.AccountType = MailMessage.accountType.Gmail;
            }
            else if ((emailMessage.UserName.ToLower()).Contains("hotmail"))
            {
                emailMessage.AccountType = MailMessage.accountType.MicrosoftAccount;
            }

            if (to != null)
            {
                emailMessage.From = App.User.Email;
                emailMessage.To = to;
                emailMessage.Subject = Subject;
                emailMessage.Body = Body;
            }
            else
            {
                emailMessage.From = App.User.Email;
                emailMessage.To = App.vCard.EmailAddress;
                emailMessage.Subject = App.vCard.EmailSubject;
                emailMessage.Body = App.vCard.EmailBody;
            }

            if (App.FavoritesList != null)
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    IsolatedStorageFileStream save = new IsolatedStorageFileStream("contact.vcf", System.IO.FileMode.Create, store);
                    StreamWriter writer = new StreamWriter(save);
                    writer.Write(vCard);
                    writer.Close();
                }
            }

            emailMessage.AddAttachment("contact.vcf");
            emailMessage.Error += emailMessage_Error;
            emailMessage.MailSent += emailMessage_MailSent;
            emailMessage.Send();
        }

        void emailMessage_MailSent(object sender, Venetasoft.WP7.ValueEventArgs<bool> e)
        {
            App.vcards = null;
            App.vCard = null;
            
        }

        void emailMessage_Error(object sender, Venetasoft.WP7.ErrorEventArgs e)
        {
            MessageBox.Show("Something went wrong Jim!");
        }

    }
}
