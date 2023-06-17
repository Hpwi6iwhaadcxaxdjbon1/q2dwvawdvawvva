using System;
using UnityEngine;

// Token: 0x02000690 RID: 1680
[CreateAssetMenu(menuName = "Rust/Terrain Atlas Set")]
public class TerrainAtlasSet : ScriptableObject
{
	// Token: 0x0400275A RID: 10074
	public const int SplatCount = 8;

	// Token: 0x0400275B RID: 10075
	public const int SplatSize = 2048;

	// Token: 0x0400275C RID: 10076
	public const int MaxSplatSize = 2047;

	// Token: 0x0400275D RID: 10077
	public const int SplatPadding = 256;

	// Token: 0x0400275E RID: 10078
	public const int AtlasSize = 8192;

	// Token: 0x0400275F RID: 10079
	public const int RegionSize = 2560;

	// Token: 0x04002760 RID: 10080
	public const int SplatsPerLine = 3;

	// Token: 0x04002761 RID: 10081
	public const int SourceTypeCount = 3;

	// Token: 0x04002762 RID: 10082
	public const int AtlasMipCount = 10;

	// Token: 0x04002763 RID: 10083
	public static string[] sourceTypeNames = new string[]
	{
		"Albedo",
		"Normal",
		"Packed"
	};

	// Token: 0x04002764 RID: 10084
	public static string[] sourceTypeNamesExt = new string[]
	{
		"Albedo (rgb)",
		"Normal (rgb)",
		"Metal[ignored]_Height_AO_Gloss (rgba)"
	};

	// Token: 0x04002765 RID: 10085
	public static string[] sourceTypePostfix = new string[]
	{
		"_albedo",
		"_normal",
		"_metal_hm_ao_gloss"
	};

	// Token: 0x04002766 RID: 10086
	public string[] splatNames;

	// Token: 0x04002767 RID: 10087
	public bool[] albedoHighpass;

	// Token: 0x04002768 RID: 10088
	public string[] albedoPaths;

	// Token: 0x04002769 RID: 10089
	public Color[] defaultValues;

	// Token: 0x0400276A RID: 10090
	public TerrainAtlasSet.SourceMapSet[] sourceMaps;

	// Token: 0x0400276B RID: 10091
	public bool highQualityCompression = true;

	// Token: 0x0400276C RID: 10092
	public bool generateTextureAtlases = true;

	// Token: 0x0400276D RID: 10093
	public bool generateTextureArrays;

	// Token: 0x0400276E RID: 10094
	public string splatSearchPrefix = "terrain_";

	// Token: 0x0400276F RID: 10095
	public string splatSearchFolder = "Assets/Content/Nature/Terrain";

	// Token: 0x04002770 RID: 10096
	public string albedoAtlasSavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_albedo_atlas";

	// Token: 0x04002771 RID: 10097
	public string normalAtlasSavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_normal_atlas";

	// Token: 0x04002772 RID: 10098
	public string albedoArraySavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_albedo_array";

	// Token: 0x04002773 RID: 10099
	public string normalArraySavePath = "Assets/Content/Nature/Terrain/Atlas/terrain_normal_array";

	// Token: 0x06002FD4 RID: 12244 RVA: 0x0011F6BC File Offset: 0x0011D8BC
	public void CheckReset()
	{
		if (this.splatNames == null)
		{
			this.splatNames = new string[]
			{
				"Dirt",
				"Snow",
				"Sand",
				"Rock",
				"Grass",
				"Forest",
				"Stones",
				"Gravel"
			};
		}
		else if (this.splatNames.Length != 8)
		{
			Array.Resize<string>(ref this.splatNames, 8);
		}
		if (this.albedoHighpass == null)
		{
			this.albedoHighpass = new bool[8];
		}
		else if (this.albedoHighpass.Length != 8)
		{
			Array.Resize<bool>(ref this.albedoHighpass, 8);
		}
		if (this.albedoPaths == null)
		{
			this.albedoPaths = new string[8];
		}
		else if (this.albedoPaths.Length != 8)
		{
			Array.Resize<string>(ref this.albedoPaths, 8);
		}
		if (this.defaultValues == null)
		{
			this.defaultValues = new Color[]
			{
				new Color(1f, 1f, 1f, 0.5f),
				new Color(0.5f, 0.5f, 1f, 0f),
				new Color(0f, 0f, 1f, 0.5f)
			};
		}
		else if (this.defaultValues.Length != 3)
		{
			Array.Resize<Color>(ref this.defaultValues, 3);
		}
		if (this.sourceMaps == null)
		{
			this.sourceMaps = new TerrainAtlasSet.SourceMapSet[3];
		}
		else if (this.sourceMaps.Length != 3)
		{
			Array.Resize<TerrainAtlasSet.SourceMapSet>(ref this.sourceMaps, 3);
		}
		for (int i = 0; i < 3; i++)
		{
			this.sourceMaps[i] = ((this.sourceMaps[i] != null) ? this.sourceMaps[i] : new TerrainAtlasSet.SourceMapSet());
			this.sourceMaps[i].CheckReset();
		}
	}

	// Token: 0x02000DB8 RID: 3512
	public enum SourceType
	{
		// Token: 0x040048C6 RID: 18630
		ALBEDO,
		// Token: 0x040048C7 RID: 18631
		NORMAL,
		// Token: 0x040048C8 RID: 18632
		PACKED,
		// Token: 0x040048C9 RID: 18633
		COUNT
	}

	// Token: 0x02000DB9 RID: 3513
	[Serializable]
	public class SourceMapSet
	{
		// Token: 0x040048CA RID: 18634
		public Texture2D[] maps;

		// Token: 0x06005150 RID: 20816 RVA: 0x001AC312 File Offset: 0x001AA512
		internal void CheckReset()
		{
			if (this.maps == null)
			{
				this.maps = new Texture2D[8];
				return;
			}
			if (this.maps.Length != 8)
			{
				Array.Resize<Texture2D>(ref this.maps, 8);
			}
		}
	}
}
