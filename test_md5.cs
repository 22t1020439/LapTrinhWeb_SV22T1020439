using System;
using System.Security.Cryptography;
using System.Text;

public class Program
{
    public static void Main()
    {
        string input = "123";
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            Console.WriteLine(sb.ToString());
        }
    }
}
