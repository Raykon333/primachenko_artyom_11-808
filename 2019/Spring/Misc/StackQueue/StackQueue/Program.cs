using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackQueue
{
    class QueueByStack<TItem>
    {
        private Stack<TItem> stack;

        public QueueByStack(params TItem[] items)
        {
            stack = new Stack<TItem>(items);
        }
    
        public void Enqueue(TItem item)
        {
            stack.Push(item);
        }

        public TItem Dequeue()
        {
            if (stack.Count == 0)
                throw new InvalidOperationException();
            List<TItem> list = stack.ToList();
            TItem result = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            list.Reverse();
            stack = new Stack<TItem>(list);
            return result;
        }
    }

    class StackByQueue<TItem>
    {
        private Queue<TItem> queue;

        public StackByQueue(params TItem[] items)
        {
            queue = new Queue<TItem>(items);
        }

        public void Push(TItem item)
        {
            queue.Enqueue(item);
        }

        public TItem Pop()
        {
            if (queue.Count == 0)
                throw new InvalidOperationException();
            List<TItem> list = queue.ToList();
            TItem result = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            queue = new Queue<TItem>(list);
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            QueueByStack<int> queue = new QueueByStack<int>(new int[] { 0, 1, 2, 3 });
            Console.WriteLine(queue.Dequeue());
            Console.WriteLine(queue.Dequeue());
            Console.WriteLine(queue.Dequeue());
            queue.Enqueue(4);
            Console.WriteLine(queue.Dequeue());
            Console.WriteLine(queue.Dequeue());

            StackByQueue<int> stack = new StackByQueue<int>(new int[] { 0, 1, 2, 3 });
            Console.WriteLine(stack.Pop());
            Console.WriteLine(stack.Pop());
            Console.WriteLine(stack.Pop());
            stack.Push(4);
            Console.WriteLine(stack.Pop());
            Console.WriteLine(stack.Pop());
        }
    }
}
