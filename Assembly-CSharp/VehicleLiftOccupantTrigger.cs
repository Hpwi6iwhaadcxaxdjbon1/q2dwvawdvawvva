using System;
using Rust;
using UnityEngine;

// Token: 0x0200049B RID: 1179
public class VehicleLiftOccupantTrigger : TriggerBase
{
	// Token: 0x17000335 RID: 821
	// (get) Token: 0x060026B5 RID: 9909 RVA: 0x000F2728 File Offset: 0x000F0928
	// (set) Token: 0x060026B6 RID: 9910 RVA: 0x000F2730 File Offset: 0x000F0930
	public ModularCar carOccupant { get; private set; }

	// Token: 0x060026B7 RID: 9911 RVA: 0x000F2739 File Offset: 0x000F0939
	protected override void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.OnDisable();
		if (this.carOccupant != null)
		{
			this.carOccupant = null;
		}
	}

	// Token: 0x060026B8 RID: 9912 RVA: 0x000F2760 File Offset: 0x000F0960
	internal override GameObject InterestedInObject(GameObject obj)
	{
		if (base.InterestedInObject(obj) == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null || baseEntity.isClient)
		{
			return null;
		}
		if (!(baseEntity is ModularCar))
		{
			return null;
		}
		return obj;
	}

	// Token: 0x060026B9 RID: 9913 RVA: 0x000F27A3 File Offset: 0x000F09A3
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (this.carOccupant == null && ent.isServer)
		{
			this.carOccupant = (ModularCar)ent;
		}
	}

	// Token: 0x060026BA RID: 9914 RVA: 0x000F27D0 File Offset: 0x000F09D0
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (this.carOccupant == ent)
		{
			this.carOccupant = null;
			if (this.entityContents != null && this.entityContents.Count > 0)
			{
				foreach (BaseEntity baseEntity in this.entityContents)
				{
					if (baseEntity != null)
					{
						this.carOccupant = (ModularCar)baseEntity;
						break;
					}
				}
			}
		}
	}
}
