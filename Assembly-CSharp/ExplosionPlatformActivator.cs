using System;
using UnityEngine;

// Token: 0x0200098F RID: 2447
public class ExplosionPlatformActivator : MonoBehaviour
{
	// Token: 0x04003487 RID: 13447
	public GameObject Effect;

	// Token: 0x04003488 RID: 13448
	public float TimeDelay;

	// Token: 0x04003489 RID: 13449
	public float DefaultRepeatTime = 5f;

	// Token: 0x0400348A RID: 13450
	public float NearRepeatTime = 3f;

	// Token: 0x0400348B RID: 13451
	private float currentTime;

	// Token: 0x0400348C RID: 13452
	private float currentRepeatTime;

	// Token: 0x0400348D RID: 13453
	private bool canUpdate;

	// Token: 0x06003A41 RID: 14913 RVA: 0x00158E77 File Offset: 0x00157077
	private void Start()
	{
		this.currentRepeatTime = this.DefaultRepeatTime;
		base.Invoke("Init", this.TimeDelay);
	}

	// Token: 0x06003A42 RID: 14914 RVA: 0x00158E96 File Offset: 0x00157096
	private void Init()
	{
		this.canUpdate = true;
		this.Effect.SetActive(true);
	}

	// Token: 0x06003A43 RID: 14915 RVA: 0x00158EAC File Offset: 0x001570AC
	private void Update()
	{
		if (!this.canUpdate || this.Effect == null)
		{
			return;
		}
		this.currentTime += Time.deltaTime;
		if (this.currentTime > this.currentRepeatTime)
		{
			this.currentTime = 0f;
			this.Effect.SetActive(false);
			this.Effect.SetActive(true);
		}
	}

	// Token: 0x06003A44 RID: 14916 RVA: 0x00158F13 File Offset: 0x00157113
	private void OnTriggerEnter(Collider coll)
	{
		this.currentRepeatTime = this.NearRepeatTime;
	}

	// Token: 0x06003A45 RID: 14917 RVA: 0x00158F21 File Offset: 0x00157121
	private void OnTriggerExit(Collider other)
	{
		this.currentRepeatTime = this.DefaultRepeatTime;
	}
}
