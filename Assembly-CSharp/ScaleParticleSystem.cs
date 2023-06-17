using System;
using UnityEngine;

// Token: 0x0200034F RID: 847
public class ScaleParticleSystem : ScaleRenderer
{
	// Token: 0x04001889 RID: 6281
	public ParticleSystem pSystem;

	// Token: 0x0400188A RID: 6282
	public bool scaleGravity;

	// Token: 0x0400188B RID: 6283
	[NonSerialized]
	private float startSize;

	// Token: 0x0400188C RID: 6284
	[NonSerialized]
	private float startLifeTime;

	// Token: 0x0400188D RID: 6285
	[NonSerialized]
	private float startSpeed;

	// Token: 0x0400188E RID: 6286
	[NonSerialized]
	private float startGravity;

	// Token: 0x06001F21 RID: 7969 RVA: 0x000D32DC File Offset: 0x000D14DC
	public override void GatherInitialValues()
	{
		base.GatherInitialValues();
		this.startGravity = this.pSystem.gravityModifier;
		this.startSpeed = this.pSystem.startSpeed;
		this.startSize = this.pSystem.startSize;
		this.startLifeTime = this.pSystem.startLifetime;
	}

	// Token: 0x06001F22 RID: 7970 RVA: 0x000D3334 File Offset: 0x000D1534
	public override void SetScale_Internal(float scale)
	{
		base.SetScale_Internal(scale);
		this.pSystem.startSize = this.startSize * scale;
		this.pSystem.startLifetime = this.startLifeTime * scale;
		this.pSystem.startSpeed = this.startSpeed * scale;
		this.pSystem.gravityModifier = this.startGravity * scale;
	}
}
