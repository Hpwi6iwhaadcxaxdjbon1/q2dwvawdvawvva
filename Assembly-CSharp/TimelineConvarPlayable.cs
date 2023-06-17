using System;
using UnityEngine.Playables;

// Token: 0x0200076C RID: 1900
[Serializable]
public class TimelineConvarPlayable : PlayableBehaviour
{
	// Token: 0x04002AF9 RID: 11001
	[NonSerialized]
	public string convar;

	// Token: 0x04002AFA RID: 11002
	public float ConvarValue;

	// Token: 0x060034C2 RID: 13506 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
	}
}
