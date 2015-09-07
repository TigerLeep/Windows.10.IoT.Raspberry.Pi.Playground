using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.UI.Xaml.Controls;

namespace PiGlow.CS
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var task = new Task(() => Run());
            task.Start();
        }

        public async void Run()
        {
            var deviceSelector = I2cDevice.GetDeviceSelector("I2C1");

            var devices = await DeviceInformation.FindAllAsync(deviceSelector);
            //Message.Text = deviceSelector.Count().ToString();
            if (devices.Count == 0)
            {
                return;
            }

            var settings = new I2cConnectionSettings(0x54); // 0x54 is the I2C device address for the PiGlow

            using (I2cDevice device = await I2cDevice.FromIdAsync(devices[0].Id, settings))
            {
                device.Write(new byte[] { 0x00, 0x01 });
                device.Write(new byte[] { 0x13, 0xFF });
                device.Write(new byte[] { 0x14, 0xFF });
                device.Write(new byte[] { 0x15, 0xFF });

                device.Write(new byte[] { 0x06, 0x01});
                device.Write(new byte[] { 0x04, 0x01 });
                device.Write(new byte[] { 0x0E, 0x01 });
                device.Write(new byte[] { 0x16, 0xFF });
            }
        }
    }
}
