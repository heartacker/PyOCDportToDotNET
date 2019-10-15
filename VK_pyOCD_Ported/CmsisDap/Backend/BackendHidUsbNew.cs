using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

//namespace pyDAPAccess.Interface
//{  
//    public static class hidapi_backend
//    {
//        static hidapi_backend()
//        {
//            //isAvailable = false;
//            //Trace.TraceError("cython-hidapi is required on a Mac OS X Machine");
//        }
//    public class HidApiUSB : Interface


namespace openocd.CmsisDap.Backend
{

    // This class provides basic functions to access
    // a USB HID device using cython-hidapi:
    //     - write/read an endpoint
    // 
    public class BackendHidUsbNew : IBackend
    {
        public readonly string vendor_name;
        public readonly string product_name;
        public readonly object device_info;
        public readonly UInt16 vid;
        public readonly UInt16 pid;
        public byte packet_count { get; set; }
        internal UInt16 packet_size;
        internal string serial_number { get; }
        internal readonly Hid.Net.Windows.WindowsHidDevice device;

        public BackendHidUsbNew(Hid.Net.Windows.WindowsHidDevice deviceInfo)
        {
            // Vendor page and usage_id = 2
            this.packet_size = 64;
#if false
            if (deviceInfo.ReadManufacturer(out byte[] data))
            {
                this.vendor_name = UnicodeEncoding.Unicode.GetString(data);
            }
            else
            {
                throw new Exception();
            }
            if (deviceInfo.ReadProduct(out data))
            {
                this.product_name = UnicodeEncoding.Unicode.GetString(data);
            }
            else
            {
                throw new Exception();
            }
            if (deviceInfo.ReadSerialNumber(out data))
            {
                this.serial_number = UnicodeEncoding.Unicode.GetString(data);
            }
            else
            {
                throw new Exception();
            }
            this.vid = (UInt16)deviceInfo.Attributes.VendorId;
            this.pid = (UInt16)deviceInfo.Attributes.ProductId; 
#endif
            this.device_info = deviceInfo;
            this.device = deviceInfo;
        }

        public bool isAvailable { get; set; }

        public string getInfo()
        {
            throw new NotImplementedException();
        }

        public void init()
        {
        }

        public void open()
        {
            try
            {
                // open_path(this.device_info["path"]);
                this.device.InitializeAsync();
#if false
                this.device.OpenDevice(DeviceMode.NonOverlapped, DeviceMode.NonOverlapped, ShareMode.Exclusive);

#endif
            }
            catch // (IOError)
            {
                throw new Exception("Unable to open device"); //DAPAccessIntf.DeviceError
            }
        }


        // 
        //         write data on the OUT endpoint associated to the HID interface
        //         
        public void write(List<byte> data)
        {
            foreach (var _ in Enumerable.Range(0, (int)this.packet_size - data.Count))
            {
                data.Add(0);
            }
            Debug.Assert(this.packet_size == 64);
            Debug.Assert(data.Count == 64);
            //Trace.TraceInformation("send: {0}", data);
            List<byte> packet = new List<byte>() { 0 };
            packet.AddRange(data);
            this.device.WriteAsync(packet.ToArray());
            // HidReport report = new HidReport(data.Count)
            // {
            //     Data = data.ToArray()
            // };
            // this.device.WriteReport(report);
        }

        // 
        //         read data on the IN endpoint associated to the HID interface
        //         
        public List<byte> read(int size = -1, int timeout = -1)
        {
            // HidReport report = this.device.ReadReport();
            var result = this.device.ReadAsync();
            var r = result.Wait(2000);
            if (r)
            //if (report.Exists)
            {
                // return report.Data.ToList(); 
                List<byte> bytes = result.Result.Data.ToList();
                return bytes.GetRange(1, bytes.Count - 1);
            }
            else
            {
                throw new Exception();
            }
        }

        public virtual string getSerialNumber()
        {
            return this.serial_number;
        }

        // 
        //         close the interface
        //         
        public void close()
        {
            Trace.TraceInformation("closing HID USB interface");
            //this.device.CloseDevice();
            this.device.Close();
        }

        public void setPacketSize(UInt16 size)
        {
            this.packet_size = size;
        }
    }
}

