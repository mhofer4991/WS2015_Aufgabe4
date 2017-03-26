//-----------------------------------------------------------------------
// <copyright file="PrimeGenerator.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents a prime generator, which can be started and stopped.</summary>
//-----------------------------------------------------------------------
namespace PrimeGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents a prime generator, which can be started and stopped.
    /// </summary>
    public class PrimeGenerator
    {
        /// <summary> Used for locking certain objects. </summary>
        private readonly object lockObject = new object();

        /// <summary> Contains all found primes. </summary>
        private SortedSet<long> foundPrimes;

        /// <summary> The current number will be examined to find out if it is a prime or not. </summary>
        private long currentNumber;

        /// <summary> The begin of the numbers which will be returned. </summary>
        private long offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimeGenerator"/> class.
        /// </summary>
        public PrimeGenerator()
        {
            this.foundPrimes = new SortedSet<long>();

            this.Reset();
        }

        /// <summary>
        /// Delegate for event OnPrimeFound.
        /// </summary>
        /// <param name="prime">The found prime.</param>
        public delegate void PrimeFound(long prime);

        /// <summary>
        /// Gets called after the generator found a prime.
        /// </summary>
        public event PrimeFound OnPrimeFound;

        /// <summary> Gets a value indicating whether the generator is currently running or not. </summary>
        /// <value>A value indicating whether the generator is currently running or not.</value>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Resets the prime generator.
        /// </summary>
        public void Reset()
        {
            this.foundPrimes.Clear();
            this.foundPrimes.Add(2);

            this.currentNumber = 0;
        }

        /// <summary>
        /// Starts the prime generator with the given parameters.
        /// </summary>
        /// <param name="threadCount">The amount of threads which will be used.</param>
        /// <param name="offset">The offset where the prime generator will start.</param>
        /// <param name="updateInterval">Interval between the examination of two numbers.</param>
        public void Start(int threadCount, long offset, int updateInterval)
        {
            if (offset < 2)
            {
                offset = 2;
            }

            this.currentNumber = 3;
            this.offset = offset;

            if (threadCount == 0)
            {
                threadCount = Environment.ProcessorCount - 1;

                if (threadCount == 0)
                {
                    threadCount = 1;
                }
            }

            Thread[] tds = new Thread[threadCount];

            this.IsRunning = true;

            for (int i = 0; i < tds.Length; i++)
            {
                tds[i] = new Thread(new ParameterizedThreadStart(this.GeneratePrimeNumbers));
                tds[i].Priority = ThreadPriority.Normal;
                tds[i].Start(updateInterval);
            }
        }

        /// <summary>
        /// Stops the prime generator.
        /// </summary>
        public void Stop()
        {
            this.IsRunning = false;
        }

        /// <summary>
        /// Generates prime numbers until it gets stopped.
        /// </summary>
        /// <param name="updateInterval">The given update interval.</param>
        private void GeneratePrimeNumbers(object updateInterval)
        {
            int interval = (int)updateInterval;
            long temp;

            while (this.IsRunning)
            {
                Monitor.Enter(this.lockObject);

                try
                {
                    temp = this.currentNumber;
                    this.currentNumber++;
                }
                finally
                {
                    Monitor.Exit(this.lockObject);
                }

                if (this.IsPrime(temp))
                {
                    Monitor.Enter(this.lockObject);

                    try
                    {
                        this.foundPrimes.Add(temp);
                    }
                    finally
                    {
                        Monitor.Exit(this.lockObject);

                        if (temp >= this.offset)
                        {
                            if (this.OnPrimeFound != null)
                            {
                                this.OnPrimeFound(temp);
                            }

                            Thread.Sleep(interval);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decides if some number is a prime.
        /// </summary>
        /// <param name="number">Represents the reviewed number.</param>
        /// <returns>A boolean, whose value depends on whether the given number is a prime or not.</returns>
        private bool IsPrime(long number)
        {
            Monitor.Enter(this.lockObject);

            try
            {
                foreach (long prime in this.foundPrimes)
                {
                    if (Math.Pow(prime, 2) <= number)
                    {
                        if (number % prime == 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            finally
            {
                Monitor.Exit(this.lockObject);
            }

            return true;
        }
    }
}
