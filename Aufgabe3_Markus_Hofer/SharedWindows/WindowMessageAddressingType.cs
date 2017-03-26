//-----------------------------------------------------------------------
// <copyright file="WindowMessageAddressingType.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This enumeration contains different types of addressing a window.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This enumeration contains different types of addressing a window.
    /// </summary>
    public enum WindowMessageAddressingType
    {
        /// <summary>
        /// Indicates that the window will be addressed by the ID.
        /// </summary>
        ID,

        /// <summary>
        /// Indicates that the window will be addressed by the unique ID.
        /// </summary>
        UniqueID
    }
}
