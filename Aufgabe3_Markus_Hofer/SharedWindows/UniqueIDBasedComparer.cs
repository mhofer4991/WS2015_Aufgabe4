//-----------------------------------------------------------------------
// <copyright file="UniqueIDBasedComparer.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class compares the unique ID of two windows.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class compares the unique ID of two windows.
    /// </summary>
    public class UniqueIDBasedComparer : IComparer<IWindow>
    {
        /// <summary>
        /// Compares the given windows and returns an integer value.
        /// </summary>
        /// <param name="x">The first window.</param>
        /// <param name="y">The second window.</param>
        /// <returns>An integer which describes the result of the comparison.</returns>
        public int Compare(IWindow x, IWindow y)
        {
            return x.GetUniqueID().CompareTo(y.GetUniqueID());
        }
    }
}
