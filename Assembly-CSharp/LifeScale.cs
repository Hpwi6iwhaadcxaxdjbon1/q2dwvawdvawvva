using System;
using UnityEngine;

// Token: 0x0200043D RID: 1085
public class LifeScale : BaseMonoBehaviour
{
	// Token: 0x04001C7A RID: 7290
	[NonSerialized]
	private bool initialized;

	// Token: 0x04001C7B RID: 7291
	[NonSerialized]
	private Vector3 initialScale;

	// Token: 0x04001C7C RID: 7292
	public Vector3 finalScale = Vector3.one;

	// Token: 0x04001C7D RID: 7293
	private Vector3 targetLerpScale = Vector3.zero;

	// Token: 0x04001C7E RID: 7294
	private Action updateScaleAction;

	// Token: 0x06002467 RID: 9319 RVA: 0x000E7916 File Offset: 0x000E5B16
	protected void Awake()
	{
		this.updateScaleAction = new Action(this.UpdateScale);
	}

	// Token: 0x06002468 RID: 9320 RVA: 0x000E792A File Offset: 0x000E5B2A
	public void OnEnable()
	{
		this.Init();
		base.transform.localScale = this.initialScale;
	}

	// Token: 0x06002469 RID: 9321 RVA: 0x000E7943 File Offset: 0x000E5B43
	public void SetProgress(float progress)
	{
		this.Init();
		this.targetLerpScale = Vector3.Lerp(this.initialScale, this.finalScale, progress);
		base.InvokeRepeating(this.updateScaleAction, 0f, 0.015f);
	}

	// Token: 0x0600246A RID: 9322 RVA: 0x000E7979 File Offset: 0x000E5B79
	public void Init()
	{
		if (!this.initialized)
		{
			this.initialScale = base.transform.localScale;
			this.initialized = true;
		}
	}

	// Token: 0x0600246B RID: 9323 RVA: 0x000E799C File Offset: 0x000E5B9C
	public void UpdateScale()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, this.targetLerpScale, Time.deltaTime);
		if (base.transform.localScale == this.targetLerpScale)
		{
			this.targetLerpScale = Vector3.zero;
			base.CancelInvoke(this.updateScaleAction);
		}
	}
}
