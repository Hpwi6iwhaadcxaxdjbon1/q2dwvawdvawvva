using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Facepunch;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AAB RID: 2731
	[ConsoleSystem.Factory("console")]
	public class Console : ConsoleSystem
	{
		// Token: 0x0600417A RID: 16762 RVA: 0x00184988 File Offset: 0x00182B88
		[ServerVar]
		[Help("Return the last x lines of the console. Default is 200")]
		public static IEnumerable<Output.Entry> tail(ConsoleSystem.Arg arg)
		{
			int @int = arg.GetInt(0, 200);
			int num = Output.HistoryOutput.Count - @int;
			if (num < 0)
			{
				num = 0;
			}
			return Output.HistoryOutput.Skip(num);
		}

		// Token: 0x0600417B RID: 16763 RVA: 0x001849C0 File Offset: 0x00182BC0
		[ServerVar]
		[Help("Search the console for a particular string")]
		public static IEnumerable<Output.Entry> search(ConsoleSystem.Arg arg)
		{
			string search = arg.GetString(0, null);
			if (search == null)
			{
				return Enumerable.Empty<Output.Entry>();
			}
			return from x in Output.HistoryOutput
			where x.Message.Length < 4096 && x.Message.Contains(search, CompareOptions.IgnoreCase)
			select x;
		}
	}
}
