using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200074E RID: 1870
[CreateAssetMenu(menuName = "Rust/Hair Set")]
public class HairSet : ScriptableObject
{
	// Token: 0x04002A6A RID: 10858
	public HairSet.MeshReplace[] MeshReplacements;

	// Token: 0x06003461 RID: 13409 RVA: 0x001447D8 File Offset: 0x001429D8
	public void Process(PlayerModelHair playerModelHair, HairDyeCollection dyeCollection, HairDye dye, MaterialPropertyBlock block)
	{
		List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
		playerModelHair.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in list)
		{
			if (!(skinnedMeshRenderer.sharedMesh == null) && !(skinnedMeshRenderer.sharedMaterial == null))
			{
				string name = skinnedMeshRenderer.sharedMesh.name;
				string name2 = skinnedMeshRenderer.sharedMaterial.name;
				if (!skinnedMeshRenderer.gameObject.activeSelf)
				{
					skinnedMeshRenderer.gameObject.SetActive(true);
				}
				for (int i = 0; i < this.MeshReplacements.Length; i++)
				{
					this.MeshReplacements[i].Test(name);
				}
				if (dye != null && skinnedMeshRenderer.gameObject.activeSelf)
				{
					dye.Apply(dyeCollection, block);
				}
			}
		}
		Pool.FreeList<SkinnedMeshRenderer>(ref list);
	}

	// Token: 0x06003462 RID: 13410 RVA: 0x000063A5 File Offset: 0x000045A5
	public void ProcessMorphs(GameObject obj, int blendShapeIndex = -1)
	{
	}

	// Token: 0x02000E60 RID: 3680
	[Serializable]
	public class MeshReplace
	{
		// Token: 0x04004B49 RID: 19273
		[HideInInspector]
		public string FindName;

		// Token: 0x04004B4A RID: 19274
		public Mesh Find;

		// Token: 0x04004B4B RID: 19275
		public Mesh[] ReplaceShapes;

		// Token: 0x06005290 RID: 21136 RVA: 0x001B0832 File Offset: 0x001AEA32
		public bool Test(string materialName)
		{
			return this.FindName == materialName;
		}
	}
}
