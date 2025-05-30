using System;
using System.Security.Cryptography;
using System.Text;

namespace KR.MBE.CommonLibrary.Utils
{
    public class AES
    {
        private static string m_sKey = "!#$TG!Q$#RGASEDA";
        public static string getEncryptString(string strPassword)
        {
            return EncryptString(strPassword, m_sKey);
        }

        public static string getDecryptString(string strEncryptPassword)
        {
            return DecryptString(strEncryptPassword, m_sKey);
        }

        #region private Function

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInputText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string DecryptString(string strInputText, string key)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;

            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;

            byte[] encryptedData = Convert.FromBase64String(strInputText);
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }

            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);

            return Encoding.UTF8.GetString(plainText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInputText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string EncryptString(string strInputText, string key)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;

            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(strInputText);

            return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
        }

        #endregion
    }

    public class EncryptionHelper
    {
        public static string EncryptionSHA256(string message)
        {
            //입력받은 문자열을 바이트배열로 변환
            byte[] array = Encoding.Default.GetBytes(message);
            byte[] hashValue;
            string result = string.Empty;

            //바이트배열을 암호화된 32byte 해쉬값으로 생성
            using (SHA256 mySHA256 = SHA256.Create())
            {
                hashValue = mySHA256.ComputeHash(array);
            }
            //32byte 해쉬값을 16진수로변환하여 64자리로 만듬
            for (int i = 0; i < hashValue.Length; i++)
            {
                result += hashValue[i].ToString("x2");
            }
            return result;
        }
    }
}
