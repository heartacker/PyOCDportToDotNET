using HidLibrary;
using openocd.CmsisDap.Backend;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hid.Net;
using Hid.Net.Windows;
using Device.Net.Windows;
using Device.Net;
using Usb.Net.Windows;

namespace openocd.CmsisDap
{
    public class DapAccessConfiguration : IDapAccessConfiguration
    {
        private static IEnumerable<ConnectedDeviceDefinition> _devices;
        private static bool _TransUsbFromHost = false;
        public static void TransUsbFromHost(IEnumerable<ConnectedDeviceDefinition> devices)
        {
            _devices = devices;
            _TransUsbFromHost = true;
        }
        public bool isAvailable
        { get; private set; }

        public List<IDapAccessLink> get_connected_devices()
        {
            throw new NotImplementedException();
        }

        public object set_args(object arg_list)
        {
            throw new NotImplementedException(); ;
        }

        public IDapAccessLink get_device(object device_id)
        {
            throw new NotImplementedException(); ;
        }


        // 
        //         returns all the connected devices which matches HidApiUSB.vid/HidApiUSB.pid.
        //         returns an array of HidApiUSB (Interface) objects
        //         
        // [staticmethod]
        public static List<IBackend> getAllConnectedInterface()
        {
            List<IBackend> boards = new List<IBackend>();
            IEnumerable<HidDevice> devices = HidDevices.Enumerate();
            if (!devices.Any())
            {
                Trace.TraceInformation("No Mbed device connected");
                return boards;
            }
            foreach (HidDevice deviceInfo in devices)
            {
                deviceInfo.ReadProduct(out byte[] data);
                string product_name = UnicodeEncoding.Unicode.GetString(data);
                Console.WriteLine(product_name);
                if (!product_name.Contains("CMSIS-DAP"))
                {
                    // Skip non cmsis-dap devices
                    continue;
                }
                HidDevice dev = deviceInfo;
                try
                {
                    //dev = hid.device(vendor_id: deviceInfo["vendor_id"], product_id: deviceInfo["product_id"], path: deviceInfo["path"]);
                }
                catch //(IOError)
                {
                    Trace.TraceInformation("Failed to open Mbed device");
                    continue;
                }
                BackendHidUsb new_board = new BackendHidUsb(dev);
                boards.Add(new_board);
            }
            return boards;
        }

        /// <summary>
        /// TODO: Test these!
        /// </summary>
        private static readonly DebugLogger logger = new DebugLogger();
        private static readonly DebugTracer tracer = new DebugTracer();

        public async static Task<List<IBackend>> getAllConnectedInterface(bool _new)
        {

            if (!_new)
            {
                return getAllConnectedInterface();
            }
            if (_TransUsbFromHost)
            {
                return getAllConnectedInterface(_devices);
            }


            WindowsUsbDeviceFactory.Register(logger, tracer);
            WindowsHidDeviceFactory.Register(logger, tracer);

            var devices = await DeviceManager.Current.GetConnectedDeviceDefinitionsAsync(null);
#if false
            var w = task.Wait(2000);
            if (!w) return boards; 
            var devices = task.Result;
#endif

            return getAllConnectedInterface(devices);
        }

        private static List<IBackend> getAllConnectedInterface(IEnumerable<ConnectedDeviceDefinition> devices)
        {
            List<IBackend> boards = new List<IBackend>();

            if (!devices.Any())
            {
                Trace.TraceInformation("No Mbed device connected");
                return boards;
            }
            _devices = devices;
            foreach (var deviceInfo in devices)
            {
                //deviceInfo.ReadProduct(out byte[] data);
                string product_name = deviceInfo.ProductName;
                Console.WriteLine(product_name);
                if (!product_name.Contains("CMSIS-DAP"))
                {
                    // Skip non cmsis-dap devices
                    continue;
                }
                WindowsHidDevice dev = new WindowsHidDevice(deviceInfo.DeviceId)
                {
                    ConnectedDeviceDefinition = deviceInfo
                };
                try
                {
                    //dev = hid.device(vendor_id: deviceInfo["vendor_id"], product_id: deviceInfo["product_id"], path: deviceInfo["path"]);
                }
                catch //(IOError)
                {
                    Trace.TraceInformation("Failed to open Mbed device");
                    continue;
                }
                BackendHidUsbNew new_board = new BackendHidUsbNew(dev);
                boards.Add(new_board);
            }
            return boards;
        }
    }
}
