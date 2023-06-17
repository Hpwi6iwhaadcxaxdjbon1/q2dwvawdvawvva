using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000233 RID: 563
public class MusicManager : SingletonComponent<MusicManager>, IClientComponent
{
	// Token: 0x04001434 RID: 5172
	public AudioMixerGroup mixerGroup;

	// Token: 0x04001435 RID: 5173
	public List<MusicTheme> themes;

	// Token: 0x04001436 RID: 5174
	public MusicTheme currentTheme;

	// Token: 0x04001437 RID: 5175
	public List<AudioSource> sources = new List<AudioSource>();

	// Token: 0x04001438 RID: 5176
	public double nextMusic;

	// Token: 0x04001439 RID: 5177
	public double nextMusicFromIntensityRaise;

	// Token: 0x0400143A RID: 5178
	[Range(0f, 1f)]
	public float intensity;

	// Token: 0x0400143B RID: 5179
	public Dictionary<MusicTheme.PositionedClip, MusicManager.ClipPlaybackData> clipPlaybackData = new Dictionary<MusicTheme.PositionedClip, MusicManager.ClipPlaybackData>();

	// Token: 0x0400143C RID: 5180
	public int holdIntensityUntilBar;

	// Token: 0x0400143D RID: 5181
	public bool musicPlaying;

	// Token: 0x0400143E RID: 5182
	public bool loadingFirstClips;

	// Token: 0x0400143F RID: 5183
	public MusicTheme nextTheme;

	// Token: 0x04001440 RID: 5184
	public double lastClipUpdate;

	// Token: 0x04001441 RID: 5185
	public float clipUpdateInterval = 0.1f;

	// Token: 0x04001442 RID: 5186
	public double themeStartTime;

	// Token: 0x04001443 RID: 5187
	public int lastActiveClipRefresh = -10;

	// Token: 0x04001444 RID: 5188
	public int activeClipRefreshInterval = 1;

	// Token: 0x04001445 RID: 5189
	public bool forceThemeChange;

	// Token: 0x04001446 RID: 5190
	public float randomIntensityJumpChance;

	// Token: 0x04001447 RID: 5191
	public int clipScheduleBarsEarly = 1;

	// Token: 0x04001448 RID: 5192
	public List<MusicTheme.PositionedClip> activeClips = new List<MusicTheme.PositionedClip>();

	// Token: 0x04001449 RID: 5193
	public List<MusicTheme.PositionedClip> activeMusicClips = new List<MusicTheme.PositionedClip>();

	// Token: 0x0400144A RID: 5194
	public List<MusicTheme.PositionedClip> activeControlClips = new List<MusicTheme.PositionedClip>();

	// Token: 0x0400144B RID: 5195
	public List<MusicZone> currentMusicZones = new List<MusicZone>();

	// Token: 0x0400144C RID: 5196
	public int currentBar;

	// Token: 0x0400144D RID: 5197
	public int barOffset;

	// Token: 0x17000258 RID: 600
	// (get) Token: 0x06001BE1 RID: 7137 RVA: 0x000C395A File Offset: 0x000C1B5A
	public double currentThemeTime
	{
		get
		{
			return UnityEngine.AudioSettings.dspTime - this.themeStartTime;
		}
	}

	// Token: 0x17000259 RID: 601
	// (get) Token: 0x06001BE2 RID: 7138 RVA: 0x000C3968 File Offset: 0x000C1B68
	public int themeBar
	{
		get
		{
			return this.currentBar + this.barOffset;
		}
	}

	// Token: 0x06001BE3 RID: 7139 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void RaiseIntensityTo(float amount, int holdLengthBars = 0)
	{
	}

	// Token: 0x06001BE4 RID: 7140 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopMusic()
	{
	}

	// Token: 0x02000C77 RID: 3191
	[Serializable]
	public class ClipPlaybackData
	{
		// Token: 0x04004358 RID: 17240
		public AudioSource source;

		// Token: 0x04004359 RID: 17241
		public MusicTheme.PositionedClip positionedClip;

		// Token: 0x0400435A RID: 17242
		public bool isActive;

		// Token: 0x0400435B RID: 17243
		public bool fadingIn;

		// Token: 0x0400435C RID: 17244
		public bool fadingOut;

		// Token: 0x0400435D RID: 17245
		public double fadeStarted;

		// Token: 0x0400435E RID: 17246
		public bool needsSync;
	}
}
