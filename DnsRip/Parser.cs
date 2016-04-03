using System.Text.RegularExpressions;

namespace DnsRip
{
    public partial class DnsRip
    {
        public class Parser
        {
            public Parser(string input)
            {
                Input = input;
                Evaluated = input.Trim().ToLower();
                ParseInput();
            }

            public string Input { get; }
            public string Evaluated { get; }
            public string Parsed { get; set; }
            public InputType Type { get; set; }

            private void ParseInput()
            {
                var result = Regex.Match(Evaluated, @"((?:[0-9]{1,3}\.){3}[0-9]{1,3}|([A-Fa-f0-9]{1,4}::?){1,7}[A-Fa-f0-9]{1,4})");

                if (result.Success)
                {
                    Type = InputType.Ip;
                    Parsed = result.Value;
                    return;
                }

                result = Regex.Match(Evaluated, @"((([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)+([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9]))(.$|$|/|:)");

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
}