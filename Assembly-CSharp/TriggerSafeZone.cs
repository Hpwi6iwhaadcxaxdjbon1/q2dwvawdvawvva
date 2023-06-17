using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000596 RID: 1430
public class TriggerSafeZone : TriggerBase
{
	// Token: 0x04002349 RID: 9033
	public static List<TriggerSafeZone> allSafeZones = new List<TriggerSafeZone>();

	// Token: 0x0400234A RID: 9034
	public float maxDepth = 20f;

	// Token: 0x0400234B RID: 9035
	public float maxAltitude = -1f;

	// Token: 0x17000399 RID: 921
	// (get) Token: 0x06002B96 RID: 11158 RVA: 0x001081AB File Offset: 0x001063AB
	// (set) Token: 0x06002B97 RID: 11159 RVA: 0x001081B3 File Offset: 0x001063B3
	public Collider triggerCollider { get; private set; }

	// Token: 0x06002B98 RID: 11160 RVA: 0x001081BC File Offset: 0x001063BC
	protected void Awake()
	{
		this.triggerCollider = base.GetComponent<Collider>();
	}

	// Token: 0x06002B99 RID: 11161 RVA: 0x001081CA File Offset: 0x001063CA
	protected void OnEnable()
	{
		TriggerSafeZone.allSafeZones.Add(this);
	}

	// Token: 0x06002B9A RID: 11162 RVA: 0x001081D7 File Offset: 0x001063D7
	protected override void OnDisable()
	{
		base.OnDisable();
		TriggerSafeZone.allSafeZones.Remove(this);
	}

	// Token: 0x06002B9B RID: 11163 RVA: 0x001081EC File Offset: 0x001063EC
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002B9C RID: 11164 RVA: 0x00108230 File Offset: 0x00106430
	public bool PassesHeightChecks(Vector3 entPos)
	{
		Vector3 position = base.transform.position;
		float num = Mathf.Abs(position.y - entPos.y);
		return (this.maxDepth == -1f || entPos.y >= position.y || num <= this.maxDepth) && (this.maxAltitude == -1f || entPos.y <= position.y || num <= this.maxAltitude);
	}

	// Token: 0x06002B9D RID: 11165 RVA: 0x001082A9 File Offset: 0x001064A9
	public float GetSafeLevel(Vector3 pos)
	{
		if (!this.PassesHeightChecks(pos))
		{
			return 0f;
		}
		return 1f;
	}
}
