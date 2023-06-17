using System;
using Rust;

// Token: 0x02000446 RID: 1094
public class PlayerInput : EntityComponent<BasePlayer>
{
	// Token: 0x04001CB2 RID: 7346
	public InputState state = new InputState();

	// Token: 0x04001CB3 RID: 7347
	[NonSerialized]
	public bool hadInputBuffer = true;

	// Token: 0x0600249D RID: 9373 RVA: 0x000E8758 File Offset: 0x000E6958
	protected void OnDisable()
	{
		if (Application.isQuitting)
		{
			return;
		}
		this.state.Clear();
	}
}
