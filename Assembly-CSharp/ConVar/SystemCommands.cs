using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000ADF RID: 2783
	[ConsoleSystem.Factory("system")]
	public static class SystemCommands
	{
		// Token: 0x06004312 RID: 17170 RVA: 0x0018CE58 File Offset: 0x0018B058
		[ServerVar]
		[ClientVar]
		public static void cpu_affinity(ConsoleSystem.Arg arg)
		{
			long num = 0L;
			if (!arg.HasArgs(1))
			{
				arg.ReplyWith("Format is 'cpu_affinity {core,core1-core2,etc}'");
				return;
			}
			string[] array = arg.GetString(0, "").Split(new char[]
			{
				','
			});
			HashSet<int> hashSet = new HashSet<int>();
			foreach (string text in array)
			{
				int item;
				if (int.TryParse(text, out item))
				{
					hashSet.Add(item);
				}
				else if (text.Contains('-'))
				{
					string[] array3 = text.Split(new char[]
					{
						'-'
					});
					int num2;
					int num3;
					if (array3.Length != 2)
					{
						arg.ReplyWith("Failed to parse section " + text + ", format should be '0-15'");
					}
					else if (!int.TryParse(array3[0], out num2) || !int.TryParse(array3[1], out num3))
					{
						arg.ReplyWith("Core range in section " + text + " are not valid numbers, format should be '0-15'");
					}
					else if (num2 > num3)
					{
						arg.ReplyWith("Core range in section " + text + " are not ordered from least to greatest, format should be '0-15'");
					}
					else
					{
						if (num3 - num2 > 64)
						{
							arg.ReplyWith("Core range in section " + text + " are too big of a range, must be <64");
							return;
						}
						for (int j = num2; j <= num3; j++)
						{
							hashSet.Add(j);
						}
					}
				}
			}
			if (hashSet.Any((int x) => x < 0 || x > 63))
			{
				arg.ReplyWith("Cores provided out of range! Must be in between 0 and 63");
				return;
			}
			for (int k = 0; k < 64; k++)
			{
				if (hashSet.Contains(k))
				{
					num |= 1L << k;
				}
			}
			if (num == 0L)
			{
				arg.ReplyWith("No cores provided (bitmask empty)! Format is 'cpu_affinity {core,core1-core2,etc}'");
				return;
			}
			try
			{
				WindowsAffinityShim.SetProcessAffinityMask(Process.GetCurrentProcess().Handle, new IntPtr(num));
			}
			catch (Exception arg2)
			{
				UnityEngine.Debug.LogWarning(string.Format("Unable to set cpu affinity: {0}", arg2));
				return;
			}
			arg.ReplyWith("Successfully changed cpu affinity");
		}

		// Token: 0x06004313 RID: 17171 RVA: 0x0018D058 File Offset: 0x0018B258
		[ServerVar]
		[ClientVar]
		public static void cpu_priority(ConsoleSystem.Arg arg)
		{
			if (Application.platform == RuntimePlatform.OSXPlayer)
			{
				arg.ReplyWith("OSX is not a supported platform");
				return;
			}
			string @string = arg.GetString(0, "");
			string a = @string.Replace("-", "").Replace("_", "");
			ProcessPriorityClass mask;
			if (!(a == "belownormal"))
			{
				if (!(a == "normal"))
				{
					if (!(a == "abovenormal"))
					{
						if (!(a == "high"))
						{
							arg.ReplyWith("Unknown priority '" + @string + "', possible values: below_normal, normal, above_normal, high");
							return;
						}
						mask = ProcessPriorityClass.High;
					}
					else
					{
						mask = ProcessPriorityClass.AboveNormal;
					}
				}
				else
				{
					mask = ProcessPriorityClass.Normal;
				}
			}
			else
			{
				mask = ProcessPriorityClass.BelowNormal;
			}
			try
			{
				WindowsAffinityShim.SetPriorityClass(Process.GetCurrentProcess().Handle, (uint)mask);
			}
			catch (Exception arg2)
			{
				UnityEngine.Debug.LogWarning(string.Format("Unable to set cpu priority: {0}", arg2));
				return;
			}
			arg.ReplyWith("Successfully changed cpu priority to " + mask.ToString());
		}
	}
}
