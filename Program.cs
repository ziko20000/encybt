using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
namespace ConsoleApp3
{

    class Program
    {




        public static string DoEncrypt(string unencryptedString)
        {
            string encryptedString = "";
            unencryptedString = new string(unencryptedString.ToCharArray().Reverse().ToArray());
            foreach (char character in unencryptedString.ToCharArray())
            {
                string randomizationSeed = (encryptedString.Length > 0) ? unencryptedString.Substring(0, encryptedString.Length) : "";
                encryptedString += GetRandomSubstitutionArray(randomizationSeed)[int.Parse(character.ToString())];
            }

            return Shuffle(encryptedString);
        }

        public static string DoDecrypt(string encryptedString)
        {
            // Unshuffle the string first to make processing easier.
            encryptedString = Unshuffle(encryptedString);

            string unencryptedString = "";
            foreach (char character in encryptedString.ToCharArray().ToArray())
                unencryptedString += GetRandomSubstitutionArray(unencryptedString).IndexOf(int.Parse(character.ToString()));

            // Reverse string since encrypted string was reversed while processing.
            return new string(unencryptedString.ToCharArray().Reverse().ToArray());
        }

        private static string Shuffle(string unshuffled)
        {
            char[] unshuffledCharacters = unshuffled.ToCharArray();
            char[] shuffledCharacters = new char[12];
            shuffledCharacters[0] = unshuffledCharacters[2];
            shuffledCharacters[1] = unshuffledCharacters[7];
            shuffledCharacters[2] = unshuffledCharacters[10];
            shuffledCharacters[3] = unshuffledCharacters[5];
            shuffledCharacters[4] = unshuffledCharacters[3];
            shuffledCharacters[5] = unshuffledCharacters[1];
            shuffledCharacters[6] = unshuffledCharacters[0];
            shuffledCharacters[7] = unshuffledCharacters[4];
            shuffledCharacters[8] = unshuffledCharacters[8];
            shuffledCharacters[9] = unshuffledCharacters[11];
            shuffledCharacters[10] = unshuffledCharacters[6];
            shuffledCharacters[11] = unshuffledCharacters[9];
            return new string(shuffledCharacters);
        }

        private static string Unshuffle(string shuffled)
        {
            char[] shuffledCharacters = shuffled.ToCharArray();
            char[] unshuffledCharacters = new char[12];
            unshuffledCharacters[0] = shuffledCharacters[6];
            unshuffledCharacters[1] = shuffledCharacters[5];
            unshuffledCharacters[2] = shuffledCharacters[0];
            unshuffledCharacters[3] = shuffledCharacters[4];
            unshuffledCharacters[4] = shuffledCharacters[7];
            unshuffledCharacters[5] = shuffledCharacters[3];
            unshuffledCharacters[6] = shuffledCharacters[10];
            unshuffledCharacters[7] = shuffledCharacters[1];
            unshuffledCharacters[8] = shuffledCharacters[8];
            unshuffledCharacters[9] = shuffledCharacters[11];
            unshuffledCharacters[10] = shuffledCharacters[2];
            unshuffledCharacters[11] = shuffledCharacters[9];
            return new string(unshuffledCharacters);
        }

        public static string DoPrefixCipherEncrypt(string strIn, byte[] btKey)
        {
            if (strIn.Length < 1)
                return strIn;

            // Convert the input string to a byte array 
            byte[] btToEncrypt = System.Text.Encoding.Unicode.GetBytes(strIn);
            RijndaelManaged cryptoRijndael = new RijndaelManaged();
            cryptoRijndael.Mode =
            CipherMode.ECB;//Doesn't require Initialization Vector 
            cryptoRijndael.Padding =
            PaddingMode.PKCS7;


            // Create a key (No IV needed because we are using ECB mode) 
            ASCIIEncoding textConverter = new ASCIIEncoding();

            // Get an encryptor 
            ICryptoTransform ictEncryptor = cryptoRijndael.CreateEncryptor(btKey, null);


            // Encrypt the data... 
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, ictEncryptor, CryptoStreamMode.Write);


            // Write all data to the crypto stream to encrypt it 
            csEncrypt.Write(btToEncrypt, 0, btToEncrypt.Length);
            csEncrypt.Close();


            //flush, close, dispose 
            // Get the encrypted array of bytes 
            byte[] btEncrypted = msEncrypt.ToArray();


            // Convert the resulting encrypted byte array to string for return 
            return (Convert.ToBase64String(btEncrypted));
        }

        private static List<int> GetRandomSubstitutionArray(string number)
        {
            // Pad number as needed to achieve longer key length and seed more randomly.
            // NOTE I didn't want to make the code here available and it would take too longer to clean, so I'll tell you what I did. I basically took every number seed that was passed in and prefixed it and  postfixed it with some values to make it 16 characters long and to get a more unique result. For example:
            // if (number.Length = 15)
            //    number = "Y" + number;
            // if (number.Length = 14)
            //    number = "7" + number + "z";
            // etc - hey I already said this is a hack ;)

            // We pass in the current number as the password to an AES encryption of each of the
            // digits 0 - 9. This returns us a set of values that we can then sort and get a 
            // random order for the digits based on the current state of the number.
            Dictionary<string, int> prefixCipherResults = new Dictionary<string, int>();
            for (int ndx = 0; ndx < 10; ndx++)
                prefixCipherResults.Add(DoPrefixCipherEncrypt(ndx.ToString(), Encoding.UTF8.GetBytes(number)), ndx);

            // Order the results and loop through to build your int array.
            List<int> group = new List<int>();
            foreach (string key in prefixCipherResults.Keys.OrderBy(k => k))
                group.Add(prefixCipherResults[key]);

            return group;
        }
        static void Main(string[] args)


        {

           
        }



    }
}






