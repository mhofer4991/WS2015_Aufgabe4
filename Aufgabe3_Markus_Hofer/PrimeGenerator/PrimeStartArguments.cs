//-----------------------------------------------------------------------
// <copyright file="PrimeStartArguments.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents arguments, which will be passed to the prime generator starter.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents arguments, which will be passed to the prime generator starter.
    /// </summary>
    [Serializable]
    public class PrimeStartArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimeStartArguments"/> class.
        /// </summary>
        /// <param name="threadCount">The amount of threads.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="updateInterval">The update interval.</param>
        public PrimeStartArguments(string threadCount, string offset, string updateInterval)
        {
            this.ThreadCount = threadCount;
            this.Offset = offset;
            this.UpdateInterval = updateInterval;
        }

        /// <summary> Gets the amount of threads. </summary>
        /// <value> The amount of threads. </value>
        public string ThreadCount { get; private set; }

        /// <summary> Gets the offset. </summary>
        /// <value> The offset. </value>
        public string Offset { get; private set; }

        /// <summary> Gets the update interval. </summary>
        /// <value> The update interval. </value>
        public string UpdateInterval { get; private set; }
        
        /// <summary>
        /// Gets a string of the arguments.
        /// </summary>
        /// <returns>A string of the arguments.</returns>
        public override string ToString()
        {
            string args = string.Empty;

            args += "-ThreadCount:" + this.ThreadCount;

            args += " -Offset:" + this.Offset;

            if (!string.IsNullOrWhiteSpace(this.UpdateInterval))
            {
                args += " -UpdateInterval:" + this.UpdateInterval;
            }

            return args;
        }
    }
}
