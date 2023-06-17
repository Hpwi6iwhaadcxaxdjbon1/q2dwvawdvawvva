using System;
using UnityEngine;

// Token: 0x02000913 RID: 2323
public class RigidbodyInfo : PrefabAttribute, IClientComponent
{
	// Token: 0x0400332C RID: 13100
	[NonSerialized]
	public float mass;

	// Token: 0x0400332D RID: 13101
	[NonSerialized]
	public float drag;

	// Token: 0x0400332E RID: 13102
	[NonSerialized]
	public float angularDrag;

	// Token: 0x06003824 RID: 14372 RVA: 0x0014F115 File Offset: 0x0014D315
	protected override Type GetIndexedType()
	{
		return typeof(RigidbodyInfo);
	}

	// Token: 0x06003825 RID: 14373 RVA: 0x0014F124 File Offset: 0x0014D324
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		Rigidbody component = rootObj.GetComponent<Rigidbody>();
		if (component == null)
		{
			Debug.LogError(base.GetType().Name + ": RigidbodyInfo couldn't find a rigidbody on " + name + "! If a RealmedRemove is removing it, make sure this script is above the RealmedRemove script so that this gets processed first.");
			return;
		}
		this.mass = component.mass;
		this.drag = component.drag;
		this.angularDrag = component.angularDrag;
	}
}
