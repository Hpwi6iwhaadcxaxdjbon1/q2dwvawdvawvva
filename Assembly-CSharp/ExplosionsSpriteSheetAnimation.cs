using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000999 RID: 2457
internal class ExplosionsSpriteSheetAnimation : MonoBehaviour
{
	// Token: 0x040034C0 RID: 13504
	public int TilesX = 4;

	// Token: 0x040034C1 RID: 13505
	public int TilesY = 4;

	// Token: 0x040034C2 RID: 13506
	public float AnimationFPS = 30f;

	// Token: 0x040034C3 RID: 13507
	public bool IsInterpolateFrames;

	// Token: 0x040034C4 RID: 13508
	public int StartFrameOffset;

	// Token: 0x040034C5 RID: 13509
	public bool IsLoop = true;

	// Token: 0x040034C6 RID: 13510
	public float StartDelay;

	// Token: 0x040034C7 RID: 13511
	public AnimationCurve FrameOverTime = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	// Token: 0x040034C8 RID: 13512
	private bool isInizialised;

	// Token: 0x040034C9 RID: 13513
	private int index;

	// Token: 0x040034CA RID: 13514
	private int count;

	// Token: 0x040034CB RID: 13515
	private int allCount;

	// Token: 0x040034CC RID: 13516
	private float animationLifeTime;

	// Token: 0x040034CD RID: 13517
	private bool isVisible;

	// Token: 0x040034CE RID: 13518
	private bool isCorutineStarted;

	// Token: 0x040034CF RID: 13519
	private Renderer currentRenderer;

	// Token: 0x040034D0 RID: 13520
	private Material instanceMaterial;

	// Token: 0x040034D1 RID: 13521
	private float currentInterpolatedTime;

	// Token: 0x040034D2 RID: 13522
	private float animationStartTime;

	// Token: 0x040034D3 RID: 13523
	private bool animationStoped;

	// Token: 0x06003A69 RID: 14953 RVA: 0x0015979C File Offset: 0x0015799C
	private void Start()
	{
		this.currentRenderer = base.GetComponent<Renderer>();
		this.InitDefaultVariables();
		this.isInizialised = true;
		this.isVisible = true;
		this.Play();
	}

	// Token: 0x06003A6A RID: 14954 RVA: 0x001597C4 File Offset: 0x001579C4
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
		this.animationStoped = false;
		this.animationLifeTime = (float)(this.TilesX * this.TilesY) / this.AnimationFPS;
		this.count = this.TilesY * this.TilesX;
		this.index = this.TilesX - 1;
		Vector3 zero = Vector3.zero;
		this.StartFrameOffset -= this.StartFrameOffset / this.count * this.count;
		Vector2 value = new Vector2(1f / (float)this.TilesX, 1f / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial = this.currentRenderer.material;
			this.instanceMaterial.SetTextureScale("_MainTex", value);
			this.instanceMaterial.SetTextureOffset("_MainTex", zero);
		}
	}

	// Token: 0x06003A6B RID: 14955 RVA: 0x001598E7 File Offset: 0x00157AE7
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

	// Token: 0x06003A6C RID: 14956 RVA: 0x00159926 File Offset: 0x00157B26
	private void PlayDelay()
	{
		base.StartCoroutine(this.UpdateCorutine());
	}

	// Token: 0x06003A6D RID: 14957 RVA: 0x00159935 File Offset: 0x00157B35
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

	// Token: 0x06003A6E RID: 14958 RVA: 0x00159953 File Offset: 0x00157B53
	private void OnDisable()
	{
		this.isCorutineStarted = false;
		this.isVisible = false;
		base.StopAllCoroutines();
		base.CancelInvoke("PlayDelay");
	}

	// Token: 0x06003A6F RID: 14959 RVA: 0x00159974 File Offset: 0x00157B74
	private IEnumerator UpdateCorutine()
	{
		this.animationStartTime = Time.time;
		while (this.isVisible && (this.IsLoop || !this.animationStoped))
		{
			this.UpdateFrame();
			if (!this.IsLoop && this.animationStoped)
			{
				break;
			}
			float value = (Time.time - this.animationStartTime) / this.animationLifeTime;
			float num = this.FrameOverTime.Evaluate(Mathf.Clamp01(value));
			yield return new WaitForSeconds(1f / (this.AnimationFPS * num));
		}
		this.isCorutineStarted = false;
		this.currentRenderer.enabled = false;
		yield break;
	}

	// Token: 0x06003A70 RID: 14960 RVA: 0x00159984 File Offset: 0x00157B84
	private void UpdateFrame()
	{
		this.allCount++;
		this.index++;
		if (this.index >= this.count)
		{
			this.index = 0;
		}
		if (this.count == this.allCount)
		{
			this.animationStartTime = Time.time;
			this.allCount = 0;
			this.animationStoped = true;
		}
		Vector2 value = new Vector2((float)this.index / (float)this.TilesX - (float)(this.index / this.TilesX), 1f - (float)(this.index / this.TilesX) / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetTextureOffset("_MainTex", value);
		}
		if (this.IsInterpolateFrames)
		{
			this.currentInterpolatedTime = 0f;
		}
	}

	// Token: 0x06003A71 RID: 14961 RVA: 0x00159A5C File Offset: 0x00157C5C
	private void Update()
	{
		if (!this.IsInterpolateFrames)
		{
			return;
		}
		this.currentInterpolatedTime += Time.deltaTime;
		int num = this.index + 1;
		if (this.allCount == 0)
		{
			num = this.index;
		}
		Vector4 value = new Vector4(1f / (float)this.TilesX, 1f / (float)this.TilesY, (float)num / (float)this.TilesX - (float)(num / this.TilesX), 1f - (float)(num / this.TilesX) / (float)this.TilesY);
		if (this.currentRenderer != null)
		{
			this.instanceMaterial.SetVector("_MainTex_NextFrame", value);
			float value2 = (Time.time - this.animationStartTime) / this.animationLifeTime;
			float num2 = this.FrameOverTime.Evaluate(Mathf.Clamp01(value2));
			this.instanceMaterial.SetFloat("InterpolationValue", Mathf.Clamp01(this.currentInterpolatedTime * this.AnimationFPS * num2));
		}
	}

	// Token: 0x06003A72 RID: 14962 RVA: 0x00159B51 File Offset: 0x00157D51
	private void OnDestroy()
	{
		if (this.instanceMaterial != null)
		{
			UnityEngine.Object.Destroy(this.instanceMaterial);
			this.instanceMaterial = null;
		}
	}
}
