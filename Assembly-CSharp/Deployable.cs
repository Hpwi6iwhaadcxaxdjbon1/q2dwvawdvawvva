using System;
using UnityEngine;

// Token: 0x020004FC RID: 1276
public class Deployable : PrefabAttribute
{
	// Token: 0x04002111 RID: 8465
	public Mesh guideMesh;

	// Token: 0x04002112 RID: 8466
	public Vector3 guideMeshScale = Vector3.one;

	// Token: 0x04002113 RID: 8467
	public bool guideLights = true;

	// Token: 0x04002114 RID: 8468
	public bool wantsInstanceData;

	// Token: 0x04002115 RID: 8469
	public bool copyInventoryFromItem;

	// Token: 0x04002116 RID: 8470
	public bool setSocketParent;

	// Token: 0x04002117 RID: 8471
	public bool toSlot;

	// Token: 0x04002118 RID: 8472
	public BaseEntity.Slot slot;

	// Token: 0x04002119 RID: 8473
	public GameObjectRef placeEffect;

	// Token: 0x0400211A RID: 8474
	[NonSerialized]
	public Bounds bounds;

	// Token: 0x06002918 RID: 10520 RVA: 0x000FCAD6 File Offset: 0x000FACD6
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
	}

	// Token: 0x06002919 RID: 10521 RVA: 0x000FCAF6 File Offset: 0x000FACF6
	protected override Type GetIndexedType()
	{
		return typeof(Deployable);
	}
}
