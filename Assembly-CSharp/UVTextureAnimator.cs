using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000985 RID: 2437
internal class UVTextureAnimator : MonoBehaviour
{
	// Token: 0x04003457 RID: 13399
	public int Rows = 4;

	// Token: 0x04003458 RID: 13400
	public int Columns = 4;

	// Token: 0x04003459 RID: 13401
	public float Fps = 20f;

	// Token: 0x0400345A RID: 13402
	public int OffsetMat;

	// Token: 0x0400345B RID: 13403
	public bool IsLoop = true;

	// Token: 0x0400345C RID: 13404
	public float StartDelay;

	// Token: 0x0400345D RID: 13405
	private bool isInizialised;

	// Token: 0x0400345E RID: 13406
	private int index;

	// Token: 0x0400345F RID: 13407
	private int count;

	// Token: 0x04003460 RID: 13408
	private int allCount;

	// Token: 0x04003461 RID: 13409
	private float deltaFps;

	// Token: 0x04003462 RID: 13410
	private bool isVisible;

	// Token: 0x04003463 RID: 13411
	private bool isCorutineStarted;

	// Token: 0x04003464 RID: 13412
	private Renderer currentRenderer;

	// Token: 0x04003465 RID: 13413
	private Material instanceMaterial;

	// Token: 0x06003A01 RID: 14849 RVA: 0x00157A28 File Offset: 0x00155C28
	private void Start()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		this.InitDefaultVariables();
		this.isInizialised = true;
		this.isVisible = true;
		this.Play();
	}

	// Token: 0x06003A02 RID: 14850 RVA: 0x00157A50 File Offset: 0x00155C50
	private void InitDefaultVariables()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		if (this.currentRenderer == null)
		{
			throw new Exception("UvTextureAnimator can't get renderer");
		}
		if (!this.currentRenderer.enabled)
		{
			this.currentRenderer.enabled = true;
		}
		this.allCount = 0;
		this.deltaFps = 1f / this.Fps;
		this.count = this.Rows * this.Columns;
		this.index = this.Columns - 1;
		Vector3 zero = Vector3.zero;
		this.OffsetMat -= this.OffsetMat / this.count * this.count;
		Vector2 value = new Vector2(1f / (float)this.Columns, 1f / (float)this.Rows);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial = this.currentRenderer.material;
			this.instanceMaterial.SetTextureScale("_MainTex", value);
			this.instanceMaterial.SetTextureOffset("_MainTex", zero);
		}
	}

	// Token: 0x06003A03 RID: 14851 RVA: 0x00157B63 File Offset: 0x00155D63
	private void Play()
	{
		if (this.isCorutineStarted)
		{
			return;
		}
		if (this.StartDelay > 0.0001f)
		{
			base.Invoke("PlayDelay", this.StartDelay);
		}
		else
		{
			base.StartCoroutine(this.UpdateCorutine());
		}
		this.isCorutineStarted = true;
	}

	// Token: 0x06003A04 RID: 14852 RVA: 0x00157BA2 File Offset: 0x00155DA2
	private void PlayDelay()
	{
		base.StartCoroutine(this.UpdateCorutine());
	}

	// Token: 0x06003A05 RID: 14853 RVA: 0x00157BB1 File Offset: 0x00155DB1
	private void OnEnable()
	{
		if (!this.isInizialised)
		{
			return;
		}
		this.InitDefaultVariables();
		this.isVisible = true;
		this.Play();
	}

	// Token: 0x06003A06 RID: 14854 RVA: 0x00157BCF File Offset: 0x00155DCF
	private void OnDisable()
	{
		this.isCorutineStarted = false;
		this.isVisible = false;
		base.StopAllCoroutines();
		base.CancelInvoke("PlayDelay");
	}

	// Token: 0x06003A07 RID: 14855 RVA: 0x00157BF0 File Offset: 0x00155DF0
	private IEnumerator UpdateCorutine()
	{
		while (this.isVisible && (this.IsLoop || this.allCount != this.count))
		{
			this.UpdateCorutineFrame();
			if (!this.IsLoop && this.allCount == this.count)
			{
				break;
			}
			yield return new WaitForSeconds(this.deltaFps);
		}
		this.isCorutineStarted = false;
		this.currentRenderer.enabled = false;
		yield break;
	}

	// Token: 0x06003A08 RID: 14856 RVA: 0x00157C00 File Offset: 0x00155E00
	private void UpdateCorutineFrame()
	{
		this.allCount++;
		this.index++;
		if (this.index >= this.count)
		{
			this.index = 0;
		}
		Vector2 value = new Vector2((float)this.index / (float)this.Columns - (float)(this.index / this.Columns), 1f - (float)(this.index / this.Columns) / (float)this.Rows);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetTextureOffset("_MainTex", value);
		}
	}

	// Token: 0x06003A09 RID: 14857 RVA: 0x00157C9E File Offset: 0x00155E9E
	private void OnDestroy()
	{
		if (this.instanceMaterial != null)
		{
			UnityEngine.Object.Destroy(this.instanceMaterial);
			this.instanceMaterial = null;
		}
	}
}
