using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200093F RID: 2367
public static class HierarchyUtil
{
	// Token: 0x0400334C RID: 13132
	public static Dictionary<string, GameObject> rootDict = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x060038B3 RID: 14515 RVA: 0x00152488 File Offset: 0x00150688
	public static GameObject GetRoot(string strName, bool groupActive = true, bool persistant = false)
	{
		GameObject gameObject;
		if (HierarchyUtil.rootDict.TryGetValue(strName, out gameObject))
		{
			if (gameObject != null)
			{
				return gameObject;
			}
			HierarchyUtil.rootDict.Remove(strName);
		}
		gameObject = new GameObject(strName);
		gameObject.SetActive(groupActive);
		HierarchyUtil.rootDict.Add(strName, gameObject);
		if (persistant)
		{
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		return gameObject;
	}
}
