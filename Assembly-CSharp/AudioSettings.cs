using System;
using ConVar;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000291 RID: 657
public class AudioSettings : MonoBehaviour
{
	// Token: 0x040015DD RID: 5597
	public static float duckingFactor = 1f;

	// Token: 0x040015DE RID: 5598
	public AudioMixer mixer;

	// Token: 0x06001D0E RID: 7438 RVA: 0x000C8C00 File Offset: 0x000C6E00
	private void Update()
	{
		if (this.mixer == null)
		{
			return;
		}
		this.mixer.SetFloat("MasterVol", this.LinearToDecibel(Audio.master * global::AudioSettings.duckingFactor));
		float a;
		this.mixer.GetFloat("MusicVol", out a);
		if (!LevelManager.isLoaded || !MainCamera.isValid)
		{
			this.mixer.SetFloat("MusicVol", Mathf.Lerp(a, this.LinearToDecibel(Audio.musicvolumemenu), UnityEngine.Time.deltaTime));
		}
		else
		{
			this.mixer.SetFloat("MusicVol", Mathf.Lerp(a, this.LinearToDecibel(Audio.musicvolume), UnityEngine.Time.deltaTime));
		}
		float num = 1f - ((SingletonComponent<MixerSnapshotManager>.Instance == null) ? 0f : SingletonComponent<MixerSnapshotManager>.Instance.deafness);
		this.mixer.SetFloat("WorldVol", this.LinearToDecibel(Audio.game * num));
		this.mixer.SetFloat("WorldVolFlashbang", this.LinearToDecibel(Audio.game));
		this.mixer.SetFloat("VoiceVol", this.LinearToDecibel(Audio.voices * num));
		this.mixer.SetFloat("InstrumentVol", this.LinearToDecibel(Audio.instruments * num));
		float num2 = this.LinearToDecibel(Audio.voiceProps * num) - 28.7f;
		this.mixer.SetFloat("VoicePropsVol", num2 * num);
		this.mixer.SetFloat("SeasonalEventsVol", this.LinearToDecibel(Audio.eventAudio * num));
	}

	// Token: 0x06001D0F RID: 7439 RVA: 0x000C8D90 File Offset: 0x000C6F90
	private float LinearToDecibel(float linear)
	{
		float result;
		if (linear > 0f)
		{
			result = 20f * Mathf.Log10(linear);
		}
		else
		{
			result = -144f;
		}
		return result;
	}
}
