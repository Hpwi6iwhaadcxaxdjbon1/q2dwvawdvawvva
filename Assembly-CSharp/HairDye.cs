using System;
using UnityEngine;

// Token: 0x0200074C RID: 1868
[Serializable]
public class HairDye
{
	// Token: 0x04002A5F RID: 10847
	[ColorUsage(false, true)]
	public Color capBaseColor;

	// Token: 0x04002A60 RID: 10848
	public Material sourceMaterial;

	// Token: 0x04002A61 RID: 10849
	[InspectorFlags]
	public HairDye.CopyPropertyMask copyProperties;

	// Token: 0x04002A62 RID: 10850
	private static MaterialPropertyDesc[] transferableProps = new MaterialPropertyDesc[]
	{
		new MaterialPropertyDesc("_DyeColor", typeof(Color)),
		new MaterialPropertyDesc("_RootColor", typeof(Color)),
		new MaterialPropertyDesc("_TipColor", typeof(Color)),
		new MaterialPropertyDesc("_Brightness", typeof(float)),
		new MaterialPropertyDesc("_DyeRoughness", typeof(float)),
		new MaterialPropertyDesc("_DyeScatter", typeof(float)),
		new MaterialPropertyDesc("_HairSpecular", typeof(float)),
		new MaterialPropertyDesc("_HairRoughness", typeof(float))
	};

	// Token: 0x04002A63 RID: 10851
	private static int _HairBaseColorUV1 = Shader.PropertyToID("_HairBaseColorUV1");

	// Token: 0x04002A64 RID: 10852
	private static int _HairBaseColorUV2 = Shader.PropertyToID("_HairBaseColorUV2");

	// Token: 0x04002A65 RID: 10853
	private static int _HairPackedMapUV1 = Shader.PropertyToID("_HairPackedMapUV1");

	// Token: 0x04002A66 RID: 10854
	private static int _HairPackedMapUV2 = Shader.PropertyToID("_HairPackedMapUV2");

	// Token: 0x0600345B RID: 13403 RVA: 0x0014450C File Offset: 0x0014270C
	public void Apply(HairDyeCollection collection, MaterialPropertyBlock block)
	{
		if (this.sourceMaterial != null)
		{
			for (int i = 0; i < 8; i++)
			{
				if ((this.copyProperties & (HairDye.CopyPropertyMask)(1 << i)) != (HairDye.CopyPropertyMask)0)
				{
					MaterialPropertyDesc materialPropertyDesc = HairDye.transferableProps[i];
					if (this.sourceMaterial.HasProperty(materialPropertyDesc.nameID))
					{
						if (materialPropertyDesc.type == typeof(Color))
						{
							block.SetColor(materialPropertyDesc.nameID, this.sourceMaterial.GetColor(materialPropertyDesc.nameID));
						}
						else if (materialPropertyDesc.type == typeof(float))
						{
							block.SetFloat(materialPropertyDesc.nameID, this.sourceMaterial.GetFloat(materialPropertyDesc.nameID));
						}
					}
				}
			}
		}
	}

	// Token: 0x0600345C RID: 13404 RVA: 0x001445D8 File Offset: 0x001427D8
	public void ApplyCap(HairDyeCollection collection, HairType type, MaterialPropertyBlock block)
	{
		if (collection.applyCap)
		{
			if (type == HairType.Head || type == HairType.Armpit || type == HairType.Pubic)
			{
				block.SetColor(HairDye._HairBaseColorUV1, this.capBaseColor.gamma);
				block.SetTexture(HairDye._HairPackedMapUV1, (collection.capMask != null) ? collection.capMask : Texture2D.blackTexture);
				return;
			}
			if (type == HairType.Facial)
			{
				block.SetColor(HairDye._HairBaseColorUV2, this.capBaseColor.gamma);
				block.SetTexture(HairDye._HairPackedMapUV2, (collection.capMask != null) ? collection.capMask : Texture2D.blackTexture);
			}
		}
	}

	// Token: 0x02000E5E RID: 3678
	public enum CopyProperty
	{
		// Token: 0x04004B37 RID: 19255
		DyeColor,
		// Token: 0x04004B38 RID: 19256
		RootColor,
		// Token: 0x04004B39 RID: 19257
		TipColor,
		// Token: 0x04004B3A RID: 19258
		Brightness,
		// Token: 0x04004B3B RID: 19259
		DyeRoughness,
		// Token: 0x04004B3C RID: 19260
		DyeScatter,
		// Token: 0x04004B3D RID: 19261
		Specular,
		// Token: 0x04004B3E RID: 19262
		Roughness,
		// Token: 0x04004B3F RID: 19263
		Count
	}

	// Token: 0x02000E5F RID: 3679
	[Flags]
	public enum CopyPropertyMask
	{
		// Token: 0x04004B41 RID: 19265
		DyeColor = 1,
		// Token: 0x04004B42 RID: 19266
		RootColor = 2,
		// Token: 0x04004B43 RID: 19267
		TipColor = 4,
		// Token: 0x04004B44 RID: 19268
		Brightness = 8,
		// Token: 0x04004B45 RID: 19269
		DyeRoughness = 16,
		// Token: 0x04004B46 RID: 19270
		DyeScatter = 32,
		// Token: 0x04004B47 RID: 19271
		Specular = 64,
		// Token: 0x04004B48 RID: 19272
		Roughness = 128
	}
}
