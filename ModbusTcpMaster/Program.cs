using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ModbusLib.Device;
using ModbusLib;

using System.Net;
using System.Net.Sockets;

namespace ModbusTcpMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input IP address of modbus slave");
            var ip = Console.ReadLine();
            Console.WriteLine("Input port");
            int port;
            var t = int.TryParse(Console.ReadLine(),  out port);
            byte slaveId;
            Console.WriteLine("Input slaveId");
            t = byte.TryParse(Console.ReadLine(), out slaveId);
            while (true)
                try
                {
                    using (TcpClient client = new TcpClient(ip, port))
                    {
                        ModbusIpMaster master = ModbusIpMaster.CreateIp(client);

                        // read five input values
                        ushort startAddress = 0;
                        ushort numHoldings = 10;
                        while (true)
                        {
                            ushort[] inputs = master.ReadHoldingRegisters(slaveId, startAddress, numHoldings);
                            //bool[] inputs = master.ReadCoils(slaveId, startAddress, numHoldings);
                            Console.Write(startAddress + "::");
                            for (int i = 0; i < inputs.Length; i++)
                            {
                                Console.Write(" "+(inputs[i]));
                            }
                            Console.WriteLine();
                            startAddress+=10;
                            Thread.Sleep(500);
                        }
                        Console.Read();
                    }
                }
                catch (Exception ex)
                {
                    var tmp = (SlaveException)ex;
                    Console.WriteLine(tmp.Message + tmp.SlaveExceptionCode);
                    Console.Read();
                }
        }
    }
}
