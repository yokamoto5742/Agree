using System;
using System.Data;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class DB
{
	public enum ParamType
	{
		Char = 1,
		String = 2,
		Int = 11,
		Float = 12,
		DateTime = 21
	}

	private static DB db_macs;

	private static DB db_open;

	private string ConnectionString = "";

	private OleDbConnection Con;

	private OleDbCommand Cmd;

	private bool ToClose;

	public static DB MACS
	{
		get
		{
			if (db_macs == null)
			{
				db_macs = new DB("Provider=" + Env.PROVIDER + ";Data Source=" + Env.MAIN_DB + ";User ID=" + Env.MAIN_USER + ";Password=" + Env.MAIN_PWD);
			}
			return db_macs;
		}
	}

	public static DB OPEN
	{
		get
		{
			if (db_open == null)
			{
				db_open = new DB("Provider=" + Env.PROVIDER + ";Data Source=" + Env.OPEN_DB + ";User ID=" + Env.OPEN_USER + ";Password=" + Env.OPEN_PWD);
			}
			return db_open;
		}
	}

	public string CommandText
	{
		get
		{
			return Cmd.CommandText;
		}
		set
		{
			Cmd.CommandText = value;
		}
	}

	public OleDbParameterCollection Parameters => Cmd.Parameters;

	public DB(string con_str)
	{
		ConnectionString = con_str;
		Con = new OleDbConnection(ConnectionString);
		Cmd = new OleDbCommand();
		Cmd.Connection = Con;
	}

	public void ParamClear()
	{
		Cmd.Parameters.Clear();
	}

	public void ParamAdd(string param, ParamType type, string value)
	{
		switch (type)
		{
		case ParamType.Char:
			if (value.Length > 0)
			{
				Cmd.Parameters.Add(param, OleDbType.Char).Value = value[0];
			}
			else
			{
				Cmd.Parameters.Add(param, OleDbType.Char).Value = "";
			}
			break;
		case ParamType.String:
			Cmd.Parameters.Add(param, OleDbType.VarWChar).Value = value;
			break;
		case ParamType.Int:
			if (value.Length > 0)
			{
				Cmd.Parameters.Add(param, OleDbType.Numeric).Value = value;
			}
			else
			{
				Cmd.Parameters.Add(param, OleDbType.Numeric).Value = 0;
			}
			break;
		case ParamType.Float:
			if (value.Length > 0)
			{
				Cmd.Parameters.Add(param, OleDbType.Double).Value = value;
			}
			else
			{
				Cmd.Parameters.Add(param, OleDbType.Double).Value = 0f;
			}
			break;
		case ParamType.DateTime:
		{
			DateTime result = DateTime.Now;
			if (DateTime.TryParse(value, out result))
			{
				Cmd.Parameters.Add(param, OleDbType.DBTimeStamp).Value = result;
			}
			else
			{
				Cmd.Parameters.Add(param, OleDbType.DBTimeStamp).Value = DateTime.Now;
			}
			break;
		}
		}
	}

	public void ParamAdd(string param, ParamType type, int value)
	{
		switch (type)
		{
		case ParamType.Char:
			if (value.ToString().Length > 0)
			{
				Cmd.Parameters.Add(param, OleDbType.Char).Value = value.ToString()[0];
			}
			else
			{
				Cmd.Parameters.Add(param, OleDbType.Char).Value = "";
			}
			break;
		case ParamType.String:
			Cmd.Parameters.Add(param, OleDbType.VarWChar).Value = value.ToString();
			break;
		case ParamType.Int:
			Cmd.Parameters.Add(param, OleDbType.Numeric).Value = value;
			break;
		case ParamType.Float:
			Cmd.Parameters.Add(param, OleDbType.Double).Value = (float)value;
			break;
		case ParamType.DateTime:
			Cmd.Parameters.Add(param, OleDbType.DBTimeStamp).Value = DateTimeAgent.DateTimeFromInt(value);
			break;
		}
	}

	public void ParamAdd(string param, ParamType type, float value)
	{
		switch (type)
		{
		case ParamType.Char:
			if (value.ToString().Length > 0)
			{
				Cmd.Parameters.Add(param, OleDbType.Char).Value = value.ToString()[0];
			}
			else
			{
				Cmd.Parameters.Add(param, OleDbType.Char).Value = "";
			}
			break;
		case ParamType.String:
			Cmd.Parameters.Add(param, OleDbType.VarWChar).Value = value.ToString();
			break;
		case ParamType.Int:
			Cmd.Parameters.Add(param, OleDbType.Numeric).Value = (int)value;
			break;
		case ParamType.Float:
			Cmd.Parameters.Add(param, OleDbType.Double).Value = value;
			break;
		case ParamType.DateTime:
			Cmd.Parameters.Add(param, OleDbType.DBTimeStamp).Value = DateTimeAgent.DateTimeFromInt((int)value);
			break;
		}
	}

	public OleDbDataReader ExecuteReader()
	{
		return Cmd.ExecuteReader();
	}

	public int ExecuteNonQuery()
	{
		return Cmd.ExecuteNonQuery();
	}

	public void Open()
	{
		if (Con.State != ConnectionState.Open)
		{
			Con.Open();
			ToClose = true;
		}
		else
		{
			ToClose = false;
		}
		Cmd.Parameters.Clear();
	}

	public void Close()
	{
		if (ToClose)
		{
			Con.Close();
			ToClose = false;
		}
		Cmd.Parameters.Clear();
	}
}
