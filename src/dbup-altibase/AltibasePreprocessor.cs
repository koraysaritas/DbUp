using DbUp.Engine;

namespace DbUp.Altibase
{
    public class AltibasePreprocessor : IScriptPreprocessor
    {
        public string Process(string contents) => contents;
    }
}
