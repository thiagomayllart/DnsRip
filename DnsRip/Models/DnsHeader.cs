using System;
using System.Collections.Generic;

namespace DnsRip.Models
{
    public class DnsHeader
    {
        public DnsHeader()
        {
            _id = (ushort)new Random().Next();

            OpCode = DnsRip.OpCode.Query;
            QdCount = 1;
            Recursive = true;
        }

        public DnsHeader(RecordReader rr)
        {
            _id = rr.ReadUInt16();
            _flags = rr.ReadUInt16();

            QdCount = rr.ReadUInt16();
            AnCount = rr.ReadUInt16();
            NsCount = rr.ReadUInt16();
            ArCount = rr.ReadUInt16();
        }

        public ushort QdCount;
        public ushort AnCount;
        public ushort NsCount;
        public ushort ArCount;

        public DnsRip.OpCode OpCode
        {
            get { return (DnsRip.OpCode)GetBits(_flags, 11, 4); }
            set { _flags = SetBits(_flags, 11, 4, (ushort)value); }
        }

        public bool Recursive
        {
            get { return GetBits(_flags, 8, 1) == 1; }
            set { _flags = SetBits(_flags, 8, 1, value); }
        }

        public byte[] Data
        {
            get
            {
                var data = new List<byte>();
                data.AddRange(DnsRip.Tools.ToNetByteOrder(_id));
                data.AddRange(DnsRip.Tools.ToNetByteOrder(_flags));
                data.AddRange(DnsRip.Tools.ToNetByteOrder(QdCount));
                data.AddRange(DnsRip.Tools.ToNetByteOrder(AnCount));
                data.AddRange(DnsRip.Tools.ToNetByteOrder(NsCount));
                data.AddRange(DnsRip.Tools.ToNetByteOrder(ArCount));
                return data.ToArray();
            }
        }

        private readonly ushort _id;
        private ushort _flags;

        private static ushort GetBits(ushort oldValue, int position, int length)
        {
            if (length <= 0 || position >= 16)
                return 0;

            var mask = (2 << (length - 1)) - 1;

            return (ushort)((oldValue >> position) & mask);
        }

        private static ushort SetBits(ushort oldValue, int position, int length, ushort newValue)
        {
            if (length <= 0 || position >= 16)
                return oldValue;

            var mask = (2 << (length - 1)) - 1;

            oldValue &= (ushort)~(mask << position);
            oldValue |= (ushort)((newValue & mask) << position);

            return oldValue;
        }

        private static ushort SetBits(ushort oldValue, int position, int length, bool blnValue)
        {
            return SetBits(oldValue, position, length, blnValue ? (ushort)1 : (ushort)0);
        }
    }
}