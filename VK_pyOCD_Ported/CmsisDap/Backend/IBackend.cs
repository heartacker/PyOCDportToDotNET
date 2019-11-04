using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace openocd.CmsisDap
{
    [ServiceContract]
    public interface IBackend
    {
        bool isAvailable { get; set; }

        byte packet_count { get; set; }


        [OperationContract]
        byte getset_packet_count(byte? valu, bool get);


        [OperationContract]
        void open();

        [OperationContract]
        void init();

        [OperationContract]
        void write(List<byte> data);

        [OperationContract]
        List<byte> read(int size = -1, int timeout = -1);

        Task<List<byte>> readAsync(int size = -1, int timeout = -1);

        [OperationContract]
        List<byte> WriteAndRead(List<byte> data);
        Task<List<byte>> WriteAndReadAsync(List<byte> data);

        [OperationContract]
        string getInfo();

        [OperationContract]
        void close();

        [OperationContract]
        void setPacketSize(UInt16 size);

        [OperationContract]
        string getSerialNumber();
    }
}
