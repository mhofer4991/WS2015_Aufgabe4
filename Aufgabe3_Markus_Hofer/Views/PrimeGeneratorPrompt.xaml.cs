//-----------------------------------------------------------------------
// <copyright file="PrimeGeneratorPrompt.xaml.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This window will be used to prompt the user for the start arguments of the prime generator.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    /// <summary>
    /// This window will be used to prompt the user for the start arguments of the prime generator.
    /// </summary>
    public partial class PrimeGeneratorPrompt : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimeGeneratorPrompt"/> class.
        /// </summary>
        public PrimeGeneratorPrompt()
        {
            this.InitializeComponent();

            this.threadCountTextBox.Focus();
        }

        /// <summary> Gets the thread count. </summary>
        /// <value> The thread count. </value>
        public string ThreadCount
        {
            get
            {
                return this.threadCountTextBox.Text;
            }
        }

        /// <summary> Gets the offset. </summary>
        /// <value> The offset. </value>
        public string Offset
        {
            get
            {
                return this.offsetTextBox.Text;
            }
        }

        /// <summary> Gets the update interval. </summary>
        /// <value> The update interval. </value>
        public string UpdateInterval
        {
            get
            {
                return this.updateIntervalTextBox.Text;
            }
        }

        /// <summary>
        /// Gets called after the user clicks on the ok button.
        /// </summary>
        /// <param name="sender">The sender who fired the event.</param>
        /// <param name="e">Arguments which have been passed by this event.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
