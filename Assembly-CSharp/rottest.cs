using System;
using UnityEngine;

// Token: 0x020001A1 RID: 417
public class rottest : MonoBehaviour
{
	// Token: 0x0400113F RID: 4415
	public Transform turretBase;

	// Token: 0x04001140 RID: 4416
	public Vector3 aimDir;

	// Token: 0x06001878 RID: 6264 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x06001879 RID: 6265 RVA: 0x000B6F6A File Offset: 0x000B516A
	private void Update()
	{
		this.aimDir = new Vector3(0f, 45f * Mathf.Sin(Time.time * 6f), 0f);
		this.UpdateAiming();
	}

	// Token: 0x0600187A RID: 6266 RVA: 0x000B6FA0 File Offset: 0x000B51A0
	public void UpdateAiming()
	{
		if (this.aimDir == Vector3.zero)
		{
			return;
		}
		Quaternion quaternion = Quaternion.Euler(0f, this.aimDir.y, 0f);
		if (base.transform.localRotation != quaternion)
		{
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, quaternion, Time.deltaTime * 8f);
		}
	}
}
