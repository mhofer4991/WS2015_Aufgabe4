//-----------------------------------------------------------------------
// <copyright file="PrimeFoundEventArgs.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>Event args, which will be passed when the generator found a prime.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Event args, which will be passed when the generator found a prime.
    /// </summary>
    [Serializable]
    public class PrimeFoundEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimeFoundEventArgs"/> class.
        /// </summary>
        /// <param name="prime">The found prime.</param>
        /// <param name="processID">The ID of the process, which ran the generator.</param>
        public PrimeFoundEventArgs(long prime, int processID)
        {
            this.Prime = prime;
            this.ProcessID = processID;
        }

        /// <summary> Gets the found prime. </summary>
        /// <value> The found prime. </value>
        public long Prime { get; private set; }

        /// <summary> Gets the ID of the process, which ran the generator. </summary>
        /// <value> The ID of the process, which ran the generator. </value>
        public int ProcessID { get; private set; }
    }
}
