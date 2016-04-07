using DnsRip.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace DnsRip
{
    public partial class DnsRip
    {
        public class Tools
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

            public static bool IsIp(string query)
            {
                return Uri.CheckHostName(query) == UriHostNameType.IPv4 ||
                    Uri.CheckHostName(query) == UriHostNameType.IPv6;
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

            public static bool IsSoa(string query)
            {
                if (!query.Contains(" "))
                    return false;

                var values = query.Split(' ');
                var index = 0;

                foreach (var value in values)
                {
                    index++;

                    if (index <= 2 && !IsDns(value))
                        return false;

                    if (index > 2 && !IsInteger(value))
                        return false;
                }

                return true;
            }
        }
    }
}