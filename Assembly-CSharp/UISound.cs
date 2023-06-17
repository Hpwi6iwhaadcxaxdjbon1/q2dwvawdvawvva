using System;
using ConVar;
using UnityEngine;

// Token: 0x020008A1 RID: 2209
public static class UISound
{
	// Token: 0x0400319D RID: 12701
	private static AudioSource source;

	// Token: 0x06003705 RID: 14085 RVA: 0x0014C2C8 File Offset: 0x0014A4C8
	private static AudioSource GetAudioSource()
	{
		if (UISound.source != null)
		{
			return UISound.source;
		}
		UISound.source = new GameObject("UISound").AddComponent<AudioSource>();
		UISound.source.spatialBlend = 0f;
		UISound.source.volume = 1f;
		return UISound.source;
	}

	// Token: 0x06003706 RID: 14086 RVA: 0x0014C31F File Offset: 0x0014A51F
	public static void Play(AudioClip clip, float volume = 1f)
	{
		if (clip == null)
		{
			return;
		}
		UISound.GetAudioSource().volume = volume * Audio.master * 0.4f;
		UISound.GetAudioSource().PlayOneShot(clip);
	}
}
