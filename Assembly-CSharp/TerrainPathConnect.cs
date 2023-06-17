using System;
using UnityEngine;

// Token: 0x0200067F RID: 1663
public class TerrainPathConnect : MonoBehaviour
{
	// Token: 0x0400272C RID: 10028
	public InfrastructureType Type;

	// Token: 0x06002FB0 RID: 12208 RVA: 0x0011EAD4 File Offset: 0x0011CCD4
	public PathFinder.Point GetPathFinderPoint(int res, Vector3 worldPos)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return new PathFinder.Point
		{
			x = Mathf.Clamp((int)(num * (float)res), 0, res - 1),
			y = Mathf.Clamp((int)(num2 * (float)res), 0, res - 1)
		};
	}

	// Token: 0x06002FB1 RID: 12209 RVA: 0x0011EB2C File Offset: 0x0011CD2C
	public PathFinder.Point GetPathFinderPoint(int res)
	{
		return this.GetPathFinderPoint(res, base.transform.position);
	}
}
