using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200071D RID: 1821
public class SubsurfaceProfileTexture
{
	// Token: 0x04002986 RID: 10630
	public const int SUBSURFACE_RADIUS_SCALE = 1024;

	// Token: 0x04002987 RID: 10631
	public const int SUBSURFACE_KERNEL_SIZE = 3;

	// Token: 0x04002988 RID: 10632
	private List<SubsurfaceProfileTexture.SubsurfaceProfileEntry> entries = new List<SubsurfaceProfileTexture.SubsurfaceProfileEntry>(16);

	// Token: 0x04002989 RID: 10633
	private Texture2D texture;

	// Token: 0x17000435 RID: 1077
	// (get) Token: 0x06003317 RID: 13079 RVA: 0x0013A4D8 File Offset: 0x001386D8
	public Texture2D Texture
	{
		get
		{
			if (!(this.texture == null))
			{
				return this.texture;
			}
			return this.CreateTexture();
		}
	}

	// Token: 0x06003318 RID: 13080 RVA: 0x0013A4F5 File Offset: 0x001386F5
	public SubsurfaceProfileTexture()
	{
		this.AddProfile(SubsurfaceProfileData.Default, null);
	}

	// Token: 0x06003319 RID: 13081 RVA: 0x0013A518 File Offset: 0x00138718
	public int FindEntryIndex(SubsurfaceProfile profile)
	{
		for (int i = 0; i < this.entries.Count; i++)
		{
			if (this.entries[i].profile == profile)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600331A RID: 13082 RVA: 0x0013A558 File Offset: 0x00138758
	public int AddProfile(SubsurfaceProfileData data, SubsurfaceProfile profile)
	{
		int num = -1;
		for (int i = 0; i < this.entries.Count; i++)
		{
			if (this.entries[i].profile == profile)
			{
				num = i;
				this.entries[num] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, profile);
				break;
			}
		}
		if (num < 0)
		{
			num = this.entries.Count;
			this.entries.Add(new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, profile));
		}
		this.ReleaseTexture();
		return num;
	}

	// Token: 0x0600331B RID: 13083 RVA: 0x0013A5D6 File Offset: 0x001387D6
	public void UpdateProfile(int id, SubsurfaceProfileData data)
	{
		if (id >= 0)
		{
			this.entries[id] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, this.entries[id].profile);
			this.ReleaseTexture();
		}
	}

	// Token: 0x0600331C RID: 13084 RVA: 0x0013A605 File Offset: 0x00138805
	public void RemoveProfile(int id)
	{
		if (id >= 0)
		{
			this.entries[id] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(SubsurfaceProfileData.Invalid, null);
			this.CheckReleaseTexture();
		}
	}

	// Token: 0x0600331D RID: 13085 RVA: 0x0013A628 File Offset: 0x00138828
	public static Color ColorClamp(Color color, float min = 0f, float max = 1f)
	{
		Color result;
		result.r = Mathf.Clamp(color.r, min, max);
		result.g = Mathf.Clamp(color.g, min, max);
		result.b = Mathf.Clamp(color.b, min, max);
		result.a = Mathf.Clamp(color.a, min, max);
		return result;
	}

	// Token: 0x0600331E RID: 13086 RVA: 0x0013A688 File Offset: 0x00138888
	private Texture2D CreateTexture()
	{
		if (this.entries.Count > 0)
		{
			int num = 32;
			int num2 = Mathf.Max(this.entries.Count, 64);
			this.ReleaseTexture();
			this.texture = new Texture2D(num, num2, TextureFormat.RGBAHalf, false, true);
			this.texture.name = "SubsurfaceProfiles";
			this.texture.wrapMode = TextureWrapMode.Clamp;
			this.texture.filterMode = FilterMode.Bilinear;
			Color[] pixels = this.texture.GetPixels(0);
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = Color.clear;
			}
			Color[] array = new Color[num];
			for (int j = 0; j < this.entries.Count; j++)
			{
				SubsurfaceProfileData data = this.entries[j].data;
				data.SubsurfaceColor = SubsurfaceProfileTexture.ColorClamp(data.SubsurfaceColor, 0f, 1f);
				data.FalloffColor = SubsurfaceProfileTexture.ColorClamp(data.FalloffColor, 0.009f, 1f);
				array[0] = data.SubsurfaceColor;
				array[0].a = 0f;
				SeparableSSS.CalculateKernel(array, 1, 13, data.SubsurfaceColor, data.FalloffColor);
				SeparableSSS.CalculateKernel(array, 14, 9, data.SubsurfaceColor, data.FalloffColor);
				SeparableSSS.CalculateKernel(array, 23, 6, data.SubsurfaceColor, data.FalloffColor);
				int num3 = num * (num2 - j - 1);
				for (int k = 0; k < 29; k++)
				{
					Color color = array[k] * new Color(1f, 1f, 1f, 0.33333334f);
					color.a *= data.ScatterRadius / 1024f;
					pixels[num3 + k] = color;
				}
			}
			this.texture.SetPixels(pixels, 0);
			this.texture.Apply(false, false);
			return this.texture;
		}
		return null;
	}

	// Token: 0x0600331F RID: 13087 RVA: 0x0013A88C File Offset: 0x00138A8C
	private void CheckReleaseTexture()
	{
		int num = 0;
		for (int i = 0; i < this.entries.Count; i++)
		{
			num += ((this.entries[i].profile == null) ? 1 : 0);
		}
		if (this.entries.Count == num)
		{
			this.ReleaseTexture();
		}
	}

	// Token: 0x06003320 RID: 13088 RVA: 0x0013A8E5 File Offset: 0x00138AE5
	private void ReleaseTexture()
	{
		if (this.texture != null)
		{
			UnityEngine.Object.DestroyImmediate(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x02000E3A RID: 3642
	private struct SubsurfaceProfileEntry
	{
		// Token: 0x04004AB5 RID: 19125
		public SubsurfaceProfileData data;

		// Token: 0x04004AB6 RID: 19126
		public SubsurfaceProfile profile;

		// Token: 0x06005245 RID: 21061 RVA: 0x001AFB86 File Offset: 0x001ADD86
		public SubsurfaceProfileEntry(SubsurfaceProfileData data, SubsurfaceProfile profile)
		{
			this.data = data;
			this.profile = profile;
		}
	}
}
