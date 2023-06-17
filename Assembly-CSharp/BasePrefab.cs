using System;
using UnityEngine;

// Token: 0x020008F2 RID: 2290
public class BasePrefab : BaseMonoBehaviour, IPrefabPreProcess
{
	// Token: 0x040032B7 RID: 12983
	[HideInInspector]
	public uint prefabID;

	// Token: 0x040032B8 RID: 12984
	[HideInInspector]
	public bool isClient;

	// Token: 0x1700046E RID: 1134
	// (get) Token: 0x060037C0 RID: 14272 RVA: 0x0014DFD9 File Offset: 0x0014C1D9
	public bool isServer
	{
		get
		{
			return !this.isClient;
		}
	}

	// Token: 0x060037C1 RID: 14273 RVA: 0x0014DFE4 File Offset: 0x0014C1E4
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.prefabID = StringPool.Get(name);
		this.isClient = clientside;
	}
}
