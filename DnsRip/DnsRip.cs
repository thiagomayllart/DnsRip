using System.Text.RegularExpressions;

namespace DnsRip
{
    public class DnsRip
    {
        public DnsRip(string input)
        {
            Input = input;
            Evaluated = input.Trim().ToLower();
            ParseInput();
        }

        public string Input { get; }
        public string Evaluated { get; }
        public string Parsed { get; set; }
        public InputType Type { get; set; }

        public enum InputType
        {
            Ip,
            Hostname,
            Invalid
        }

        private void ParseInput()
        {
            var result = Regex.Match(Evaluated, @"(?:[0-9]{1,3}\.){3}[0-9]{1,3}");

            if (result.Success)
            {
                Type = InputType.Ip;
                Parsed = result.Value;
                return;
            }

            result = Regex.Match(Evaluated, @"((([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)+([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9]))($|/)");

            if (result.Success)
            {
                Type = InputType.Hostname;
                Parsed = result.Groups[1].Value;
                return;
            }

            Type = InputType.Invalid;
        }
    }
}