using System;
using UnityEngine;

namespace Windows
{
	// Token: 0x02000A19 RID: 2585
	public class ConsoleInput
	{
		// Token: 0x0400375A RID: 14170
		public string inputString = "";

		// Token: 0x0400375B RID: 14171
		public string[] statusText = new string[]
		{
			"",
			"",
			""
		};

		// Token: 0x0400375C RID: 14172
		internal float nextUpdate;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06003D6C RID: 15724 RVA: 0x00169284 File Offset: 0x00167484
		// (remove) Token: 0x06003D6D RID: 15725 RVA: 0x001692BC File Offset: 0x001674BC
		public event Action<string> OnInputText;

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06003D6E RID: 15726 RVA: 0x001692F1 File Offset: 0x001674F1
		public bool valid
		{
			get
			{
				return Console.BufferWidth > 0;
			}
		}

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x06003D6F RID: 15727 RVA: 0x001692FB File Offset: 0x001674FB
		public int lineWidth
		{
			get
			{
				return Console.BufferWidth;
			}
		}

		// Token: 0x06003D70 RID: 15728 RVA: 0x00169302 File Offset: 0x00167502
		public void ClearLine(int numLines)
		{
			Console.CursorLeft = 0;
			Console.Write(new string(' ', this.lineWidth * numLines));
			Console.CursorTop -= numLines;
			Console.CursorLeft = 0;
		}

		// Token: 0x06003D71 RID: 15729 RVA: 0x00169330 File Offset: 0x00167530
		public void RedrawInputLine()
		{
			ConsoleColor backgroundColor = Console.BackgroundColor;
			ConsoleColor foregroundColor = Console.ForegroundColor;
			try
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.CursorTop++;
				for (int i = 0; i < this.statusText.Length; i++)
				{
					Console.CursorLeft = 0;
					Console.Write(this.statusText[i].PadRight(this.lineWidth));
				}
				Console.CursorTop -= this.statusText.Length + 1;
				Console.CursorLeft = 0;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Green;
				this.ClearLine(1);
				if (this.inputString.Length == 0)
				{
					Console.BackgroundColor = backgroundColor;
					Console.ForegroundColor = foregroundColor;
					return;
				}
				if (this.inputString.Length < this.lineWidth - 2)
				{
					Console.Write(this.inputString);
				}
				else
				{
					Console.Write(this.inputString.Substring(this.inputString.Length - (this.lineWidth - 2)));
				}
			}
			catch (Exception)
			{
			}
			Console.BackgroundColor = backgroundColor;
			Console.ForegroundColor = foregroundColor;
		}

		// Token: 0x06003D72 RID: 15730 RVA: 0x00169440 File Offset: 0x00167640
		internal void OnBackspace()
		{
			if (this.inputString.Length < 1)
			{
				return;
			}
			this.inputString = this.inputString.Substring(0, this.inputString.Length - 1);
			this.RedrawInputLine();
		}

		// Token: 0x06003D73 RID: 15731 RVA: 0x00169476 File Offset: 0x00167676
		internal void OnEscape()
		{
			this.inputString = "";
			this.RedrawInputLine();
		}

		// Token: 0x06003D74 RID: 15732 RVA: 0x0016948C File Offset: 0x0016768C
		internal void OnEnter()
		{
			this.ClearLine(this.statusText.Length);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("> " + this.inputString);
			string obj = this.inputString;
			this.inputString = "";
			if (this.OnInputText != null)
			{
				this.OnInputText(obj);
			}
			this.RedrawInputLine();
		}

		// Token: 0x06003D75 RID: 15733 RVA: 0x001694F0 File Offset: 0x001676F0
		public void Update()
		{
			if (!this.valid)
			{
				return;
			}
			if (this.nextUpdate < Time.realtimeSinceStartup)
			{
				this.RedrawInputLine();
				this.nextUpdate = Time.realtimeSinceStartup + 0.5f;
			}
			try
			{
				if (!Console.KeyAvailable)
				{
					return;
				}
			}
			catch (Exception)
			{
				return;
			}
			ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
			if (consoleKeyInfo.Key == ConsoleKey.Enter)
			{
				this.OnEnter();
				return;
			}
			if (consoleKeyInfo.Key == ConsoleKey.Backspace)
			{
				this.OnBackspace();
				return;
			}
			if (consoleKeyInfo.Key == ConsoleKey.Escape)
			{
				this.OnEscape();
				return;
			}
			if (consoleKeyInfo.KeyChar != '\0')
			{
				this.inputString += consoleKeyInfo.KeyChar.ToString();
				this.RedrawInputLine();
				return;
			}
		}
	}
}
