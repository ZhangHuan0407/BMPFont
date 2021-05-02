using System.Collections.Generic;

namespace Encoder
{
    public static class QueueExtend
    {
        /* func */
        public static void Add<T>(this Queue<T> queue, T item) => queue.Enqueue(item);
    }
}