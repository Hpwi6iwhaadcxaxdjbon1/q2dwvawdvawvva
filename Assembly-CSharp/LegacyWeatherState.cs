using System;
using UnityEngine;

// Token: 0x020005B2 RID: 1458
public class LegacyWeatherState
{
	// Token: 0x040023A9 RID: 9129
	private WeatherPreset preset;

	// Token: 0x06002C05 RID: 11269 RVA: 0x0010AFF3 File Offset: 0x001091F3
	public LegacyWeatherState(WeatherPreset preset)
	{
		this.preset = preset;
	}

	// Token: 0x1700039A RID: 922
	// (get) Token: 0x06002C06 RID: 11270 RVA: 0x0010B002 File Offset: 0x00109202
	// (set) Token: 0x06002C07 RID: 11271 RVA: 0x0010B00F File Offset: 0x0010920F
	public float Wind
	{
		get
		{
			return this.preset.Wind;
		}
		set
		{
			this.preset.Wind = value;
		}
	}

	// Token: 0x1700039B RID: 923
	// (get) Token: 0x06002C08 RID: 11272 RVA: 0x0010B01D File Offset: 0x0010921D
	// (set) Token: 0x06002C09 RID: 11273 RVA: 0x0010B02A File Offset: 0x0010922A
	public float Rain
	{
		get
		{
			return this.preset.Rain;
		}
		set
		{
			this.preset.Rain = value;
		}
	}

	// Token: 0x1700039C RID: 924
	// (get) Token: 0x06002C0A RID: 11274 RVA: 0x0010B038 File Offset: 0x00109238
	// (set) Token: 0x06002C0B RID: 11275 RVA: 0x0010B04A File Offset: 0x0010924A
	public float Clouds
	{
		get
		{
			return this.preset.Clouds.Coverage;
		}
		set
		{
			this.preset.Clouds.Opacity = Mathf.Sign(value);
			this.preset.Clouds.Coverage = value;
		}
	}

	// Token: 0x1700039D RID: 925
	// (get) Token: 0x06002C0C RID: 11276 RVA: 0x0010B073 File Offset: 0x00109273
	// (set) Token: 0x06002C0D RID: 11277 RVA: 0x0010B085 File Offset: 0x00109285
	public float Fog
	{
		get
		{
			return this.preset.Atmosphere.Fogginess;
		}
		set
		{
			this.preset.Atmosphere.Fogginess = value;
		}
	}
}
