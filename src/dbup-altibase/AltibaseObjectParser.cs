using DbUp.Support;

namespace DbUp.Altibase
{
    public class AltibaseObjectParser : SqlObjectParser
    {
        public AltibaseObjectParser() : base("\"", "\"")
        {
        }
    }
}
