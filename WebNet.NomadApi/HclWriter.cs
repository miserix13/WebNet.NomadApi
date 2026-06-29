using System.Text;

namespace WebNet.NomadApi
{
    public class HclWriter
    {
        private readonly StringBuilder _sb = new();
        private int _indent = 0;

        private string Indent => new string(' ', _indent * 2);

        public HclWriter AppendLine(string line = "")
        {
            _sb.AppendLine(Indent + line);
            return this;
        }

        public HclWriter Block(string name, string label, Action body)
        {
            AppendLine($"{name} \"{label}\" {{");
            _indent++;
            body();
            _indent--;
            AppendLine("}");
            return this;
        }

        public HclWriter Attribute(string key, object value)
        {
            string formatted = value switch
            {
                string s => $"\"{s}\"",
                bool b => b.ToString().ToLower(),
                _ => value.ToString()
            };

            AppendLine($"{key} = {formatted}");
            return this;
        }

        public override string ToString() => _sb.ToString();
    }
}