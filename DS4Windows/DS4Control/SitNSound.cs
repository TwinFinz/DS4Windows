using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DS4Windows
{
    internal class SitNSound
    {
        #region Variables
        private static byte LargeMotorVibration = 0;
        private static byte SmallMotorVibration = 0;
        private static bool Stopped = false;
        private static string CurStream = @"";
        private static SoundPlayer TonePlayer = new(CurStream);

        #endregion

        #region Initialization


        public SitNSound(ControlService controlService)
        {
            controlService.VibrationRecieved += RumbleReceived;
            controlService.ServiceStopped += ControlService_ServiceStopped;
            controlService.ServiceStarted += ControlService_ServiceStarted;

            InternalLoop();
        }

        private void ControlService_ServiceStarted(object sender, EventArgs e)
        {
            Stopped = false;
        }

        private void ControlService_ServiceStopped(object sender, EventArgs e)
        {
            Stopped = true;
        }
        #endregion

        #region Dealing with Rumble
        public void RumbleReceived(byte LargeMotor, byte SmallMotor, int devIndex)
        {
            if (devIndex < 1)
            {
                LargeMotorVibration = LargeMotor;
                SmallMotorVibration = SmallMotor;
            }
        }

        private async void InternalLoop()
        {
            while (!Stopped)
            {
                if (LargeMotorVibration > 10)
                {

                }
                if (SmallMotorVibration > 10)
                {

                }

                await Task.Delay(50);
            }

        }


        #endregion
    }
}
