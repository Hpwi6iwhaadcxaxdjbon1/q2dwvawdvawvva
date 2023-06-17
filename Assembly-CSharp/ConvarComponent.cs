using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020008F7 RID: 2295
public class ConvarComponent : MonoBehaviour
{
	// Token: 0x040032C7 RID: 12999
	public bool runOnServer = true;

	// Token: 0x040032C8 RID: 13000
	public bool runOnClient = true;

	// Token: 0x040032C9 RID: 13001
	public List<ConvarComponent.ConvarEvent> List = new List<ConvarComponent.ConvarEvent>();

	// Token: 0x060037D2 RID: 14290 RVA: 0x0014E594 File Offset: 0x0014C794
	protected void OnEnable()
	{
		if (!this.ShouldRun())
		{
			return;
		}
		foreach (ConvarComponent.ConvarEvent convarEvent in this.List)
		{
			convarEvent.OnEnable();
		}
	}

	// Token: 0x060037D3 RID: 14291 RVA: 0x0014E5F0 File Offset: 0x0014C7F0
	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (!this.ShouldRun())
		{
			return;
		}
		foreach (ConvarComponent.ConvarEvent convarEvent in this.List)
		{
			convarEvent.OnDisable();
		}
	}

	// Token: 0x060037D4 RID: 14292 RVA: 0x0014E654 File Offset: 0x0014C854
	private bool ShouldRun()
	{
		return this.runOnServer;
	}

	// Token: 0x02000EAF RID: 3759
	[Serializable]
	public class ConvarEvent
	{
		// Token: 0x04004CA6 RID: 19622
		public string convar;

		// Token: 0x04004CA7 RID: 19623
		public string on;

		// Token: 0x04004CA8 RID: 19624
		public MonoBehaviour component;

		// Token: 0x04004CA9 RID: 19625
		internal ConsoleSystem.Command cmd;

		// Token: 0x06005311 RID: 21265 RVA: 0x001B1700 File Offset: 0x001AF900
		public void OnEnable()
		{
			this.cmd = ConsoleSystem.Index.Client.Find(this.convar);
			if (this.cmd == null)
			{
				this.cmd = ConsoleSystem.Index.Server.Find(this.convar);
			}
			if (this.cmd == null)
			{
				return;
			}
			this.cmd.OnValueChanged += this.cmd_OnValueChanged;
			this.cmd_OnValueChanged(this.cmd);
		}

		// Token: 0x06005312 RID: 21266 RVA: 0x001B1764 File Offset: 0x001AF964
		private void cmd_OnValueChanged(ConsoleSystem.Command obj)
		{
			if (this.component == null)
			{
				return;
			}
			bool flag = obj.String == this.on;
			if (this.component.enabled == flag)
			{
				return;
			}
			this.component.enabled = flag;
		}

		// Token: 0x06005313 RID: 21267 RVA: 0x001B17AD File Offset: 0x001AF9AD
		public void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			if (this.cmd == null)
			{
				return;
			}
			this.cmd.OnValueChanged -= this.cmd_OnValueChanged;
		}
	}
}
