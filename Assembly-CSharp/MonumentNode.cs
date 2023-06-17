using System;
using UnityEngine;

// Token: 0x020006B0 RID: 1712
public class MonumentNode : MonoBehaviour
{
	// Token: 0x040027E0 RID: 10208
	public string ResourceFolder = string.Empty;

	// Token: 0x06003157 RID: 12631 RVA: 0x00127384 File Offset: 0x00125584
	protected void Awake()
	{
		if (!(SingletonComponent<WorldSetup>.Instance == null))
		{
			if (SingletonComponent<WorldSetup>.Instance.MonumentNodes == null)
			{
				Debug.LogError("WorldSetup.Instance.MonumentNodes is null.", this);
				return;
			}
			SingletonComponent<WorldSetup>.Instance.MonumentNodes.Add(this);
		}
	}

	// Token: 0x06003158 RID: 12632 RVA: 0x001273BC File Offset: 0x001255BC
	public void Process(ref uint seed)
	{
		if (World.Networked)
		{
			World.Spawn("Monument", "assets/bundled/prefabs/autospawn/" + this.ResourceFolder + "/");
			return;
		}
		Prefab<MonumentInfo>[] array = Prefab.Load<MonumentInfo>("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, null, null, true);
		if (array == null || array.Length == 0)
		{
			return;
		}
		Prefab<MonumentInfo> random = array.GetRandom(ref seed);
		float height = TerrainMeta.HeightMap.GetHeight(base.transform.position);
		Vector3 position = new Vector3(base.transform.position.x, height, base.transform.position.z);
		Quaternion localRotation = random.Object.transform.localRotation;
		Vector3 localScale = random.Object.transform.localScale;
		random.ApplyDecorComponents(ref position, ref localRotation, ref localScale);
		World.AddPrefab("Monument", random, position, localRotation, localScale);
	}
}
