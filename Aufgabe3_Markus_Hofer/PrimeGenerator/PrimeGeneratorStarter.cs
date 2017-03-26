//-----------------------------------------------------------------------
// <copyright file="PrimeGeneratorStarter.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents the prime generator starter, which is able to start a prime generator.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents the prime generator starter, which is able to start a prime generator.
    /// </summary>
    public class PrimeGeneratorStarter
    {
        /// <summary> The process, which runs the generator. </summary>
        private Process runningProcess;

        /// <summary> Tells if it already has been started. </summary>
        private bool started;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimeGeneratorStarter"/> class.
        /// </summary>
        /// <param name="initiatingWindow">The window, which wanted to start the prime generator.</param>
        public PrimeGeneratorStarter(IWindow initiatingWindow)
        {
            this.InitiatingWindow = initiatingWindow;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PrimeGeneratorStarter"/> class.
        /// If this instance is destructed, the process must also be killed.
        /// </summary>
        ~PrimeGeneratorStarter()
        {
            if (this.runningProcess != null)
            {
                if (this.started && !this.runningProcess.HasExited)
                {
                    this.runningProcess.Kill();
                }
            }
        }

        /// <summary>
        /// Delegate for event OnPrimeGeneratorNotFound.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        public delegate void GeneratorNotFound(PrimeGeneratorStarter sender);

        /// <summary>
        /// Gets called when the prime generator could not be found.
        /// </summary>
        public event GeneratorNotFound OnGeneratorNotFound;

        /// <summary>
        /// Gets called when the prime generator found a prime.
        /// </summary>
        public event EventHandler<PrimeFoundEventArgs> OnPrimeFound;

        /// <summary>
        /// Gets called when the prime generator exited.
        /// </summary>
        public event EventHandler<PrimeGeneratorExitedEventArgs> OnGeneratorExited;

        /// <summary> Gets the window, which wanted to start the prime generator. </summary>
        /// <value> The window, which wanted to start the prime generator. </value>
        public IWindow InitiatingWindow { get; private set; }

        /// <summary>
        /// Starts the prime generator with the given arguments.
        /// </summary>
        /// <param name="args">The given arguments.</param>
        public void Start(PrimeStartArguments args)
        {
            Task.Factory.StartNew(() =>
            {
                this.runningProcess = new Process();

                this.runningProcess.StartInfo.FileName = "PrimeGenerator.exe";
                this.runningProcess.StartInfo.Arguments = args.ToString();
                this.runningProcess.StartInfo.RedirectStandardOutput = true;
                this.runningProcess.StartInfo.UseShellExecute = false;
                this.runningProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                this.runningProcess.StartInfo.CreateNoWindow = true;

                this.runningProcess.OutputDataReceived += RunningProcess_OutputDataReceived;

                try
                {
                    this.runningProcess.Start();

                    this.started = true;

                    this.runningProcess.BeginOutputReadLine();
                    this.runningProcess.WaitForExit();

                    if (this.OnGeneratorExited != null)
                    {
                        this.OnGeneratorExited(this, new PrimeGeneratorExitedEventArgs(this.runningProcess.ExitCode, this.runningProcess.ExitTime));
                    }
                }
                catch (Win32Exception ex)
                {
                    if (this.OnGeneratorNotFound != null)
                    {
                        this.OnGeneratorNotFound(this);
                    }
                }
            });
        }

        /// <summary>
        /// Gets called when the process received output data from the prime generator.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void RunningProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            long prime;

            if (long.TryParse(e.Data, out prime))
            {
                if (this.OnPrimeFound != null)
                {
                    this.OnPrimeFound(this, new PrimeFoundEventArgs(prime, this.runningProcess.Id));
                }
            }
        }
    }
}
