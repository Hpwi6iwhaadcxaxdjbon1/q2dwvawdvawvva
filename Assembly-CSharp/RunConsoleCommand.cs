using System;
using UnityEngine;

// Token: 0x02000915 RID: 2325
public class RunConsoleCommand : MonoBehaviour
{
	// Token: 0x06003829 RID: 14377 RVA: 0x0014F216 File Offset: 0x0014D416
	public void ClientRun(string command)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, command, Array.Empty<object>());
	}
}
