using System;
using UnityEngine;

// Token: 0x0200075E RID: 1886
[CreateAssetMenu(menuName = "Rust/Skin Set Collection")]
public class SkinSetCollection : ScriptableObject
{
	// Token: 0x04002AC9 RID: 10953
	public SkinSet[] Skins;

	// Token: 0x0600349A RID: 13466 RVA: 0x00145AFA File Offset: 0x00143CFA
	public int GetIndex(float MeshNumber)
	{
		return Mathf.Clamp(Mathf.FloorToInt(MeshNumber * (float)this.Skins.Length), 0, this.Skins.Length - 1);
	}

	// Token: 0x0600349B RID: 13467 RVA: 0x00145B1C File Offset: 0x00143D1C
	public SkinSet Get(float MeshNumber)
	{
		return this.Skins[this.GetIndex(MeshNumber)];
	}
}
