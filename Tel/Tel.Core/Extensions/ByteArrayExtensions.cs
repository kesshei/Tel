using System.Text;

namespace Tel.Core.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string GetString(this byte[] buffer, int offset, int count)
        {
            return Encoding.UTF8.GetString(buffer, offset, count);
        }
    }
}
