using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000504 RID: 1284
public class EnvironmentManager : SingletonComponent<EnvironmentManager>
{
	// Token: 0x0600292B RID: 10539 RVA: 0x000FCD1C File Offset: 0x000FAF1C
	public static EnvironmentType Get(OBB obb)
	{
		EnvironmentType environmentType = (EnvironmentType)0;
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		GamePhysics.OverlapOBB<EnvironmentVolume>(obb, list, 262144, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			environmentType |= list[i].Type;
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return environmentType;
	}

	// Token: 0x0600292C RID: 10540 RVA: 0x000FCD68 File Offset: 0x000FAF68
	public static EnvironmentType Get(Vector3 pos, ref List<EnvironmentVolume> list)
	{
		EnvironmentType environmentType = (EnvironmentType)0;
		GamePhysics.OverlapSphere<EnvironmentVolume>(pos, 0.01f, list, 262144, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			environmentType |= list[i].Type;
		}
		return environmentType;
	}

	// Token: 0x0600292D RID: 10541 RVA: 0x000FCDB0 File Offset: 0x000FAFB0
	public static EnvironmentType Get(Vector3 pos)
	{
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		EnvironmentType result = EnvironmentManager.Get(pos, ref list);
		Pool.FreeList<EnvironmentVolume>(ref list);
		return result;
	}

	// Token: 0x0600292E RID: 10542 RVA: 0x000FCDD2 File Offset: 0x000FAFD2
	public static bool Check(OBB obb, EnvironmentType type)
	{
		return (EnvironmentManager.Get(obb) & type) > (EnvironmentType)0;
	}

	// Token: 0x0600292F RID: 10543 RVA: 0x000FCDDF File Offset: 0x000FAFDF
	public static bool Check(Vector3 pos, EnvironmentType type)
	{
		return (EnvironmentManager.Get(pos) & type) > (EnvironmentType)0;
	}
}
