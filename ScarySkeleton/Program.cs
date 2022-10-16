using Iot.Device.Hcsr04.Esp32;
using Iot.Device.ServoMotor;
using nanoFramework.Hardware.Esp32;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;
using UnitsNet;

namespace NFApp1
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Starting...");
            Configuration.SetPinFunction(18, DeviceFunction.PWM1);
            using var pwmChannel = PwmChannel.CreateFromPin(18, 50);
            var servoMotor = new ServoMotor(
                pwmChannel,
                180,
                900,
                2100);

            servoMotor.Start();
            //Open and close to indicate startup
            servoMotor.WriteAngle(0);
            Thread.Sleep(1000);
            servoMotor.WriteAngle(90);
            Thread.Sleep(1000);
            servoMotor.WriteAngle(0);

            while (true)
            {

                using var sonar = new Hcsr04(17, 16);
                if (sonar.TryGetDistance(out Length distance))
                {
                    Debug.WriteLine($"Distance: {distance.Centimeters} cm");
                    if (distance.Centimeters < 30)
                    {
                        Debug.WriteLine("Scared!");
                        servoMotor.WriteAngle(90);
                    }
                    else
                    {
                        Debug.WriteLine("Not scared!");
                        servoMotor.WriteAngle(0);
                    }

                }
                else
                {
                    Debug.WriteLine("Error reading sensor");
                }
                Thread.Sleep(200);
            }
        }
    }
}