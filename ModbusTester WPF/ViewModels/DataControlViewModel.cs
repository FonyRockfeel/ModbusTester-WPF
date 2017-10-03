using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ModbusTester_WPF.Models;
using System.Windows.Input;
using ModbusTester_WPF.Command;
using Modbus.Utility;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using ModbusTester_WPF.Annotations;


namespace ModbusTester_WPF.ViewModels
{
    public class DataControlViewModel:AbstractViewModel
    {
        int i = 0;
        public DataControlViewModel(ModBusDriver driver, ModbusConfig config)
        {
            _config = config;
            _config.SelectedMemTypeChanged += OnSelectedMemTypeChanged;
            _driver = driver;
            //_lastCount = _config.Amount;
            //_lastIntro = _config.StartAddress;

            SetUshortFormat = new RelayCommand(o => {
                _mode = 0;
                _lastCount = 0;
                UpdateTableSource(_mode);
            });
            SetUIntFormat = new RelayCommand(o => {
                _mode = 1;
                _lastCount = 0;
                UpdateTableSource(_mode);
            });
            SetULongFormat = new RelayCommand(o => {
                _mode = 2;
                _lastCount = 0;
                UpdateTableSource(_mode);
            });
            SetHexFormat = new RelayCommand(o => {
                _mode = 3;
                _lastCount = 0;
                UpdateTableSource(_mode);
            });
            SetBinFormat = new RelayCommand(o => {
                
                _lastCount = 0;
                if (_config.SelectedMemType == Memtype.Coils | _config.SelectedMemType == Memtype.Inputs)
                {
                    _mode = 7;
                    UpdateTableSource(_mode);
                }
                else
                {
                    _mode = 4;
                    UpdateTableSource(_mode);
                }
            });
            SetFloatFormat = new RelayCommand(o => {
                _mode = 5;
                _lastCount = 0;
                UpdateTableSource(_mode);
            });
            SetDoubleFormat = new RelayCommand(o => {
                _mode = 6;
                _lastCount = 0;
                UpdateTableSource(_mode);
            });
            OpenEditDialog = new RelayCommand(o =>{
                if(ControlTableSource.Count==0) return;
                EditRegister.BeginEdit(_mode, ControlTableSource[ControlPoint]);
            });

            WriteSingleCommand = new RelayCommand(o => {
                WriteSingleEvent?.Invoke(this, null);
            },p=>CanExecuteSingle());

            WriteMultipleCommand = new RelayCommand(o => {
                WriteMultipleEvent?.Invoke(this, null);
            });

            CoilList = new BindingList<CoilPoint>();
            UshortList = new BindingList<UshortPoint>();
            UIntList = new BindingList<UIntPoint>();
            ULongList = new BindingList<ULongPoint>();
            FloatList = new BindingList<FloatPoint>();
            DoubleList = new BindingList<DoublePoint>();
            BitList = new BindingList<BitPoint>();
            HexList = new BindingList<HexPoint>();

            if (_config.SelectedMemType == Memtype.Coils | _config.SelectedMemType == Memtype.Inputs)
            {
                _mode = 7;
                ControlTableSource = CoilList;
            }
            else
            {
                _mode = 0;
                ControlTableSource = UshortList;
            }
            EditRegister = new EditionViewModel(_config);
            ControlTableSource.ListChanged += OnListChanged;
        }

        private void OnSelectedMemTypeChanged(object sender, Memtype e)
        {
            _lastCount = 0;//initiate recollect data to the ControlTableSource
            if (e == Memtype.Coils|e==Memtype.Inputs) ControlTableSource = CoilList;
            else
            {
                ControlTableSource = UshortList;
                _mode = 0;
            }
            
        }

        private void OnListChanged(object sender, ListChangedEventArgs e)
        {
            if(e.ListChangedType==ListChangedType.ItemChanged)
            {
                i++;
            }
        }


        #region Models of Datapoint to be add to data control table in view

        public abstract class AbstractPoint : INotifyPropertyChanged
        {
            private int _address;

            public int Address
            {
                get { return _address; }
                set
                {
                    _address = value; 
                    RaisePropertyChanged();
                }
            }

            #region INPC

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion
        }
        public class CoilPoint:AbstractPoint
        {
            public bool Value { get; set; }

            public CoilPoint(int address, bool coil)
            {
                Address = address;
                Value = coil;
            }
        }

        public class UshortPoint: AbstractPoint
        {
            private ushort _value;

            public ushort Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    RaisePropertyChanged();
                }
            }

            public UshortPoint(int address, ushort value)
            {
                Address = address;
                Value = value;
            }
        }

        public class UIntPoint: AbstractPoint
        {
            private uint _value;

            public uint Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    RaisePropertyChanged();
                }
            }

            public UIntPoint(int address, uint value)
            {
                Address = address;
                Value = value;
            }
        }
        public class ULongPoint: AbstractPoint
        {
            private ulong _value;

            public ulong Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    RaisePropertyChanged();
                }
            }

            public ULongPoint(int address, ulong value)
            {
                Address = address;
                Value = value;
            }
        }

        public class FloatPoint: AbstractPoint
        {
            private float _value;

            public float Value
            {
                get { return _value; }
                set
                {
                    _value = value; 
                    RaisePropertyChanged();
                }
            }

            public FloatPoint(int address, float value)
            {
                Address = address;
                Value = value;
            }
        }

        public class DoublePoint: AbstractPoint
        {
            private double _value;

            public double Value
            {
                get { return _value; }
                set
                {
                    _value = value; 
                    RaisePropertyChanged();
                }
            }

            public DoublePoint(int address, double value)
            {
                Address = address;
                Value = value;
            }
        }

        public class BitPoint: AbstractPoint
        {
            private string _value;

            public string Value
            {
                get { return _value; }
                set
                {
                    _value = value; 
                    RaisePropertyChanged();
                }
            }

            public BitPoint(int address, string value)
            {
                Address = address;
                Value = value;
            }
        }
        public class HexPoint: AbstractPoint
        {
            private string _value;

            public string Value
            {
                get { return _value; }
                set
                {
                    _value = value; 
                    RaisePropertyChanged();
                }
            }

            public HexPoint(int address, string value)
            {
                Address = address;
                Value = value;
            }
        }

        #endregion

        internal BindingList<CoilPoint> CoilList;
        internal BindingList<UshortPoint> UshortList;
        internal BindingList<UIntPoint> UIntList;
        internal BindingList<ULongPoint> ULongList;
        internal BindingList<FloatPoint> FloatList;
        internal BindingList<DoublePoint> DoubleList;
        internal BindingList<BitPoint> BitList;
        internal BindingList<HexPoint> HexList;       

        public IBindingList ControlTableSource {
            get
            {
                return _controlTableSource;
            }
            set
            {
                _controlTableSource = value;
                RaisePropertyChanged();

            }
        }   
        
        public int ControlPoint { get; set; }
        public EditionViewModel EditRegister { get; set; }

        public ICommand SetUshortFormat { get; private set; }
        public ICommand SetUIntFormat { get; private set; }
        public ICommand SetULongFormat { get; private set; }
        public ICommand SetHexFormat { get; private set; }
        public ICommand SetBinFormat { get; private set; }
        public ICommand SetFloatFormat { get; private set; }
        public ICommand SetDoubleFormat { get; private set; }

        public ICommand OpenEditDialog { get; private set; }
        public ICommand WriteSingleCommand { get; private set; }
        public ICommand WriteMultipleCommand { get; private set; }
       
        public event EventHandler WriteSingleEvent;
        public event EventHandler WriteMultipleEvent;


        ModbusConfig _config;
        ModBusDriver _driver;

        int _mode=0; //0-ushort,1-uint,2-ulong,3-hex,4-bin,5-float,6-double,7-coil
        int _lastCount;
        int _lastIntro;
        private IBindingList _controlTableSource;

        public void ApplyConfig(ModBusDriver driver, ModbusConfig config)
        {
            _config = config;
            _driver = driver;
        }   
        private void UpdateTableSource(int mode)
        {
            switch(mode)
            {
                case 0:
                    ControlTableSource = UshortList;
                    break;
                case 1:
                    ControlTableSource = UIntList;
                    break;
                case 2:
                    ControlTableSource = ULongList;
                    break;
                case 3:
                    ControlTableSource = HexList;
                    break;
                case 4:
                    ControlTableSource = BitList;
                    break;
                case 5:
                    ControlTableSource = FloatList;
                    break;
                case 6:
                    ControlTableSource = DoubleList;
                    break;
                case 7:
                    ControlTableSource = CoilList;
                    break;
            }
        }

        public void UpdateData(bool[] coils, int startAddress)
        {
            _mode = 7;
            if (_config.Amount == _lastCount & _lastIntro == _config.StartAddress)
            {
                for (int i = 0; i < coils.Length; i++)
                    CoilList[i].Value = coils[i];
            }
            else
            {
                _lastCount = _config.Amount;
                _lastIntro = _config.StartAddress;
                CoilList.Clear();
                for (int i = 0; i < coils.Length; i++)
                    CoilList.Add(new CoilPoint(_config.StartAddress + i, coils[i]));
            }          
        }

        public void UpdateData(ushort[] registers, int startAddress)
        {
            //TODO проверить режим отоброжанения, и изменять нужный массив
            
            int t = 0;
            switch (_mode)
            {
                case 0://ushort format - 1 word
                    if (_config.Amount == _lastCount & _lastIntro == _config.StartAddress)
                    {
                        for (int i = 0; i < registers.Length; i++)
                            UshortList[i].Value = registers[i];                        
                    }
                    else
                    {
                        _lastCount = _config.Amount;
                        _lastIntro = _config.StartAddress;
                        UshortList.Clear();
                        for (int i = 0; i < registers.Length; i++)
                            UshortList.Add(new UshortPoint(_config.StartAddress + i, registers[i]));                  
                        
                    }
                    break;


                case 1://uint format - 2 words
                    t = (_config.Amount / 2) * 2;                    
                    if (_config.Amount == _lastCount & _lastIntro == _config.StartAddress)
                    {
                        int r = 0;
                        for (int i = 0; i < t; i += 2)
                            UIntList[r++].Value = ModbusUtility.GetUInt32(registers[i], registers[i + 1]);
                    }
                    else
                    {
                        _lastCount = _config.Amount;
                        _lastIntro = _config.StartAddress;
                        UIntList.Clear();
                        for (int i = 0; i < t; i+=2)
                            UIntList.Add(new UIntPoint(_config.StartAddress + i, ModbusUtility.GetUInt32(registers[i], registers[i + 1])));                        
                    }
                    break;



                case 2://ulong format - 4 words
                    t = (_config.Amount / 4) * 4;
                    if (_config.Amount == _lastCount & _lastIntro == _config.StartAddress)
                    {
                        int r = 0;
                        for (int i = 0; i < t; i += 4)
                            ULongList[r++].Value = ModbusUtility.GetUInt64(registers[i], registers[i + 1], registers[i + 2], registers[i + 3]);
                    }
                    else
                    {
                        _lastCount = _config.Amount;
                        _lastIntro = _config.StartAddress;
                        ULongList.Clear();
                        for (int i = 0; i < t; i+=4)
                            ULongList.Add(new ULongPoint(_config.StartAddress + i, ModbusUtility.GetUInt64(registers[i], registers[i + 1], registers[i + 2], registers[i + 3])));
                        
                    }
                    break;


                case 3://hex format
                    if(_config.Amount == _lastCount & _lastIntro == _config.StartAddress)
                    {
                        for (int i = 0; i < registers.Length; i++)
                            HexList[i].Value = registers[i].ToString("X2");
                    }
                    else
                    {
                        _lastCount = _config.Amount;
                        _lastIntro = _config.StartAddress;
                        HexList.Clear();
                        for (int i = 0; i < registers.Length; i++)
                            HexList.Add(new HexPoint(_config.StartAddress + i, registers[i].ToString("X2")));                        
                    }
                    break;
                case 4:                                   

                    if (_config.Amount == _lastCount & _lastIntro == _config.StartAddress)
                    {
                        for (int i = 0; i < registers.Length; i++)
                            BitList[i].Value = Convert.ToString(registers[i], 2).PadLeft(16,'0');
                    }
                    else
                    {
                        _lastCount = _config.Amount;
                        _lastIntro = _config.StartAddress;
                        BitList.Clear();
                        for (int i = 0; i < registers.Length; i++)
                            BitList.Add(new BitPoint(_config.StartAddress + i, Convert.ToString(registers[i],2).PadLeft(16, '0')));                        
                    }
                    break;
                case 5:
                    t = (_config.Amount / 2) * 2;
                    if (_config.Amount == _lastCount & _lastIntro == _config.StartAddress)
                    {
                        int r = 0;
                        for (int i = 0; i < t; i += 2)
                            FloatList[r++].Value = ModbusUtility.GetSingle(registers[i], registers[i + 1]);
                    }
                    else
                    {
                        _lastCount = _config.Amount;
                        _lastIntro = _config.StartAddress;
                        FloatList.Clear();
                        for (int i = 0; i < t; i += 2)
                            FloatList.Add(new FloatPoint(_config.StartAddress + i, ModbusUtility.GetSingle(registers[i], registers[i + 1])));
                    }
                    break;
                case 6:
                    t = (_config.Amount / 4) * 4;
                    if (_config.Amount == _lastCount & _lastIntro == _config.StartAddress)
                    {
                        int r = 0;
                        for (int i = 0; i < t; i += 4)
                            DoubleList[r++].Value = ModbusUtility.GetDouble(registers[i+3], registers[i + 2], registers[i + 1], registers[i]);
                    }
                    else
                    {
                        _lastCount = _config.Amount;
                        _lastIntro = _config.StartAddress;
                        DoubleList.Clear();
                        for (int i = 0; i < t; i += 4)
                            DoubleList.Add(new DoublePoint(_config.StartAddress + i, ModbusUtility.GetDouble(registers[i+3], registers[i + 2], registers[i + 1], registers[i])));
                    }
                    break;
            }
            UpdateTableSource(_mode);
        }        

        public int GetMode()
        {
            return EditRegister.Mode;
        }       
        public bool CanExecuteSingle()
        {
            return _mode==0|_mode==4|_mode==7;
        }
    }
}
