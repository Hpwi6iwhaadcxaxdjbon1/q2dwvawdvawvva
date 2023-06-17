using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000510 RID: 1296
[CreateAssetMenu(menuName = "Rust/Game Mode Manifest")]
public class GameModeManifest : ScriptableObject
{
	// Token: 0x04002168 RID: 8552
	public static GameModeManifest instance;

	// Token: 0x04002169 RID: 8553
	public List<GameObjectRef> gameModePrefabs;

	// Token: 0x06002963 RID: 10595 RVA: 0x000FDEC3 File Offset: 0x000FC0C3
	public static GameModeManifest Get()
	{
		if (GameModeManifest.instance == null)
		{
			GameModeManifest.instance = Resources.Load<GameModeManifest>("GameModeManifest");
		}
		return GameModeManifest.instance;
	}
}
