using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PiGlow.CS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
