//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>The user can use this program to create primes.</summary>
//-----------------------------------------------------------------------
namespace PrimeGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The user can use this program to create primes.
    /// </summary>
    public class Program
    {
        /// <summary> Gets the argument key for the thread count. </summary>
        private const string ThreadCountKey = "threadcount";

        /// <summary> Gets the argument key for the offset. </summary>
        private const string OffsetKey = "offset";

        /// <summary> Gets the argument key for the update interval. </summary>
        private const string UpdateIntervalKey = "updateinterval";

        /// <summary>
        /// This method represents the entry point of the program.
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        private static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                int threadCount = GetThreadCountFromArgs(args);

                long offset = GetOffsetFromArgs(args);

                int updateInterval = GetUpdateIntervalFromArgs(args);

                PrimeGenerator generator = new PrimeGenerator();
                generator.OnPrimeFound += Generator_OnPrimeFound;

                generator.Start(threadCount, offset, updateInterval);
            }
            else
            {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Gets called after the generator found a prime.
        /// </summary>
        /// <param name="prime">The found prime.</param>
        private static void Generator_OnPrimeFound(long prime)
        {
            Console.WriteLine(prime);
        }

        /// <summary>
        /// Tries to get the thread count from the given array of arguments.
        /// </summary>
        /// <param name="args">The given array of arguments.</param>
        /// <returns>The amount of threads.</returns>
        private static int GetThreadCountFromArgs(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.ToLower().StartsWith("-" + ThreadCountKey))
                {
                    int count = 0;

                    if (int.TryParse(arg.Substring(ThreadCountKey.Length + 2), out count))
                    {
                        if (count >= 0 && count <= 1024)
                        {
                            return count;
                        }
                    }

                    Environment.Exit(2);
                }
            }

            Environment.Exit(1);

            return 0;
        }

        /// <summary>
        /// Tries to get the offset from the given array of arguments.
        /// </summary>
        /// <param name="args">The given array of arguments.</param>
        /// <returns>The offset.</returns>
        private static long GetOffsetFromArgs(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.ToLower().StartsWith("-" + OffsetKey))
                {
                    long offset = 0;

                    if (long.TryParse(arg.Substring(OffsetKey.Length + 2), out offset))
                    {
                        if (offset >= 0 && offset <= long.MaxValue)
                        {
                            return offset;
                        }
                    }

                    Environment.Exit(3);
                }
            }

            Environment.Exit(1);

            return 0;
        }

        /// <summary>
        /// Tries to get the update interval from the given array of arguments.
        /// </summary>
        /// <param name="args">The given array of arguments.</param>
        /// <returns>The update interval.</returns>
        private static int GetUpdateIntervalFromArgs(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.ToLower().StartsWith("-" + UpdateIntervalKey))
                {
                    int interval = 0;

                    if (int.TryParse(arg.Substring(UpdateIntervalKey.Length + 2), out interval))
                    {
                        if (interval >= 500 && interval <= 10000)
                        {
                            return interval;
                        }
                    }

                    Environment.Exit(4);
                }
            }

            return 1000;
        }
    }
}
