using System;
using System.Collections.Generic;
using System.Net;

namespace DnsRip
{
    public partial class DnsRip
    {
        //TODO: Needs tests

        public class Utilities
        {
            public static bool IsInteger(object query)
            {
                int integer;
                return int.TryParse(query.ToString(), out integer);
            }

            internal static IEnumerable<byte> ToNetByteOrder(ushort value)
            {
                return BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)value));
            }

            internal static string ToNameFormat(string query)
            {
                if (!query.EndsWith("."))
                    query += ".";

                return query;
            }
        }
    }
}