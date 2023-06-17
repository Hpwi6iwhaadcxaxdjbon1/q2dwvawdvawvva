using System;
using UnityEngine;

// Token: 0x02000506 RID: 1286
public class EnvironmentVolume : MonoBehaviour
{
	// Token: 0x0400212F RID: 8495
	[InspectorFlags]
	public EnvironmentType Type = EnvironmentType.Underground;

	// Token: 0x04002130 RID: 8496
	public Vector3 Center = Vector3.zero;

	// Token: 0x04002131 RID: 8497
	public Vector3 Size = Vector3.one;

	// Token: 0x1700037F RID: 895
	// (get) Token: 0x06002931 RID: 10545 RVA: 0x000FCDF4 File Offset: 0x000FAFF4
	// (set) Token: 0x06002932 RID: 10546 RVA: 0x000FCDFC File Offset: 0x000FAFFC
	public Collider trigger { get; private set; }

	// Token: 0x06002933 RID: 10547 RVA: 0x000FCE05 File Offset: 0x000FB005
	protected virtual void Awake()
	{
		this.UpdateTrigger();
	}

	// Token: 0x06002934 RID: 10548 RVA: 0x000FCE0D File Offset: 0x000FB00D
	protected void OnEnable()
	{
		if (this.trigger && !this.trigger.enabled)
		{
			this.trigger.enabled = true;
		}
	}

	// Token: 0x06002935 RID: 10549 RVA: 0x000FCE35 File Offset: 0x000FB035
	protected void OnDisable()
	{
		if (this.trigger && this.trigger.enabled)
		{
			this.trigger.enabled = false;
		}
	}

	// Token: 0x06002936 RID: 10550 RVA: 0x000FCE60 File Offset: 0x000FB060
	public void UpdateTrigger()
	{
		if (!this.trigger)
		{
			this.trigger = base.gameObject.GetComponent<Collider>();
		}
		if (!this.trigger)
		{
			this.trigger = base.gameObject.AddComponent<BoxCollider>();
		}
		this.trigger.isTrigger = true;
		BoxCollider boxCollider = this.trigger as BoxCollider;
		if (boxCollider)
		{
			boxCollider.center = this.Center;
			boxCollider.size = this.Size;
		}
	}
}
