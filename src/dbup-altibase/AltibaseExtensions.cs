using System.Linq;
using DbUp.Builder;
using DbUp.Engine.Transactions;

namespace DbUp.Altibase
{
    public static class AltibaseExtensions
    {
        public static UpgradeEngineBuilder AltibaseDatabase(this SupportedDatabases supported, string connectionString)
        {
            foreach (var pair in connectionString.Split(';').Select(s => s.Split('=')).Where(pair => pair.Length == 2).Where(pair => pair[0].ToLower() == "database"))
            {
                return AltibaseDatabase(new AltibaseConnectionManager(connectionString), pair[1]);
            }

            return AltibaseDatabase(new AltibaseConnectionManager(connectionString));
        }

        /// <summary>
        /// Creates an upgrader for Altibase databases.
        /// </summary>
        /// <param name="supported">Fluent helper type.</param>
        /// <param name="connectionString">Altibase database connection string.</param>
        /// <param name="schema">Which Altibase schema to check for changes</param>
        /// <returns>
        /// A builder for a database upgrader designed for Altibase databases.
        /// </returns>
        public static UpgradeEngineBuilder AltibaseDatabase(this SupportedDatabases supported, string connectionString, string schema)
        {
            return AltibaseDatabase(new AltibaseConnectionManager(connectionString), schema);
        }

        /// <summary>
        /// Creates an upgrader for Altibase databases.
        /// </summary>
        /// <param name="supported">Fluent helper type.</param>
        /// <param name="connectionManager">The <see cref="AltibaseConnectionManager"/> to be used during a database upgrade.</param>
        /// <returns>
        /// A builder for a database upgrader designed for Altibase databases.
        /// </returns>
        public static UpgradeEngineBuilder AltibaseDatabase(this SupportedDatabases supported, IConnectionManager connectionManager)
            => AltibaseDatabase(connectionManager);

        /// <summary>
        /// Creates an upgrader for Altibase databases.
        /// </summary>
        /// <param name="connectionManager">The <see cref="AltibaseConnectionManager"/> to be used during a database upgrade.</param>
        /// <returns>
        /// A builder for a database upgrader designed for Altibase databases.
        /// </returns>
        public static UpgradeEngineBuilder AltibaseDatabase(IConnectionManager connectionManager)
        {
            return AltibaseDatabase(connectionManager, null);
        }

        /// <summary>
        /// Creates an upgrader for Altibase databases.
        /// </summary>
        /// <param name="connectionManager">The <see cref="AltibaseConnectionManager"/> to be used during a database upgrade.</param>
        /// /// <param name="schema">Which Altibase schema to check for changes</param>
        /// <returns>
        /// A builder for a database upgrader designed for Altibase databases.
        /// </returns>
        public static UpgradeEngineBuilder AltibaseDatabase(IConnectionManager connectionManager, string schema)
        {
            var builder = new UpgradeEngineBuilder();
            builder.Configure(c => c.ConnectionManager = connectionManager);
            builder.Configure(c => c.ScriptExecutor = new AltibaseScriptExecutor(() => c.ConnectionManager, () => c.Log, null, () => c.VariablesEnabled, c.ScriptPreprocessors, () => c.Journal));
            builder.Configure(c => c.Journal = new AltibaseTableJournal(() => c.ConnectionManager, () => c.Log, schema, "schemaversions"));
            builder.WithPreprocessor(new AltibasePreprocessor());
            return builder;
        }
    }
}
