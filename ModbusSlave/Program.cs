using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModbusLib;
using ModbusLib.Data;
using ModbusLib.Device;

namespace ModbusSlave
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SerialPort slavePort = new SerialPort("COM2"))
            {
                // configure serial port
                slavePort.BaudRate = 19200;
                slavePort.DataBits = 8;
                slavePort.Parity = Parity.None;
                slavePort.Open();
                slavePort.StopBits = StopBits.One;

                byte unitId = 1;

                var adapter = new SerialPortAdapter(slavePort);
                // create modbus slave
                ModbusLib.Device.ModbusSlave slave = ModbusSerialSlave.CreateRtu(unitId, adapter);
                slave.DataStore = DataStoreFactory.CreateDefaultDataStore();
                for(int i=1; i<21; i++)
                {
                    slave.DataStore.HoldingRegisters[i] = (ushort)i;
                    slave.DataStore.InputRegisters[i] = (ushort)i;
                }
                slave.DataStore.CoilDiscretes[1] = true;
                slave.DataStore.CoilDiscretes[8] = true;
                slave.DataStore.CoilDiscretes[9] = true;

                slave.ListenAsync().GetAwaiter().GetResult();
            }
        }
    }
}
