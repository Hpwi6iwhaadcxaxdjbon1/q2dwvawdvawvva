using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

namespace Windows
{
	// Token: 0x02000A1A RID: 2586
	[SuppressUnmanagedCodeSecurity]
	public class ConsoleWindow
	{
		// Token: 0x0400375D RID: 14173
		private TextWriter oldOutput;

		// Token: 0x0400375E RID: 14174
		private const int STD_INPUT_HANDLE = -10;

		// Token: 0x0400375F RID: 14175
		private const int STD_OUTPUT_HANDLE = -11;

		// Token: 0x06003D77 RID: 15735 RVA: 0x001695EC File Offset: 0x001677EC
		public void Initialize()
		{
			ConsoleWindow.FreeConsole();
			if (!ConsoleWindow.AttachConsole(4294967295U))
			{
				ConsoleWindow.AllocConsole();
			}
			this.oldOutput = Console.Out;
			try
			{
				Console.OutputEncoding = Encoding.UTF8;
				Console.SetOut(new StreamWriter(new FileStream(new SafeFileHandle(ConsoleWindow.GetStdHandle(-11), true), FileAccess.Write), Encoding.UTF8)
				{
					AutoFlush = true
				});
			}
			catch (Exception ex)
			{
				Debug.Log("Couldn't redirect output: " + ex.Message);
			}
		}

		// Token: 0x06003D78 RID: 15736 RVA: 0x00169678 File Offset: 0x00167878
		public void Shutdown()
		{
			Console.SetOut(this.oldOutput);
			ConsoleWindow.FreeConsole();
		}

		// Token: 0x06003D79 RID: 15737 RVA: 0x0016968B File Offset: 0x0016788B
		public void SetTitle(string strName)
		{
			ConsoleWindow.SetConsoleTitleA(strName);
		}

		// Token: 0x06003D7A RID: 15738
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AttachConsole(uint dwProcessId);

		// Token: 0x06003D7B RID: 15739
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AllocConsole();

		// Token: 0x06003D7C RID: 15740
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool FreeConsole();

		// Token: 0x06003D7D RID: 15741
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		// Token: 0x06003D7E RID: 15742
		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleTitleA(string lpConsoleTitle);
	}
}
