using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using ModbusLib.Device;
using ModbusLib.Data;
using System.Threading;
using ModbusLib;

namespace ModbusTCpSlave
{
    class Program
    {
        static void Main(string[] args)
        {
            byte slaveId = 1;
            int port = 502;
            string data;
            IPAddress address = new IPAddress(new byte[] { 0, 0, 0, 0});
            do
            {
                Console.WriteLine("попытка получить IP");
                data = Console.ReadLine();
            }
            while (!IPAddress.TryParse(data, out address));

            Console.WriteLine("введите адрес");
            slaveId = Convert.ToByte(Console.ReadLine());

            // create and start the TCP slave
            TcpListener slaveTcpListener = new TcpListener(address, port);
            slaveTcpListener.Start();

            ModbusSlave slave = ModbusTcpSlave.CreateTcp(slaveId, slaveTcpListener,false);
            //slave = ModbusSerialSlave.CreateRtu(1,);

            slave.DataStore = DataStoreFactory.CreateDefaultDataStore();
            
            Console.WriteLine("начинаю слушать");
            slave.ListenAsync();          
            
            while(true)
            {
                Console.WriteLine("введите вид регистра: 1 - 4");
                int type = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("введите адрес регистра");
                ushort addr = Convert.ToUInt16(Console.ReadLine());
                Console.WriteLine("введите значение регистра");
                ushort val = Convert.ToUInt16(Console.ReadLine());

                switch(type)
                {
                    case 1:
                        slave.DataStore.CoilDiscretes[addr] = val > 0;
                        break;
                    case 2:
                        slave.DataStore.InputDiscretes[addr] = val > 0;
                        break;
                    case 3:
                        slave.DataStore.InputRegisters[addr] = val;
                        break;
                    case 4:
                        slave.DataStore.HoldingRegisters[addr] = val;
                        break;
                }
            }
        }
    }
}
