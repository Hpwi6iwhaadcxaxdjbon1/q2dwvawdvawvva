using System;
using UnityEngine;

// Token: 0x02000807 RID: 2055
public class UIFadeOut : MonoBehaviour
{
	// Token: 0x04002E17 RID: 11799
	public float secondsToFadeOut = 3f;

	// Token: 0x04002E18 RID: 11800
	public bool destroyOnFaded = true;

	// Token: 0x04002E19 RID: 11801
	public CanvasGroup targetGroup;

	// Token: 0x04002E1A RID: 11802
	public float fadeDelay;

	// Token: 0x04002E1B RID: 11803
	private float timeStarted;

	// Token: 0x060035BE RID: 13758 RVA: 0x001481EF File Offset: 0x001463EF
	private void Start()
	{
		this.timeStarted = Time.realtimeSinceStartup;
	}

	// Token: 0x060035BF RID: 13759 RVA: 0x001481FC File Offset: 0x001463FC
	private void Update()
	{
		float num = this.timeStarted;
		this.targetGroup.alpha = Mathf.InverseLerp(num + this.secondsToFadeOut, num, Time.realtimeSinceStartup - this.fadeDelay);
		if (this.destroyOnFaded && Time.realtimeSinceStartup - this.fadeDelay > this.timeStarted + this.secondsToFadeOut)
		{
			GameManager.Destroy(base.gameObject, 0f);
		}
	}
}
