using System;
using System.Dynamic;
using System.Runtime.Remoting.Activation;

namespace ModbusTester_WPF.Models
{
    public class ModbusDataPoint
    {
        public uint Index { get; set; }
        public string RequestData { get; set; }
        public string ResponseData { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorLevel { get; set; }
        
    }
}