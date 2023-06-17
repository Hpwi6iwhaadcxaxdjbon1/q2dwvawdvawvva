using System;
using UnityEngine;

// Token: 0x0200059D RID: 1437
public class ValidBounds : SingletonComponent<ValidBounds>
{
	// Token: 0x0400235D RID: 9053
	public Bounds worldBounds;

	// Token: 0x06002BB8 RID: 11192 RVA: 0x00108895 File Offset: 0x00106A95
	public static bool Test(Vector3 vPos)
	{
		return !SingletonComponent<ValidBounds>.Instance || SingletonComponent<ValidBounds>.Instance.IsInside(vPos);
	}

	// Token: 0x06002BB9 RID: 11193 RVA: 0x001088B0 File Offset: 0x00106AB0
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(this.worldBounds.center, this.worldBounds.size);
	}

	// Token: 0x06002BBA RID: 11194 RVA: 0x001088D8 File Offset: 0x00106AD8
	internal bool IsInside(Vector3 vPos)
	{
		if (vPos.IsNaNOrInfinity())
		{
			return false;
		}
		if (!this.worldBounds.Contains(vPos))
		{
			return false;
		}
		if (TerrainMeta.Terrain != null)
		{
			if (World.Procedural && vPos.y < TerrainMeta.Position.y)
			{
				return false;
			}
			if (TerrainMeta.OutOfMargin(vPos))
			{
				return false;
			}
		}
		return true;
	}
}
