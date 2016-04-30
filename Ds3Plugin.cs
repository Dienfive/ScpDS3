using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScpControl;
using ODIF;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Management;
using ODIF.Extensions;
using System.Threading;

namespace ScpControl
{
    [PluginInfo(
        PluginName = "DS3 Device",
        PluginDescription = "Detects DS3 devices using Scarlet Crush's custom DS3 driver.",
        PluginID = 40,
        PluginAuthorName = "Scarlet Crush, Input Mapper",
        PluginAuthorEmail = "a",
        PluginAuthorURL = "a",
        PluginIconPath = @"pack://application:,,,/ScpControl;component/Resources/128.png"
    )]
    public class Ds3Plugin:InputDevicePlugin
    {
        ScpControl.RootHub rootHub = new RootHub();

        public Ds3Plugin()
        {
            rootHub.Open();
            rootHub.Start();
            ODIF.Global.DeviceAdded += Global_DeviceAdded;
            ODIF.Global.DeviceRemoved += Global_DeviceRemoved;
            DetectDevices();
        }

        private void Global_DeviceRemoved(object sender, EventArrivedEventArgs e)
        {
            var instance = ((PropertyData)(e.NewEvent.Properties["TargetInstance"]));
            var obj = (ManagementBaseObject)instance.Value;
            string guid = "{E2824A09-DBAA-4407-85CA-C8E8FF5F6FFA}";
            string devid = @"\\?\" + obj.Properties["DeviceID"].Value.ToString().Replace("\\", "#") + "#" + guid;
            rootHub.Notify(ScpDevice.Notified.Removal, guid, devid);
            DetectDevices();
        }

        private void Global_DeviceAdded(object sender, EventArrivedEventArgs e)
        {
            var instance = ((PropertyData)(e.NewEvent.Properties["TargetInstance"]));
            var obj = (ManagementBaseObject)instance.Value;
            string guid = "{E2824A09-DBAA-4407-85CA-C8E8FF5F6FFA}";
            string devid = @"\\?\" + obj.Properties["DeviceID"].Value.ToString().Replace("\\", "#") + "#" + guid;
            rootHub.Notify(ScpDevice.Notified.Arrival, guid, devid);
            DetectDevices();
        }

        internal void DetectDevices()
        {
            for (int i = 0; i <= 3; i++)
            {
                if (rootHub.Pad[i].State == DsState.Connected && this.Devices.Where(d => (d as ScDs3Device).deviceNum == i).Count() == 0)
                    Devices.Add(new ScDs3Device(i, rootHub));
                if (rootHub.Pad[i].State != DsState.Connected && this.Devices.Where(d => (d as ScDs3Device).deviceNum == i).Count() > 0)
                    Devices.Remove(this.Devices.Where(d => (d as ScDs3Device).deviceNum == i).First());
            }
        }
        protected override void Dispose(bool disposing)
        {
            rootHub.Stop();
            base.Dispose(disposing);
        }
    }

    public class ScDs3Device : InputDevice
    {
        internal int deviceNum;
        internal DS3Device deviceClass = new DS3Device();
        internal RootHub rootHub;

        public ScDs3Device(int deviceNum,RootHub rootHub)
        {
            this.rootHub = rootHub;
            this.deviceNum = deviceNum;
            this.DeviceName = "Dualshock 3 controller " + deviceNum + 1;
            this.StatusIcon = Properties.Resources._128.ToImageSource();

            InputChannels.Add(deviceClass.Circle);
            InputChannels.Add(deviceClass.Cross);
            InputChannels.Add(deviceClass.Square);
            InputChannels.Add(deviceClass.Triangle);

            InputChannels.Add(deviceClass.vCircle);
            InputChannels.Add(deviceClass.vCross);
            InputChannels.Add(deviceClass.vSquare);
            InputChannels.Add(deviceClass.vTriangle);

            InputChannels.Add(deviceClass.LSx);
            InputChannels.Add(deviceClass.LSy);
            InputChannels.Add(deviceClass.RSx);
            InputChannels.Add(deviceClass.RSy);

            InputChannels.Add(deviceClass.L1);
            InputChannels.Add(deviceClass.L2);
            InputChannels.Add(deviceClass.L3);
            InputChannels.Add(deviceClass.R1);
            InputChannels.Add(deviceClass.R2);
            InputChannels.Add(deviceClass.R3);

            InputChannels.Add(deviceClass.PS);
            InputChannels.Add(deviceClass.Select);
            InputChannels.Add(deviceClass.Start);

            InputChannels.Add(deviceClass.DUp);
            InputChannels.Add(deviceClass.DDown);
            InputChannels.Add(deviceClass.DLeft);
            InputChannels.Add(deviceClass.DRight);

            OutputChannels.Add(deviceClass.BigRumble);
            OutputChannels.Add(deviceClass.SmallRumble);
            OutputChannels.Add(deviceClass.LightBar);

            (rootHub.Pad[deviceNum] as UsbDevice).Report += RootHub_USBReport;
        }

        private void RootHub_USBReport(object sender, ReportEventArgs e)
        {
            //if ((sender as UsbDevice).PadId != (DsPadId)deviceNum)
            //    return;

            deviceClass.Cross.Value = ((byte)e.Report[11] & (1 << 6)) != 0;
            deviceClass.Circle.Value = ((byte)e.Report[11] & (1 << 5)) != 0;
            deviceClass.Square.Value = ((byte)e.Report[11] & (1 << 7)) != 0;
            deviceClass.Triangle.Value = ((byte)e.Report[11] & (1 << 4)) != 0;

            deviceClass.L1.Value = ((byte)e.Report[11] & (1 << 2)) != 0;
            deviceClass.R1.Value = ((byte)e.Report[11] & (1 << 3)) != 0;

            deviceClass.L3.Value = ((byte)e.Report[10] & (1 << 1)) != 0;
            deviceClass.R3.Value = ((byte)e.Report[10] & (1 << 2)) != 0;

            deviceClass.DUp.Value = ((byte)e.Report[10] & (1 << 4)) != 0;
            deviceClass.DDown.Value = ((byte)e.Report[10] & (1 << 6)) != 0;
            deviceClass.DLeft.Value = ((byte)e.Report[10] & (1 << 7)) != 0;
            deviceClass.DRight.Value = ((byte)e.Report[10] & (1 << 5)) != 0;

            deviceClass.L2.Value = e.Report[26] / 255f;
            deviceClass.R2.Value = e.Report[27] / 255f;

            deviceClass.LSx.Value = SafeStickValue(e.Report[14] - 127) / 127f;
            deviceClass.LSy.Value = SafeStickValue(e.Report[15] - 127) / 127f;
            deviceClass.RSx.Value = SafeStickValue(e.Report[16] - 127) / 127f;
            deviceClass.RSy.Value = SafeStickValue(e.Report[17] - 127) / 127f;

            deviceClass.Select.Value = ((byte)e.Report[10] & (1 << 0)) != 0;
            deviceClass.Start.Value = ((byte)e.Report[10] & (1 << 3)) != 0;

            deviceClass.PS.Value = ((byte)e.Report[12] & (1 << 0)) != 0;

            deviceClass.vCross.Value = e.Report[32] / 255f;
            deviceClass.vCircle.Value = e.Report[31] / 255f;
            deviceClass.vSquare.Value = e.Report[33] / 255f;
            deviceClass.vTriangle.Value = e.Report[30] / 255f;

            //Console.WriteLine(String.Join(" ", e.Report));
        }
        private int SafeStickValue(int value)
        {
            value = value > 127 ? 127 : value;
            value = value < -127 ? -127 : value;
            return value;
        }
    }
}
