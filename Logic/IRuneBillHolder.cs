using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runes
{
    /// <summary>
    /// Defines the Thing as being able to hold rune bills.
    /// </summary>
    public interface IRuneBillHolder
    {
        /// <summary>
        /// Accessible rune bill stack.
        /// </summary>
        RuneBillstack BillStack { get; }
    }
}
