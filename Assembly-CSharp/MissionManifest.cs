using System;
using UnityEngine;

// Token: 0x02000610 RID: 1552
[CreateAssetMenu(menuName = "Rust/MissionManifest")]
public class MissionManifest : ScriptableObject
{
	// Token: 0x0400257C RID: 9596
	public ScriptableObjectRef[] missionList;

	// Token: 0x0400257D RID: 9597
	public WorldPositionGenerator[] positionGenerators;

	// Token: 0x0400257E RID: 9598
	public static MissionManifest instance;

	// Token: 0x06002DE6 RID: 11750 RVA: 0x00113C10 File Offset: 0x00111E10
	public static MissionManifest Get()
	{
		if (MissionManifest.instance == null)
		{
			MissionManifest.instance = Resources.Load<MissionManifest>("MissionManifest");
			foreach (WorldPositionGenerator worldPositionGenerator in MissionManifest.instance.positionGenerators)
			{
				if (worldPositionGenerator != null)
				{
					worldPositionGenerator.PrecalculatePositions();
				}
			}
		}
		return MissionManifest.instance;
	}

	// Token: 0x06002DE7 RID: 11751 RVA: 0x00113C6C File Offset: 0x00111E6C
	public static BaseMission GetFromShortName(string shortname)
	{
		ScriptableObjectRef[] array = MissionManifest.Get().missionList;
		for (int i = 0; i < array.Length; i++)
		{
			BaseMission baseMission = array[i].Get() as BaseMission;
			if (baseMission.shortname == shortname)
			{
				return baseMission;
			}
		}
		return null;
	}

	// Token: 0x06002DE8 RID: 11752 RVA: 0x00113CB4 File Offset: 0x00111EB4
	public static BaseMission GetFromID(uint id)
	{
		MissionManifest missionManifest = MissionManifest.Get();
		if (missionManifest.missionList == null)
		{
			return null;
		}
		ScriptableObjectRef[] array = missionManifest.missionList;
		for (int i = 0; i < array.Length; i++)
		{
			BaseMission baseMission = array[i].Get() as BaseMission;
			if (baseMission.id == id)
			{
				return baseMission;
			}
		}
		return null;
	}
}
