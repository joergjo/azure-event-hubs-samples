using System;
using System.Collections.Concurrent;

namespace ReceiveWorker
{
    public class ReceiveJournal : ConcurrentDictionary<string, int>
    {
        public void EnumeratePartitions(Action<string, int> enumerateAction)
        {
            foreach (string partitionId in Keys)
            {
                enumerateAction(partitionId, this[partitionId]);
            }
        }
    }
}
