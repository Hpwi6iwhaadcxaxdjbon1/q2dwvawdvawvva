using System;
using UnityEngine;

// Token: 0x02000350 RID: 848
public class ScaleRenderer : MonoBehaviour
{
	// Token: 0x0400188F RID: 6287
	public bool useRandomScale;

	// Token: 0x04001890 RID: 6288
	public float scaleMin = 1f;

	// Token: 0x04001891 RID: 6289
	public float scaleMax = 1f;

	// Token: 0x04001892 RID: 6290
	private float lastScale = -1f;

	// Token: 0x04001893 RID: 6291
	protected bool hasInitialValues;

	// Token: 0x04001894 RID: 6292
	public Renderer myRenderer;

	// Token: 0x06001F24 RID: 7972 RVA: 0x000D339C File Offset: 0x000D159C
	private bool ScaleDifferent(float newScale)
	{
		return newScale != this.lastScale;
	}

	// Token: 0x06001F25 RID: 7973 RVA: 0x000D33AA File Offset: 0x000D15AA
	public void Start()
	{
		if (this.useRandomScale)
		{
			this.SetScale(UnityEngine.Random.Range(this.scaleMin, this.scaleMax));
		}
	}

	// Token: 0x06001F26 RID: 7974 RVA: 0x000D33CC File Offset: 0x000D15CC
	public void SetScale(float scale)
	{
		if (!this.hasInitialValues)
		{
			this.GatherInitialValues();
		}
		if (this.ScaleDifferent(scale) || (scale > 0f && !this.myRenderer.enabled))
		{
			this.SetRendererEnabled(scale != 0f);
			this.SetScale_Internal(scale);
		}
	}

	// Token: 0x06001F27 RID: 7975 RVA: 0x000D341D File Offset: 0x000D161D
	public virtual void SetScale_Internal(float scale)
	{
		this.lastScale = scale;
	}

	// Token: 0x06001F28 RID: 7976 RVA: 0x000D3426 File Offset: 0x000D1626
	public virtual void SetRendererEnabled(bool isEnabled)
	{
		if (this.myRenderer && this.myRenderer.enabled != isEnabled)
		{
			this.myRenderer.enabled = isEnabled;
		}
	}

	// Token: 0x06001F29 RID: 7977 RVA: 0x000D344F File Offset: 0x000D164F
	public virtual void GatherInitialValues()
	{
		this.hasInitialValues = true;
	}
}
