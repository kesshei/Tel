using System.Text.Json;

namespace Tel.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object message)
        {
            if (message == null)
            {
                return null;
            }

            var jsonOptions = new JsonSerializerOptions { WriteIndented = false };
            return JsonSerializer.Serialize(message, message.GetType(), jsonOptions);
        }
    }
}
