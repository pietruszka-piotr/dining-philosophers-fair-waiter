using System;
using System.Collections.Generic;
using System.Threading;

namespace wspolbiezne2
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 5;                                  // liczba filozofów
            var simulation = TimeSpan.FromSeconds(30);  // czas symulacji

            Console.WriteLine("Start uczty");
            Console.WriteLine($"N = {n}, czas = {simulation.TotalSeconds} s");
            Console.WriteLine();

            var waiter = new Waiter(n);
            var stats = new Stats[n];
            var threads = new Thread[n];
            using var cts = new CancellationTokenSource(simulation);
            var start = new CountdownEvent(n);

            for (int i = 0; i < n; i++)
            {
                int id = i;
                stats[id] = new Stats();
                threads[id] = new Thread(() => Philosopher(id, waiter, stats[id], start, cts.Token))
                { IsBackground = true, Name = $"Philosopher-{id}" };
                threads[id].Start();
            }

            // czeka aż wszyscy wystartują
            start.Wait();

            // czeka aż wszystkie wątki zakończą się po upływie czasu
            foreach (var t in threads)
                t.Join();

            Console.WriteLine();
            Console.WriteLine("=== PODSUMOWANIE ===");
            for (int i = 0; i < n; i++)
                Console.WriteLine($"F{i} Myslal: {stats[i].Think}, Jadl: {stats[i].Eat}");
            Console.WriteLine("Koniec.");
        }

        static void Philosopher(int id, Waiter waiter, Stats s, CountdownEvent start, CancellationToken token)
        {
            var rng = new Random(Environment.TickCount ^ (id * 7919));

            start.Signal();
            start.Wait();

            while (!token.IsCancellationRequested)
            {
                // think
                Thread.Sleep(rng.Next(8, 26));
                s.Think++;

                if (token.IsCancellationRequested) break;

                // prosba do kelnera
                waiter.RequestToEat(id, token);
                if (token.IsCancellationRequested) break;

                // eat
                Thread.Sleep(rng.Next(8, 21));
                s.Eat++;

                waiter.DoneEating(id);
            }
        }
    }

    class Stats { public int Think; public int Eat; }

    // Kelner – pojedynczy monitor z kolejką FIFO (brak deadlocków i zagłodzenia)
    class Waiter
    {
        private readonly object _lock = new object();
        private readonly bool[] _forkFree;
        private readonly Queue<int> _fifo = new Queue<int>();

        public Waiter(int n)
        {
            _forkFree = new bool[n];
            for (int i = 0; i < n; i++) _forkFree[i] = true;
        }

        private int Left(int i) => i;
        private int Right(int i) => (i + 1) % _forkFree.Length;

        public void RequestToEat(int i, CancellationToken token)
        {
            lock (_lock)
            {
                _fifo.Enqueue(i);

                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        RemoveFromQueue(i);
                        return;
                    }

                    bool iIsHead = _fifo.Count > 0 && _fifo.Peek() == i;

                    if (iIsHead && _forkFree[Left(i)] && _forkFree[Right(i)])
                    {
                        _forkFree[Left(i)] = false;
                        _forkFree[Right(i)] = false;
                        _fifo.Dequeue();
                        return;
                    }

                    Monitor.Wait(_lock, 100);
                }
            }
        }

        public void DoneEating(int i)
        {
            lock (_lock)
            {
                _forkFree[Left(i)] = true;
                _forkFree[Right(i)] = true;
                Monitor.PulseAll(_lock);
            }
        }

        private void RemoveFromQueue(int id)
        {
            if (_fifo.Count == 0) return;
            var tmp = new Queue<int>(_fifo.Count);
            while (_fifo.Count > 0)
            {
                var x = _fifo.Dequeue();
                if (x != id) tmp.Enqueue(x);
            }
            while (tmp.Count > 0) _fifo.Enqueue(tmp.Dequeue());
            Monitor.PulseAll(_lock);
        }
    }
}