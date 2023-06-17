using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200018C RID: 396
public class NeonMeshPaintableSource : MeshPaintableSource
{
	// Token: 0x040010A5 RID: 4261
	public NeonSign neonSign;

	// Token: 0x040010A6 RID: 4262
	public float editorEmissionScale = 2f;

	// Token: 0x040010A7 RID: 4263
	public AnimationCurve lightingCurve;

	// Token: 0x040010A8 RID: 4264
	[NonSerialized]
	public Color topLeft;

	// Token: 0x040010A9 RID: 4265
	[NonSerialized]
	public Color topRight;

	// Token: 0x040010AA RID: 4266
	[NonSerialized]
	public Color bottomLeft;

	// Token: 0x040010AB RID: 4267
	[NonSerialized]
	public Color bottomRight;

	// Token: 0x060017D0 RID: 6096 RVA: 0x000B378C File Offset: 0x000B198C
	public override void UpdateMaterials(MaterialPropertyBlock block, Texture2D textureOverride = null, bool forEditing = false, bool isSelected = false)
	{
		base.UpdateMaterials(block, textureOverride, forEditing, false);
		if (forEditing)
		{
			block.SetFloat("_EmissionScale", this.editorEmissionScale);
			block.SetFloat("_Power", (float)(isSelected ? 1 : 0));
			if (!isSelected)
			{
				block.SetColor("_TubeInner", Color.clear);
				block.SetColor("_TubeOuter", Color.clear);
				return;
			}
		}
		else if (this.neonSign != null)
		{
			block.SetFloat("_Power", (float)((isSelected && this.neonSign.HasFlag(BaseEntity.Flags.Reserved8)) ? 1 : 0));
		}
	}

	// Token: 0x060017D1 RID: 6097 RVA: 0x000B3824 File Offset: 0x000B1A24
	public override Color32[] UpdateFrom(Texture2D input)
	{
		NeonMeshPaintableSource.<>c__DisplayClass8_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		base.Init();
		CS$<>8__locals1.pixels = input.GetPixels32();
		this.texture.SetPixels32(CS$<>8__locals1.pixels);
		this.texture.Apply(true, false);
		CS$<>8__locals1.width = input.width;
		int height = input.height;
		int num = CS$<>8__locals1.width / 2;
		int num2 = height / 2;
		this.topLeft = this.<UpdateFrom>g__GetColorForRegion|8_0(0, num2, num, num2, ref CS$<>8__locals1);
		this.topRight = this.<UpdateFrom>g__GetColorForRegion|8_0(num, num2, num, num2, ref CS$<>8__locals1);
		this.bottomLeft = this.<UpdateFrom>g__GetColorForRegion|8_0(0, 0, num, num2, ref CS$<>8__locals1);
		this.bottomRight = this.<UpdateFrom>g__GetColorForRegion|8_0(num, 0, num, num2, ref CS$<>8__locals1);
		return CS$<>8__locals1.pixels;
	}

	// Token: 0x060017D3 RID: 6099 RVA: 0x000B38EC File Offset: 0x000B1AEC
	[CompilerGenerated]
	private Color <UpdateFrom>g__GetColorForRegion|8_0(int x, int y, int regionWidth, int regionHeight, ref NeonMeshPaintableSource.<>c__DisplayClass8_0 A_5)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		int num4 = y + regionHeight;
		for (int i = y; i < num4; i++)
		{
			int num5 = i * A_5.width + x;
			int num6 = num5 + regionWidth;
			for (int j = num5; j < num6; j++)
			{
				Color32 color = A_5.pixels[j];
				float num7 = (float)color.a / 255f;
				num += (float)color.r * num7;
				num2 += (float)color.g * num7;
				num3 += (float)color.b * num7;
			}
		}
		int num8 = regionWidth * regionHeight * 255;
		return new Color(this.lightingCurve.Evaluate(num / (float)num8), this.lightingCurve.Evaluate(num2 / (float)num8), this.lightingCurve.Evaluate(num3 / (float)num8), 1f);
	}
}
