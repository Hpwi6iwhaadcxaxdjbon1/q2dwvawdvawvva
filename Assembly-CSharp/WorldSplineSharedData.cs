using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000956 RID: 2390
[CreateAssetMenu(menuName = "Rust/Vehicles/WorldSpline Shared Data", fileName = "WorldSpline Prefab Shared Data")]
public class WorldSplineSharedData : ScriptableObject
{
	// Token: 0x04003398 RID: 13208
	[SerializeField]
	private List<WorldSplineData> dataList;

	// Token: 0x04003399 RID: 13209
	public static WorldSplineSharedData instance;

	// Token: 0x0400339A RID: 13210
	private static string[] worldSplineFolders = new string[]
	{
		"Assets/Content/Structures",
		"Assets/bundled/Prefabs/autospawn"
	};

	// Token: 0x06003987 RID: 14727 RVA: 0x00155DB9 File Offset: 0x00153FB9
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		WorldSplineSharedData.instance = Resources.Load<WorldSplineSharedData>("WorldSpline Prefab Shared Data");
	}

	// Token: 0x06003988 RID: 14728 RVA: 0x00155DCC File Offset: 0x00153FCC
	public static bool TryGetDataFor(WorldSpline worldSpline, out WorldSplineData data)
	{
		if (WorldSplineSharedData.instance == null)
		{
			Debug.LogError("No instance of WorldSplineSharedData found.");
			data = null;
			return false;
		}
		if (worldSpline.dataIndex < 0 || worldSpline.dataIndex >= WorldSplineSharedData.instance.dataList.Count)
		{
			data = null;
			return false;
		}
		data = WorldSplineSharedData.instance.dataList[worldSpline.dataIndex];
		return true;
	}
}
