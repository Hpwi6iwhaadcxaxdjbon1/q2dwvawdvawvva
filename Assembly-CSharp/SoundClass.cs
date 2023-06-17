using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200023D RID: 573
[CreateAssetMenu(menuName = "Rust/Sound Class")]
public class SoundClass : ScriptableObject
{
	// Token: 0x0400148D RID: 5261
	[Header("Mixer Settings")]
	public AudioMixerGroup output;

	// Token: 0x0400148E RID: 5262
	public AudioMixerGroup firstPersonOutput;

	// Token: 0x0400148F RID: 5263
	[Header("Occlusion Settings")]
	public bool enableOcclusion;

	// Token: 0x04001490 RID: 5264
	public bool playIfOccluded = true;

	// Token: 0x04001491 RID: 5265
	public float occlusionGain = 1f;

	// Token: 0x04001492 RID: 5266
	[Tooltip("Use this mixer group when the sound is occluded to save DSP CPU usage. Only works for non-looping sounds.")]
	public AudioMixerGroup occludedOutput;

	// Token: 0x04001493 RID: 5267
	[Header("Voice Limiting")]
	public int globalVoiceMaxCount = 100;

	// Token: 0x04001494 RID: 5268
	public int priority = 128;

	// Token: 0x04001495 RID: 5269
	public List<SoundDefinition> definitions = new List<SoundDefinition>();
}
