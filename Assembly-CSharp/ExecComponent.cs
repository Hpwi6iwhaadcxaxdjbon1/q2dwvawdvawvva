using System;
using UnityEngine;

// Token: 0x020008FA RID: 2298
public class ExecComponent : MonoBehaviour
{
	// Token: 0x040032D0 RID: 13008
	public string ExecToRun = string.Empty;

	// Token: 0x060037D8 RID: 14296 RVA: 0x0014E6AB File Offset: 0x0014C8AB
	public void Run()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, this.ExecToRun, Array.Empty<object>());
	}
}
