using System;
using UnityEngine;

// Token: 0x02000251 RID: 593
public class ConditionalModel : PrefabAttribute
{
	// Token: 0x040014E1 RID: 5345
	public GameObjectRef prefab;

	// Token: 0x040014E2 RID: 5346
	public bool onClient = true;

	// Token: 0x040014E3 RID: 5347
	public bool onServer = true;

	// Token: 0x040014E4 RID: 5348
	[NonSerialized]
	public ModelConditionTest[] conditions;

	// Token: 0x06001C2C RID: 7212 RVA: 0x000C4783 File Offset: 0x000C2983
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.conditions = base.GetComponentsInChildren<ModelConditionTest>(true);
	}

	// Token: 0x06001C2D RID: 7213 RVA: 0x000C47A0 File Offset: 0x000C29A0
	public bool RunTests(BaseEntity parent)
	{
		for (int i = 0; i < this.conditions.Length; i++)
		{
			if (!this.conditions[i].DoTest(parent))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001C2E RID: 7214 RVA: 0x000C47D4 File Offset: 0x000C29D4
	public GameObject InstantiateSkin(BaseEntity parent)
	{
		if (!this.onServer && this.isServer)
		{
			return null;
		}
		GameObject gameObject = this.gameManager.CreatePrefab(this.prefab.resourcePath, parent.transform, false);
		if (gameObject)
		{
			gameObject.transform.localPosition = this.worldPosition;
			gameObject.transform.localRotation = this.worldRotation;
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	// Token: 0x06001C2F RID: 7215 RVA: 0x000C4842 File Offset: 0x000C2A42
	protected override Type GetIndexedType()
	{
		return typeof(ConditionalModel);
	}
}
