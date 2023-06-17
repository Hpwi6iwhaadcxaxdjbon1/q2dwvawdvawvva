using System;
using Facepunch.Extend;
using UnityEngine;

// Token: 0x02000695 RID: 1685
[RequireComponent(typeof(TerrainMeta))]
public abstract class TerrainExtension : MonoBehaviour
{
	// Token: 0x04002779 RID: 10105
	[NonSerialized]
	public bool isInitialized;

	// Token: 0x0400277A RID: 10106
	internal Terrain terrain;

	// Token: 0x0400277B RID: 10107
	internal TerrainConfig config;

	// Token: 0x06002FE9 RID: 12265 RVA: 0x0011FD4D File Offset: 0x0011DF4D
	public void Init(Terrain terrain, TerrainConfig config)
	{
		this.terrain = terrain;
		this.config = config;
	}

	// Token: 0x06002FEA RID: 12266 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void Setup()
	{
	}

	// Token: 0x06002FEB RID: 12267 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PostSetup()
	{
	}

	// Token: 0x06002FEC RID: 12268 RVA: 0x0011FD5D File Offset: 0x0011DF5D
	public void LogSize(object obj, ulong size)
	{
		Debug.Log(obj.GetType() + " allocated: " + size.FormatBytes(false));
	}
}
