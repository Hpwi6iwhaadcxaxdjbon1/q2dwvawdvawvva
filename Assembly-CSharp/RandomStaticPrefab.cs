using System;
using UnityEngine;

// Token: 0x02000679 RID: 1657
public class RandomStaticPrefab : MonoBehaviour
{
	// Token: 0x04002719 RID: 10009
	public uint Seed;

	// Token: 0x0400271A RID: 10010
	public float Probability = 0.5f;

	// Token: 0x0400271B RID: 10011
	public string ResourceFolder = string.Empty;

	// Token: 0x06002FA4 RID: 12196 RVA: 0x0011E6F0 File Offset: 0x0011C8F0
	protected void Start()
	{
		uint num = base.transform.position.Seed(World.Seed + this.Seed);
		if (SeedRandom.Value(ref num) > this.Probability)
		{
			GameManager.Destroy(this, 0f);
			return;
		}
		Prefab.LoadRandom("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, ref num, null, null, true).Spawn(base.transform, true);
		GameManager.Destroy(this, 0f);
	}
}
