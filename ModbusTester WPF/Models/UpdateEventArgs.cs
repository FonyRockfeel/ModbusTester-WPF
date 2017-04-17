using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusTester_WPF.Models
{
    public class UpdateEventArgs: EventArgs
    {
        public int Address { get; set; }
        public ushort[] Array { get; set; }

        public UpdateEventArgs(int addr, ushort[] arr)
        {
            Address = addr;
            Array = arr;
        }
    }
}
