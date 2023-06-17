using System;
using UnityEngine;

// Token: 0x02000750 RID: 1872
[CreateAssetMenu(menuName = "Rust/Hair Set Collection")]
public class HairSetCollection : ScriptableObject
{
	// Token: 0x04002A72 RID: 10866
	public HairSetCollection.HairSetEntry[] Head;

	// Token: 0x04002A73 RID: 10867
	public HairSetCollection.HairSetEntry[] Eyebrow;

	// Token: 0x04002A74 RID: 10868
	public HairSetCollection.HairSetEntry[] Facial;

	// Token: 0x04002A75 RID: 10869
	public HairSetCollection.HairSetEntry[] Armpit;

	// Token: 0x04002A76 RID: 10870
	public HairSetCollection.HairSetEntry[] Pubic;

	// Token: 0x06003464 RID: 13412 RVA: 0x001448CC File Offset: 0x00142ACC
	public HairSetCollection.HairSetEntry[] GetListByType(HairType hairType)
	{
		switch (hairType)
		{
		case HairType.Head:
			return this.Head;
		case HairType.Eyebrow:
			return this.Eyebrow;
		case HairType.Facial:
			return this.Facial;
		case HairType.Armpit:
			return this.Armpit;
		case HairType.Pubic:
			return this.Pubic;
		default:
			return null;
		}
	}

	// Token: 0x06003465 RID: 13413 RVA: 0x00144919 File Offset: 0x00142B19
	public int GetIndex(HairSetCollection.HairSetEntry[] list, float typeNum)
	{
		return Mathf.Clamp(Mathf.FloorToInt(typeNum * (float)list.Length), 0, list.Length - 1);
	}

	// Token: 0x06003466 RID: 13414 RVA: 0x00144934 File Offset: 0x00142B34
	public int GetIndex(HairType hairType, float typeNum)
	{
		HairSetCollection.HairSetEntry[] listByType = this.GetListByType(hairType);
		return this.GetIndex(listByType, typeNum);
	}

	// Token: 0x06003467 RID: 13415 RVA: 0x00144954 File Offset: 0x00142B54
	public HairSetCollection.HairSetEntry Get(HairType hairType, float typeNum)
	{
		HairSetCollection.HairSetEntry[] listByType = this.GetListByType(hairType);
		return listByType[this.GetIndex(listByType, typeNum)];
	}

	// Token: 0x02000E61 RID: 3681
	[Serializable]
	public struct HairSetEntry
	{
		// Token: 0x04004B4C RID: 19276
		public HairSet HairSet;

		// Token: 0x04004B4D RID: 19277
		public GameObjectRef HairPrefab;

		// Token: 0x04004B4E RID: 19278
		public HairDyeCollection HairDyeCollection;
	}
}
