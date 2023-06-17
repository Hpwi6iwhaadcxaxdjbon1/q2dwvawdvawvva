using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000353 RID: 851
public abstract class BaseScreenShake : MonoBehaviour
{
	// Token: 0x0400189B RID: 6299
	public static List<BaseScreenShake> list = new List<BaseScreenShake>();

	// Token: 0x0400189C RID: 6300
	internal static float punchFadeScale = 0f;

	// Token: 0x0400189D RID: 6301
	internal static float bobScale = 0f;

	// Token: 0x0400189E RID: 6302
	internal static float animPunchMagnitude = 10f;

	// Token: 0x0400189F RID: 6303
	public float length = 2f;

	// Token: 0x040018A0 RID: 6304
	internal float timeTaken;

	// Token: 0x040018A1 RID: 6305
	private int currentFrame = -1;

	// Token: 0x06001F31 RID: 7985 RVA: 0x000D35F4 File Offset: 0x000D17F4
	public static void Apply(Camera cam, BaseViewModel vm)
	{
		CachedTransform<Camera> cachedTransform = new CachedTransform<Camera>(cam);
		CachedTransform<BaseViewModel> cachedTransform2 = new CachedTransform<BaseViewModel>(vm);
		for (int i = 0; i < BaseScreenShake.list.Count; i++)
		{
			BaseScreenShake.list[i].Run(ref cachedTransform, ref cachedTransform2);
		}
		cachedTransform.Apply();
		cachedTransform2.Apply();
	}

	// Token: 0x06001F32 RID: 7986 RVA: 0x000D3648 File Offset: 0x000D1848
	protected void OnEnable()
	{
		BaseScreenShake.list.Add(this);
		this.timeTaken = 0f;
		this.Setup();
	}

	// Token: 0x06001F33 RID: 7987 RVA: 0x000D3666 File Offset: 0x000D1866
	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		BaseScreenShake.list.Remove(this);
	}

	// Token: 0x06001F34 RID: 7988 RVA: 0x000D367C File Offset: 0x000D187C
	public void Run(ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		if (this.timeTaken > this.length)
		{
			return;
		}
		if (Time.frameCount != this.currentFrame)
		{
			this.timeTaken += Time.deltaTime;
			this.currentFrame = Time.frameCount;
		}
		float delta = Mathf.InverseLerp(0f, this.length, this.timeTaken);
		this.Run(delta, ref cam, ref vm);
	}

	// Token: 0x06001F35 RID: 7989
	public abstract void Setup();

	// Token: 0x06001F36 RID: 7990
	public abstract void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm);
}
