using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200023B RID: 571
public class SlicedGranularAudioClip : MonoBehaviour, IClientComponent
{
	// Token: 0x04001476 RID: 5238
	public AudioClip sourceClip;

	// Token: 0x04001477 RID: 5239
	public AudioClip granularClip;

	// Token: 0x04001478 RID: 5240
	public int sampleRate = 44100;

	// Token: 0x04001479 RID: 5241
	public float grainAttack = 0.1f;

	// Token: 0x0400147A RID: 5242
	public float grainSustain = 0.1f;

	// Token: 0x0400147B RID: 5243
	public float grainRelease = 0.1f;

	// Token: 0x0400147C RID: 5244
	public float grainFrequency = 0.1f;

	// Token: 0x0400147D RID: 5245
	public int grainAttackSamples;

	// Token: 0x0400147E RID: 5246
	public int grainSustainSamples;

	// Token: 0x0400147F RID: 5247
	public int grainReleaseSamples;

	// Token: 0x04001480 RID: 5248
	public int grainFrequencySamples;

	// Token: 0x04001481 RID: 5249
	public int samplesUntilNextGrain;

	// Token: 0x04001482 RID: 5250
	public List<SlicedGranularAudioClip.Grain> grains = new List<SlicedGranularAudioClip.Grain>();

	// Token: 0x04001483 RID: 5251
	public List<int> startPositions = new List<int>();

	// Token: 0x04001484 RID: 5252
	public int lastStartPositionIdx = int.MaxValue;

	// Token: 0x02000C7B RID: 3195
	public class Grain
	{
		// Token: 0x04004370 RID: 17264
		private float[] sourceData;

		// Token: 0x04004371 RID: 17265
		private int startSample;

		// Token: 0x04004372 RID: 17266
		private int currentSample;

		// Token: 0x04004373 RID: 17267
		private int attackTimeSamples;

		// Token: 0x04004374 RID: 17268
		private int sustainTimeSamples;

		// Token: 0x04004375 RID: 17269
		private int releaseTimeSamples;

		// Token: 0x04004376 RID: 17270
		private float gain;

		// Token: 0x04004377 RID: 17271
		private float gainPerSampleAttack;

		// Token: 0x04004378 RID: 17272
		private float gainPerSampleRelease;

		// Token: 0x04004379 RID: 17273
		private int attackEndSample;

		// Token: 0x0400437A RID: 17274
		private int releaseStartSample;

		// Token: 0x0400437B RID: 17275
		private int endSample;

		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x06004F04 RID: 20228 RVA: 0x001A565A File Offset: 0x001A385A
		public bool finished
		{
			get
			{
				return this.currentSample >= this.endSample;
			}
		}

		// Token: 0x06004F05 RID: 20229 RVA: 0x001A5670 File Offset: 0x001A3870
		public void Init(float[] source, int start, int attack, int sustain, int release)
		{
			this.sourceData = source;
			this.startSample = start;
			this.currentSample = start;
			this.attackTimeSamples = attack;
			this.sustainTimeSamples = sustain;
			this.releaseTimeSamples = release;
			this.gainPerSampleAttack = 0.5f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -0.5f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}

		// Token: 0x06004F06 RID: 20230 RVA: 0x001A5714 File Offset: 0x001A3914
		public float GetSample()
		{
			if (this.currentSample >= this.sourceData.Length)
			{
				return 0f;
			}
			float num = this.sourceData[this.currentSample];
			if (this.currentSample <= this.attackEndSample)
			{
				this.gain += this.gainPerSampleAttack;
				if (this.gain > 0.5f)
				{
					this.gain = 0.5f;
				}
			}
			else if (this.currentSample >= this.releaseStartSample)
			{
				this.gain += this.gainPerSampleRelease;
				if (this.gain < 0f)
				{
					this.gain = 0f;
				}
			}
			this.currentSample++;
			return num * this.gain;
		}

		// Token: 0x06004F07 RID: 20231 RVA: 0x001A57CD File Offset: 0x001A39CD
		public void FadeOut()
		{
			this.releaseStartSample = this.currentSample;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
		}
	}
}
