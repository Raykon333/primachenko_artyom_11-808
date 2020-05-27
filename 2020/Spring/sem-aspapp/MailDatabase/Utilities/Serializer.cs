using System.Text.Json;
using System.Collections.Generic;

namespace MailDatabase.Utilities
{
    public static class Serializer
    {
        public static string Serialize<T>(IEnumerable<T> items)
        {
            return JsonSerializer.Serialize(items);
        }
    }
}
