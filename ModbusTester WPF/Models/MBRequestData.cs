using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusTester_WPF.Models
{
    public class MBRequestData
    {
        public int RequestQuantity { get; set; }
        public int ValidResponseQuantity { get; set; }
        public double ValidResponseProportion { get; set; }
        public string ErrorMessage { get; set; }

        public void ClearRequestData()
        {
            RequestQuantity = 0;
            ValidResponseQuantity = 0;
            ValidResponseProportion = 0;
            ErrorMessage = "No error";
        }
    }
}
