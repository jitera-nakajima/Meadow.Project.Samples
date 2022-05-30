﻿using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WifiClock
{
    // public class MeadowApp : App<F7Micro, MeadowApp> <- If you have a Meadow F7v1.*
    public class MeadowApp : App<F7FeatherV2, MeadowApp>
    {
        PushButton pushButton;
        MicroGraphics graphics;
        AnalogTemperature analogTemperature;

        bool showDate;

        public MeadowApp()
        {
            Initialize().Wait();

            Start().Wait();
        }

        async Task Initialize()
        { 
            var onboardLed = new RgbPwmLed(
                device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            var display = new Max7219(
                device: Device, 
                spiBus: Device.CreateSpiBus(), 
                chipSelectPin: Device.Pins.D01, 
                deviceCount: 4, 
                maxMode: Max7219.Max7219Mode.Display);
            graphics = new MicroGraphics(display);
            graphics.CurrentFont = new Font4x8();
            graphics.Rotation = RotationType._180Degrees;

            graphics.Clear();
            graphics.DrawText(0, 1,  "WI");
            graphics.DrawText(0, 9,  "FI");
            graphics.DrawText(0, 17, "TI");
            graphics.DrawText(0, 25, "ME");
            graphics.Show();

            pushButton = new PushButton(Device, Device.Pins.D04, ResistorMode.InternalPullUp);
            pushButton.Clicked += PushButtonClicked;

            analogTemperature = new AnalogTemperature(
                device: Device,
                analogPin: Device.Pins.A00,
                sensorType: AnalogTemperature.KnownSensorType.LM35
            );

            var connectionResult = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            if (connectionResult.ConnectionStatus != ConnectionStatus.Success)
            {
                throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
            }

            onboardLed.StartPulse(Color.Green);
        }

        void PushButtonClicked(object sender, EventArgs e)
        {
            showDate = true;
            Thread.Sleep(5000);
            showDate = false;
        }

        async Task Start() 
        {
            while (true)
            {
                DateTime clock = DateTime.Now.AddHours(-8);

                graphics.Clear();

                graphics.DrawText(0, 1, $"{clock:hh}");
                graphics.DrawText(0, 9, $"{clock:mm}");
                graphics.DrawText(0, 17, $"{clock:ss}");
                graphics.DrawText(0, 25, $"{clock:tt}");

                if (showDate)
                {
                    graphics.Clear();

                    graphics.DrawText(0, 1, $"{clock:dd}");
                    graphics.DrawText(0, 9, $"{clock:MM}");
                    graphics.DrawText(0, 17, $"{clock:yy}");

                    graphics.DrawHorizontalLine(0, 24, 7, true);

                    var temperature = await analogTemperature.Read();

                    graphics.DrawText(0, 26, $"{(int) temperature.Celsius}");
                }
                
                graphics.Show();
                Thread.Sleep(1000);
            }
        }
    }
}