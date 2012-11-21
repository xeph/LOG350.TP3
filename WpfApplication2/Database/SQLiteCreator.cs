using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Database
{
	class SQLiteCreator:IDatabaseCreator
	{
		public override SQLite createDatabase(string path)
		{
			return new SQLite(path);
		}
	}
}
