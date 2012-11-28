namespace WpfApplication2
{
    class Util
    {
        public static System.Collections.Generic.IEnumerable<string> SplitTags(System.Collections.Generic.IEnumerable<char> source)
        {
            const char openingBracket = '[';
            const char closingBracket = ']';

            bool isInTag = false;
            var builder = new System.Text.StringBuilder();

            foreach (char character in source)
            {
                if (isInTag)
                {
                    if (character == closingBracket)
                    {
                        if (builder.Length == 0)
                            throw new System.FormatException("Empty tags are not allowed.");

                        yield return builder.ToString();
                        isInTag = false;
                        builder.Clear();
                    }
                    else if (character == openingBracket)
                    {
                        throw new System.FormatException("Missing closing bracket.");
                    }
                    else
                    {
                        builder.Append(character);
                    }
                }
                else
                {
                    if (character != openingBracket)
                        throw new System.FormatException("Missing opening bracket.");
                    else
                        isInTag = true;
                }
            }

            if (builder.Length != 0)
                throw new System.FormatException("Missing closing bracket.");
        }

        public static System.Tuple<string, System.Collections.Generic.IEnumerable<System.Data.SQLite.SQLiteParameter>> SqlParametersList<T>(System.Collections.Generic.IEnumerable<T> values)
        {
            var builder = new System.Text.StringBuilder();
            var enumerator = values.GetEnumerator();
            var list = new System.Collections.Generic.List<System.Data.SQLite.SQLiteParameter>();

            if (enumerator.MoveNext())
            {
                builder.Append("@__val0");
                list.Add(new System.Data.SQLite.SQLiteParameter("@__val0", enumerator.Current));

                int i = 1;
                while (enumerator.MoveNext())
                {
                    var name = "@__val" + i++;
                    builder.Append(", " + name);
                    list.Add(new System.Data.SQLite.SQLiteParameter(name, enumerator.Current));
                }
            }

            return new System.Tuple<string, System.Collections.Generic.IEnumerable<System.Data.SQLite.SQLiteParameter>>(builder.ToString(), list);
        }

        public static long InsertInto<T>(System.Data.SQLite.SQLiteConnection connection, string tableName, params System.Tuple<string, T>[] columnsAndValues)
        {
            if (columnsAndValues.Length != 0)
            {
                var columnsNames = new System.Text.StringBuilder();
                var columnsValues = new System.Text.StringBuilder();
                var parameters = new System.Collections.Generic.List<System.Data.SQLite.SQLiteParameter>();
                var enumerator = columnsAndValues.GetEnumerator();

                if (enumerator.MoveNext())
                {
                    columnsNames.Append(((System.Tuple<string, T>)enumerator.Current).Item1);
                    columnsValues.Append("@__val0");
                    parameters.Add(new System.Data.SQLite.SQLiteParameter("@__val0", ((System.Tuple<string, T>)enumerator.Current).Item2));

                    int i = 1;
                    while (enumerator.MoveNext())
                    {
                        var tuple = (System.Tuple<string, T>)enumerator.Current;
                        var name = "@__val" + i++;
                        columnsNames.Append(", " + tuple.Item1);
                        columnsValues.Append(", " + name);
                        parameters.Add(new System.Data.SQLite.SQLiteParameter(name, tuple.Item2));
                    }
                }

                var command = new System.Data.SQLite.SQLiteCommand("INSERT INTO " + tableName + "(" + columnsNames.ToString() + ") VALUES(" + columnsValues.ToString() + "); SELECT last_insert_rowid()", connection);
                foreach (var param in parameters)
                    command.Parameters.Add(param);

                var reader = command.ExecuteReader();
                if (reader.Read())
                    return reader.GetInt64(0);
            }

            return 0;
        }
    }
}
