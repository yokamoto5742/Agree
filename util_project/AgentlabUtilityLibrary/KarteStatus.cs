using System;
using System.Data.OleDb;

namespace AgentlabUtilityLibrary;

public class KarteStatus
{
	public static void Start(string date, string uke_no, string ren_no, string dept, string doctor)
	{
		if (ParamCheck(date, uke_no, ren_no, dept, doctor))
		{
			OleDbConnection dBConn = DBConn.GetDBConn();
			dBConn.Open();
			OleDbCommand oleDbCommand = new OleDbCommand();
			oleDbCommand.Connection = dBConn;
			oleDbCommand.CommandText = "update ADT_診察状況データ set 診察開始時間 = " + DateTime.Now.ToString("HHmm") + ", 診察中断時間 = 0, 変更科コード = " + dept + ", 変更ＤＲコード = " + doctor + " where 受付日 = " + date + " and 受付ＮＯ = " + uke_no + " and 連番 = " + ren_no;
			oleDbCommand.ExecuteNonQuery();
			dBConn.Close();
		}
	}

	public static void Interrupt(string date, string uke_no, string ren_no, string dept, string doctor)
	{
		if (ParamCheck(date, uke_no, ren_no, dept, doctor))
		{
			OleDbConnection dBConn = DBConn.GetDBConn();
			dBConn.Open();
			OleDbCommand oleDbCommand = new OleDbCommand();
			oleDbCommand.Connection = dBConn;
			oleDbCommand.CommandText = "update ADT_診察状況データ set 診察中断時間 = " + DateTime.Now.ToString("HHmm") + ", 診察終了時間 = 0, 変更科コード = " + dept + ", 変更ＤＲコード = " + doctor + " where 受付日 = " + date + " and 受付ＮＯ = " + uke_no + " and 連番 = " + ren_no;
			oleDbCommand.ExecuteNonQuery();
			dBConn.Close();
		}
	}

	public static void End(string date, string uke_no, string ren_no, string dept, string doctor)
	{
		if (ParamCheck(date, uke_no, ren_no, dept, doctor))
		{
			OleDbConnection dBConn = DBConn.GetDBConn();
			dBConn.Open();
			OleDbCommand oleDbCommand = new OleDbCommand();
			oleDbCommand.Connection = dBConn;
			oleDbCommand.CommandText = "update ADT_診察状況データ set 診察中断時間 = 0, 診察終了時間 = " + DateTime.Now.ToString("HHmm") + ", 変更科コード = " + dept + ", 変更ＤＲコード = " + doctor + " where 受付日 = " + date + " and 受付ＮＯ = " + uke_no + " and 連番 = " + ren_no;
			oleDbCommand.ExecuteNonQuery();
			dBConn.Close();
		}
	}

	private static bool ParamCheck(string date, string uke_no, string ren_no, string dept, string doctor)
	{
		if (date.Length != 8)
		{
			return false;
		}
		if (!Dict.DeptDict.ContainsKey(dept))
		{
			return false;
		}
		if (!Dict.DoctorDict.ContainsKey(doctor))
		{
			return false;
		}
		return true;
	}
}
