using System;
using System.Collections.Generic;

namespace DnsRip.Models
{
    public class ResolveOptions
    {
        public ResolveOptions()
        { }

        public ResolveOptions(string server)
        {
            Servers = new[] { server };
        }

        public ResolveOptions(IEnumerable<string> servers)
        {
            Servers = servers;
        }

        public IEnumerable<string> Servers { get; set; }

        public int Retries
        {
            get { return _retries == 0 ? 3 : _retries; }
            set { _retries = value; }
        }

        public TimeSpan Timeout
        {
            get { return _timeout.Ticks == 0 ? TimeSpan.FromSeconds(1) : _timeout; }
            set { _timeout = value; }
        }

        private int _retries;
        private TimeSpan _timeout;
    }
}