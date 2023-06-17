using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000234 RID: 564
[CreateAssetMenu(menuName = "Rust/MusicTheme")]
public class MusicTheme : ScriptableObject
{
	// Token: 0x0400144E RID: 5198
	[Header("Basic info")]
	public float tempo = 80f;

	// Token: 0x0400144F RID: 5199
	public int intensityHoldBars = 4;

	// Token: 0x04001450 RID: 5200
	public int lengthInBars;

	// Token: 0x04001451 RID: 5201
	[Header("Playback restrictions")]
	public bool canPlayInMenus = true;

	// Token: 0x04001452 RID: 5202
	[Horizontal(2, -1)]
	public MusicTheme.ValueRange rain = new MusicTheme.ValueRange(0f, 1f);

	// Token: 0x04001453 RID: 5203
	[Horizontal(2, -1)]
	public MusicTheme.ValueRange wind = new MusicTheme.ValueRange(0f, 1f);

	// Token: 0x04001454 RID: 5204
	[Horizontal(2, -1)]
	public MusicTheme.ValueRange snow = new MusicTheme.ValueRange(0f, 1f);

	// Token: 0x04001455 RID: 5205
	[InspectorFlags]
	public TerrainBiome.Enum biomes = (TerrainBiome.Enum)(-1);

	// Token: 0x04001456 RID: 5206
	[InspectorFlags]
	public TerrainTopology.Enum topologies = (TerrainTopology.Enum)(-1);

	// Token: 0x04001457 RID: 5207
	public AnimationCurve time = AnimationCurve.Linear(0f, 0f, 24f, 0f);

	// Token: 0x04001458 RID: 5208
	[Header("Clip data")]
	public List<MusicTheme.PositionedClip> clips = new List<MusicTheme.PositionedClip>();

	// Token: 0x04001459 RID: 5209
	public List<MusicTheme.Layer> layers = new List<MusicTheme.Layer>();

	// Token: 0x0400145A RID: 5210
	private Dictionary<int, List<MusicTheme.PositionedClip>> activeClips = new Dictionary<int, List<MusicTheme.PositionedClip>>();

	// Token: 0x0400145B RID: 5211
	private List<AudioClip> firstAudioClips = new List<AudioClip>();

	// Token: 0x0400145C RID: 5212
	private Dictionary<AudioClip, bool> audioClipDict = new Dictionary<AudioClip, bool>();

	// Token: 0x1700025A RID: 602
	// (get) Token: 0x06001BE6 RID: 7142 RVA: 0x000C39EE File Offset: 0x000C1BEE
	public int layerCount
	{
		get
		{
			return this.layers.Count;
		}
	}

	// Token: 0x1700025B RID: 603
	// (get) Token: 0x06001BE7 RID: 7143 RVA: 0x000C39FB File Offset: 0x000C1BFB
	public int samplesPerBar
	{
		get
		{
			return MusicUtil.BarsToSamples(this.tempo, 1f, 44100);
		}
	}

	// Token: 0x06001BE8 RID: 7144 RVA: 0x000C3A14 File Offset: 0x000C1C14
	private void OnValidate()
	{
		this.audioClipDict.Clear();
		this.activeClips.Clear();
		this.UpdateLengthInBars();
		for (int i = 0; i < this.clips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = this.clips[i];
			int num = this.ActiveClipCollectionID(positionedClip.startingBar - 8);
			int num2 = this.ActiveClipCollectionID(positionedClip.endingBar);
			for (int j = num; j <= num2; j++)
			{
				if (!this.activeClips.ContainsKey(j))
				{
					this.activeClips.Add(j, new List<MusicTheme.PositionedClip>());
				}
				if (!this.activeClips[j].Contains(positionedClip))
				{
					this.activeClips[j].Add(positionedClip);
				}
			}
			if (positionedClip.musicClip != null)
			{
				AudioClip audioClip = positionedClip.musicClip.audioClip;
				if (!this.audioClipDict.ContainsKey(audioClip))
				{
					this.audioClipDict.Add(audioClip, true);
				}
				if (positionedClip.startingBar < 8 && !this.firstAudioClips.Contains(audioClip))
				{
					this.firstAudioClips.Add(audioClip);
				}
				positionedClip.musicClip.lengthInBarsWithTail = Mathf.CeilToInt(MusicUtil.SecondsToBars(this.tempo, (double)positionedClip.musicClip.audioClip.length));
			}
		}
	}

	// Token: 0x06001BE9 RID: 7145 RVA: 0x000C3B5C File Offset: 0x000C1D5C
	public List<MusicTheme.PositionedClip> GetActiveClipsForBar(int bar)
	{
		int key = this.ActiveClipCollectionID(bar);
		if (!this.activeClips.ContainsKey(key))
		{
			return null;
		}
		return this.activeClips[key];
	}

	// Token: 0x06001BEA RID: 7146 RVA: 0x000C3B8D File Offset: 0x000C1D8D
	private int ActiveClipCollectionID(int bar)
	{
		return Mathf.FloorToInt(Mathf.Max((float)(bar / 4), 0f));
	}

	// Token: 0x06001BEB RID: 7147 RVA: 0x000C3BA2 File Offset: 0x000C1DA2
	public MusicTheme.Layer LayerById(int id)
	{
		if (this.layers.Count <= id)
		{
			return null;
		}
		return this.layers[id];
	}

	// Token: 0x06001BEC RID: 7148 RVA: 0x000C3BC0 File Offset: 0x000C1DC0
	public void AddLayer()
	{
		MusicTheme.Layer layer = new MusicTheme.Layer();
		layer.name = "layer " + this.layers.Count;
		this.layers.Add(layer);
	}

	// Token: 0x06001BED RID: 7149 RVA: 0x000C3C00 File Offset: 0x000C1E00
	private void UpdateLengthInBars()
	{
		int num = 0;
		for (int i = 0; i < this.clips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = this.clips[i];
			if (!(positionedClip.musicClip == null))
			{
				int num2 = positionedClip.startingBar + positionedClip.musicClip.lengthInBars;
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		this.lengthInBars = num;
	}

	// Token: 0x06001BEE RID: 7150 RVA: 0x000C3C60 File Offset: 0x000C1E60
	public bool CanPlayInEnvironment(int currentBiome, int currentTopology, float currentRain, float currentSnow, float currentWind)
	{
		return (!TOD_Sky.Instance || this.time.Evaluate(TOD_Sky.Instance.Cycle.Hour) >= 0f) && (this.biomes == (TerrainBiome.Enum)(-1) || (this.biomes & (TerrainBiome.Enum)currentBiome) != (TerrainBiome.Enum)0) && (this.topologies == (TerrainTopology.Enum)(-1) || (this.topologies & (TerrainTopology.Enum)currentTopology) == (TerrainTopology.Enum)0) && ((this.rain.min <= 0f && this.rain.max >= 1f) || currentRain >= this.rain.min) && currentRain <= this.rain.max && ((this.snow.min <= 0f && this.snow.max >= 1f) || currentSnow >= this.snow.min) && currentSnow <= this.snow.max && ((this.wind.min <= 0f && this.wind.max >= 1f) || currentWind >= this.wind.min) && currentWind <= this.wind.max;
	}

	// Token: 0x06001BEF RID: 7151 RVA: 0x000C3D94 File Offset: 0x000C1F94
	public bool FirstClipsLoaded()
	{
		for (int i = 0; i < this.firstAudioClips.Count; i++)
		{
			if (this.firstAudioClips[i].loadState != AudioDataLoadState.Loaded)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001BF0 RID: 7152 RVA: 0x000C3DCE File Offset: 0x000C1FCE
	public bool ContainsAudioClip(AudioClip clip)
	{
		return this.audioClipDict.ContainsKey(clip);
	}

	// Token: 0x02000C78 RID: 3192
	[Serializable]
	public class Layer
	{
		// Token: 0x0400435F RID: 17247
		public string name = "layer";
	}

	// Token: 0x02000C79 RID: 3193
	[Serializable]
	public class PositionedClip
	{
		// Token: 0x04004360 RID: 17248
		public MusicTheme theme;

		// Token: 0x04004361 RID: 17249
		public MusicClip musicClip;

		// Token: 0x04004362 RID: 17250
		public int startingBar;

		// Token: 0x04004363 RID: 17251
		public int layerId;

		// Token: 0x04004364 RID: 17252
		public float minIntensity;

		// Token: 0x04004365 RID: 17253
		public float maxIntensity = 1f;

		// Token: 0x04004366 RID: 17254
		public bool allowFadeIn = true;

		// Token: 0x04004367 RID: 17255
		public bool allowFadeOut = true;

		// Token: 0x04004368 RID: 17256
		public float fadeInTime = 1f;

		// Token: 0x04004369 RID: 17257
		public float fadeOutTime = 0.5f;

		// Token: 0x0400436A RID: 17258
		public float intensityReduction;

		// Token: 0x0400436B RID: 17259
		public int jumpBarCount;

		// Token: 0x0400436C RID: 17260
		public float jumpMinimumIntensity = 0.5f;

		// Token: 0x0400436D RID: 17261
		public float jumpMaximumIntensity = 0.5f;

		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x06004EFE RID: 20222 RVA: 0x001A550F File Offset: 0x001A370F
		public int endingBar
		{
			get
			{
				if (!(this.musicClip == null))
				{
					return this.startingBar + this.musicClip.lengthInBarsWithTail;
				}
				return this.startingBar;
			}
		}

		// Token: 0x06004EFF RID: 20223 RVA: 0x001A5538 File Offset: 0x001A3738
		public bool CanPlay(float intensity)
		{
			return (intensity > this.minIntensity || (this.minIntensity == 0f && intensity == 0f)) && intensity <= this.maxIntensity;
		}

		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x06004F00 RID: 20224 RVA: 0x001A5566 File Offset: 0x001A3766
		public bool isControlClip
		{
			get
			{
				return this.musicClip == null;
			}
		}

		// Token: 0x06004F01 RID: 20225 RVA: 0x001A5574 File Offset: 0x001A3774
		public void CopySettingsFrom(MusicTheme.PositionedClip otherClip)
		{
			if (this.isControlClip != otherClip.isControlClip)
			{
				return;
			}
			if (otherClip == this)
			{
				return;
			}
			this.allowFadeIn = otherClip.allowFadeIn;
			this.fadeInTime = otherClip.fadeInTime;
			this.allowFadeOut = otherClip.allowFadeOut;
			this.fadeOutTime = otherClip.fadeOutTime;
			this.maxIntensity = otherClip.maxIntensity;
			this.minIntensity = otherClip.minIntensity;
			this.intensityReduction = otherClip.intensityReduction;
		}
	}

	// Token: 0x02000C7A RID: 3194
	[Serializable]
	public class ValueRange
	{
		// Token: 0x0400436E RID: 17262
		public float min;

		// Token: 0x0400436F RID: 17263
		public float max;

		// Token: 0x06004F03 RID: 20227 RVA: 0x001A5644 File Offset: 0x001A3844
		public ValueRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}
}
