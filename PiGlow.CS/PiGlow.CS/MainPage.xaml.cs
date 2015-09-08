using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.UI.Xaml.Controls;

namespace PiGlow.CS
{
    public sealed partial class MainPage : Page
    {
        #region LED Constants

        private const byte PYGLOW = 0x54;

        private const byte WHITE1 = 0x0D;
        private const byte WHITE2 = 0x0A;
        private const byte WHITE3 = 0x0B;
        private const byte BLUE1 = 0x0F;
        private const byte BLUE2 = 0x05;
        private const byte BLUE3 = 0x0C;
        private const byte GREEN1 = 0x04;
        private const byte GREEN2 = 0x06;
        private const byte GREEN3 = 0x0E;
        private const byte YELLOW1 = 0x03;
        private const byte YELLOW2 = 0x09;
        private const byte YELLOW3 = 0x10;
        private const byte ORANGE1 = 0x02;
        private const byte ORANGE2 = 0x08;
        private const byte ORANGE3 = 0x11;
        private const byte RED1 = 0x01;
        private const byte RED2 = 0x07;
        private const byte RED3 = 0x12;

        private const int INNER_BAND = 0;
        private const int OUTER_BAND = 5;

        #endregion

        private byte[][] _arms =
        {
            new byte[] { WHITE1, BLUE1, GREEN1, YELLOW1, ORANGE1, RED1 },
            new byte[] { WHITE2, BLUE2, GREEN2, YELLOW2, ORANGE2, RED2 },
            new byte[] { WHITE3, BLUE3, GREEN3, YELLOW3, ORANGE3, RED3 },
        };

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

            if (devices.Count == 0)
            {
                return;
            }

            var settings = new I2cConnectionSettings(0x54); // 0x54 is the I2C device address for the PiGlow

            using (I2cDevice device = await I2cDevice.FromIdAsync(devices[0].Id, settings))
            {
                InitPyGlow(device);

                await Pulsate(
                    device,
                    brightness: 64,
                    numberOfPulses: 3,
                    speedOfPulses: 100);

                await Pinwheel(
                    device,
                    brightness: 64,
                    numberOfSpins: 5,
                    speedOfSpin: 300);

                ClearLeds(device);
            }
        }

        private void InitPyGlow(I2cDevice device)
        {
            device.Write(new byte[] { 0x00, 0x01 });
            device.Write(new byte[] { 0x13, 0xFF });
            device.Write(new byte[] { 0x14, 0xFF });
            device.Write(new byte[] { 0x15, 0xFF });
            ClearLeds(device);
        }

        private async Task Pulsate(I2cDevice device, byte brightness = 1, int numberOfPulses = 3, int speedOfPulses = 100)
        {
            for (int iteration = 0; iteration < numberOfPulses; iteration++)
            {
                for (int band = INNER_BAND; band <= OUTER_BAND; band++)
                {
                    device.Write(new byte[] { _arms[0][band], brightness });
                    device.Write(new byte[] { _arms[1][band], brightness });
                    device.Write(new byte[] { _arms[2][band], brightness });
                    CommitLeds(device);
                    await Task.Delay(TimeSpan.FromMilliseconds(speedOfPulses));
                }

                for (int band = OUTER_BAND; band >= INNER_BAND; band--)
                {
                    device.Write(new byte[] { _arms[0][band], 0 });
                    device.Write(new byte[] { _arms[1][band], 0 });
                    device.Write(new byte[] { _arms[2][band], 0 });
                    CommitLeds(device);
                    await Task.Delay(TimeSpan.FromMilliseconds(speedOfPulses));
                }
            }
        }

        private async Task Pinwheel(I2cDevice device, byte brightness = 1, int numberOfSpins = 3, int speedOfSpin = 100)
        {
            for (int iteration = 0; iteration < numberOfSpins; iteration++)
            {
                for (int arm = 0; arm <= 2; arm++)
                {
                    SetArm(device, arm, brightness);
                    await Task.Delay(TimeSpan.FromMilliseconds(speedOfSpin));
                    SetArm(device, arm, 0);
                }
            }
        }

        private void SetArm(I2cDevice device, int arm, byte brightness)
        {
            for (int band = INNER_BAND; band <= OUTER_BAND; band++)
            {
                device.Write(new byte[] { _arms[arm][band], brightness });
            }
            CommitLeds(device);
        }

        private void CommitLeds(I2cDevice device)
        {
            device.Write(new byte[] { 0x16, 0xFF });
        }

        private void ClearLeds(I2cDevice device)
        {
            for (int band = INNER_BAND; band <= OUTER_BAND; band++)
            {
                device.Write(new byte[] { _arms[0][band], 0 });
                device.Write(new byte[] { _arms[1][band], 0 });
                device.Write(new byte[] { _arms[2][band], 0 });
            }
            device.Write(new byte[] { 0x16, 0xFF });
        }

    }
}
