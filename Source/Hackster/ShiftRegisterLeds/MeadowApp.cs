﻿using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Leds;
using System.Threading;

namespace ShiftRegisterLeds
{
    // public class MeadowApp : App<F7Micro, MeadowApp> <- If you have a Meadow F7v1.*
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        x74595 shiftRegister;

        public MeadowApp()
        {
            Initialize();

            TestX74595();
        }

        void Initialize() 
        {
            var onboardLed = new RgbPwmLed(
                device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            shiftRegister = new x74595(
                device: Device,
                spiBus: Device.CreateSpiBus(),
                pinChipSelect: Device.Pins.D03,
                pins: 8);

            onboardLed.SetColor(Color.Green);
        }

        void TestX74595()
        {
            shiftRegister.Clear(true);

            while (true)
            {
                shiftRegister.Clear();
                for(int i = 0; i < shiftRegister.Pins.AllPins.Count; i++)
                {
                    shiftRegister.WriteToPin(shiftRegister.Pins.AllPins[i], true);
                    Thread.Sleep(500);
                    shiftRegister.WriteToPin(shiftRegister.Pins.AllPins[i], false);
                }
            }
        }
    }
}