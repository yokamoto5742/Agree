using System;
using System.Windows.Forms;

namespace AgentlabUtilityLibrary;

public class TimeTextBox : TextBox
{
	public TimeTextBox()
	{
		base.ImeMode = ImeMode.Disable;
		base.TextAlign = HorizontalAlignment.Center;
		base.KeyDown += TimeTextBox_KeyDown;
		base.Leave += TimeTextBox_Leave;
	}

	private void TimeTextBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return && !Check())
		{
			if (MessageBox.Show("時刻形式が正しくありません。HHmm 形式（例 8:15 または 08:15 または 815 または 0815）で入力してください。\r\n入力内容をクリアしますか？", "確認", MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				Clear();
			}
			Select();
		}
	}

	private void TimeTextBox_Leave(object sender, EventArgs e)
	{
		if (!Check())
		{
			if (MessageBox.Show("時刻形式が正しくありません。HHmm 形式（例 8:15 または 08:15 または 815 または 0815）で入力してください。\r\n入力内容をクリアしますか？", "確認", MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				Clear();
			}
			Select();
		}
	}

	private bool Check()
	{
		bool flag = true;
		string text = base.Text;
		if (text.Length > 0)
		{
			if (text.Contains(":"))
			{
				text = text.Replace(":", "");
			}
			text = text.PadLeft(4, '0').Insert(2, ":");
			DateTime result = default(DateTime);
			flag = DateTime.TryParse(text, out result);
			if (flag)
			{
				base.Text = result.ToString("HH:mm");
			}
		}
		return flag;
	}

	public int ToInt()
	{
		int result = 0;
		if (Check())
		{
			int.TryParse(base.Text.Replace(":", ""), out result);
		}
		return result;
	}

	public void FromInt(int time)
	{
		base.Text = "";
		if (time.ToString().Length <= 4)
		{
			DateTime result = default(DateTime);
			if (DateTime.TryParse(time.ToString().PadLeft(4, '0').Insert(2, ":"), out result))
			{
				base.Text = result.ToString("HH:mm");
			}
		}
	}
}
