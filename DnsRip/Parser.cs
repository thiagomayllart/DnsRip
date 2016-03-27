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
                string parsed;

                if (Utilities.IsIp(Evaluated, out parsed))
                {
                    Type = InputType.Ip;
                    Parsed = parsed;
                    return;
                }

                if (Utilities.IsHostname(Evaluated, out parsed))
                {
                    Type = InputType.Hostname;
                    Parsed = parsed;
                    return;
                }

                Type = InputType.Invalid;
            }
        }
    }
}