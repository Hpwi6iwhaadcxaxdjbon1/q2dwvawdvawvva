using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AC3 RID: 2755
	[ConsoleSystem.Factory("hierarchy")]
	public class Hierarchy : ConsoleSystem
	{
		// Token: 0x04003B7D RID: 15229
		private static GameObject currentDir;

		// Token: 0x06004248 RID: 16968 RVA: 0x001885E8 File Offset: 0x001867E8
		private static Transform[] GetCurrent()
		{
			if (Hierarchy.currentDir == null)
			{
				return TransformUtil.GetRootObjects().ToArray<Transform>();
			}
			List<Transform> list = new List<Transform>();
			for (int i = 0; i < Hierarchy.currentDir.transform.childCount; i++)
			{
				list.Add(Hierarchy.currentDir.transform.GetChild(i));
			}
			return list.ToArray();
		}

		// Token: 0x06004249 RID: 16969 RVA: 0x0018864C File Offset: 0x0018684C
		[ServerVar]
		public static void ls(ConsoleSystem.Arg args)
		{
			string text = "";
			string filter = args.GetString(0, "");
			if (Hierarchy.currentDir)
			{
				text = text + "Listing " + Hierarchy.currentDir.transform.GetRecursiveName("") + "\n\n";
			}
			else
			{
				text += "Listing .\n\n";
			}
			IEnumerable<Transform> current = Hierarchy.GetCurrent();
			Func<Transform, bool> <>9__0;
			Func<Transform, bool> predicate;
			if ((predicate = <>9__0) == null)
			{
				predicate = (<>9__0 = ((Transform x) => string.IsNullOrEmpty(filter) || x.name.Contains(filter)));
			}
			foreach (Transform transform in current.Where(predicate).Take(40))
			{
				text += string.Format("   {0} [{1}]\n", transform.name, transform.childCount);
			}
			text += "\n";
			args.ReplyWith(text);
		}

		// Token: 0x0600424A RID: 16970 RVA: 0x00188754 File Offset: 0x00186954
		[ServerVar]
		public static void cd(ConsoleSystem.Arg args)
		{
			if (args.FullString == ".")
			{
				Hierarchy.currentDir = null;
				args.ReplyWith("Changed to .");
				return;
			}
			if (args.FullString == "..")
			{
				if (Hierarchy.currentDir)
				{
					Hierarchy.currentDir = (Hierarchy.currentDir.transform.parent ? Hierarchy.currentDir.transform.parent.gameObject : null);
				}
				Hierarchy.currentDir = null;
				if (Hierarchy.currentDir)
				{
					args.ReplyWith("Changed to " + Hierarchy.currentDir.transform.GetRecursiveName(""));
					return;
				}
				args.ReplyWith("Changed to .");
				return;
			}
			else
			{
				Transform transform = Hierarchy.GetCurrent().FirstOrDefault((Transform x) => x.name.ToLower() == args.FullString.ToLower());
				if (transform == null)
				{
					transform = Hierarchy.GetCurrent().FirstOrDefault((Transform x) => x.name.StartsWith(args.FullString, StringComparison.CurrentCultureIgnoreCase));
				}
				if (transform)
				{
					Hierarchy.currentDir = transform.gameObject;
					args.ReplyWith("Changed to " + Hierarchy.currentDir.transform.GetRecursiveName(""));
					return;
				}
				args.ReplyWith("Couldn't find \"" + args.FullString + "\"");
				return;
			}
		}

		// Token: 0x0600424B RID: 16971 RVA: 0x001888DC File Offset: 0x00186ADC
		[ServerVar]
		public static void del(ConsoleSystem.Arg args)
		{
			if (!args.HasArgs(1))
			{
				return;
			}
			IEnumerable<Transform> enumerable = from x in Hierarchy.GetCurrent()
			where x.name.ToLower() == args.FullString.ToLower()
			select x;
			if (enumerable.Count<Transform>() == 0)
			{
				enumerable = from x in Hierarchy.GetCurrent()
				where x.name.StartsWith(args.FullString, StringComparison.CurrentCultureIgnoreCase)
				select x;
			}
			if (enumerable.Count<Transform>() == 0)
			{
				args.ReplyWith("Couldn't find  " + args.FullString);
				return;
			}
			foreach (Transform transform in enumerable)
			{
				BaseEntity baseEntity = transform.gameObject.ToBaseEntity();
				if (baseEntity.IsValid())
				{
					if (baseEntity.isServer)
					{
						baseEntity.Kill(BaseNetworkable.DestroyMode.None);
					}
				}
				else
				{
					GameManager.Destroy(transform.gameObject, 0f);
				}
			}
			args.ReplyWith("Deleted " + enumerable.Count<Transform>() + " objects");
		}
	}
}
