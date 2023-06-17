using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000232 RID: 562
public class MusicClipLoader
{
	// Token: 0x04001430 RID: 5168
	public List<MusicClipLoader.LoadedAudioClip> loadedClips = new List<MusicClipLoader.LoadedAudioClip>();

	// Token: 0x04001431 RID: 5169
	public Dictionary<AudioClip, MusicClipLoader.LoadedAudioClip> loadedClipDict = new Dictionary<AudioClip, MusicClipLoader.LoadedAudioClip>();

	// Token: 0x04001432 RID: 5170
	public List<AudioClip> clipsToLoad = new List<AudioClip>();

	// Token: 0x04001433 RID: 5171
	public List<AudioClip> clipsToUnload = new List<AudioClip>();

	// Token: 0x06001BDD RID: 7133 RVA: 0x000C3714 File Offset: 0x000C1914
	public void Update()
	{
		for (int i = this.clipsToLoad.Count - 1; i >= 0; i--)
		{
			AudioClip audioClip = this.clipsToLoad[i];
			if (audioClip.loadState != AudioDataLoadState.Loaded && audioClip.loadState != AudioDataLoadState.Loading)
			{
				audioClip.LoadAudioData();
				this.clipsToLoad.RemoveAt(i);
				return;
			}
		}
		for (int j = this.clipsToUnload.Count - 1; j >= 0; j--)
		{
			AudioClip audioClip2 = this.clipsToUnload[j];
			if (audioClip2.loadState == AudioDataLoadState.Loaded)
			{
				audioClip2.UnloadAudioData();
				this.clipsToUnload.RemoveAt(j);
				return;
			}
		}
	}

	// Token: 0x06001BDE RID: 7134 RVA: 0x000C37B0 File Offset: 0x000C19B0
	public void Refresh()
	{
		for (int i = 0; i < SingletonComponent<MusicManager>.Instance.activeMusicClips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = SingletonComponent<MusicManager>.Instance.activeMusicClips[i];
			MusicClipLoader.LoadedAudioClip loadedAudioClip = this.FindLoadedClip(positionedClip.musicClip.audioClip);
			if (loadedAudioClip == null)
			{
				loadedAudioClip = Pool.Get<MusicClipLoader.LoadedAudioClip>();
				loadedAudioClip.clip = positionedClip.musicClip.audioClip;
				loadedAudioClip.unloadTime = (float)UnityEngine.AudioSettings.dspTime + loadedAudioClip.clip.length + 1f;
				this.loadedClips.Add(loadedAudioClip);
				this.loadedClipDict.Add(loadedAudioClip.clip, loadedAudioClip);
				this.clipsToLoad.Add(loadedAudioClip.clip);
			}
			else
			{
				loadedAudioClip.unloadTime = (float)UnityEngine.AudioSettings.dspTime + loadedAudioClip.clip.length + 1f;
				this.clipsToUnload.Remove(loadedAudioClip.clip);
			}
		}
		for (int j = this.loadedClips.Count - 1; j >= 0; j--)
		{
			MusicClipLoader.LoadedAudioClip loadedAudioClip2 = this.loadedClips[j];
			if (UnityEngine.AudioSettings.dspTime > (double)loadedAudioClip2.unloadTime)
			{
				this.clipsToUnload.Add(loadedAudioClip2.clip);
				this.loadedClips.Remove(loadedAudioClip2);
				this.loadedClipDict.Remove(loadedAudioClip2.clip);
				Pool.Free<MusicClipLoader.LoadedAudioClip>(ref loadedAudioClip2);
			}
		}
	}

	// Token: 0x06001BDF RID: 7135 RVA: 0x000C3908 File Offset: 0x000C1B08
	private MusicClipLoader.LoadedAudioClip FindLoadedClip(AudioClip clip)
	{
		if (this.loadedClipDict.ContainsKey(clip))
		{
			return this.loadedClipDict[clip];
		}
		return null;
	}

	// Token: 0x02000C76 RID: 3190
	public class LoadedAudioClip
	{
		// Token: 0x04004356 RID: 17238
		public AudioClip clip;

		// Token: 0x04004357 RID: 17239
		public float unloadTime;
	}
}
