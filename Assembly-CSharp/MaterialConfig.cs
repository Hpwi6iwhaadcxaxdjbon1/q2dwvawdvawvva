using System;
using UnityEngine;

// Token: 0x02000903 RID: 2307
[CreateAssetMenu(menuName = "Rust/Material Config")]
public class MaterialConfig : ScriptableObject
{
	// Token: 0x040032EB RID: 13035
	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersFloat[] Floats;

	// Token: 0x040032EC RID: 13036
	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersColor[] Colors;

	// Token: 0x040032ED RID: 13037
	[Horizontal(4, 0)]
	public MaterialConfig.ShaderParametersTexture[] Textures;

	// Token: 0x040032EE RID: 13038
	public string[] ScaleUV;

	// Token: 0x040032EF RID: 13039
	private MaterialPropertyBlock properties;

	// Token: 0x060037F1 RID: 14321 RVA: 0x0014EA2C File Offset: 0x0014CC2C
	public MaterialPropertyBlock GetMaterialPropertyBlock(Material mat, Vector3 pos, Vector3 scale)
	{
		if (this.properties == null)
		{
			this.properties = new MaterialPropertyBlock();
		}
		this.properties.Clear();
		for (int i = 0; i < this.Floats.Length; i++)
		{
			MaterialConfig.ShaderParametersFloat shaderParametersFloat = this.Floats[i];
			float a;
			float b;
			float t = shaderParametersFloat.FindBlendParameters(pos, out a, out b);
			this.properties.SetFloat(shaderParametersFloat.Name, Mathf.Lerp(a, b, t));
		}
		for (int j = 0; j < this.Colors.Length; j++)
		{
			MaterialConfig.ShaderParametersColor shaderParametersColor = this.Colors[j];
			Color a2;
			Color b2;
			float t2 = shaderParametersColor.FindBlendParameters(pos, out a2, out b2);
			this.properties.SetColor(shaderParametersColor.Name, Color.Lerp(a2, b2, t2));
		}
		for (int k = 0; k < this.Textures.Length; k++)
		{
			MaterialConfig.ShaderParametersTexture shaderParametersTexture = this.Textures[k];
			Texture texture = shaderParametersTexture.FindBlendParameters(pos);
			if (texture)
			{
				this.properties.SetTexture(shaderParametersTexture.Name, texture);
			}
		}
		for (int l = 0; l < this.ScaleUV.Length; l++)
		{
			Vector4 vector = mat.GetVector(this.ScaleUV[l]);
			vector = new Vector4(vector.x * scale.y, vector.y * scale.y, vector.z, vector.w);
			this.properties.SetVector(this.ScaleUV[l], vector);
		}
		return this.properties;
	}

	// Token: 0x02000EB0 RID: 3760
	public class ShaderParameters<T>
	{
		// Token: 0x04004CAA RID: 19626
		public string Name;

		// Token: 0x04004CAB RID: 19627
		public T Arid;

		// Token: 0x04004CAC RID: 19628
		public T Temperate;

		// Token: 0x04004CAD RID: 19629
		public T Tundra;

		// Token: 0x04004CAE RID: 19630
		public T Arctic;

		// Token: 0x04004CAF RID: 19631
		private T[] climates;

		// Token: 0x06005315 RID: 21269 RVA: 0x001B17D8 File Offset: 0x001AF9D8
		public float FindBlendParameters(Vector3 pos, out T src, out T dst)
		{
			if (TerrainMeta.BiomeMap == null)
			{
				src = this.Temperate;
				dst = this.Tundra;
				return 0f;
			}
			if (this.climates == null || this.climates.Length == 0)
			{
				this.climates = new T[]
				{
					this.Arid,
					this.Temperate,
					this.Tundra,
					this.Arctic
				};
			}
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
			int biomeMaxType2 = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, ~biomeMaxType);
			src = this.climates[TerrainBiome.TypeToIndex(biomeMaxType)];
			dst = this.climates[TerrainBiome.TypeToIndex(biomeMaxType2)];
			return TerrainMeta.BiomeMap.GetBiome(pos, biomeMaxType2);
		}

		// Token: 0x06005316 RID: 21270 RVA: 0x001B18B8 File Offset: 0x001AFAB8
		public T FindBlendParameters(Vector3 pos)
		{
			if (TerrainMeta.BiomeMap == null)
			{
				return this.Temperate;
			}
			if (this.climates == null || this.climates.Length == 0)
			{
				this.climates = new T[]
				{
					this.Arid,
					this.Temperate,
					this.Tundra,
					this.Arctic
				};
			}
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
			return this.climates[TerrainBiome.TypeToIndex(biomeMaxType)];
		}
	}

	// Token: 0x02000EB1 RID: 3761
	[Serializable]
	public class ShaderParametersFloat : MaterialConfig.ShaderParameters<float>
	{
	}

	// Token: 0x02000EB2 RID: 3762
	[Serializable]
	public class ShaderParametersColor : MaterialConfig.ShaderParameters<Color>
	{
	}

	// Token: 0x02000EB3 RID: 3763
	[Serializable]
	public class ShaderParametersTexture : MaterialConfig.ShaderParameters<Texture>
	{
	}
}
