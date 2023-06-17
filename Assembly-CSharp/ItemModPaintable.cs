using System;
using UnityEngine;

// Token: 0x020005F1 RID: 1521
[RequireComponent(typeof(ItemModWearable))]
public class ItemModPaintable : ItemModAssociatedEntity<PaintedItemStorageEntity>
{
	// Token: 0x040024F7 RID: 9463
	public GameObjectRef ChangeSignTextDialog;

	// Token: 0x040024F8 RID: 9464
	public MeshPaintableSource[] PaintableSources;

	// Token: 0x170003C2 RID: 962
	// (get) Token: 0x06002D63 RID: 11619 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool AllowNullParenting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170003C3 RID: 963
	// (get) Token: 0x06002D64 RID: 11620 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool OwnedByParentPlayer
	{
		get
		{
			return true;
		}
	}
}
