using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000253 RID: 595
public class ConstructionGrade : PrefabAttribute
{
	// Token: 0x040014FF RID: 5375
	[NonSerialized]
	public Construction construction;

	// Token: 0x04001500 RID: 5376
	public BuildingGrade gradeBase;

	// Token: 0x04001501 RID: 5377
	public GameObjectRef skinObject;

	// Token: 0x04001502 RID: 5378
	internal List<ItemAmount> _costToBuild;

	// Token: 0x17000260 RID: 608
	// (get) Token: 0x06001C3D RID: 7229 RVA: 0x000C5115 File Offset: 0x000C3315
	public float maxHealth
	{
		get
		{
			if (!this.gradeBase || !this.construction)
			{
				return 0f;
			}
			return this.gradeBase.baseHealth * this.construction.healthMultiplier;
		}
	}

	// Token: 0x06001C3E RID: 7230 RVA: 0x000C5150 File Offset: 0x000C3350
	public List<ItemAmount> CostToBuild(BuildingGrade.Enum fromGrade = BuildingGrade.Enum.None)
	{
		if (this._costToBuild == null)
		{
			this._costToBuild = new List<ItemAmount>();
		}
		else
		{
			this._costToBuild.Clear();
		}
		float num = (fromGrade == this.gradeBase.type) ? 0.2f : 1f;
		foreach (ItemAmount itemAmount in this.gradeBase.baseCost)
		{
			this._costToBuild.Add(new ItemAmount(itemAmount.itemDef, Mathf.Ceil(itemAmount.amount * this.construction.costMultiplier * num)));
		}
		return this._costToBuild;
	}

	// Token: 0x06001C3F RID: 7231 RVA: 0x000C5214 File Offset: 0x000C3414
	protected override Type GetIndexedType()
	{
		return typeof(ConstructionGrade);
	}
}
