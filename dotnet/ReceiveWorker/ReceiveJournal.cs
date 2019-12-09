using System.Collections.Concurrent;
using System.Text;

namespace ReceiveWorker
{
    public class ReceiveJournal : ConcurrentDictionary<string, int>
    {
        public string GetStatistics()
        {
            var stringBuilder = new StringBuilder();
            foreach (var key in Keys)
            {
                stringBuilder.AppendLine($"PartitionID '{key}' snapshot: {this[key]} messages received.");
            }
            return stringBuilder.ToString();
        }
    }
}
