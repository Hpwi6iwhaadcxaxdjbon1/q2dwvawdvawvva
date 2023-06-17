using System;
using UnityEngine;

// Token: 0x020002D7 RID: 727
public class PlayerModelHairCap : MonoBehaviour
{
	// Token: 0x040016D6 RID: 5846
	[InspectorFlags]
	public HairCapMask hairCapMask;

	// Token: 0x06001DB5 RID: 7605 RVA: 0x000CB874 File Offset: 0x000C9A74
	public void SetupHairCap(SkinSetCollection skin, float hairNum, float meshNum, MaterialPropertyBlock block)
	{
		int index = skin.GetIndex(meshNum);
		SkinSet skinSet = skin.Skins[index];
		if (skinSet == null)
		{
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			if ((this.hairCapMask & (HairCapMask)(1 << i)) != (HairCapMask)0)
			{
				float typeNum;
				float seed;
				PlayerModelHair.GetRandomVariation(hairNum, i, index, out typeNum, out seed);
				HairType hairType = (HairType)i;
				HairSetCollection.HairSetEntry hairSetEntry = skinSet.HairCollection.Get(hairType, typeNum);
				if (!(hairSetEntry.HairSet == null))
				{
					HairDyeCollection hairDyeCollection = hairSetEntry.HairDyeCollection;
					if (!(hairDyeCollection == null))
					{
						HairDye hairDye = hairDyeCollection.Get(seed);
						if (hairDye != null)
						{
							hairDye.ApplyCap(hairDyeCollection, hairType, block);
						}
					}
				}
			}
		}
	}
}
