using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200022C RID: 556
public class GranularAudioClip : MonoBehaviour
{
	// Token: 0x04001403 RID: 5123
	public AudioClip sourceClip;

	// Token: 0x04001404 RID: 5124
	private float[] sourceAudioData;

	// Token: 0x04001405 RID: 5125
	private int sourceChannels = 1;

	// Token: 0x04001406 RID: 5126
	public AudioClip granularClip;

	// Token: 0x04001407 RID: 5127
	public int sampleRate = 44100;

	// Token: 0x04001408 RID: 5128
	public float sourceTime = 0.5f;

	// Token: 0x04001409 RID: 5129
	public float sourceTimeVariation = 0.1f;

	// Token: 0x0400140A RID: 5130
	public float grainAttack = 0.1f;

	// Token: 0x0400140B RID: 5131
	public float grainSustain = 0.1f;

	// Token: 0x0400140C RID: 5132
	public float grainRelease = 0.1f;

	// Token: 0x0400140D RID: 5133
	public float grainFrequency = 0.1f;

	// Token: 0x0400140E RID: 5134
	public int grainAttackSamples;

	// Token: 0x0400140F RID: 5135
	public int grainSustainSamples;

	// Token: 0x04001410 RID: 5136
	public int grainReleaseSamples;

	// Token: 0x04001411 RID: 5137
	public int grainFrequencySamples;

	// Token: 0x04001412 RID: 5138
	public int samplesUntilNextGrain;

	// Token: 0x04001413 RID: 5139
	public List<GranularAudioClip.Grain> grains = new List<GranularAudioClip.Grain>();

	// Token: 0x04001414 RID: 5140
	private System.Random random = new System.Random();

	// Token: 0x04001415 RID: 5141
	private bool inited;

	// Token: 0x06001BCE RID: 7118 RVA: 0x000C32CC File Offset: 0x000C14CC
	private void Update()
	{
		if (!this.inited && this.sourceClip.loadState == AudioDataLoadState.Loaded)
		{
			this.sampleRate = this.sourceClip.frequency;
			this.sourceAudioData = new float[this.sourceClip.samples * this.sourceClip.channels];
			this.sourceClip.GetData(this.sourceAudioData, 0);
			this.InitAudioClip();
			AudioSource component = base.GetComponent<AudioSource>();
			component.clip = this.granularClip;
			component.loop = true;
			component.Play();
			this.inited = true;
		}
		this.RefreshCachedData();
	}

	// Token: 0x06001BCF RID: 7119 RVA: 0x000C3368 File Offset: 0x000C1568
	private void RefreshCachedData()
	{
		this.grainAttackSamples = Mathf.FloorToInt(this.grainAttack * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainSustainSamples = Mathf.FloorToInt(this.grainSustain * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainReleaseSamples = Mathf.FloorToInt(this.grainRelease * (float)this.sampleRate * (float)this.sourceChannels);
		this.grainFrequencySamples = Mathf.FloorToInt(this.grainFrequency * (float)this.sampleRate * (float)this.sourceChannels);
	}

	// Token: 0x06001BD0 RID: 7120 RVA: 0x000C33FC File Offset: 0x000C15FC
	private void InitAudioClip()
	{
		int lengthSamples = 1;
		int num = 1;
		UnityEngine.AudioSettings.GetDSPBufferSize(out lengthSamples, out num);
		this.granularClip = AudioClip.Create(this.sourceClip.name + " (granular)", lengthSamples, this.sourceClip.channels, this.sampleRate, true, new AudioClip.PCMReaderCallback(this.OnAudioRead));
		this.sourceChannels = this.sourceClip.channels;
	}

	// Token: 0x06001BD1 RID: 7121 RVA: 0x000C3468 File Offset: 0x000C1668
	private void OnAudioRead(float[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			if (this.samplesUntilNextGrain <= 0)
			{
				this.SpawnGrain();
			}
			float num = 0f;
			for (int j = 0; j < this.grains.Count; j++)
			{
				num += this.grains[j].GetSample();
			}
			data[i] = num;
			this.samplesUntilNextGrain--;
		}
		this.CleanupFinishedGrains();
	}

	// Token: 0x06001BD2 RID: 7122 RVA: 0x000C34DC File Offset: 0x000C16DC
	private void SpawnGrain()
	{
		if (this.grainFrequencySamples == 0)
		{
			return;
		}
		float num = (float)(this.random.NextDouble() * (double)this.sourceTimeVariation * 2.0) - this.sourceTimeVariation;
		int start = Mathf.FloorToInt((this.sourceTime + num) * (float)this.sampleRate / (float)this.sourceChannels);
		GranularAudioClip.Grain grain = Pool.Get<GranularAudioClip.Grain>();
		grain.Init(this.sourceAudioData, start, this.grainAttackSamples, this.grainSustainSamples, this.grainReleaseSamples);
		this.grains.Add(grain);
		this.samplesUntilNextGrain = this.grainFrequencySamples;
	}

	// Token: 0x06001BD3 RID: 7123 RVA: 0x000C3574 File Offset: 0x000C1774
	private void CleanupFinishedGrains()
	{
		for (int i = this.grains.Count - 1; i >= 0; i--)
		{
			GranularAudioClip.Grain grain = this.grains[i];
			if (grain.finished)
			{
				Pool.Free<GranularAudioClip.Grain>(ref grain);
				this.grains.RemoveAt(i);
			}
		}
	}

	// Token: 0x02000C74 RID: 3188
	public class Grain
	{
		// Token: 0x04004346 RID: 17222
		private float[] sourceData;

		// Token: 0x04004347 RID: 17223
		private int sourceDataLength;

		// Token: 0x04004348 RID: 17224
		private int startSample;

		// Token: 0x04004349 RID: 17225
		private int currentSample;

		// Token: 0x0400434A RID: 17226
		private int attackTimeSamples;

		// Token: 0x0400434B RID: 17227
		private int sustainTimeSamples;

		// Token: 0x0400434C RID: 17228
		private int releaseTimeSamples;

		// Token: 0x0400434D RID: 17229
		private float gain;

		// Token: 0x0400434E RID: 17230
		private float gainPerSampleAttack;

		// Token: 0x0400434F RID: 17231
		private float gainPerSampleRelease;

		// Token: 0x04004350 RID: 17232
		private int attackEndSample;

		// Token: 0x04004351 RID: 17233
		private int releaseStartSample;

		// Token: 0x04004352 RID: 17234
		private int endSample;

		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x06004EF6 RID: 20214 RVA: 0x001A5399 File Offset: 0x001A3599
		public bool finished
		{
			get
			{
				return this.currentSample >= this.endSample;
			}
		}

		// Token: 0x06004EF7 RID: 20215 RVA: 0x001A53AC File Offset: 0x001A35AC
		public void Init(float[] source, int start, int attack, int sustain, int release)
		{
			this.sourceData = source;
			this.sourceDataLength = this.sourceData.Length;
			this.startSample = start;
			this.currentSample = start;
			this.attackTimeSamples = attack;
			this.sustainTimeSamples = sustain;
			this.releaseTimeSamples = release;
			this.gainPerSampleAttack = 1f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -1f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}

		// Token: 0x06004EF8 RID: 20216 RVA: 0x001A5460 File Offset: 0x001A3660
		public float GetSample()
		{
			int num = this.currentSample % this.sourceDataLength;
			if (num < 0)
			{
				num += this.sourceDataLength;
			}
			float num2 = this.sourceData[num];
			if (this.currentSample <= this.attackEndSample)
			{
				this.gain += this.gainPerSampleAttack;
			}
			else if (this.currentSample >= this.releaseStartSample)
			{
				this.gain += this.gainPerSampleRelease;
			}
			this.currentSample++;
			return num2 * this.gain;
		}
	}
}
