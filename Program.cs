using McMaster.Extensions.CommandLineUtils;
using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace scon
{
    class Program
    {
        private static SerialPort _serialPort;

        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption();

            var portIdOpt = app.Option("-sp|--serial-port <COM4>", "Serial Port", CommandOptionType.SingleValue)
                .IsRequired();

            var baudRateOpt = app.Option<int>("-br|--baud-rate <9600>", "Baud rate", CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.Values("300", "1200", "2400", "4800", "9600", "19200", "38400", "57600", "74880", "115200", "230400", "250000", "500000", "1000000", "2000000"));

            var parityOpt = app.Option<Parity>("-pa|--parity-bit", "Parity bit", CommandOptionType.SingleValue)
                .IsRequired();

            var stopBitOpt = app.Option<StopBits>("-sb|--stopbit", "Stop bit", CommandOptionType.SingleValue)
                .IsRequired();

            var databitsOpt = app.Option<int>("-db|--databits", "Databits", CommandOptionType.SingleValue)
                .IsRequired();

            var handshakeOpt = app.Option<Handshake>("-h|--handshake", "Handshake", CommandOptionType.SingleValue)
                .IsRequired();

            app.OnExecute(async () =>
            {
                // Initialise the serial port on COM4.
                // obviously we would normally parameterise this, but
                // this is for demonstration purposes only.
                _serialPort = new SerialPort(portIdOpt.Value())
                {
                    BaudRate = baudRateOpt.ParsedValue,
                    Parity = parityOpt.ParsedValue,
                    StopBits = stopBitOpt.ParsedValue,
                    DataBits = databitsOpt.ParsedValue,
                    Handshake = handshakeOpt.ParsedValue
                };

                // Subscribe to the DataReceived event.
                _serialPort.DataReceived += SerialPortDataReceived;

                // Now open the port.
                _serialPort.Open();

                await Task.Delay(Timeout.Infinite);

                return 0;
            });

            return app.Execute(args);
        }

        private static void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;

            // Read the data that's in the serial buffer.
            var serialdata = serialPort.ReadExisting();

            // Write to debug output.
            Console.Write(serialdata);
        }
    }
}
