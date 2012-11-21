using System;
using System.Collection.Generic;
using System.Linq;
using System.Text;

namespace Database
{
	abstract class IDatabaseCreator
	{
		public abstract Database createDatabase(string path);
	}
}
