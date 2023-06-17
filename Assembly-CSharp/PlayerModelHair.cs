using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020002D5 RID: 725
public class PlayerModelHair : MonoBehaviour
{
	// Token: 0x040016CE RID: 5838
	public HairType type;

	// Token: 0x040016CF RID: 5839
	private Dictionary<Renderer, PlayerModelHair.RendererMaterials> materials;

	// Token: 0x17000273 RID: 627
	// (get) Token: 0x06001DAE RID: 7598 RVA: 0x000CB6A4 File Offset: 0x000C98A4
	public Dictionary<Renderer, PlayerModelHair.RendererMaterials> Materials
	{
		get
		{
			return this.materials;
		}
	}

	// Token: 0x06001DAF RID: 7599 RVA: 0x000CB6AC File Offset: 0x000C98AC
	private void CacheOriginalMaterials()
	{
		if (this.materials != null)
		{
			return;
		}
		List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
		base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
		this.materials = new Dictionary<Renderer, PlayerModelHair.RendererMaterials>();
		this.materials.Clear();
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in list)
		{
			this.materials.Add(skinnedMeshRenderer, new PlayerModelHair.RendererMaterials(skinnedMeshRenderer));
		}
		Pool.FreeList<SkinnedMeshRenderer>(ref list);
	}

	// Token: 0x06001DB0 RID: 7600 RVA: 0x000CB740 File Offset: 0x000C9940
	private void Setup(HairType type, HairSetCollection hair, int meshIndex, float typeNum, float dyeNum, MaterialPropertyBlock block)
	{
		this.CacheOriginalMaterials();
		HairSetCollection.HairSetEntry hairSetEntry = hair.Get(type, typeNum);
		if (hairSetEntry.HairSet == null)
		{
			Debug.LogWarning("Hair.Get returned a NULL hair");
			return;
		}
		int blendShapeIndex = -1;
		if (type == HairType.Facial || type == HairType.Eyebrow)
		{
			blendShapeIndex = meshIndex;
		}
		HairDye dye = null;
		HairDyeCollection hairDyeCollection = hairSetEntry.HairDyeCollection;
		if (hairDyeCollection != null)
		{
			dye = hairDyeCollection.Get(dyeNum);
		}
		hairSetEntry.HairSet.Process(this, hairDyeCollection, dye, block);
		hairSetEntry.HairSet.ProcessMorphs(base.gameObject, blendShapeIndex);
	}

	// Token: 0x06001DB1 RID: 7601 RVA: 0x000CB7C0 File Offset: 0x000C99C0
	public void Setup(SkinSetCollection skin, float hairNum, float meshNum, MaterialPropertyBlock block)
	{
		int index = skin.GetIndex(meshNum);
		SkinSet skinSet = skin.Skins[index];
		if (skinSet == null)
		{
			Debug.LogError("Skin.Get returned a NULL skin");
			return;
		}
		int typeIndex = (int)this.type;
		float typeNum;
		float dyeNum;
		PlayerModelHair.GetRandomVariation(hairNum, typeIndex, index, out typeNum, out dyeNum);
		this.Setup(this.type, skinSet.HairCollection, index, typeNum, dyeNum, block);
	}

	// Token: 0x06001DB2 RID: 7602 RVA: 0x000CB81D File Offset: 0x000C9A1D
	public static void GetRandomVariation(float hairNum, int typeIndex, int meshIndex, out float typeNum, out float dyeNum)
	{
		int num = Mathf.FloorToInt(hairNum * 100000f);
		typeNum = PlayerModelHair.GetRandomHairType(hairNum, typeIndex);
		UnityEngine.Random.InitState(num + meshIndex);
		dyeNum = UnityEngine.Random.Range(0f, 1f);
	}

	// Token: 0x06001DB3 RID: 7603 RVA: 0x000CB84D File Offset: 0x000C9A4D
	public static float GetRandomHairType(float hairNum, int typeIndex)
	{
		UnityEngine.Random.InitState(Mathf.FloorToInt(hairNum * 100000f) + typeIndex);
		return UnityEngine.Random.Range(0f, 1f);
	}

	// Token: 0x02000C96 RID: 3222
	public struct RendererMaterials
	{
		// Token: 0x040043F9 RID: 17401
		public string[] names;

		// Token: 0x040043FA RID: 17402
		public Material[] original;

		// Token: 0x040043FB RID: 17403
		public Material[] replacement;

		// Token: 0x06004F24 RID: 20260 RVA: 0x001A5BB0 File Offset: 0x001A3DB0
		public RendererMaterials(Renderer r)
		{
			this.original = r.sharedMaterials;
			this.replacement = (this.original.Clone() as Material[]);
			this.names = new string[this.original.Length];
			for (int i = 0; i < this.original.Length; i++)
			{
				this.names[i] = this.original[i].name;
			}
		}
	}
}
