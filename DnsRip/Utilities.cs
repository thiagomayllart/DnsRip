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

            public static bool IsIp4(string query)
            {
                return Uri.CheckHostName(query) == UriHostNameType.IPv4;
            }

            public static bool IsIp6(string query)
            {
                return Uri.CheckHostName(query) == UriHostNameType.IPv6;
            }

            public static bool IsDns(string query)
            {
                return Uri.CheckHostName(query) == UriHostNameType.Dns;
            }

            public static bool IsMx(string query)
            {
                if (!query.Contains(" "))
                    return false;

                var pref = query.Split(' ')[0];
                var ex = query.Split(' ')[1];

                return IsInteger(pref) && IsDns(ex);
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