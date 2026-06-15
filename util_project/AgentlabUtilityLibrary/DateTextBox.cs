using System;
using System.Windows.Forms;

namespace AgentlabUtilityLibrary;

public class DateTextBox : TextBox
{
	public enum Locale
	{
		English = 1,
		Japanese
	}

	private Locale Locale1 = Locale.English;

	public DateTextBox()
	{
		base.ImeMode = ImeMode.Disable;
		base.TextAlign = HorizontalAlignment.Center;
		base.KeyDown += DateTextBox_KeyDown;
		base.Leave += DateTextBox_Leave;
	}

	public void Init(Locale locale)
	{
		Locale1 = locale;
	}

	private void DateTextBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			if (base.Text.Length == 0)
			{
				if (Locale1 == Locale.English)
				{
					base.Text = DateTime.Now.ToString("yyyy/MM/dd");
				}
				else if (Locale1 == Locale.Japanese)
				{
					base.Text = DateTime.Now.ToString("gyy/MM/dd", DateTimeAgent.DefaultCulture);
				}
			}
			else if (!Check())
			{
				if (MessageBox.Show("日付形式が正しくありません。yyyy/MM/dd 形式（例 2009/08/15）で入力してください。\r\n入力内容をクリアしますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					Clear();
				}
				Select();
			}
		}
		else if (e.KeyCode == Keys.F3)
		{
			FormDate formDate = new FormDate(this);
			formDate.ShowDialog();
		}
	}

	private void DateTextBox_Leave(object sender, EventArgs e)
	{
		if (!Check())
		{
			if (MessageBox.Show("日付形式が正しくありません。yyyy/MM/dd 形式（例 2009/08/15）で入力してください。\r\n入力内容をクリアしますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				Clear();
			}
			Select();
		}
	}

	private bool Check()
	{
		bool flag = false;
		if (base.Text.Length > 0)
		{
			DateTime result = default(DateTime);
			flag = DateTime.TryParse(base.Text, out result);
			if (flag)
			{
				if (Locale1 == Locale.English)
				{
					base.Text = result.ToString("yyyy/MM/dd");
				}
				else if (Locale1 == Locale.Japanese)
				{
					base.Text = result.ToString("gyy/MM/dd", DateTimeAgent.DefaultCulture);
				}
			}
		}
		else
		{
			flag = true;
		}
		return flag;
	}

	public int ToInt()
	{
		int result = 0;
		if (base.Text.Length > 0 && Check())
		{
			int.TryParse(DateTime.Parse(base.Text).ToString("yyyyMMdd"), out result);
		}
		return result;
	}

	public void FromInt(int date)
	{
		base.Text = "";
		if (date.ToString().Length != 8)
		{
			return;
		}
		DateTime result = default(DateTime);
		if (DateTime.TryParse(date.ToString().Insert(4, "/").Insert(7, "/"), out result))
		{
			if (Locale1 == Locale.English)
			{
				base.Text = result.ToString("yyyy/MM/dd");
			}
			else if (Locale1 == Locale.Japanese)
			{
				base.Text = result.ToString("gyy/MM/dd", DateTimeAgent.DefaultCulture);
			}
		}
	}
}
