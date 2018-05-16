using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace AssinaturaDigital.Util
{
    public static class HelperCriptografia
    {
        //private static byte[] Key = StringToByteArray(Seguranca.CriptografiaKey, 32);
        //private static byte[] IV = StringToByteArray(Seguranca.CriptografiaIV, 16);

        public static byte[] Criptografar(string TextoConteudo)
        {
            string tempIn = Path.Combine(HelperEstrutura.DirTemp, "tempcriptin" + Guid.NewGuid());
            File.WriteAllText(tempIn, TextoConteudo);

            string tempOut = Path.Combine(HelperEstrutura.DirTemp, "tempcriptout" + Guid.NewGuid());
            //File.Create(tempOut).Close();

            HelperDLLNaoGerenciada.Criptografar(tempIn, tempOut);

            byte[] result = File.ReadAllBytes(tempOut);

            File.Delete(tempIn);
            File.Delete(tempOut);

            return result;
        }

        public static byte[] Criptografar(byte[] Conteudo)
        {
            string texto = Encoding.ASCII.GetString(Conteudo);

            return Criptografar(texto);
        }

        public static string Descriptografar(byte[] ConteudoCriptografado)
        {
            string tempIn = Path.Combine(HelperEstrutura.DirTemp, "tempdescriptin" + Guid.NewGuid());
            File.WriteAllBytes(tempIn, ConteudoCriptografado);

            string tempOut = Path.Combine(HelperEstrutura.DirTemp, "tempdescriptout" + Guid.NewGuid());
            //File.Create(tempOut).Close();

            HelperDLLNaoGerenciada.Descriptografar(tempIn, tempOut);

            string result = File.ReadAllText(tempOut);

            File.Delete(tempIn);
            File.Delete(tempOut);

            return result;
        }

        //public static byte[] Criptografar(string Texto)
        //{
        //    return Criptografar(Texto, Key, IV);
        //}

        //public static string Descriptografar(byte[] ConteudoCriptografado)
        //{
        //    return Descriptografar(ConteudoCriptografado, Key, IV);
        //}

        //private static byte[] Criptografar(string plainText, byte[] Key, byte[] IV)
        //{
        //    // Check arguments.
        //    if (plainText == null || plainText.Length <= 0)
        //        throw new ArgumentNullException("plainText");
        //    if (Key == null || Key.Length <= 0)
        //        throw new ArgumentNullException("Key");
        //    if (IV == null || IV.Length <= 0)
        //        throw new ArgumentNullException("Key");

        //    // Declare the streams used
        //    // to encrypt to an in memory
        //    // array of bytes.
        //    MemoryStream msEncrypt = null;
        //    CryptoStream csEncrypt = null;
        //    StreamWriter swEncrypt = null;

        //    // Declare the RijndaelManaged object
        //    // used to encrypt the data.
        //    RijndaelManaged aesAlg = null;

        //    try
        //    {
        //        // Create a RijndaelManaged object
        //        // with the specified key and IV.
        //        aesAlg = new RijndaelManaged();
        //        aesAlg.Key = Key;
        //        aesAlg.IV = IV;

        //        // Create a decrytor to perform the stream transform.
        //        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        //        // Create the streams used for encryption.
        //        msEncrypt = new MemoryStream();
        //        csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        //        swEncrypt = new StreamWriter(csEncrypt);

        //        //Write all data to the stream.
        //        swEncrypt.Write(plainText);

        //    }
        //    finally
        //    {
        //        // Clean things up.

        //        // Close the streams.
        //        if (swEncrypt != null)
        //            swEncrypt.Close();
        //        if (csEncrypt != null)
        //            csEncrypt.Close();
        //        if (msEncrypt != null)
        //            msEncrypt.Close();

        //        // Clear the RijndaelManaged object.
        //        if (aesAlg != null)
        //            aesAlg.Clear();
        //    }

        //    // Return the encrypted bytes from the memory stream.
        //    return msEncrypt.ToArray();

        //}


        //private static string Descriptografar(byte[] cryptedText, byte[] Key, byte[] IV)
        //{
        //    // Check arguments.
        //    if (cryptedText == null || cryptedText.Length <= 0)
        //        throw new ArgumentNullException("cipherText");
        //    if (Key == null || Key.Length <= 0)
        //        throw new ArgumentNullException("Key");
        //    if (IV == null || IV.Length <= 0)
        //        throw new ArgumentNullException("Key");

        //    // TDeclare the streams used
        //    // to decrypt to an in memory
        //    // array of bytes.
        //    MemoryStream msDecrypt = null;
        //    CryptoStream csDecrypt = null;
        //    StreamReader srDecrypt = null;

        //    // Declare the RijndaelManaged object
        //    // used to decrypt the data.
        //    RijndaelManaged aesAlg = null;

        //    // Declare the string used to hold
        //    // the decrypted text.
        //    string plaintext = null;

        //    try
        //    {
        //        // Create a RijndaelManaged object
        //        // with the specified key and IV.
        //        aesAlg = new RijndaelManaged();
        //        aesAlg.Key = Key;
        //        aesAlg.IV = IV;

        //        // Create a decrytor to perform the stream transform.
        //        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        //        // Create the streams used for decryption.
        //        msDecrypt = new MemoryStream(cryptedText);
        //        csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        //        srDecrypt = new StreamReader(csDecrypt);

        //        // Read the decrypted bytes from the decrypting stream
        //        // and place them in a string.
        //        plaintext = srDecrypt.ReadToEnd();
        //    }
        //    finally
        //    {
        //        // Clean things up.

        //        // Close the streams.
        //        if (srDecrypt != null)
        //            srDecrypt.Close();
        //        if (csDecrypt != null)
        //            csDecrypt.Close();
        //        if (msDecrypt != null)
        //            msDecrypt.Close();

        //        // Clear the RijndaelManaged object.
        //        if (aesAlg != null)
        //            aesAlg.Clear();
        //    }

        //    return plaintext;
        //}

        public static string GerarHashDeArquivo(string CaminhoArquivo)
        {
            byte[] tmpHash;
            using (FileStream fileStream = new FileStream(CaminhoArquivo, FileMode.Open, FileAccess.Read))
            {
                tmpHash = new MD5CryptoServiceProvider().ComputeHash(fileStream);
            }

            string hashedValue = string.Empty;

            foreach (byte b in tmpHash)
                hashedValue += b.ToString("x2");

            return hashedValue;
        }
      


        ///// <summary>
        ///// Cria um byte[] que possuirá o tamanho especificado.
        ///// </summary>
        //private static byte[] StringToByteArray(string chave, int tamanhoArray)
        //{
        //    byte[] bytes = new byte[tamanhoArray];
        //    Buffer.BlockCopy(chave.ToCharArray(), 0, bytes, 0, bytes.Length);
        //    return bytes;
        //}

    }
}
