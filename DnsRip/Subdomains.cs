using System.Collections.Generic;

//TODO: Cleanup

namespace DnsRip
{
    internal class Subdomains
    {
        public IEnumerable<string> SubDomains
        {
            get
            {
                //if (Type == DnsRip.InputType.Ip)
                //    return null;

                return new[]
                {
                    "www.",
                    "m.",
                    "blog.",
                    "ftp.",
                    "imap.",
                    "pop.",
                    "smtp.",
                    "mail.",
                    "webmail."
                };
            }
        }
    }
}