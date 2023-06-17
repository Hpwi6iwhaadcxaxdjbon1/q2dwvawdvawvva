using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020002FD RID: 765
[ConsoleSystem.Factory("meta")]
public class Meta : ConsoleSystem
{
	// Token: 0x06001E53 RID: 7763 RVA: 0x000CE448 File Offset: 0x000CC648
	[ServerVar(Clientside = true, Help = "add <convar> <amount> - adds amount to convar")]
	public static void add(ConsoleSystem.Arg args)
	{
		string @string = args.GetString(0, "");
		float @float = args.GetFloat(1, 0.1f);
		ConsoleSystem.Command command = Meta.Find(@string);
		if (command == null)
		{
			args.ReplyWith("Convar not found: " + (@string ?? "<null>"));
			return;
		}
		if (args.IsClientside && command.Replicated)
		{
			args.ReplyWith("Cannot set replicated convars from the client (use sv to do this)");
			return;
		}
		if (args.IsServerside && command.ServerAdmin && !args.IsAdmin)
		{
			args.ReplyWith("Permission denied");
			return;
		}
		float num;
		if (!float.TryParse(command.String, out num))
		{
			args.ReplyWith("Convar value cannot be parsed as a number");
			return;
		}
		command.Set(num + @float);
	}

	// Token: 0x06001E54 RID: 7764 RVA: 0x000CE4F8 File Offset: 0x000CC6F8
	[ClientVar(Help = "if_true <command> <condition> - runs a command if the condition is true")]
	public static void if_true(ConsoleSystem.Arg args)
	{
		string @string = args.GetString(0, "");
		bool @bool = args.GetBool(1, false);
		if (@bool)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Client, @string, new object[]
			{
				@bool
			});
		}
	}

	// Token: 0x06001E55 RID: 7765 RVA: 0x000CE53C File Offset: 0x000CC73C
	[ClientVar(Help = "if_false <command> <condition> - runs a command if the condition is false")]
	public static void if_false(ConsoleSystem.Arg args)
	{
		string @string = args.GetString(0, "");
		bool @bool = args.GetBool(1, true);
		if (!@bool)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Client, @string, new object[]
			{
				@bool
			});
		}
	}

	// Token: 0x06001E56 RID: 7766 RVA: 0x000CE580 File Offset: 0x000CC780
	[ClientVar(Help = "reset_cycle <key> - resets a cycled bind to the beginning")]
	public static void reset_cycle(ConsoleSystem.Arg args)
	{
		string @string = args.GetString(0, "");
		List<KeyCode> list;
		KeyCombos.TryParse(ref @string, out list);
		Facepunch.Input.Button button = Facepunch.Input.GetButton(@string);
		if (button == null)
		{
			args.ReplyWith("Button not found");
			return;
		}
		if (!button.Cycle)
		{
			args.ReplyWith("Button does not have a cycled bind");
			return;
		}
		button.CycleIndex = 0;
	}

	// Token: 0x06001E57 RID: 7767 RVA: 0x000CE5D8 File Offset: 0x000CC7D8
	[ClientVar(Help = "exec [command_1] ... - runs all of the commands passed as arguments (also, if the last argument is true/false then that will flow into each command's arguments)")]
	public static void exec(ConsoleSystem.Arg args)
	{
		List<string> list = Pool.GetList<string>();
		for (int i = 0; i < 32; i++)
		{
			string @string = args.GetString(i, "");
			if (string.IsNullOrWhiteSpace(@string))
			{
				break;
			}
			list.Add(@string);
		}
		if (list.Count > 0)
		{
			string text = null;
			string text2 = list[list.Count - 1];
			bool flag;
			if (bool.TryParse(text2, out flag))
			{
				text = text2;
				list.RemoveAt(list.Count - 1);
			}
			foreach (string strCommand in list)
			{
				if (text != null)
				{
					ConsoleSystem.Run(ConsoleSystem.Option.Client, strCommand, new object[]
					{
						text
					});
				}
				else
				{
					ConsoleSystem.Run(ConsoleSystem.Option.Client, strCommand, Array.Empty<object>());
				}
			}
		}
		Pool.FreeList<string>(ref list);
	}

	// Token: 0x06001E58 RID: 7768 RVA: 0x000CE6C0 File Offset: 0x000CC8C0
	private static ConsoleSystem.Command Find(string name)
	{
		ConsoleSystem.Command command = ConsoleSystem.Index.Server.Find(name);
		if (command != null)
		{
			return command;
		}
		return null;
	}
}
