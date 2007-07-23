using System;
using System.Collections.Generic;
using System.Data;
using Boo.Lang;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL
{
	public abstract class BaseDataElement<T> : ContextfulObjectBase<T>, IConnectionUser
		where T : BaseDataElement<T>
	{
		private readonly string name;
		private string command;
		private ICallable commandGenerator;
		private string connection;
		private Connection connectionInstance;
		protected IDbConnection dbConnection;
		protected IDictionary<string, ICallable> parameters;

		public BaseDataElement(string name)
		{
			this.name = name;
			parameters = new Dictionary<string, ICallable>(StringComparer.InvariantCultureIgnoreCase);
		}

		public void AddParameter(string parameterName, ICallable callable)
		{
			if (parameters.ContainsKey(parameterName))
			{
				throw new DuplicateKeyException("[Source " + Name + "] already has a parameter called '" + parameterName + "'");
			}
			parameters.Add(parameterName, callable);
		}

		public object GetParameterValue(string parameterName)
		{
			if (parameters.ContainsKey(parameterName) == false)
			{
				throw new KeyNotFoundException("[Source " + Name + "] does not contains a parameter called '" + parameterName + "'");
			}
			using (EnterContext())
			{
				return parameters[parameterName].Call(new object[0]);
			}
		}

		public string Connection
		{
			get { return connection ?? Name; }
			set { connection = value; }
		}

		public ICallable CommandGenerator
		{
			get { return commandGenerator; }
			set { commandGenerator = value; }
		}

		public override string Name
		{
			get { return name; }
		}

		public Connection ConnectionInstance
		{
			get { return connectionInstance; }
		}

		public bool TryAcquireConnection()
		{
			dbConnection = ConnectionInstance.TryAcquire();
			return dbConnection != null;
		}

		public void ReleaseConnection()
		{
			ConnectionInstance.Release(dbConnection);
		}

		public string Command
		{
			get
			{
				if (CommandGenerator != null)
				{
					using (EnterContext())
					{
						return (string) CommandGenerator.Call(new object[0]);
					}
				}
				return command;
			}
			set { command = value; }
		}

		public void Parameters(ICallable block)
		{
			using (EnterContext())
			{
				block.Call(new object[] {this});
			}
		}

		public void Validate(ICollection<string> messages)
		{
			bool hasConnection = EtlConfigurationContext.Current.Connections.ContainsKey(Connection);
			if(hasConnection==false)
			{
				string msg = string.Format("Could not find connection '{0}' in context '{1}'", Connection, EtlConfigurationContext.Current.Name);
				Logger.WarnFormat("{0} failed validation: {1}",Name, msg);
				messages.Add(msg);
			}
		}

		public void PerformSecondStagePass()
		{
			connectionInstance = EtlConfigurationContext.Current.Connections[Connection];
		}

		protected void AddParameters(IDbCommand dbCommand)
		{
			foreach (KeyValuePair<string, ICallable> pair in parameters)
			{
				IDbDataParameter parameter = dbCommand.CreateParameter();
				parameter.ParameterName = pair.Key;
				object value = pair.Value.Call(new object[0]) ?? DBNull.Value;
				parameter.Value = value;
				dbCommand.Parameters.Add(parameter);
			}
		}
	}
}