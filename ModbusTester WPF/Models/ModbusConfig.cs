using System.IO.Ports;
using ModbusTester_WPF.Properties;

namespace ModbusTester_WPF.Models
{
    public enum Memtype
    {
        Coils = 0,
        Inputs = 1,
        InputRegisters = 2,
        HoldingRegisters = 3
    }
    public class ModbusConfig
    {
        /// <summary>
        /// Имя последовательного порта, соответствующего аппарату
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// Скорость обмена
        /// </summary>
        public int BaudRate { get; set; }
        /// <summary>
        /// ВРемя ожидания ответа
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// Биты данных
        /// </summary>
        public int DataBits { get; set; }
        /// <summary>
        /// Четность
        /// </summary>
        public Parity Parity { get; set; }
        /// <summary>
        /// Стоп биты
        /// </summary>
        public StopBits StopBits { get; set; }
        /// <summary>
        /// Адрес ведомого
        /// </summary>
        public byte Slaves { get; set; }
        /// <summary>
        /// Начальный адрес в памяти
        /// </summary>
        public ushort StartAddress { get; set; }
        /// <summary>
        /// Количество элементов для чтения
        /// </summary>
        public ushort Amount { get; set; }
        /// <summary>
        /// Выбранный тип памяти
        /// </summary>
        public Memtype SelectedMemType { get; set; }


        public ModbusConfig(ModbusConfig cfg)
        {
                Port = cfg.Port;
                BaudRate = cfg.BaudRate;
                Timeout = cfg.Timeout;
                DataBits = cfg.DataBits;
                Parity = cfg.Parity;
                StopBits = cfg.StopBits;
                Slaves = cfg.Slaves;
                StartAddress = cfg.StartAddress;
                Amount = cfg.Amount;
            SelectedMemType = cfg.SelectedMemType;
        }

        public ModbusConfig()
        {
            
        }

        public void Save(ModbusConfig cfg)
        {
            Settings.Default.ModBusConfig = cfg;
            Settings.Default.Save();
        }
      
        public void Apply(ModBusDriver driver)
        {
            driver.BaudRate = BaudRate;
            driver.Timeout = Timeout;
        }
    }
}
