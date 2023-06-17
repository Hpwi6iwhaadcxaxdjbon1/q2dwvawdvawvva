using System;
using UnityEngine;

// Token: 0x020006EA RID: 1770
public class ApplyTerrainModifiers : MonoBehaviour
{
	// Token: 0x0600320F RID: 12815 RVA: 0x00134BF8 File Offset: 0x00132DF8
	protected void Awake()
	{
		BaseEntity component = base.GetComponent<BaseEntity>();
		TerrainModifier[] modifiers = null;
		if (component.isServer)
		{
			modifiers = PrefabAttribute.server.FindAll<TerrainModifier>(component.prefabID);
		}
		base.transform.ApplyTerrainModifiers(modifiers);
		GameManager.Destroy(this, 0f);
	}
}
