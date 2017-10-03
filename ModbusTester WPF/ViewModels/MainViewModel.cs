using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using ModbusTester_WPF.Command;
using ModbusTester_WPF.Models;
using ModbusTester_WPF.Views;
using ModbusTester_WPF.Navigation;
using ModbusTester_WPF.Properties;
using Modbus.Utility;

namespace ModbusTester_WPF.ViewModels
{
     sealed class ConverterNull : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var temp = (ComboBoxItem) value;
            return System.Convert.ToInt32(temp.Content);
        }
    }

    public class MainViewModel : AbstractViewModel
    {
        private ModbusConfig _modbusCfg;

        private ExtensiveWindow ExtWindow;
        private CancellationTokenSource _tokenSrc;
       

        public MBRequestData ReadData
        {
            get { return _readData; }
            set
            {
                _readData = value;
                RaisePropertyChanged();
            }
        }

        public MBRequestData WriteData
        {
            get { return _writeData; }
            set
            {
                _writeData = value;
                RaisePropertyChanged();
            }
        }

        public BindingList<ModbusDataPoint> DataTab { get; set; }
        public DataControlViewModel DataControlViewModel { get; set; }
        public GraphViewModel GraphModel { get; set; }
        public event EventHandler<UpdateEventArgs> UpdateEvent;

        public double ScanRate
        {
            get { return _scanRate; }
            set
            {
                _scanRate = value;
                _timer.Interval = TimeSpan.FromMilliseconds(value);
            }
        }

        private readonly DispatcherTimer _timer =
            new DispatcherTimer(DispatcherPriority.Background);
        private readonly DispatcherTimer _linkRecoveryTimer =
            new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };

        private string _statusString = "---------";

        public string StatusString
        {
            get { return _statusString; }
            set
            {
                _statusString = value;
                RaisePropertyChanged();
            }
        }

        public string WriteStatusString
        {
            get { return _writeStatusString; }
            set
            {
                _writeStatusString = value; 
                RaisePropertyChanged();
            }
        }

        public List<string> AvaliablePorts
        {
            get { return _avaliablePorts; }
            set
            {
                _avaliablePorts = value;
                RaisePropertyChanged();
            }
        }

        public List<int> AvailableBaudrates => new[] {9600, 19200, 57600, 115200}.ToList();

        public List<Parity> ParityBits
            => new[] {Parity.Even, Parity.Mark, Parity.None, Parity.Odd, Parity.Space}.ToList();

        public List<double> UpdateInterval => new[] {50.0, 100.0, 250.0, 500.0}.ToList();
        public List<int> DataBits => new[] {8, 7}.ToList();

        public List<StopBits> StopBit
            => new[] {StopBits.None, StopBits.One, StopBits.OnePointFive, StopBits.Two}.ToList();

        public List<int> TimeOut => new[] {50, 100, 250, 500}.ToList();

        public List<Memtype> AvailableMemTypes
            => new[] {Memtype.Coils, Memtype.Inputs, Memtype.InputRegisters, Memtype.HoldingRegisters}.ToList();


        public bool WriteWithF6
        {
            get { return _writeWithF6; }
            set
            {
                _writeWithF6 = value;
                RaisePropertyChanged();
            }
        }

        public bool InfiniteWrite
        {
            get { return _infiniteWrite; }
            set
            {
                _infiniteWrite = value;
                RaisePropertyChanged();
            }
        }

        public bool IsStopped
        {
            get { return _isStopped; }
            set
            {
                _isStopped = value;
                RaisePropertyChanged();
            }
        }

        public string ReadStatus { get; set; }

        public ModbusConfig ModbusConfig
        {
            get { return _modbusCfg; }
            set
            {
                _modbusCfg = value;
                RaisePropertyChanged();
            }
        }

        public DataTable ModbusData
        {
            get { return _modbusData; }
            set
            {
                _modbusData = value;
                RaisePropertyChanged();
            }
        }

        public uint Index
        {
            get { return _index; }
            set
            {
                _index = value; 
                RaisePropertyChanged();
            }
        }

        private TabDataView _tabsView;
        private ModBusDriver _driver;            
        private bool _isStopped;
        private double _scanRate;
        private MBRequestData _writeData;
        private MBRequestData _readData;
        private DataTable _modbusData;
        private bool _writeWithF6 = false;
        private bool _infiniteWrite = false;
        private List<string> _avaliablePorts = SerialPort.GetPortNames().ToList();
        private uint _index;
        private int _retriesToRecoverLink;

        EventHandler _dataUpdateHandler = null;
        EventHandler _connectionRecoveryHandler = null;
        private string _writeStatusString="--------";

        public ModBusDriver ModBusDriver
        {
            get { return _driver; }
            set
            {
                _driver = value;
                RaisePropertyChanged();
            }
        }
        public ICommand ReadClearCommand { get; set; }
        public ICommand RunCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand WriteClearCommand { get; set; }
        public ICommand SettingsViewCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ExtensiveWindowCommand { get; set; }
        public ICommand UpdatePorts { get; set; }

        public MainViewModel()
        {
            IsStopped = true;
            _modbusCfg = Settings.Default.ModBusConfig != null
                ? new ModbusConfig(Settings.Default.ModBusConfig)
                : new ModbusConfig();
            ScanRate = 250;

            ModbusData = ModbusDataTable.CreateMbTable("Данные");

            ReadData = new MBRequestData();
            WriteData = new MBRequestData();
            _tabsView = new TabDataView();
            Navigator.Navigate(_tabsView, this);
            ReadClearCommand = new RelayCommand(p => ReadData.ClearRequestData());
            WriteClearCommand = new RelayCommand(p => WriteData.ClearRequestData());
            SaveCommand = new RelayCommand(p =>
            {
                try
                {
                    Settings.Default.ModBusConfig = _modbusCfg;
                    ModbusConfig.Save(ModbusConfig);
                    Navigator.Navigate(_tabsView, this);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.InnerException?.Message);
                }

            });
            CancelCommand = new RelayCommand(p => Navigator.Navigate(_tabsView, this));

            ExtensiveWindowCommand = new RelayCommand(p =>
            {
                ExtWindow.Show();
                ExtWindow.Activate();
            });

            SettingsViewCommand = new RelayCommand(p => Navigator.Navigate(new SettingsView(), this));

            RunCommand = new RelayCommand(p =>
            {
                var metroWindowCol = Application.Current.Windows;
                _tokenSrc = new CancellationTokenSource();
                RunPollig();
            });

            StopCommand = new RelayCommand(p =>
            {
                _timer.Stop();
                _timer.Tick -= _dataUpdateHandler;
                _tokenSrc?.Cancel();
                IsStopped = true;
            });
            UpdatePorts = new RelayCommand(p =>
            {
                AvaliablePorts = SerialPort.GetPortNames().ToList();
            });
            DataTab = new BindingList<ModbusDataPoint>();
            DataControlViewModel = new DataControlViewModel(_driver, _modbusCfg);
            GraphModel = new GraphViewModel(this);
            DataControlViewModel.WriteSingleEvent += OnSingleRegWrite;
            DataControlViewModel.WriteMultipleEvent += OnMultipleWrite;
            _dataUpdateHandler = (sender, e) => UpdateData();
            _connectionRecoveryHandler = (sender, e) => LinkRecoveryTimerOnTick();
        }

        private void OnMultipleWrite(object sender, EventArgs e)
        {
            byte[] requestArr;
            byte[] responseArr;
            string errorMess;
            int code = 0;
            var con = DataControlViewModel.GetMode();
            try
            {
                //0-ushort,1-uint,2-ulong,3-hex,4-bin,5-float,6-double,7-coil
                switch (con)
                {
                    case 0:
                        code = ModBusDriver.SetDataMultiple(out requestArr, out responseArr, DataControlViewModel.EditRegister.Address, new[] { Convert.ToUInt16(DataControlViewModel.EditRegister.NumericData) }, null, out errorMess);
                        break;
                    case 1:

                        byte[] bytes = BitConverter.GetBytes(Convert.ToUInt32(DataControlViewModel.EditRegister.NumericData));
                        ushort[] regs = { BitConverter.ToUInt16(bytes, 2), BitConverter.ToUInt16(bytes, 0) };
                        code = ModBusDriver.SetDataMultiple(out requestArr, out responseArr, DataControlViewModel.EditRegister.Address, regs, null, out errorMess);
                        break;
                    case 2:

                        bytes = BitConverter.GetBytes(Convert.ToUInt64(DataControlViewModel.EditRegister.NumericData));
                        regs = new[] { BitConverter.ToUInt16(bytes, 6), BitConverter.ToUInt16(bytes, 4), BitConverter.ToUInt16(bytes, 2), BitConverter.ToUInt16(bytes, 0) };
                        code = ModBusDriver.SetDataMultiple(out requestArr, out responseArr, DataControlViewModel.EditRegister.Address, regs, null, out errorMess);
                        break;
                    case 3:

                        ushort reg;
                        if (!UInt16.TryParse(DataControlViewModel.EditRegister.NumericData, NumberStyles.HexNumber, null, out reg))
                        {
                            WriteStatusString = "Неправильный формат значения...";
                            return;
                        }
                        code = ModBusDriver.SetDataMultiple(out requestArr, out responseArr, DataControlViewModel.EditRegister.Address, new[] { reg }, null, out errorMess);
                        break;
                    case 4:

                        reg = 0;
                        foreach (var val in DataControlViewModel.EditRegister.BinData)
                            reg += (ushort)((val.Value ? 1 : 0) * Math.Pow(2, val.Index));
                        code = ModBusDriver.SetDataSingle(out requestArr, out responseArr, DataControlViewModel.EditRegister.Address, reg, out errorMess);
                        break;
                    case 5:

                        bytes = BitConverter.GetBytes(Convert.ToSingle(DataControlViewModel.EditRegister.NumericData));
                        regs = new[] {BitConverter.ToUInt16(bytes, 2), BitConverter.ToUInt16(bytes, 0) };
                        code = ModBusDriver.SetDataMultiple(out requestArr, out responseArr, DataControlViewModel.EditRegister.Address, regs, null, out errorMess);
                        break;
                    case 6:
                        var doubleVal = Convert.ToDouble(DataControlViewModel.EditRegister.NumericData);
                        bytes = BitConverter.GetBytes(doubleVal);
                        regs = new[] { BitConverter.ToUInt16(bytes, 0), BitConverter.ToUInt16(bytes, 2), BitConverter.ToUInt16(bytes, 4), BitConverter.ToUInt16(bytes, 6) };
                        code = ModBusDriver.SetDataMultiple(out requestArr, out responseArr, DataControlViewModel.EditRegister.Address, regs, null, out errorMess);
                        break;
                    case 7:
                        code = ModBusDriver.SetDataMultiple(out requestArr, out responseArr, DataControlViewModel.EditRegister.Address,
                                                                               new ushort[] { (ushort)(DataControlViewModel.EditRegister.CoilData ? 1 : 0) }, null, out errorMess);
                        break;
                    default:
                        requestArr = new byte[1];
                        responseArr = new byte[1];
                        errorMess = "Unreconized condition";
                        break;
                }                      
               
            }
            catch (FormatException ex)
            {
                WriteStatusString = "Введите корректное значение (возможно, в качестве разделителя использована точка вместо запятой";
                return;
            }
            catch (InvalidOperationException ex)
            {
                WriteStatusString = "Порт недоступен";
                return;
            }
            catch (Exception ex)
            {
                WriteStatusString = "Неизвестная ошибка";
                return;
            }
            string rq = "||WR|| ";
            string rs = "||WR|| ";
            if (requestArr != null)
                foreach (var tmp in requestArr)
                {
                    rq += tmp.ToString("X2");
                    rq += " ";
                }
            if (responseArr != null)
                foreach (var tmp in responseArr)
                {
                    rs += tmp.ToString("X2");
                    rs += " ";
                }
            DataTab.Insert(0, new ModbusDataPoint()
            {
                ErrorLevel = code,
                ErrorMessage = errorMess,
                Index = Index,
                RequestData = rq,
                ResponseData = rs
            });

            var writeData = new MBRequestData();
            writeData.ErrorMessage = errorMess;
            writeData.RequestQuantity = WriteData.RequestQuantity + 1;
            writeData.ValidResponseQuantity = code == 0 ? WriteData.ValidResponseQuantity + 1 : WriteData.ValidResponseQuantity;
            writeData.ValidResponseProportion = (double)writeData.ValidResponseQuantity / writeData.RequestQuantity * 100;
            WriteData = writeData;
            WriteStatusString = errorMess;
            Index++;
        }


        private void OnSingleRegWrite(object sender, EventArgs e)
        {
            byte[] requestArr;
            byte[] responseArr;
            string errorMess;
            int code = 0;

            var con = DataControlViewModel.GetMode();
            
            try
            {
                //0-ushort,1-uint,2-ulong,3-hex,4-bin,5-float,6-double,7-coil
                if (con == 0)
                        code = ModBusDriver.SetDataSingle(out requestArr, out responseArr, DataControlViewModel.EditRegister.Address, Convert.ToUInt16(DataControlViewModel.EditRegister.NumericData), out errorMess);
                else if (con == 4)
                {
                    ushort reg = 0;
                    foreach (var val in DataControlViewModel.EditRegister.BinData)
                        reg += (ushort)((val.Value ? 1 : 0) * Math.Pow(2, val.Index));
                    code = ModBusDriver.SetDataSingle(out requestArr, out responseArr, DataControlViewModel.EditRegister.Address, reg, out errorMess);
                }
                else if (con == 3)
                {
                    ushort reg;
                    if (!UInt16.TryParse(DataControlViewModel.EditRegister.NumericData, System.Globalization.NumberStyles.HexNumber, null, out reg))
                    {
                        WriteStatusString = "Неправильный формат значения...";
                        return;
                    }

                    code = ModBusDriver.SetDataSingle(out requestArr, out responseArr, DataControlViewModel.EditRegister.Address, reg, out errorMess);
                }
                else code = ModBusDriver.SetDataSingle(out requestArr, out responseArr, DataControlViewModel.EditRegister.Address, 
                                                             (ushort)(DataControlViewModel.EditRegister.CoilData?1:0), out errorMess);
            }
            catch (FormatException ex)
            {
                WriteStatusString = "Введите корректное значение (возможно, в качестве разделителя использована точка вместо запятой";
                return;
            }
            catch (InvalidOperationException ex)
            {
                WriteStatusString = "Порт недоступен";
                return;
            }
            catch (Exception ex)
            {
                WriteStatusString = "Неизвестная ошибка";
                return;
            }
            string rq = "||WR|| ";
            string rs = "||WR|| ";
            if (requestArr != null)
                foreach (var tmp in requestArr)
                {
                    rq += tmp.ToString("X2");
                    rq += " ";
                }
            if (responseArr != null)
                foreach (var tmp in responseArr)
                {
                    rs += tmp.ToString("X2");
                    rs += " ";
                }
            DataTab.Insert(0, new ModbusDataPoint()
            {
                ErrorLevel = code,
                ErrorMessage = errorMess,
                Index = Index,
                RequestData = rq,
                ResponseData = rs
            });
            
            var writeData = new MBRequestData();
            writeData.ErrorMessage = errorMess;
            writeData.RequestQuantity = WriteData.RequestQuantity + 1;
            writeData.ValidResponseQuantity = code == 0 ? WriteData.ValidResponseQuantity + 1 : WriteData.ValidResponseQuantity;
            writeData.ValidResponseProportion = (double)writeData.ValidResponseQuantity / writeData.RequestQuantity * 100;
            WriteData = writeData;
            WriteStatusString = errorMess;
            Index++;
        }        
        private void RunPollig()
        {
            ModBusDriver?.Dispose();

            try
            {
                CreateModbusDriver();
            }
            catch (Exception e)
            {
                if (!_linkRecoveryTimer.IsEnabled)
                {
                    StatusString = e.Message;
                    AvaliablePorts = SerialPort.GetPortNames().ToList();
                }
                return;
            }
            _timer.Interval = TimeSpan.FromMilliseconds(ScanRate);
            ModbusConfig.Save(ModbusConfig);
            _retriesToRecoverLink = 0;
            _linkRecoveryTimer.Stop();
            _linkRecoveryTimer.Tick -= _connectionRecoveryHandler;
            _timer.Tick += _dataUpdateHandler;
            _timer.Start();
            IsStopped = false;
        }
        
        private async void UpdateData()
        {
            byte[] requestArr;
            byte[] responseArr;
            string errorMess;
            int code=0;
            try
            {
                var resData = await Task.Run(() =>
                {
                    byte[] req;
                    byte[] res;
                    string err;
                    var cd = ModBusDriver.GetData(out req, out res, out err);
                    return new {Req = req, Res = res, Err = err, Cd = cd};
                }, _tokenSrc.Token);
                if (resData == null) return;
                requestArr = resData.Req;
                responseArr = resData.Res;
                errorMess = resData.Err;
                code = resData.Cd;
            }
            catch (OperationCanceledException ex)
            {
                return;
            }
            catch (InvalidOperationException ex)
            {
                _timer.Stop();
                _timer.Tick -= _dataUpdateHandler;
                _linkRecoveryTimer.Tick+= _connectionRecoveryHandler;
                _linkRecoveryTimer.Start();
                StatusString = "Порт недоступен. Пробую найти указаный порт...";
                return;
            }
            string rq="";
            string rs="";
            if(requestArr!=null)
            foreach (var tmp in requestArr)
            {
                rq += tmp.ToString("X2");
                rq += " ";
            }
            if(responseArr!=null)
            foreach (var tmp in responseArr)
            {
                rs += tmp.ToString("X2");
                rs += " ";
            }
            DataTab.Insert(0, new ModbusDataPoint()
            {
                ErrorLevel = code,
                ErrorMessage = errorMess,
                Index = Index,
                RequestData = rq,
                ResponseData = rs
            });
            if ((ModbusConfig.SelectedMemType == Memtype.Coils | ModbusConfig.SelectedMemType == Memtype.Inputs)&& ModBusDriver.CoilsArray!=null)
                DataControlViewModel.UpdateData(ModBusDriver.CoilsArray, ModbusConfig.StartAddress);
            else if(ModBusDriver.RegisterArray!=null)
            {
                DataControlViewModel.UpdateData(ModBusDriver.RegisterArray, ModbusConfig.StartAddress);
                UpdateEvent?.Invoke(this, new UpdateEventArgs(ModbusConfig.StartAddress, ModBusDriver.RegisterArray));
            }
                

            var readData = new MBRequestData();
            readData.ErrorMessage = errorMess;
            readData.RequestQuantity = ReadData.RequestQuantity+1;
            readData.ValidResponseQuantity = code==0? ReadData.ValidResponseQuantity+1: ReadData.ValidResponseQuantity;
            readData.ValidResponseProportion = (double)readData.ValidResponseQuantity/ readData.RequestQuantity*100;
            ReadData = readData;
            StatusString = errorMess;
            Index++;
        }

        private void LinkRecoveryTimerOnTick()
        {
            if (_retriesToRecoverLink > 5)
            {
                _linkRecoveryTimer.Stop();
                _linkRecoveryTimer.Tick -= _connectionRecoveryHandler;
                StatusString = "Не удалось найти порт";
                _retriesToRecoverLink = 0;
                IsStopped = true;
                return;
            }
            StatusString = StatusString + "...";
            RunPollig();
            _retriesToRecoverLink++;
        }

        private void CreateModbusDriver()
        {
            ModBusDriver = new ModBusDriver(ModbusConfig);
        }

        public override void OnNavigatedFrom()
        {
            Settings.Default.ModBusConfig = _modbusCfg;
            ModbusConfig.Save(ModbusConfig);
        }
    }

}
