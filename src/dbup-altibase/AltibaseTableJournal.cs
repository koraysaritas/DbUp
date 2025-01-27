﻿using System;
using System.Data;
using System.Globalization;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;

namespace DbUp.Altibase
{
    public class AltibaseTableJournal : TableJournal
    {
        bool journalExists;
        /// <summary>
        /// Creates a new Altibase table journal.
        /// </summary>
        /// <param name="connectionManager">The Altibase connection manager.</param>
        /// <param name="logger">The upgrade logger.</param>
        /// <param name="schema">The name of the schema the journal is stored in.</param>
        /// <param name="table">The name of the journal table.</param>
        public AltibaseTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string schema, string table)
            : base(connectionManager, logger, new AltibaseObjectParser(), schema, table)
        {
        }

        public static CultureInfo English = new CultureInfo("en-US", false);

        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName)
        {
            var fqSchemaTableName = UnquotedSchemaTableName;
            return
                $@" CREATE TABLE {fqSchemaTableName} 
                (
                    schemaversionid NUMBER(10),
                    scriptname VARCHAR2(255) NOT NULL,
                    applied DATE NOT NULL,
                    CONSTRAINT PK_{ fqSchemaTableName } PRIMARY KEY (schemaversionid) 
                )";
        }

        protected string CreateSchemaTableSequenceSql()
        {
            var fqSchemaTableName = UnquotedSchemaTableName;
            return $@" CREATE SEQUENCE {fqSchemaTableName}_sequence";
        }

        protected string CreateSchemaTableTriggerSql()
        {
            var fqSchemaTableName = UnquotedSchemaTableName;
            return $@" CREATE OR REPLACE TRIGGER {fqSchemaTableName}_on_insert
                    BEFORE INSERT ON {fqSchemaTableName}
                    REFERENCING NEW ROW NEW_ROW
                    FOR EACH ROW
                    BEGIN
                        SELECT {fqSchemaTableName}_sequence.nextval
                        INTO :NEW_ROW.schemaversionid
                        FROM dual;
                    END;
                ";
        }

        protected override string GetInsertJournalEntrySql(string scriptName, string applied)
        {
            var unquotedSchemaTableName = UnquotedSchemaTableName.ToUpper(English);
            return $"insert into {unquotedSchemaTableName} (ScriptName, Applied) values (:" + scriptName.Replace("@", "") + ",:" + applied.Replace("@", "") + ")";
        }

        protected override string GetJournalEntriesSql()
        {
            var unquotedSchemaTableName = UnquotedSchemaTableName.ToUpper(English);
            return $"select scriptname from {unquotedSchemaTableName} order by scriptname";
        }

        protected override string DoesTableExistSql()
        {
            var unquotedSchemaTableName = UnquotedSchemaTableName.ToUpper(English);
            return $"select 1 from SYSTEM_.SYS_TABLES_ where TABLE_NAME = '{unquotedSchemaTableName}'";
        }

        protected IDbCommand GetCreateTableSequence(Func<IDbCommand> dbCommandFactory)
        {
            var command = dbCommandFactory();
            command.CommandText = CreateSchemaTableSequenceSql();
            command.CommandType = CommandType.Text;
            return command;
        }

        protected IDbCommand GetCreateTableTrigger(Func<IDbCommand> dbCommandFactory)
        {
            var command = dbCommandFactory();
            command.CommandText = CreateSchemaTableTriggerSql();
            command.CommandType = CommandType.Text;
            return command;
        }

        public override void EnsureTableExistsAndIsLatestVersion(Func<IDbCommand> dbCommandFactory)
        {
            if (!journalExists && !DoesTableExist(dbCommandFactory))
            {
                Log().WriteInformation(string.Format("Creating the {0} table", FqSchemaTableName));

                // We will never change the schema of the initial table create.
                using (var command = GetCreateTableSequence(dbCommandFactory))
                {
                    command.ExecuteNonQuery();
                }

                // We will never change the schema of the initial table create.
                using (var command = GetCreateTableCommand(dbCommandFactory))
                {
                    command.ExecuteNonQuery();
                }

                // We will never change the schema of the initial table create.
                using (var command = GetCreateTableTrigger(dbCommandFactory))
                {
                    command.ExecuteNonQuery();
                }

                Log().WriteInformation(string.Format("The {0} table has been created", FqSchemaTableName));

                OnTableCreated(dbCommandFactory);
            }

            journalExists = true;
        }
    }
}
