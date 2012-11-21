using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Database
{
	class SQLite
	{
		private string connectionString = string.Empty;
		private SQLiteConnection connection;
		private SQLiteCommand command;
		private SQLiteDataReader reader;
		private DataTable table;

		public SQLite(String connectionString)
		{
			setConnectionString(connectionString);
		}

		public bool openConnection()
		{
			try
			{
				setConnection(new SQLiteConnection("Data source=" + getConnectionString() + ";Version=3;"));
				getConnection().Open();
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				System.Diagnostics.Trace.WriteLine(System.DateTime.Now.ToString() + " :: " + e);
				return false;
			}
		}

		public bool closeConnection()
		{
			try
			{
				getConnection().Close();
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				System.Diagnostics.Trace.WriteLine(System.DateTime.Now.ToString() + " :: " + e);
				return false;
			}
		}

		public bool executeQuery(string query)
		{
			try
			{
				setTable(new DataTable());
				setCommand(new SQLiteCommand(getConnection()));
				this.command.CommandText = query;
				setReader(getCommand().ExecuteReader());
				getTable().Load(getReader());
				getReader().Close();
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				System.Diagnostics.Trace.WriteLine(System.DateTime.Now.ToString() + " :: " + e);
				return false;
			}
		}

		public bool executeNonQuery(string query)
		{
			try
			{
				setCommand(new SQLiteCommand(getConnection()));
				this.command.CommandText = query;
				getCommand().ExecuteNonQuery();
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				System.Diagnostics.Trace.WriteLine(System.DateTime.Now.ToString() + " :: " + e);
				return false;
			}
		}

		public bool beginTransaction()
		{
			try
			{
				executeNonQuery("BEGIN IMMEDIATE TRANSACTION");
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				System.Diagnostics.Trace.WriteLine(System.DateTime.Now.ToString() + " :: " + e);
				return false;
			}
		}

		public bool endTransaction()
		{
			try
			{
				executeNonQuery("COMMIT TRANSACTION");
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				System.Diagnostics.Trace.WriteLine(System.DateTime.Now.ToString() + " :: " + e);
				return false;
			}
		}

		public bool compactDatabase()
		{
			try
			{
				executeNonQuery("VACUUM");
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				System.Diagnostics.Trace.WriteLine(System.DateTime.Now.ToString() + " :: " + e);
				return false;
			}
		}

		private string getConnectionString()
		{
			return this.connectionString;
		}

		private void setConnectionString(string connectionString)
		{
			this.connectionString = connectionString;
		}

		private SQLiteConnection getConnection()
		{
			return this.connection;
		}

		private void setConnection(SQLiteConnection connection)
		{
			this.connection = connection;
		}

		private SQLiteCommand getCommand()
		{
			return this.command;
		}

		private void setCommand(SQLiteCommand command)
		{
			this.command = command;
		}

		private SQLiteDataReader getReader()
		{
			return this.reader;
		}

		private void setReader(SQLiteDataReader reader)
		{
			this.reader = reader;
		}

		public DataTable getTable()
		{
			return this.table;
		}

		public void setTable(DataTable table)
		{
			this.table = table;
		}
	}
}
