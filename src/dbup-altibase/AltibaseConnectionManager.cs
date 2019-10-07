using System.Collections.Generic;
using Altibase.Data.AltibaseClient;
using DbUp.Engine.Transactions;


namespace DbUp.Altibase
{
    public class AltibaseConnectionManager : DatabaseConnectionManager
    {
        /// <summary>
        /// Creates a new Altibase database connection.
        /// </summary>
        /// <param name="connectionString">The Altibase connection string.</param>
        public AltibaseConnectionManager(string connectionString) : base(new DelegateConnectionFactory(l => new AltibaseConnection(connectionString)))
        {
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var commandSplitter = new AltibaseCommandSplitter();
            var scriptStatements = commandSplitter.SplitScriptIntoCommands(scriptContents);
            return scriptStatements;
        }
    }
}
