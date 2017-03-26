//-----------------------------------------------------------------------
// <copyright file="PrimeGeneratorExitedEventArgs.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>Event args, which will be passed after the generator exited.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Event args, which will be passed after the generator exited.
    /// </summary>
    [Serializable]
    public class PrimeGeneratorExitedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimeGeneratorExitedEventArgs"/> class.
        /// </summary>
        /// <param name="exitCode">The exit code of the process, which ran the generator.</param>
        /// <param name="exitTime">The exit time.</param>
        public PrimeGeneratorExitedEventArgs(int exitCode, DateTime exitTime)
        {
            this.ExitCode = exitCode;
            this.ExitTime = exitTime;
        }

        /// <summary> Gets the exit code of the process, which ran the generator. </summary>
        /// <value> The exit code of the process, which ran the generator. </value>
        public int ExitCode { get; private set; }

        /// <summary> Gets the exit time. </summary>
        /// <value> The exit time. </value>
        public DateTime ExitTime { get; private set; }
    }
}
