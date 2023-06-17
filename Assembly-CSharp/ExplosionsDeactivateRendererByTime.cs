using System;
using UnityEngine;

// Token: 0x02000992 RID: 2450
public class ExplosionsDeactivateRendererByTime : MonoBehaviour
{
	// Token: 0x04003499 RID: 13465
	public float TimeDelay = 1f;

	// Token: 0x0400349A RID: 13466
	private Renderer rend;

	// Token: 0x06003A4E RID: 14926 RVA: 0x001591A0 File Offset: 0x001573A0
	private void Awake()
	{
		this.rend = base.GetComponent<Renderer>();
	}

	// Token: 0x06003A4F RID: 14927 RVA: 0x001591AE File Offset: 0x001573AE
	private void DeactivateRenderer()
	{
		this.rend.enabled = false;
	}

	// Token: 0x06003A50 RID: 14928 RVA: 0x001591BC File Offset: 0x001573BC
	private void OnEnable()
	{
		this.rend.enabled = true;
		base.Invoke("DeactivateRenderer", this.TimeDelay);
	}
}
