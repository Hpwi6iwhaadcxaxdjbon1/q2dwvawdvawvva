using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200023F RID: 575
public class SoundDefinition : ScriptableObject
{
	// Token: 0x04001498 RID: 5272
	public GameObjectRef template;

	// Token: 0x04001499 RID: 5273
	[Horizontal(2, -1)]
	public List<WeightedAudioClip> weightedAudioClips = new List<WeightedAudioClip>
	{
		new WeightedAudioClip()
	};

	// Token: 0x0400149A RID: 5274
	public List<SoundDefinition.DistanceAudioClipList> distanceAudioClips;

	// Token: 0x0400149B RID: 5275
	public SoundClass soundClass;

	// Token: 0x0400149C RID: 5276
	public bool defaultToFirstPerson;

	// Token: 0x0400149D RID: 5277
	public bool loop;

	// Token: 0x0400149E RID: 5278
	public bool randomizeStartPosition;

	// Token: 0x0400149F RID: 5279
	public bool useHighQualityFades;

	// Token: 0x040014A0 RID: 5280
	[Range(0f, 1f)]
	public float volume = 1f;

	// Token: 0x040014A1 RID: 5281
	[Range(0f, 1f)]
	public float volumeVariation;

	// Token: 0x040014A2 RID: 5282
	[Range(-3f, 3f)]
	public float pitch = 1f;

	// Token: 0x040014A3 RID: 5283
	[Range(0f, 1f)]
	public float pitchVariation;

	// Token: 0x040014A4 RID: 5284
	[Header("Voice limiting")]
	public bool dontVoiceLimit;

	// Token: 0x040014A5 RID: 5285
	public int globalVoiceMaxCount = 100;

	// Token: 0x040014A6 RID: 5286
	public int localVoiceMaxCount = 100;

	// Token: 0x040014A7 RID: 5287
	public float localVoiceRange = 10f;

	// Token: 0x040014A8 RID: 5288
	public float voiceLimitFadeOutTime = 0.05f;

	// Token: 0x040014A9 RID: 5289
	public float localVoiceDebounceTime = 0.1f;

	// Token: 0x040014AA RID: 5290
	[Header("Occlusion Settings")]
	public bool forceOccludedPlayback;

	// Token: 0x040014AB RID: 5291
	[Header("Doppler")]
	public bool enableDoppler;

	// Token: 0x040014AC RID: 5292
	public float dopplerAmount = 0.18f;

	// Token: 0x040014AD RID: 5293
	public float dopplerScale = 1f;

	// Token: 0x040014AE RID: 5294
	public float dopplerAdjustmentRate = 1f;

	// Token: 0x040014AF RID: 5295
	[Header("Custom curves")]
	public AnimationCurve falloffCurve;

	// Token: 0x040014B0 RID: 5296
	public bool useCustomFalloffCurve;

	// Token: 0x040014B1 RID: 5297
	public AnimationCurve spatialBlendCurve;

	// Token: 0x040014B2 RID: 5298
	public bool useCustomSpatialBlendCurve;

	// Token: 0x040014B3 RID: 5299
	public AnimationCurve spreadCurve;

	// Token: 0x040014B4 RID: 5300
	public bool useCustomSpreadCurve;

	// Token: 0x1700025F RID: 607
	// (get) Token: 0x06001C0F RID: 7183 RVA: 0x000C40B8 File Offset: 0x000C22B8
	public float maxDistance
	{
		get
		{
			if (this.template == null)
			{
				return 0f;
			}
			AudioSource component = this.template.Get().GetComponent<AudioSource>();
			if (component == null)
			{
				return 0f;
			}
			return component.maxDistance;
		}
	}

	// Token: 0x06001C10 RID: 7184 RVA: 0x000C40FC File Offset: 0x000C22FC
	public float GetLength()
	{
		float num = 0f;
		for (int i = 0; i < this.weightedAudioClips.Count; i++)
		{
			AudioClip audioClip = this.weightedAudioClips[i].audioClip;
			if (audioClip)
			{
				num = Mathf.Max(audioClip.length, num);
			}
		}
		for (int j = 0; j < this.distanceAudioClips.Count; j++)
		{
			List<WeightedAudioClip> audioClips = this.distanceAudioClips[j].audioClips;
			for (int k = 0; k < audioClips.Count; k++)
			{
				AudioClip audioClip2 = audioClips[k].audioClip;
				if (audioClip2)
				{
					num = Mathf.Max(audioClip2.length, num);
				}
			}
		}
		float num2 = 1f / (this.pitch - this.pitchVariation);
		return num * num2;
	}

	// Token: 0x06001C11 RID: 7185 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
	public Sound Play()
	{
		return null;
	}

	// Token: 0x06001C12 RID: 7186 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
	public Sound Play(GameObject forGameObject)
	{
		return null;
	}

	// Token: 0x02000C7C RID: 3196
	[Serializable]
	public class DistanceAudioClipList
	{
		// Token: 0x0400437C RID: 17276
		public int distance;

		// Token: 0x0400437D RID: 17277
		[Horizontal(2, -1)]
		public List<WeightedAudioClip> audioClips;
	}
}
