using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000255 RID: 597
public class ConstructionSkin : BasePrefab
{
	// Token: 0x04001507 RID: 5383
	public List<GameObject> conditionals;

	// Token: 0x06001C44 RID: 7236 RVA: 0x000C52C0 File Offset: 0x000C34C0
	public int DetermineConditionalModelState(BuildingBlock parent)
	{
		ConditionalModel[] array = PrefabAttribute.server.FindAll<ConditionalModel>(this.prefabID);
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].RunTests(parent))
			{
				num |= 1 << i;
			}
		}
		return num;
	}

	// Token: 0x06001C45 RID: 7237 RVA: 0x000C5304 File Offset: 0x000C3504
	private void CreateConditionalModels(BuildingBlock parent)
	{
		ConditionalModel[] array = PrefabAttribute.server.FindAll<ConditionalModel>(this.prefabID);
		for (int i = 0; i < array.Length; i++)
		{
			if (parent.GetConditionalModel(i))
			{
				GameObject gameObject = array[i].InstantiateSkin(parent);
				if (!(gameObject == null))
				{
					if (this.conditionals == null)
					{
						this.conditionals = new List<GameObject>();
					}
					this.conditionals.Add(gameObject);
				}
			}
		}
	}

	// Token: 0x06001C46 RID: 7238 RVA: 0x000C536C File Offset: 0x000C356C
	private void DestroyConditionalModels(BuildingBlock parent)
	{
		if (this.conditionals == null)
		{
			return;
		}
		for (int i = 0; i < this.conditionals.Count; i++)
		{
			parent.gameManager.Retire(this.conditionals[i]);
		}
		this.conditionals.Clear();
	}

	// Token: 0x06001C47 RID: 7239 RVA: 0x000C53BA File Offset: 0x000C35BA
	public virtual void Refresh(BuildingBlock parent)
	{
		this.DestroyConditionalModels(parent);
		this.CreateConditionalModels(parent);
	}

	// Token: 0x06001C48 RID: 7240 RVA: 0x000C53CA File Offset: 0x000C35CA
	public void Destroy(BuildingBlock parent)
	{
		this.DestroyConditionalModels(parent);
		parent.gameManager.Retire(base.gameObject);
	}

	// Token: 0x06001C49 RID: 7241 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual uint GetStartingDetailColour(uint playerColourIndex)
	{
		return 0U;
	}
}
