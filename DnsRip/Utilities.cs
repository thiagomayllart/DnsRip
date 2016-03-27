using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace DnsRip
{
    public partial class DnsRip
    {
        //TODO: Needs tests

        public class Utilities
        {
            public static bool IsIp(string query, out string ip)
            {
                var result = Regex.Match(query, @"(?:[0-9]{1,3}\.){3}[0-9]{1,3}");

                if (result.Success)
                {
                    ip = result.Value;
                    return true;
                }

                ip = null;
                return false;
            }

            public static bool IsIp(string query)
            {
                string ip;
                return IsIp(query, out ip);
            }

            public static bool IsHostname(string query, out string hostname)
            {
                var result = Regex.Match(query, @"((([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)+" +
                    @"([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9]))(.$|$|/)");

                if (result.Success)
                {
                    hostname = result.Groups[1].Value;
                    return true;
                }

                hostname = null;
                return false;
            }

            public static bool IsHostname(string query)
            {
                string hostname;
                return IsHostname(query, out hostname);
            }

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