using System;
using Microsoft.Isam.Esent.Interop;

namespace Rhino.DHT
{
    public static class EsentExtension
    {
        public static void WithDatabase(this Instance instance, string database, Action<Session, JET_DBID> action)
        {
            using (var session = new Session(instance))
            {
                JET_DBID dbid;
                Api.JetOpenDatabase(session, database, "", out dbid, OpenDatabaseGrbit.ReadOnly);
                try
                {
                    action(session, dbid);
                }
                finally
                {
                    Api.JetCloseDatabase(session, dbid, CloseDatabaseGrbit.None);
                }
            }
        }
    }
}