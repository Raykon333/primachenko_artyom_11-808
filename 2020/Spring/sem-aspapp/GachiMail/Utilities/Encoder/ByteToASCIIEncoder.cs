using System.Text;

namespace GachiMail.Utilities.Encoder
{
    public static class ByteToASCIIEncoder
    {
        public static byte[] WriteToBytes(string line)
        {
            return Encoding.ASCII.GetBytes(line);
        }
        public static string ReadFromBytes(byte[] array)
        {
            return Encoding.ASCII.GetString(array);
        }
    }
}
