## Modbus Tester
Modbus Tester is a tool for testing connection with devices in Modbus protocol network. Now it provides testing network over serial communication port (COM). 
The tool includes: 
* port configuration menu 
* data control component, which shows data stored in choosen registers
* data change component
* raw data logger
* chart component, which draws chart of chosen register
Unfortunately RU is the only language of interface.

- [x] Basic functionality
- [x] Serial port support
- [ ] ModbusTCP and Modbus-over-TCP support
- [ ] Add switch to interface in english

In this tool I used sources of modbus protocol implementation [NModbus4](https://github.com/NModbus4/NModbus4)  with some additions
