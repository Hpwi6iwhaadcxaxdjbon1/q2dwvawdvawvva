using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000653 RID: 1619
[CreateAssetMenu(menuName = "Rust/Terrain Config")]
public class TerrainConfig : ScriptableObject
{
	// Token: 0x0400267F RID: 9855
	public bool CastShadows = true;

	// Token: 0x04002680 RID: 9856
	public LayerMask GroundMask = 0;

	// Token: 0x04002681 RID: 9857
	public LayerMask WaterMask = 0;

	// Token: 0x04002682 RID: 9858
	public PhysicMaterial GenericMaterial;

	// Token: 0x04002683 RID: 9859
	public Material Material;

	// Token: 0x04002684 RID: 9860
	public Material MarginMaterial;

	// Token: 0x04002685 RID: 9861
	public Texture[] AlbedoArrays = new Texture[3];

	// Token: 0x04002686 RID: 9862
	public Texture[] NormalArrays = new Texture[3];

	// Token: 0x04002687 RID: 9863
	public float HeightMapErrorMin = 5f;

	// Token: 0x04002688 RID: 9864
	public float HeightMapErrorMax = 100f;

	// Token: 0x04002689 RID: 9865
	public float BaseMapDistanceMin = 100f;

	// Token: 0x0400268A RID: 9866
	public float BaseMapDistanceMax = 500f;

	// Token: 0x0400268B RID: 9867
	public float ShaderLodMin = 100f;

	// Token: 0x0400268C RID: 9868
	public float ShaderLodMax = 600f;

	// Token: 0x0400268D RID: 9869
	public TerrainConfig.SplatType[] Splats = new TerrainConfig.SplatType[8];

	// Token: 0x0400268E RID: 9870
	private string snowMatName;

	// Token: 0x0400268F RID: 9871
	private string grassMatName;

	// Token: 0x04002690 RID: 9872
	private string sandMatName;

	// Token: 0x04002691 RID: 9873
	private List<string> dirtMatNames;

	// Token: 0x04002692 RID: 9874
	private List<string> stoneyMatNames;

	// Token: 0x170003CC RID: 972
	// (get) Token: 0x06002EEE RID: 12014 RVA: 0x0011B8FA File Offset: 0x00119AFA
	public Texture AlbedoArray
	{
		get
		{
			return this.AlbedoArrays[Mathf.Clamp(QualitySettings.masterTextureLimit, 0, 2)];
		}
	}

	// Token: 0x170003CD RID: 973
	// (get) Token: 0x06002EEF RID: 12015 RVA: 0x0011B90F File Offset: 0x00119B0F
	public Texture NormalArray
	{
		get
		{
			return this.NormalArrays[Mathf.Clamp(QualitySettings.masterTextureLimit, 0, 2)];
		}
	}

	// Token: 0x06002EF0 RID: 12016 RVA: 0x0011B924 File Offset: 0x00119B24
	public PhysicMaterial[] GetPhysicMaterials()
	{
		PhysicMaterial[] array = new PhysicMaterial[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].Material;
		}
		return array;
	}

	// Token: 0x06002EF1 RID: 12017 RVA: 0x0011B964 File Offset: 0x00119B64
	public Color[] GetAridColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].AridColor;
		}
		return array;
	}

	// Token: 0x06002EF2 RID: 12018 RVA: 0x0011B9A8 File Offset: 0x00119BA8
	public void GetAridOverlayConstants(out Color[] color, out Vector4[] param)
	{
		color = new Color[this.Splats.Length];
		param = new Vector4[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			TerrainConfig.SplatOverlay aridOverlay = this.Splats[i].AridOverlay;
			color[i] = aridOverlay.Color.linear;
			param[i] = new Vector4(aridOverlay.Smoothness, aridOverlay.NormalIntensity, aridOverlay.BlendFactor, aridOverlay.BlendFalloff);
		}
	}

	// Token: 0x06002EF3 RID: 12019 RVA: 0x0011BA2C File Offset: 0x00119C2C
	public Color[] GetTemperateColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].TemperateColor;
		}
		return array;
	}

	// Token: 0x06002EF4 RID: 12020 RVA: 0x0011BA70 File Offset: 0x00119C70
	public void GetTemperateOverlayConstants(out Color[] color, out Vector4[] param)
	{
		color = new Color[this.Splats.Length];
		param = new Vector4[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			TerrainConfig.SplatOverlay temperateOverlay = this.Splats[i].TemperateOverlay;
			color[i] = temperateOverlay.Color.linear;
			param[i] = new Vector4(temperateOverlay.Smoothness, temperateOverlay.NormalIntensity, temperateOverlay.BlendFactor, temperateOverlay.BlendFalloff);
		}
	}

	// Token: 0x06002EF5 RID: 12021 RVA: 0x0011BAF4 File Offset: 0x00119CF4
	public Color[] GetTundraColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].TundraColor;
		}
		return array;
	}

	// Token: 0x06002EF6 RID: 12022 RVA: 0x0011BB38 File Offset: 0x00119D38
	public void GetTundraOverlayConstants(out Color[] color, out Vector4[] param)
	{
		color = new Color[this.Splats.Length];
		param = new Vector4[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			TerrainConfig.SplatOverlay tundraOverlay = this.Splats[i].TundraOverlay;
			color[i] = tundraOverlay.Color.linear;
			param[i] = new Vector4(tundraOverlay.Smoothness, tundraOverlay.NormalIntensity, tundraOverlay.BlendFactor, tundraOverlay.BlendFalloff);
		}
	}

	// Token: 0x06002EF7 RID: 12023 RVA: 0x0011BBBC File Offset: 0x00119DBC
	public Color[] GetArcticColors()
	{
		Color[] array = new Color[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].ArcticColor;
		}
		return array;
	}

	// Token: 0x06002EF8 RID: 12024 RVA: 0x0011BC00 File Offset: 0x00119E00
	public void GetArcticOverlayConstants(out Color[] color, out Vector4[] param)
	{
		color = new Color[this.Splats.Length];
		param = new Vector4[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			TerrainConfig.SplatOverlay arcticOverlay = this.Splats[i].ArcticOverlay;
			color[i] = arcticOverlay.Color.linear;
			param[i] = new Vector4(arcticOverlay.Smoothness, arcticOverlay.NormalIntensity, arcticOverlay.BlendFactor, arcticOverlay.BlendFalloff);
		}
	}

	// Token: 0x06002EF9 RID: 12025 RVA: 0x0011BC84 File Offset: 0x00119E84
	public float[] GetSplatTiling()
	{
		float[] array = new float[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = this.Splats[i].SplatTiling;
		}
		return array;
	}

	// Token: 0x06002EFA RID: 12026 RVA: 0x0011BCC4 File Offset: 0x00119EC4
	public float GetMaxSplatTiling()
	{
		float num = float.MinValue;
		for (int i = 0; i < this.Splats.Length; i++)
		{
			if (this.Splats[i].SplatTiling > num)
			{
				num = this.Splats[i].SplatTiling;
			}
		}
		return num;
	}

	// Token: 0x06002EFB RID: 12027 RVA: 0x0011BD0C File Offset: 0x00119F0C
	public float GetMinSplatTiling()
	{
		float num = float.MaxValue;
		for (int i = 0; i < this.Splats.Length; i++)
		{
			if (this.Splats[i].SplatTiling < num)
			{
				num = this.Splats[i].SplatTiling;
			}
		}
		return num;
	}

	// Token: 0x06002EFC RID: 12028 RVA: 0x0011BD54 File Offset: 0x00119F54
	public Vector3[] GetPackedUVMIX()
	{
		Vector3[] array = new Vector3[this.Splats.Length];
		for (int i = 0; i < this.Splats.Length; i++)
		{
			array[i] = new Vector3(this.Splats[i].UVMIXMult, this.Splats[i].UVMIXStart, this.Splats[i].UVMIXDist);
		}
		return array;
	}

	// Token: 0x06002EFD RID: 12029 RVA: 0x0011BDB8 File Offset: 0x00119FB8
	public TerrainConfig.GroundType GetCurrentGroundType(bool isGrounded, RaycastHit hit)
	{
		if (string.IsNullOrEmpty(this.grassMatName))
		{
			this.dirtMatNames = new List<string>();
			this.stoneyMatNames = new List<string>();
			TerrainConfig.SplatType[] splats = this.Splats;
			int i = 0;
			while (i < splats.Length)
			{
				TerrainConfig.SplatType splatType = splats[i];
				string text = splatType.Name.ToLower();
				string name = splatType.Material.name;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 2296799147U)
				{
					if (num <= 1328097888U)
					{
						if (num != 1180566432U)
						{
							if (num == 1328097888U)
							{
								if (text == "forest")
								{
									goto IL_183;
								}
							}
						}
						else if (text == "dirt")
						{
							goto IL_183;
						}
					}
					else if (num != 2223183858U)
					{
						if (num == 2296799147U)
						{
							if (text == "stones")
							{
								goto IL_192;
							}
						}
					}
					else if (text == "snow")
					{
						this.snowMatName = name;
					}
				}
				else if (num <= 3000956154U)
				{
					if (num != 2993663101U)
					{
						if (num == 3000956154U)
						{
							if (text == "gravel")
							{
								goto IL_192;
							}
						}
					}
					else if (text == "grass")
					{
						this.grassMatName = name;
					}
				}
				else if (num != 3189014883U)
				{
					if (num == 3912378421U)
					{
						if (text == "tundra")
						{
							goto IL_183;
						}
					}
				}
				else if (text == "sand")
				{
					this.sandMatName = name;
				}
				IL_19F:
				i++;
				continue;
				IL_183:
				this.dirtMatNames.Add(name);
				goto IL_19F;
				IL_192:
				this.stoneyMatNames.Add(name);
				goto IL_19F;
			}
		}
		if (!isGrounded)
		{
			return TerrainConfig.GroundType.None;
		}
		if (hit.collider == null)
		{
			return TerrainConfig.GroundType.HardSurface;
		}
		PhysicMaterial materialAt = hit.collider.GetMaterialAt(hit.point);
		if (materialAt == null)
		{
			return TerrainConfig.GroundType.HardSurface;
		}
		string name2 = materialAt.name;
		if (name2 == this.grassMatName)
		{
			return TerrainConfig.GroundType.Grass;
		}
		if (name2 == this.sandMatName)
		{
			return TerrainConfig.GroundType.Sand;
		}
		if (name2 == this.snowMatName)
		{
			return TerrainConfig.GroundType.Snow;
		}
		for (int j = 0; j < this.dirtMatNames.Count; j++)
		{
			if (this.dirtMatNames[j] == name2)
			{
				return TerrainConfig.GroundType.Dirt;
			}
		}
		for (int k = 0; k < this.stoneyMatNames.Count; k++)
		{
			if (this.stoneyMatNames[k] == name2)
			{
				return TerrainConfig.GroundType.Gravel;
			}
		}
		return TerrainConfig.GroundType.HardSurface;
	}

	// Token: 0x02000DAF RID: 3503
	[Serializable]
	public class SplatOverlay
	{
		// Token: 0x0400488F RID: 18575
		public Color Color = new Color(1f, 1f, 1f, 0f);

		// Token: 0x04004890 RID: 18576
		[Range(0f, 1f)]
		public float Smoothness;

		// Token: 0x04004891 RID: 18577
		[Range(0f, 1f)]
		public float NormalIntensity = 1f;

		// Token: 0x04004892 RID: 18578
		[Range(0f, 8f)]
		public float BlendFactor = 0.5f;

		// Token: 0x04004893 RID: 18579
		[Range(0.01f, 32f)]
		public float BlendFalloff = 0.5f;
	}

	// Token: 0x02000DB0 RID: 3504
	[Serializable]
	public class SplatType
	{
		// Token: 0x04004894 RID: 18580
		public string Name = "";

		// Token: 0x04004895 RID: 18581
		[FormerlySerializedAs("WarmColor")]
		public Color AridColor = Color.white;

		// Token: 0x04004896 RID: 18582
		public TerrainConfig.SplatOverlay AridOverlay = new TerrainConfig.SplatOverlay();

		// Token: 0x04004897 RID: 18583
		[FormerlySerializedAs("Color")]
		public Color TemperateColor = Color.white;

		// Token: 0x04004898 RID: 18584
		public TerrainConfig.SplatOverlay TemperateOverlay = new TerrainConfig.SplatOverlay();

		// Token: 0x04004899 RID: 18585
		[FormerlySerializedAs("ColdColor")]
		public Color TundraColor = Color.white;

		// Token: 0x0400489A RID: 18586
		public TerrainConfig.SplatOverlay TundraOverlay = new TerrainConfig.SplatOverlay();

		// Token: 0x0400489B RID: 18587
		[FormerlySerializedAs("ColdColor")]
		public Color ArcticColor = Color.white;

		// Token: 0x0400489C RID: 18588
		public TerrainConfig.SplatOverlay ArcticOverlay = new TerrainConfig.SplatOverlay();

		// Token: 0x0400489D RID: 18589
		public PhysicMaterial Material;

		// Token: 0x0400489E RID: 18590
		public float SplatTiling = 5f;

		// Token: 0x0400489F RID: 18591
		[Range(0f, 1f)]
		public float UVMIXMult = 0.15f;

		// Token: 0x040048A0 RID: 18592
		public float UVMIXStart;

		// Token: 0x040048A1 RID: 18593
		public float UVMIXDist = 100f;
	}

	// Token: 0x02000DB1 RID: 3505
	public enum GroundType
	{
		// Token: 0x040048A3 RID: 18595
		None,
		// Token: 0x040048A4 RID: 18596
		HardSurface,
		// Token: 0x040048A5 RID: 18597
		Grass,
		// Token: 0x040048A6 RID: 18598
		Sand,
		// Token: 0x040048A7 RID: 18599
		Snow,
		// Token: 0x040048A8 RID: 18600
		Dirt,
		// Token: 0x040048A9 RID: 18601
		Gravel
	}
}
