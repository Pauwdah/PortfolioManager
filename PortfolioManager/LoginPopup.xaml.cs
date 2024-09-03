using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PortfolioManager
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginPopup : Window
    {

        public LoginPopup()
        {
            InitializeComponent();

            try
            {
                HostTextBox.Text = RetrieveAndDecryptHost();
                UsernameTextBox.Text = RetrieveAndDecryptUsername();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void StoreEncryptedUserCredentialsInRegistry()
        {
            byte[] entropy = new byte[20];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }
            byte[] cipherHost = ProtectedData.Protect(Encoding.UTF8.GetBytes(HostTextBox.Text), entropy, DataProtectionScope.CurrentUser);
            byte[] cipherUser = ProtectedData.Protect(Encoding.UTF8.GetBytes(UsernameTextBox.Text), entropy, DataProtectionScope.CurrentUser);
            byte[] cipherPass = ProtectedData.Protect(Encoding.UTF8.GetBytes(PasswordBox.Password), entropy, DataProtectionScope.CurrentUser);
            // Define the registry path
            string registryPath = @"Software\Pauwdah\PortfolioManager";

            // Open or create the key in HKEY_CURRENT_USER
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath))
            {
                if (key != null)
                {
                    // Store the ciphertext and entropy as binary data
                    key.SetValue("Host", cipherHost, RegistryValueKind.Binary);
                    key.SetValue("Username", cipherUser, RegistryValueKind.Binary);
                    key.SetValue("EncryptedPassword", cipherPass, RegistryValueKind.Binary);
                    key.SetValue("Entropy", entropy, RegistryValueKind.Binary);
                    
                    
                }
            }
        }
        public string RetrieveAndDecryptHost()
        {
            string registryPath = @"Software\Pauwdah\PortfolioManager";

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath))
            {
                if (key != null)
                {
                    byte[] storedCiphertext = (byte[])key.GetValue("Host", null);
                    byte[] storedEntropy = (byte[])key.GetValue("Entropy", null);

                    if (storedCiphertext != null && storedEntropy != null)
                    {
                        // Decrypt the stored host
                        byte[] decryptedHost = ProtectedData.Unprotect(storedCiphertext, storedEntropy, DataProtectionScope.CurrentUser);
                        return Encoding.UTF8.GetString(decryptedHost);
                    }
                }
            }
            return null;
        }
        public string RetrieveAndDecryptUsername()
        {
            string registryPath = @"Software\Pauwdah\PortfolioManager";

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath))
            {
                if (key != null)
                {
                    byte[] storedCiphertext = (byte[])key.GetValue("Username", null);
                    byte[] storedEntropy = (byte[])key.GetValue("Entropy", null);

                    if (storedCiphertext != null && storedEntropy != null)
                    {
                        // Decrypt the stored username
                        byte[] decryptedUsername = ProtectedData.Unprotect(storedCiphertext, storedEntropy, DataProtectionScope.CurrentUser);
                        return Encoding.UTF8.GetString(decryptedUsername);
                    }
                }
            }
            return null;
        }
        public string RetrieveAndDecryptPassword()
        {
            string registryPath = @"Software\Pauwdah\PortfolioManager";

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath))
            {
                if (key != null)
                {
                    byte[] storedCiphertext = (byte[])key.GetValue("EncryptedPassword", null);
                    byte[] storedEntropy = (byte[])key.GetValue("Entropy", null);

                    if (storedCiphertext != null && storedEntropy != null)
                    {
                        // Decrypt the stored password
                        byte[] decryptedPassword = ProtectedData.Unprotect(storedCiphertext, storedEntropy, DataProtectionScope.CurrentUser);
                        return Encoding.UTF8.GetString(decryptedPassword);
                    }
                }
            }
            return null;
        }


        
        

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            StoreEncryptedUserCredentialsInRegistry();
            this.Close();
        }   


    }
}
