using System;
using UnityEngine;

// Token: 0x0200079E RID: 1950
public class ConvarToggleChildren : MonoBehaviour
{
	// Token: 0x04002B4A RID: 11082
	public string ConvarName;

	// Token: 0x04002B4B RID: 11083
	public string ConvarEnabled = "True";

	// Token: 0x04002B4C RID: 11084
	private bool state;

	// Token: 0x04002B4D RID: 11085
	private ConsoleSystem.Command Command;

	// Token: 0x060034FF RID: 13567 RVA: 0x001463BC File Offset: 0x001445BC
	protected void Awake()
	{
		this.Command = ConsoleSystem.Index.Client.Find(this.ConvarName);
		if (this.Command == null)
		{
			this.Command = ConsoleSystem.Index.Server.Find(this.ConvarName);
		}
		if (this.Command != null)
		{
			this.SetState(this.Command.String == this.ConvarEnabled);
		}
	}

	// Token: 0x06003500 RID: 13568 RVA: 0x00146418 File Offset: 0x00144618
	protected void Update()
	{
		if (this.Command != null)
		{
			bool flag = this.Command.String == this.ConvarEnabled;
			if (this.state != flag)
			{
				this.SetState(flag);
			}
		}
	}

	// Token: 0x06003501 RID: 13569 RVA: 0x00146454 File Offset: 0x00144654
	private void SetState(bool newState)
	{
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(newState);
		}
		this.state = newState;
	}
}
