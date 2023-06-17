using System;
using UnityEngine;

// Token: 0x0200043B RID: 1083
public class LightListener : BaseEntity
{
	// Token: 0x04001C76 RID: 7286
	public string onMessage = "";

	// Token: 0x04001C77 RID: 7287
	public string offMessage = "";

	// Token: 0x04001C78 RID: 7288
	[Tooltip("Must be part of this prefab")]
	public LightGroupAtTime onLights;

	// Token: 0x04001C79 RID: 7289
	[Tooltip("Must be part of this prefab")]
	public LightGroupAtTime offLights;

	// Token: 0x06002463 RID: 9315 RVA: 0x000E78A5 File Offset: 0x000E5AA5
	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		base.OnEntityMessage(from, msg);
		if (msg == this.onMessage)
		{
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			return;
		}
		if (msg == this.offMessage)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
	}
}
