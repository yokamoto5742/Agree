using System.Drawing;

namespace AgentlabUtilityLibrary;

public class Barcode128
{
	public enum CODE
	{
		A = 1,
		B,
		C
	}

	// Code128 の各パターン。要素は Draw 内のペン番号(1〜4: 黒の太さ、5〜8: 白の太さ)。103〜105 は開始、106 は停止
	private static readonly int[][] barTable = new int[107][]
	{
		new int[6] { 2, 5, 2, 6, 2, 6 },
		new int[6] { 2, 6, 2, 5, 2, 6 },
		new int[6] { 2, 6, 2, 6, 2, 5 },
		new int[6] { 1, 6, 1, 6, 2, 7 },
		new int[6] { 1, 6, 1, 7, 2, 6 },
		new int[6] { 1, 7, 1, 6, 2, 6 },
		new int[6] { 1, 6, 2, 6, 1, 7 },
		new int[6] { 1, 6, 2, 7, 1, 6 },
		new int[6] { 1, 7, 2, 6, 1, 6 },
		new int[6] { 2, 6, 1, 6, 1, 7 },
		new int[6] { 2, 6, 1, 7, 1, 6 },
		new int[6] { 2, 7, 1, 6, 1, 6 },
		new int[6] { 1, 5, 2, 6, 3, 6 },
		new int[6] { 1, 6, 2, 5, 3, 6 },
		new int[6] { 1, 6, 2, 6, 3, 5 },
		new int[6] { 1, 5, 3, 6, 2, 6 },
		new int[6] { 1, 6, 3, 5, 2, 6 },
		new int[6] { 1, 6, 3, 6, 2, 5 },
		new int[6] { 2, 6, 3, 6, 1, 5 },
		new int[6] { 2, 6, 1, 5, 3, 6 },
		new int[6] { 2, 6, 1, 6, 3, 5 },
		new int[6] { 2, 5, 3, 6, 1, 6 },
		new int[6] { 2, 6, 3, 5, 1, 6 },
		new int[6] { 3, 5, 2, 5, 3, 5 },
		new int[6] { 3, 5, 1, 6, 2, 6 },
		new int[6] { 3, 6, 1, 5, 2, 6 },
		new int[6] { 3, 6, 1, 6, 2, 5 },
		new int[6] { 3, 5, 2, 6, 1, 6 },
		new int[6] { 3, 6, 2, 5, 1, 6 },
		new int[6] { 3, 6, 2, 6, 1, 5 },
		new int[6] { 2, 5, 2, 5, 2, 7 },
		new int[6] { 2, 5, 2, 7, 2, 5 },
		new int[6] { 2, 7, 2, 5, 2, 5 },
		new int[6] { 1, 5, 1, 7, 2, 7 },
		new int[6] { 1, 7, 1, 5, 2, 7 },
		new int[6] { 1, 7, 1, 7, 2, 5 },
		new int[6] { 1, 5, 2, 7, 1, 7 },
		new int[6] { 1, 7, 2, 5, 1, 7 },
		new int[6] { 1, 7, 2, 7, 1, 5 },
		new int[6] { 2, 5, 1, 7, 1, 7 },
		new int[6] { 2, 7, 1, 5, 1, 7 },
		new int[6] { 2, 7, 1, 7, 1, 5 },
		new int[6] { 1, 5, 2, 5, 3, 7 },
		new int[6] { 1, 5, 2, 7, 3, 5 },
		new int[6] { 1, 7, 2, 5, 3, 5 },
		new int[6] { 1, 5, 3, 5, 2, 7 },
		new int[6] { 1, 5, 3, 7, 2, 5 },
		new int[6] { 1, 7, 3, 5, 2, 5 },
		new int[6] { 3, 5, 3, 5, 2, 5 },
		new int[6] { 2, 5, 1, 7, 3, 5 },
		new int[6] { 2, 7, 1, 5, 3, 5 },
		new int[6] { 2, 5, 3, 5, 1, 7 },
		new int[6] { 2, 5, 3, 7, 1, 5 },
		new int[6] { 2, 5, 3, 5, 3, 5 },
		new int[6] { 3, 5, 1, 5, 2, 7 },
		new int[6] { 3, 5, 1, 7, 2, 5 },
		new int[6] { 3, 7, 1, 5, 2, 5 },
		new int[6] { 3, 5, 2, 5, 1, 7 },
		new int[6] { 3, 5, 2, 7, 1, 5 },
		new int[6] { 3, 7, 2, 5, 1, 5 },
		new int[6] { 3, 5, 4, 5, 1, 5 },
		new int[6] { 2, 6, 1, 8, 1, 5 },
		new int[6] { 4, 7, 1, 5, 1, 5 },
		new int[6] { 1, 5, 1, 6, 2, 8 },
		new int[6] { 1, 5, 1, 8, 2, 6 },
		new int[6] { 1, 6, 1, 5, 2, 8 },
		new int[6] { 1, 6, 1, 8, 2, 5 },
		new int[6] { 1, 8, 1, 5, 2, 6 },
		new int[6] { 1, 8, 1, 6, 2, 5 },
		new int[6] { 1, 5, 2, 6, 1, 8 },
		new int[6] { 1, 5, 2, 8, 1, 6 },
		new int[6] { 1, 6, 2, 5, 1, 8 },
		new int[6] { 1, 6, 2, 8, 1, 5 },
		new int[6] { 1, 8, 2, 5, 1, 6 },
		new int[6] { 1, 8, 2, 6, 1, 5 },
		new int[6] { 2, 8, 1, 6, 1, 5 },
		new int[6] { 2, 6, 1, 5, 1, 8 },
		new int[6] { 4, 5, 3, 5, 1, 5 },
		new int[6] { 2, 8, 1, 5, 1, 6 },
		new int[6] { 1, 7, 4, 5, 1, 5 },
		new int[6] { 1, 5, 1, 6, 4, 6 },
		new int[6] { 1, 6, 1, 5, 4, 6 },
		new int[6] { 1, 6, 1, 6, 4, 5 },
		new int[6] { 1, 5, 4, 6, 1, 6 },
		new int[6] { 1, 6, 4, 5, 1, 6 },
		new int[6] { 1, 6, 4, 6, 1, 5 },
		new int[6] { 4, 5, 1, 6, 1, 6 },
		new int[6] { 4, 6, 1, 5, 1, 6 },
		new int[6] { 4, 6, 1, 6, 1, 5 },
		new int[6] { 2, 5, 2, 5, 4, 5 },
		new int[6] { 2, 5, 4, 5, 2, 5 },
		new int[6] { 4, 5, 2, 5, 2, 5 },
		new int[6] { 1, 5, 1, 5, 4, 7 },
		new int[6] { 1, 5, 1, 7, 4, 5 },
		new int[6] { 1, 7, 1, 5, 4, 5 },
		new int[6] { 1, 5, 4, 5, 1, 7 },
		new int[6] { 1, 5, 4, 7, 1, 5 },
		new int[6] { 4, 5, 1, 5, 1, 7 },
		new int[6] { 4, 5, 1, 7, 1, 5 },
		new int[6] { 1, 5, 3, 5, 4, 5 },
		new int[6] { 1, 5, 4, 5, 3, 5 },
		new int[6] { 3, 5, 1, 5, 4, 5 },
		new int[6] { 4, 5, 1, 5, 3, 5 },
		new int[6] { 2, 5, 1, 8, 1, 6 },
		new int[6] { 2, 5, 1, 6, 1, 8 },
		new int[6] { 2, 5, 1, 6, 3, 6 },
		new int[7] { 2, 7, 3, 5, 1, 5, 2 }
	};

	public bool Draw(CODE code, string bar, Graphics g, float left, float top, float height, float line_width)
	{
		foreach (char c in bar)
		{
			if (c < '0' || c > '9')
			{
				return false;
			}
		}
		Pen[] pens = new Pen[9]
		{
			new Pen(Brushes.White, 0f),
			new Pen(Brushes.Black, line_width),
			new Pen(Brushes.Black, line_width * 2f),
			new Pen(Brushes.Black, line_width * 3f),
			new Pen(Brushes.Black, line_width * 4f),
			new Pen(Brushes.White, line_width),
			new Pen(Brushes.White, line_width * 2f),
			new Pen(Brushes.White, line_width * 3f),
			new Pen(Brushes.White, line_width * 4f)
		};
		try
		{
			float x = left;
			void DrawPattern(int index)
			{
				foreach (int penIndex in barTable[index])
				{
					Pen pen = pens[penIndex];
					g.DrawLine(pen, x + pen.Width / 2f, top, x + pen.Width / 2f, top + height);
					x += pen.Width;
				}
			}

			int checksum = 105;
			switch (code)
			{
			case CODE.A:
				checksum = 103;
				break;
			case CODE.B:
				checksum = 104;
				break;
			}
			DrawPattern(checksum);
			if (code == CODE.A || code == CODE.B)
			{
				for (int i = 0; i < bar.Length; i++)
				{
					int value = bar[i] - 32;
					DrawPattern(value);
					checksum += value * (i + 1);
				}
			}
			else if (code == CODE.C)
			{
				for (int i = 0; i < bar.Length / 2; i++)
				{
					int value = short.Parse(bar.Substring(i * 2, 2));
					DrawPattern(value);
					checksum += value * (i + 1);
				}
			}
			DrawPattern(checksum % 103);
			DrawPattern(106);
			return true;
		}
		finally
		{
			foreach (Pen pen in pens)
			{
				pen.Dispose();
			}
		}
	}
}
