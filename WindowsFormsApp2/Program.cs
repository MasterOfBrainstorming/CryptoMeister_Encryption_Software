using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace WindowsFormsApp2
{
    static class Program
    {
        //Static initializing vector 16characters
        private static string iv = "OKw8YOZ9xkb1pujs";
        //Static key 32characters
        private static string key = "nGdNi28h2Ts7p5w1GSZpafXjlSKp9JrC";
        private static string mesg;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        /// <summary>
        /// Function that defines if encryption or decryption and what kind of algorithm
        /// </summary>
        /// <param name="file">file is users chosen file</param>
        /// <param name="aes">program defined boolean</param>
        /// <param name="tdes">program defined boolean</param>
        /// <param name="rsa">program defined boolean</param>
        /// <param name="encrypt">program defined boolean</param>
        /// <param name="decrypt">program defined boolean</param>
        /// <returns>the encrypted or decrypted message back to form where it will be saved to a user defined location</returns>
        public static string RunAlgorithm(string file, bool aes, bool tdes, bool rsa, bool encrypt, bool decrypt)
        {
            if (encrypt == true)
            {
                if (aes == true)
                { mesg = AESEncryption(file);}
                else if (tdes == true)
                {mesg = TDESEncryption(file);}
                else if (rsa == true)
                {mesg = RC2Encryption(file);}
            }
            else if (decrypt == true)
            {
                if (aes == true)
                {mesg = AESDecryption(file);}
                else if (tdes == true)
                {mesg = TDESDecryption(file);}
                else if (rsa == true)
                {mesg = RC2Decryption(file);}
            }return mesg;
        }

        /// <summary>
        /// Function to encrypt file to RC2
        /// </summary>
        /// <param name="file">Takes user opened file</param>
        /// <returns>encrypted text</returns>
        public static string RC2Encryption(string file)
        {
            //Try to run the encryption
            try
            {
                //We change the file string to a byte array
                byte[] bytes = Encoding.ASCII.GetBytes(file);
                //Hashing the key to MD5
                MD5CryptoServiceProvider mD5CryptoService = new MD5CryptoServiceProvider();
                byte[] hashedkey = mD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                mD5CryptoService.Clear();
                //Starting rc2crypto provider, for defining the ciphering mode and padding used
                RC2CryptoServiceProvider rC2CryptoServiceProvider = new RC2CryptoServiceProvider();
                //key size to max
                rC2CryptoServiceProvider.KeySize = 128;
                //block size to max
                rC2CryptoServiceProvider.BlockSize = 64;
                //We used CDC (Code Blokc Chaining)
                rC2CryptoServiceProvider.Mode = CipherMode.CBC;
                //Padding mode PKCS7 which is the commonly used padding
                rC2CryptoServiceProvider.Padding = PaddingMode.PKCS7;
                //We transform into crypted text
                ICryptoTransform cryptoTransform = rC2CryptoServiceProvider.CreateEncryptor(hashedkey, Encoding.ASCII.GetBytes(iv));
                //Crpyted text to byte array
                byte[] encryption = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
                //Dispose of the crypted text
                cryptoTransform.Dispose();
                //retunrn encrypted back to 
                return Convert.ToBase64String(encryption);
            }
            //try to catch Exceptions and pop up a message box if error occurs
            catch (IOException ioException)
            {
                MessageBox.Show("Input output error!", "IOException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException unAuthorizedAccess)
            {
                MessageBox.Show("Unauthorized file in use!", "UnauthorizedAccessException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (CryptographicException cryptoGraphicException)
            {
                MessageBox.Show("Cryptographic error!", "CryptographicException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MessageBox.Show("Wrong type of file!", "File ERROR",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "";
        }

        /// <summary>
        /// Decrypts users opened file and the returns the value back for saving to a file
        /// </summary>
        /// <param name="file">users chosen file</param>
        /// <returns>decrypted text</returns>
        //Same thing as RC2Encryption but for decryption
        public static string RC2Decryption(string file)
        {
            try
            {
                //file to bytes
                byte[] bytes = Convert.FromBase64String(file);
                //hashing key
                MD5CryptoServiceProvider mD5CryptoService = new MD5CryptoServiceProvider();
                byte[] hashedkey = mD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //clearing key
                mD5CryptoService.Clear();
                //padding, mode ...
                RC2CryptoServiceProvider rC2CryptoServiceProvider = new RC2CryptoServiceProvider();
                //key size to max
                rC2CryptoServiceProvider.KeySize = 128;
                //block size to max
                rC2CryptoServiceProvider.BlockSize = 64;
                //CBC code block chaining
                rC2CryptoServiceProvider.Mode = CipherMode.CBC;
                //PKCS7 padding
                rC2CryptoServiceProvider.Padding = PaddingMode.PKCS7;
                ICryptoTransform cryptoTransform = rC2CryptoServiceProvider.CreateDecryptor(hashedkey, Encoding.ASCII.GetBytes(iv));
                //decryprted bytes to byte array
                byte[] decryption = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
                //disposing of the decrypter
                cryptoTransform.Dispose();
                //return decrypted string
                return ASCIIEncoding.ASCII.GetString(decryption);
            }
            // Catching exceptions
            catch (IOException ioException)
            {
                MessageBox.Show("Input output error!", "IOException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException unAuthorizedAccess)
            {
                MessageBox.Show("Unauthorized file in use!", "UnauthorizedAccessException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (CryptographicException cryptoGraphicException)
            {
                MessageBox.Show("Cryptographic error!", "CryptographicException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MessageBox.Show("Wrong type of file!", "File ERROR",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "";
        }

        /// <summary>
        /// Encrypts with Triple Des and returns encrypted message back to text file
        /// </summary>
        /// <param name="file">user defined file</param>
        /// <returns>encrypted TDES</returns>
        public static string TDESEncryption(string file)
        {
            try
            {
                // converting to bytes
                byte[] bytes = UTF8Encoding.UTF8.GetBytes(file);
                //hashing key to md5
                MD5CryptoServiceProvider mD5CryptoService = new MD5CryptoServiceProvider();
                byte[] hashedkey = mD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                mD5CryptoService.Clear();

                TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
                //defining max block size, keysize, key and padding
                //max block size
                tripleDES.BlockSize = 64;
                //max key size
                tripleDES.KeySize = 192;
                //encryption key
                tripleDES.Key = hashedkey;
                tripleDES.Mode = CipherMode.CBC;
                tripleDES.Padding = PaddingMode.PKCS7;
                //Encrypting
                ICryptoTransform cryptoTransform = tripleDES.CreateEncryptor();
                //converting to bytes
                byte[] encryption = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
                cryptoTransform.Dispose();
                //returning cipher
                return Convert.ToBase64String(encryption);
            }
            //catch exceptions
            catch (IOException ioException)
            {
                MessageBox.Show("Input output error!", "IOException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException unAuthorizedAccess)
            {
                MessageBox.Show("Unauthorized file in use!", "UnauthorizedAccessException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (CryptographicException cryptoGraphicException)
            {
                MessageBox.Show("Cryptographic error!", "CryptographicException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MessageBox.Show("Wrong type of file!", "File ERROR",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "";
        }
        /// <summary>
        /// Decrypts TDES file
        /// </summary>
        /// <param name="file">user defined</param>
        /// <returns>plaintext</returns>
        public static string TDESDecryption(string file)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(file);
                MD5CryptoServiceProvider mD5CryptoService = new MD5CryptoServiceProvider();
                byte[] hashedkey = mD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                mD5CryptoService.Clear();

                TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
                tripleDES.BlockSize = 64;
                tripleDES.KeySize = 192;
                tripleDES.Key = hashedkey;
                tripleDES.Mode = CipherMode.CBC;
                tripleDES.Padding = PaddingMode.PKCS7;
                // DEcrypting
                ICryptoTransform cryptoTransform = tripleDES.CreateDecryptor();
                byte[] decryption = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
                cryptoTransform.Dispose();
                return UTF8Encoding.UTF8.GetString(decryption);
            }
            // Catching exceptions
            catch (IOException ioException)
            {
                MessageBox.Show("Input output error!", "IOException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException unAuthorizedAccess)
            {
                MessageBox.Show("Unauthorized file in use!", "UnauthorizedAccessException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (CryptographicException cryptoGraphicException)
            {
                MessageBox.Show("Cryptographic error!", "CryptographicException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MessageBox.Show("Wrong type of file!", "File ERROR",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "";
        }


        // DONE

        /// <summary>
        /// function for AES ENCRYPTION
        /// </summary>
        /// <param name="file">
        /// Takes the chosen file text and encrypts it
        /// </param>
        /// <returns>
        /// Encrypted string
        /// </returns>
        public static string AESEncryption(string file)
        {
            try
            {
                //convert to byte array
                byte[] bytes = ASCIIEncoding.ASCII.GetBytes(file);
                AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
                // max block size
                aesCryptoServiceProvider.BlockSize = 128;
                // max key size
                aesCryptoServiceProvider.KeySize = 256;
                // defined key
                aesCryptoServiceProvider.Key = ASCIIEncoding.ASCII.GetBytes(key);
                // defined initializing vector
                aesCryptoServiceProvider.IV = ASCIIEncoding.ASCII.GetBytes(iv);
                // padding used PKCS7
                aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
                // mode used CBC
                aesCryptoServiceProvider.Mode = CipherMode.CBC;
                // Encrypting
                ICryptoTransform cryptoTransform = aesCryptoServiceProvider.CreateEncryptor(aesCryptoServiceProvider.Key, aesCryptoServiceProvider.IV);
                // converting to byte array
                byte[] encryption = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
                // disposing of the encrypted text
                cryptoTransform.Dispose();
                //retrun to forms
                return Convert.ToBase64String(encryption);
            }
            //Catching exceptions
            catch (IOException ioException)
            {
                MessageBox.Show("Input output error!", "IOException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException unAuthorizedAccess)
            {
                MessageBox.Show("Unauthorized file in use!", "UnauthorizedAccessException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (CryptographicException cryptoGraphicException)
            {
                MessageBox.Show("Cryptographic error!", "CryptographicException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MessageBox.Show("Wrong type of file!", "File ERROR",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "";
        }

        /// <summary>
        /// function for AES DECRYPTION
        /// </summary>
        /// <param name="file">
        /// Same thing as AESEncryption function
        /// </param>
        /// <returns>
        /// Decrypted string
        /// </returns>
        public static string AESDecryption(string file)
        {
            try
            {
                // convert file to byte array
                byte[] bytes = Convert.FromBase64String(file);
                AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
                //blocksize
                aesCryptoServiceProvider.BlockSize = 128;
                //keysize
                aesCryptoServiceProvider.KeySize = 256;
                //converting key
                aesCryptoServiceProvider.Key = ASCIIEncoding.ASCII.GetBytes(key);
                //converting initializing vector
                aesCryptoServiceProvider.IV = ASCIIEncoding.ASCII.GetBytes(iv);
                //padding
                aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
                //ciphering mode
                aesCryptoServiceProvider.Mode = CipherMode.CBC;
                //decrypting with key and initializing vector
                ICryptoTransform cryptoTransform = aesCryptoServiceProvider.CreateDecryptor(aesCryptoServiceProvider.Key, aesCryptoServiceProvider.IV);
                //converting back to bytes
                byte[] decryption = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
                //Disposing decryted text
                cryptoTransform.Dispose();
                //retunr plaintext
                return ASCIIEncoding.ASCII.GetString(decryption);
            }
            //catching exceptions
            catch (IOException ioException)
            {
                MessageBox.Show("Input output error!", "IOException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException unAuthorizedAccess)
            {
                MessageBox.Show("Unauthorized file in use!", "UnauthorizedAccessException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (CryptographicException cryptoGraphicException)
            {
                MessageBox.Show("Cryptographic error!", "CryptographicException",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MessageBox.Show("Wrong type of file!", "File ERROR",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "";
        }
    }
}
