using System;
using UnityEngine;

// Token: 0x02000962 RID: 2402
[Serializable]
public struct LayerSelect
{
	// Token: 0x040033C3 RID: 13251
	[SerializeField]
	private int layer;

	// Token: 0x060039B3 RID: 14771 RVA: 0x00156BE6 File Offset: 0x00154DE6
	public LayerSelect(int layer)
	{
		this.layer = layer;
	}

	// Token: 0x060039B4 RID: 14772 RVA: 0x00156BEF File Offset: 0x00154DEF
	public static implicit operator int(LayerSelect layer)
	{
		return layer.layer;
	}

	// Token: 0x060039B5 RID: 14773 RVA: 0x00156BF7 File Offset: 0x00154DF7
	public static implicit operator LayerSelect(int layer)
	{
		return new LayerSelect(layer);
	}

	// Token: 0x17000497 RID: 1175
	// (get) Token: 0x060039B6 RID: 14774 RVA: 0x00156BFF File Offset: 0x00154DFF
	public int Mask
	{
		get
		{
			return 1 << this.layer;
		}
	}

	// Token: 0x17000498 RID: 1176
	// (get) Token: 0x060039B7 RID: 14775 RVA: 0x00156C0C File Offset: 0x00154E0C
	public string Name
	{
		get
		{
			return LayerMask.LayerToName(this.layer);
		}
	}
}
