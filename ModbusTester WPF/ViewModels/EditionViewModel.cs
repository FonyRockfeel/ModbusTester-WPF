using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusTester_WPF.Models;
using System.Windows.Input;
using ModbusTester_WPF.Command;
using System.ComponentModel;

namespace ModbusTester_WPF.ViewModels
{
    public class EditionViewModel : AbstractViewModel
    {

        ModbusConfig _config;
        private string _numericData;
        private ushort _address;
        private int _mode;
        private bool _coilData;
        private ObservableCollection<BinPoint> _binData;
        private bool _autoWrite;

        public ushort Address
        {
            get { return _address; }
            set
            {
                _address = value;
                RaisePropertyChanged();
            }
        }
        public string NumericData
        {
            get { return _numericData; }
            set
            {
                _numericData = value;
                RaisePropertyChanged();
            }
        }

        public bool CoilData
        {
            get { return _coilData; }
            set
            {
                _coilData = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<BinPoint> BinData
        {
            get { return _binData; }
            set
            {
                _binData = value;
                RaisePropertyChanged();
            }
        }
        public int Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                RaisePropertyChanged();
            }
        }      
        
        public EditionViewModel(ModbusConfig config)        
        {            
            _config = config;
        }
        public void BeginEdit(int mode,dynamic data)
        {
           
            if (_config.SelectedMemType == Memtype.Coils)
            {
                Address = (ushort)data.Address;
                CoilData = data.Value;
            }
            else if (_config.SelectedMemType == Memtype.HoldingRegisters)
            {
                Address = (ushort)data.Address;
                if (mode == 4)
                {
                    char[] chars = data.Value.ToCharArray();
                    BinData = new ObservableCollection<BinPoint>(chars.Select((value,index) => new BinPoint {Index = 15-index, Value = value=='1'}).ToArray());
                }
                else NumericData = data.Value.ToString();
            }
            else return;
            Mode = mode;
        }  
        public class BinPoint
        {
            public int Index { get; set; }
            public bool Value { get; set; }
        }

        public bool AutoWrite
        {
            get { return _autoWrite; }
            set
            {
                _autoWrite = value; 
                RaisePropertyChanged();
            }
        }
    }
}
