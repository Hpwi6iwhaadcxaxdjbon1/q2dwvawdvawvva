using System;
using UnityEngine;

// Token: 0x0200067C RID: 1660
public class PowerlineNode : MonoBehaviour
{
	// Token: 0x04002720 RID: 10016
	public GameObjectRef WirePrefab;

	// Token: 0x04002721 RID: 10017
	public float MaxDistance = 50f;

	// Token: 0x06002FAA RID: 12202 RVA: 0x0011E7AD File Offset: 0x0011C9AD
	protected void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.AddWire(this);
		}
	}
}
