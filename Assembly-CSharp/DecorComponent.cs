using System;
using UnityEngine;

// Token: 0x02000657 RID: 1623
public abstract class DecorComponent : PrefabAttribute
{
	// Token: 0x040026AD RID: 9901
	internal bool isRoot;

	// Token: 0x06002F59 RID: 12121
	public abstract void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale);

	// Token: 0x06002F5A RID: 12122 RVA: 0x0011D4D2 File Offset: 0x0011B6D2
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.isRoot = (rootObj == base.gameObject);
	}

	// Token: 0x06002F5B RID: 12123 RVA: 0x0011D4F3 File Offset: 0x0011B6F3
	protected override Type GetIndexedType()
	{
		return typeof(DecorComponent);
	}
}
