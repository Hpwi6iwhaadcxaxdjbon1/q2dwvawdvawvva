using System;
using UnityEngine;

// Token: 0x02000991 RID: 2449
public class ExplosionsBillboard : MonoBehaviour
{
	// Token: 0x04003492 RID: 13458
	public Camera Camera;

	// Token: 0x04003493 RID: 13459
	public bool Active = true;

	// Token: 0x04003494 RID: 13460
	public bool AutoInitCamera = true;

	// Token: 0x04003495 RID: 13461
	private GameObject myContainer;

	// Token: 0x04003496 RID: 13462
	private Transform t;

	// Token: 0x04003497 RID: 13463
	private Transform camT;

	// Token: 0x04003498 RID: 13464
	private Transform contT;

	// Token: 0x06003A4B RID: 14923 RVA: 0x00159030 File Offset: 0x00157230
	private void Awake()
	{
		if (this.AutoInitCamera)
		{
			this.Camera = Camera.main;
			this.Active = true;
		}
		this.t = base.transform;
		Vector3 localScale = this.t.parent.transform.localScale;
		localScale.z = localScale.x;
		this.t.parent.transform.localScale = localScale;
		this.camT = this.Camera.transform;
		Transform parent = this.t.parent;
		this.myContainer = new GameObject
		{
			name = "Billboard_" + this.t.gameObject.name
		};
		this.contT = this.myContainer.transform;
		this.contT.position = this.t.position;
		this.t.parent = this.myContainer.transform;
		this.contT.parent = parent;
	}

	// Token: 0x06003A4C RID: 14924 RVA: 0x00159130 File Offset: 0x00157330
	private void Update()
	{
		if (this.Active)
		{
			this.contT.LookAt(this.contT.position + this.camT.rotation * Vector3.back, this.camT.rotation * Vector3.up);
		}
	}
}
