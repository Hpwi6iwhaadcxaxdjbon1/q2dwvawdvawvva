using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200057A RID: 1402
public class StripRig : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x040022EE RID: 8942
	public Transform root;

	// Token: 0x040022EF RID: 8943
	public bool fromClient;

	// Token: 0x040022F0 RID: 8944
	public bool fromServer;

	// Token: 0x06002B04 RID: 11012 RVA: 0x00105740 File Offset: 0x00103940
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (this.root && ((serverside && this.fromServer) || (clientside && this.fromClient)))
		{
			SkinnedMeshRenderer component = base.GetComponent<SkinnedMeshRenderer>();
			this.Strip(preProcess, component);
		}
		preProcess.RemoveComponent(this);
	}

	// Token: 0x06002B05 RID: 11013 RVA: 0x00105788 File Offset: 0x00103988
	public void Strip(IPrefabProcessor preProcess, SkinnedMeshRenderer skinnedMeshRenderer)
	{
		List<Transform> list = Pool.GetList<Transform>();
		this.root.GetComponentsInChildren<Transform>(list);
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (preProcess != null)
			{
				preProcess.NominateForDeletion(list[i].gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(list[i].gameObject);
			}
		}
		Pool.FreeList<Transform>(ref list);
	}
}
