namespace AgentlabUtilityLibrary;

public class StringAgent
{
	public static string Wrap(string str, int line_length, int indent_length, char indent_char, bool indent_first)
	{
		string text = "";
		if (line_length < 1)
		{
			line_length = 1;
		}
		if (indent_length < 0)
		{
			indent_length = 0;
		}
		int num = 0;
		for (int i = 0; i < indent_length; i++)
		{
			if (indent_first)
			{
				text += indent_char;
			}
			num++;
		}
		for (int j = 0; j < str.Length; j++)
		{
			if (num >= line_length || str[j] == '\r')
			{
				text += "\r\n";
				num = 0;
				for (int k = 0; k < indent_length; k++)
				{
					text += indent_char;
					num++;
				}
			}
			if (str[j] != '\r' && str[j] != '\n')
			{
				text += str[j];
				num++;
			}
		}
		return text;
	}
}
