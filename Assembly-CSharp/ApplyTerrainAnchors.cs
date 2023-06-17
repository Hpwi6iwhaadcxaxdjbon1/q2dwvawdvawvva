using System;
using UnityEngine;

// Token: 0x02000687 RID: 1671
public class ApplyTerrainAnchors : MonoBehaviour
{
	// Token: 0x06002FC4 RID: 12228 RVA: 0x0011F22C File Offset: 0x0011D42C
	protected void Awake()
	{
		BaseEntity component = base.GetComponent<BaseEntity>();
		TerrainAnchor[] anchors = null;
		if (component.isServer)
		{
			anchors = PrefabAttribute.server.FindAll<TerrainAnchor>(component.prefabID);
		}
		base.transform.ApplyTerrainAnchors(anchors);
		GameManager.Destroy(this, 0f);
	}
}
