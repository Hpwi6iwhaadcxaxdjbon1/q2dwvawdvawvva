using System;
using UnityEngine;

// Token: 0x020006A6 RID: 1702
public class TerrainPhysics : TerrainExtension
{
	// Token: 0x040027C6 RID: 10182
	private TerrainSplatMap splat;

	// Token: 0x040027C7 RID: 10183
	private PhysicMaterial[] materials;

	// Token: 0x0600312C RID: 12588 RVA: 0x001264F1 File Offset: 0x001246F1
	public override void Setup()
	{
		this.splat = this.terrain.GetComponent<TerrainSplatMap>();
		this.materials = this.config.GetPhysicMaterials();
	}

	// Token: 0x0600312D RID: 12589 RVA: 0x00126515 File Offset: 0x00124715
	public PhysicMaterial GetMaterial(Vector3 worldPos)
	{
		return this.materials[this.splat.GetSplatMaxIndex(worldPos, -1)];
	}
}
