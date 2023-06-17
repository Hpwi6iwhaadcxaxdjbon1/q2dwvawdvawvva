using System;
using System.Collections.Generic;
using JSON;
using UnityEngine;

// Token: 0x02000229 RID: 553
public class EngineAudioClip : MonoBehaviour, IClientComponent
{
	// Token: 0x040013E7 RID: 5095
	public AudioClip granularClip;

	// Token: 0x040013E8 RID: 5096
	public AudioClip accelerationClip;

	// Token: 0x040013E9 RID: 5097
	public TextAsset accelerationCyclesJson;

	// Token: 0x040013EA RID: 5098
	public List<EngineAudioClip.EngineCycle> accelerationCycles = new List<EngineAudioClip.EngineCycle>();

	// Token: 0x040013EB RID: 5099
	public List<EngineAudioClip.EngineCycleBucket> cycleBuckets = new List<EngineAudioClip.EngineCycleBucket>();

	// Token: 0x040013EC RID: 5100
	public Dictionary<int, EngineAudioClip.EngineCycleBucket> accelerationCyclesByRPM = new Dictionary<int, EngineAudioClip.EngineCycleBucket>();

	// Token: 0x040013ED RID: 5101
	public Dictionary<int, int> rpmBucketLookup = new Dictionary<int, int>();

	// Token: 0x040013EE RID: 5102
	public int sampleRate = 44100;

	// Token: 0x040013EF RID: 5103
	public int samplesUntilNextGrain;

	// Token: 0x040013F0 RID: 5104
	public int lastCycleId;

	// Token: 0x040013F1 RID: 5105
	public List<EngineAudioClip.Grain> grains = new List<EngineAudioClip.Grain>();

	// Token: 0x040013F2 RID: 5106
	public int currentRPM;

	// Token: 0x040013F3 RID: 5107
	public int targetRPM = 1500;

	// Token: 0x040013F4 RID: 5108
	public int minRPM;

	// Token: 0x040013F5 RID: 5109
	public int maxRPM;

	// Token: 0x040013F6 RID: 5110
	public int cyclePadding;

	// Token: 0x040013F7 RID: 5111
	[Range(0f, 1f)]
	public float RPMControl;

	// Token: 0x040013F8 RID: 5112
	public AudioSource source;

	// Token: 0x040013F9 RID: 5113
	public float rpmLerpSpeed = 0.025f;

	// Token: 0x040013FA RID: 5114
	public float rpmLerpSpeedDown = 0.01f;

	// Token: 0x06001BCA RID: 7114 RVA: 0x000C3227 File Offset: 0x000C1427
	private int GetBucketRPM(int RPM)
	{
		return Mathf.RoundToInt((float)(RPM / 25)) * 25;
	}

	// Token: 0x02000C70 RID: 3184
	[Serializable]
	public class EngineCycle
	{
		// Token: 0x0400432E RID: 17198
		public int RPM;

		// Token: 0x0400432F RID: 17199
		public int startSample;

		// Token: 0x04004330 RID: 17200
		public int endSample;

		// Token: 0x04004331 RID: 17201
		public float period;

		// Token: 0x04004332 RID: 17202
		public int id;

		// Token: 0x06004EED RID: 20205 RVA: 0x001A50F0 File Offset: 0x001A32F0
		public EngineCycle(int RPM, int startSample, int endSample, float period, int id)
		{
			this.RPM = RPM;
			this.startSample = startSample;
			this.endSample = endSample;
			this.period = period;
			this.id = id;
		}
	}

	// Token: 0x02000C71 RID: 3185
	public class EngineCycleBucket
	{
		// Token: 0x04004333 RID: 17203
		public int RPM;

		// Token: 0x04004334 RID: 17204
		public List<EngineAudioClip.EngineCycle> cycles = new List<EngineAudioClip.EngineCycle>();

		// Token: 0x04004335 RID: 17205
		public List<int> remainingCycles = new List<int>();

		// Token: 0x06004EEE RID: 20206 RVA: 0x001A511D File Offset: 0x001A331D
		public EngineCycleBucket(int RPM)
		{
			this.RPM = RPM;
		}

		// Token: 0x06004EEF RID: 20207 RVA: 0x001A5144 File Offset: 0x001A3344
		public EngineAudioClip.EngineCycle GetCycle(System.Random random, int lastCycleId)
		{
			if (this.remainingCycles.Count == 0)
			{
				this.ResetRemainingCycles(random);
			}
			int index = this.remainingCycles.Pop<int>();
			if (this.cycles[index].id == lastCycleId)
			{
				if (this.remainingCycles.Count == 0)
				{
					this.ResetRemainingCycles(random);
				}
				index = this.remainingCycles.Pop<int>();
			}
			return this.cycles[index];
		}

		// Token: 0x06004EF0 RID: 20208 RVA: 0x001A51B4 File Offset: 0x001A33B4
		private void ResetRemainingCycles(System.Random random)
		{
			for (int i = 0; i < this.cycles.Count; i++)
			{
				this.remainingCycles.Add(i);
			}
			this.remainingCycles.Shuffle((uint)random.Next());
		}

		// Token: 0x06004EF1 RID: 20209 RVA: 0x001A51F4 File Offset: 0x001A33F4
		public void Add(EngineAudioClip.EngineCycle cycle)
		{
			if (!this.cycles.Contains(cycle))
			{
				this.cycles.Add(cycle);
			}
		}
	}

	// Token: 0x02000C72 RID: 3186
	public class Grain
	{
		// Token: 0x04004336 RID: 17206
		private float[] sourceData;

		// Token: 0x04004337 RID: 17207
		private int startSample;

		// Token: 0x04004338 RID: 17208
		private int currentSample;

		// Token: 0x04004339 RID: 17209
		private int attackTimeSamples;

		// Token: 0x0400433A RID: 17210
		private int sustainTimeSamples;

		// Token: 0x0400433B RID: 17211
		private int releaseTimeSamples;

		// Token: 0x0400433C RID: 17212
		private float gain;

		// Token: 0x0400433D RID: 17213
		private float gainPerSampleAttack;

		// Token: 0x0400433E RID: 17214
		private float gainPerSampleRelease;

		// Token: 0x0400433F RID: 17215
		private int attackEndSample;

		// Token: 0x04004340 RID: 17216
		private int releaseStartSample;

		// Token: 0x04004341 RID: 17217
		private int endSample;

		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x06004EF2 RID: 20210 RVA: 0x001A5210 File Offset: 0x001A3410
		public bool finished
		{
			get
			{
				return this.currentSample >= this.endSample;
			}
		}

		// Token: 0x06004EF3 RID: 20211 RVA: 0x001A5224 File Offset: 0x001A3424
		public void Init(float[] source, EngineAudioClip.EngineCycle cycle, int cyclePadding)
		{
			this.sourceData = source;
			this.startSample = cycle.startSample - cyclePadding;
			this.currentSample = this.startSample;
			this.attackTimeSamples = cyclePadding;
			this.sustainTimeSamples = cycle.endSample - cycle.startSample;
			this.releaseTimeSamples = cyclePadding;
			this.gainPerSampleAttack = 1f / (float)this.attackTimeSamples;
			this.gainPerSampleRelease = -1f / (float)this.releaseTimeSamples;
			this.attackEndSample = this.startSample + this.attackTimeSamples;
			this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
			this.endSample = this.releaseStartSample + this.releaseTimeSamples;
			this.gain = 0f;
		}

		// Token: 0x06004EF4 RID: 20212 RVA: 0x001A52E0 File Offset: 0x001A34E0
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
				if (this.gain > 0.8f)
				{
					this.gain = 0.8f;
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
	}
}
