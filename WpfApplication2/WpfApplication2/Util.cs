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
    }
}
