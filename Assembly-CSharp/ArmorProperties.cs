using System;
using UnityEngine;

// Token: 0x02000558 RID: 1368
[CreateAssetMenu(menuName = "Rust/Armor Properties")]
public class ArmorProperties : ScriptableObject
{
	// Token: 0x0400225C RID: 8796
	[InspectorFlags]
	public HitArea area;

	// Token: 0x06002A38 RID: 10808 RVA: 0x00101571 File Offset: 0x000FF771
	public bool Contains(HitArea hitArea)
	{
		return (this.area & hitArea) > (HitArea)0;
	}
}
