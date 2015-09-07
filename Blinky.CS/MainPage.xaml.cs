// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Blinky
{
    public sealed partial class MainPage : Page
    {
        private const int LED_PIN = 5;
        private GpioPin _pin;
        private GpioPinValue _pinValue;
        private DispatcherTimer _timer;
        private SolidColorBrush _greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private SolidColorBrush _blackBrush = new SolidColorBrush(Windows.UI.Colors.Black);

        public MainPage()
        {
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Timer_Tick;
            InitGPIO();
            if (_pin != null)
            {
                _timer.Start();
            }        
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                _pin = null;
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            _pin = gpio.OpenPin(LED_PIN);
            _pinValue = GpioPinValue.High;
            _pin.Write(_pinValue);
            _pin.SetDriveMode(GpioPinDriveMode.Output);

            GpioStatus.Text = "GPIO pin initialized correctly.";
        }

        private void Timer_Tick(object sender, object e)
        {
            if (_pinValue == GpioPinValue.High)
            {
                _pinValue = GpioPinValue.Low;
                _pin.Write(_pinValue);
                LED.Fill = _greenBrush;
            }
            else
            {
                _pinValue = GpioPinValue.High;
                _pin.Write(_pinValue);
                LED.Fill = _blackBrush;
            }
        }

    }
}
