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
            public static ParseResult Parse(string input)
            {
                var result = new ParseResult
                {
                    Input = input,
                    Evaluated = input.Trim().ToLower()
                };

                var match = Regex.Match(result.Evaluated, @"((?:[0-9]{1,3}\.){3}[0-9]{1,3}|([A-Fa-f0-9]{1,4}::?){1,7}[A-Fa-f0-9]{1,4})");

                if (match.Success)
                {
                    result.Type = InputType.Ip;
                    result.Parsed = match.Value;
                    return result;
                }

                match = Regex.Match(result.Evaluated, @"((([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)+([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9]))(.$|$|/|:)");

                if (match.Success)
                {
                    result.Type = InputType.Hostname;
                    result.Parsed = match.Groups[1].Value;
                    return result;
                }

                result.Type = InputType.Invalid;
                return result;
            }

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

            internal static string ToArpaRequest(string query)
            {
                IPAddress ip;

                if (IPAddress.TryParse(query, out ip))
                {
                    var result = new StringBuilder();

                    switch (ip.AddressFamily)
                    {
                        case AddressFamily.InterNetwork:
                            {
                                result.Append("in-addr.arpa.");

                                foreach (var b in ip.GetAddressBytes())
                                    result.Insert(0, $"{b}.");

                                return result.ToString();
                            }
                        case AddressFamily.InterNetworkV6:
                            {
                                result.Append("ip6.arpa.");

                                foreach (var b in ip.GetAddressBytes())
                                {
                                    result.Insert(0, $"{(b >> 4) & 0xf:x}.");
                                    result.Insert(0, $"{(b >> 0) & 0xf:x}.");
                                }

                                return result.ToString();
                            }
                    }
                }

                return query;
            }
        }
    }
}