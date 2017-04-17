using System;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using Modbus;
using ModbusLib;
using ModbusLib.Device;

namespace ModbusTester_WPF.Models

{
    public class ModBusDriver: INotifyPropertyChanged, IDisposable
    {
        private SerialPort _port;
        private ModbusSerialMaster _modbus;
        private ModbusConfig _config;
        private bool _disposed;     

        public bool[] CoilsArray { get; set; }
        public ushort[] RegisterArray { get; set; }
        public int BaudRate
        {
            get { return _port.BaudRate; }
            set
            {
                _port.BaudRate = value;
                OnPropertyChanged();
            }
        }
        public int Timeout
        {
            get { return _modbus.Transport.ReadTimeout; }
            set
            {
                _modbus.Transport.ReadTimeout = value;
                OnPropertyChanged();
            }
        }
        public string PortName { get; set; }
        
        public ModBusDriver(ModbusConfig cfg)
        {
            _config = cfg;
            PortName = cfg.Port;
            
            _port = new SerialPort(PortName) {BaudRate = cfg.BaudRate, ReadTimeout = cfg.Timeout, WriteTimeout = cfg.Timeout, DataBits = cfg.DataBits, Parity = cfg.Parity}; //таймаут нужно синхронизировать между модбас и сом портом
            _port.Open();

            _modbus = ModbusSerialMaster.CreateRtu(_port);
           
            _modbus.Transport.Retries = 3;
            _modbus.Transport.WaitToRetryMilliseconds = 250;
        }

        public int GetData(out byte[] requestBytes, out byte[] responseBytes, out string errCode)
        {
            errCode = "OK";
            try
            {
                if (_config.SelectedMemType == Memtype.Coils)
                    CoilsArray = _modbus.ReadCoils(_config.Slaves, _config.StartAddress, _config.Amount);
                if (_config.SelectedMemType == Memtype.Inputs)
                    CoilsArray = _modbus.ReadInputs(_config.Slaves, _config.StartAddress, _config.Amount);
                if (_config.SelectedMemType == Memtype.InputRegisters)
                    RegisterArray = _modbus.ReadInputRegisters(_config.Slaves, _config.StartAddress, _config.Amount);
                if (_config.SelectedMemType == Memtype.HoldingRegisters)
                    RegisterArray = _modbus.ReadHoldingRegisters(_config.Slaves, _config.StartAddress, _config.Amount);
                return 0;
            }
            catch (SlaveException ex)
            {
                errCode = ex.Message;
                return 1;
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                    throw;
                errCode = ex.Message;
                return 2;
            }
            finally
            {
                requestBytes = _modbus.Transport.RequestFrame;
                responseBytes = _modbus.Transport.ResponseFrame;
            }
        }
        public int SetDataSingle(out byte[] requestBytes, out byte[] responseBytes, ushort start, ushort reg, out string errCode)
        {
            errCode = "OK";
            try
            {
                if (_config.SelectedMemType == Memtype.Coils)
                    _modbus.WriteSingleCoil(_config.Slaves, start, reg>0);
                if (_config.SelectedMemType == Memtype.HoldingRegisters)
                    _modbus.WriteSingleRegister(_config.Slaves, start, reg);
                return 0;
            }
            catch (SlaveException ex)
            {
                errCode = ex.Message;
                return 1;
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                    throw;
                errCode = ex.Message;
                return 2;
            }
            finally
            {
                requestBytes = _modbus.Transport.RequestFrame;
                responseBytes = _modbus.Transport.ResponseFrame;
            }
        }

        public int SetDataMultiple(out byte[] requestBytes, out byte[] responseBytes, ushort start, ushort[] ushortRegs, bool[] boolRegs, out string errCode)
        {
            errCode = "OK";
            try
            {
                if (_config.SelectedMemType == Memtype.Coils)
                    _modbus.WriteMultipleCoils(_config.Slaves, start, boolRegs);
                if (_config.SelectedMemType == Memtype.HoldingRegisters)
                    _modbus.WriteMultipleRegisters(_config.Slaves, start, ushortRegs);
                return 0;
            }
            catch (SlaveException ex)
            {
                errCode = ex.Message;
                return 1;
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                    throw;
                errCode = ex.Message;
                return 2;
            }
            finally
            {
                requestBytes = _modbus.Transport.RequestFrame;
                responseBytes = _modbus.Transport.ResponseFrame;
            }
        }

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool dispose)
        {
            if (_disposed) return;

            if (dispose)
            {
                if (_port.IsOpen)
                    _port.Close();
                _port.Dispose();
                _modbus.Dispose();
            }

            _disposed = true;

        }
        #endregion
    
    
    }
}
