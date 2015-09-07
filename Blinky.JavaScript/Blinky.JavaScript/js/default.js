// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
	"use strict";

	var app = WinJS.Application;
	var activation = Windows.ApplicationModel.Activation;
	var gpio = Windows.Devices.Gpio;
	var threading = Windows.System.Threading;

	var LED_PIN = 5;

	var _blinking = false;
	var _ledOn = false;
	var _pin = null;
	var _timer = null;
	var _status = null;
	var _header = null;


	app.onactivated = function (args) {
		if (args.detail.kind === activation.ActivationKind.launch) {
			if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
			    // TODO: This application has been newly launched. Initialize your application here.
			    initGPIO();
			} else {
				// TODO: This application was suspended and then terminated.
				// To create a smooth user experience, restore application state here so that it looks like the app never stopped running.
			}
			args.setPromise(WinJS.UI.processAll());
			var toggleButton = document.getElementById("ToggleButton");
			toggleButton.addEventListener("click", toggleButtonClickHandler, false);
			_status = document.getElementById("Status");
			_header = document.getElementById("Header");
		}
	};

	app.oncheckpoint = function (args) {
		// TODO: This application is about to be suspended. Save any state that needs to persist across suspensions here.
		// You might use the WinJS.Application.sessionState object, which is automatically saved and restored across suspension.
		// If you need to complete an asynchronous operation before your application is suspended, call args.setPromise().
	};

	function initGPIO()
	{
	    var gpioController = gpio.GpioController.getDefault();
	    if (gpioController == null)
	    {
	        _pin = null;
	        setStatus("There is no GPIO controller.");
	        return;
	    }

	    _pin = gpioController.openPin(LED_PIN);
	    _blinking = false;
	    _ledOn = false;
	    _pin.write(gpio.GpioPinValue.high);
	    _pin.setDriveMode(gpio.GpioPinDriveMode.output);
	    setStatus("GPIO pin initialized correctly.")
	}

	function toggleButtonClickHandler(eventInfo)
	{
	    if (_blinking && _timer)
	    {
	        clearInterval(_timer);
	    }
	    _blinking = !_blinking;
	    if (_blinking)
	    {
	        _timer = setInterval(toggleLed, 500);
	        setStatus("Blinking On");
	    }
	    else
	    {
	        setStatus("Blinking Off");
	    }
	}

	function toggleLed()
	{
	    var pinState;
	    _ledOn = !_ledOn;
	    if (_ledOn)
	    {
	        pinState = gpio.GpioPinValue.low;
	        removeClassName("off", _header);
	        addClassName("on", _header);
	    }
	    else
	    {
	        pinState = gpio.GpioPinValue.high;
	        removeClassName("on", _header);
	        addClassName("off", _header);
        }
	    _pin.write(pinState);
	}

	function setStatus(status)
	{
	    if (_status != null)
	    {
	        _status.innerText = status;
	    }
	}

	function removeClassName(className, element)
	{
	    var regEx = new RegExp("(?:^|\\s)" + className + "(?!\\S)", "g");
	    element.className = element.className.replace(regEx, '');
	}

	function addClassName(className, element)
	{
	    element.className += " " + className;
	}

	app.start();
})();
