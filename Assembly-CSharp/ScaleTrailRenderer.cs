using System;
using UnityEngine;

// Token: 0x02000351 RID: 849
public class ScaleTrailRenderer : ScaleRenderer
{
	// Token: 0x04001895 RID: 6293
	private TrailRenderer trailRenderer;

	// Token: 0x04001896 RID: 6294
	[NonSerialized]
	private float startWidth;

	// Token: 0x04001897 RID: 6295
	[NonSerialized]
	private float endWidth;

	// Token: 0x04001898 RID: 6296
	[NonSerialized]
	private float duration;

	// Token: 0x04001899 RID: 6297
	[NonSerialized]
	private float startMultiplier;

	// Token: 0x06001F2B RID: 7979 RVA: 0x000D3484 File Offset: 0x000D1684
	public override void GatherInitialValues()
	{
		base.GatherInitialValues();
		if (this.myRenderer)
		{
			this.trailRenderer = this.myRenderer.GetComponent<TrailRenderer>();
		}
		else
		{
			this.trailRenderer = base.GetComponentInChildren<TrailRenderer>();
		}
		this.startWidth = this.trailRenderer.startWidth;
		this.endWidth = this.trailRenderer.endWidth;
		this.duration = this.trailRenderer.time;
		this.startMultiplier = this.trailRenderer.widthMultiplier;
	}

	// Token: 0x06001F2C RID: 7980 RVA: 0x000D3508 File Offset: 0x000D1708
	public override void SetScale_Internal(float scale)
	{
		if (scale == 0f)
		{
			this.trailRenderer.emitting = false;
			this.trailRenderer.enabled = false;
			this.trailRenderer.time = 0f;
			this.trailRenderer.Clear();
			return;
		}
		if (!this.trailRenderer.emitting)
		{
			this.trailRenderer.Clear();
		}
		this.trailRenderer.emitting = true;
		this.trailRenderer.enabled = true;
		base.SetScale_Internal(scale);
		this.trailRenderer.widthMultiplier = this.startMultiplier * scale;
		this.trailRenderer.time = this.duration * scale;
	}
}
