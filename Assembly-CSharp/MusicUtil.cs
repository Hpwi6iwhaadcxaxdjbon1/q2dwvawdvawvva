using System;
using UnityEngine;

// Token: 0x02000235 RID: 565
public class MusicUtil
{
	// Token: 0x0400145D RID: 5213
	public const float OneSixteenth = 0.0625f;

	// Token: 0x06001BF2 RID: 7154 RVA: 0x000C3EAB File Offset: 0x000C20AB
	public static double BeatsToSeconds(float tempo, float beats)
	{
		return 60.0 / (double)tempo * (double)beats;
	}

	// Token: 0x06001BF3 RID: 7155 RVA: 0x000C3EBC File Offset: 0x000C20BC
	public static double BarsToSeconds(float tempo, float bars)
	{
		return MusicUtil.BeatsToSeconds(tempo, bars * 4f);
	}

	// Token: 0x06001BF4 RID: 7156 RVA: 0x000C3ECB File Offset: 0x000C20CB
	public static int SecondsToSamples(double seconds)
	{
		return MusicUtil.SecondsToSamples(seconds, UnityEngine.AudioSettings.outputSampleRate);
	}

	// Token: 0x06001BF5 RID: 7157 RVA: 0x000C3ED8 File Offset: 0x000C20D8
	public static int SecondsToSamples(double seconds, int sampleRate)
	{
		return (int)((double)sampleRate * seconds);
	}

	// Token: 0x06001BF6 RID: 7158 RVA: 0x000C3EDF File Offset: 0x000C20DF
	public static int SecondsToSamples(float seconds)
	{
		return MusicUtil.SecondsToSamples(seconds, UnityEngine.AudioSettings.outputSampleRate);
	}

	// Token: 0x06001BF7 RID: 7159 RVA: 0x000C3EEC File Offset: 0x000C20EC
	public static int SecondsToSamples(float seconds, int sampleRate)
	{
		return (int)((float)sampleRate * seconds);
	}

	// Token: 0x06001BF8 RID: 7160 RVA: 0x000C3EF3 File Offset: 0x000C20F3
	public static int BarsToSamples(float tempo, float bars, int sampleRate)
	{
		return MusicUtil.SecondsToSamples(MusicUtil.BarsToSeconds(tempo, bars), sampleRate);
	}

	// Token: 0x06001BF9 RID: 7161 RVA: 0x000C3F02 File Offset: 0x000C2102
	public static int BarsToSamples(float tempo, float bars)
	{
		return MusicUtil.SecondsToSamples(MusicUtil.BarsToSeconds(tempo, bars));
	}

	// Token: 0x06001BFA RID: 7162 RVA: 0x000C3F10 File Offset: 0x000C2110
	public static int BeatsToSamples(float tempo, float beats)
	{
		return MusicUtil.SecondsToSamples(MusicUtil.BeatsToSeconds(tempo, beats));
	}

	// Token: 0x06001BFB RID: 7163 RVA: 0x000C3F1E File Offset: 0x000C211E
	public static float SecondsToBeats(float tempo, double seconds)
	{
		return tempo / 60f * (float)seconds;
	}

	// Token: 0x06001BFC RID: 7164 RVA: 0x000C3F2A File Offset: 0x000C212A
	public static float SecondsToBars(float tempo, double seconds)
	{
		return MusicUtil.SecondsToBeats(tempo, seconds) / 4f;
	}

	// Token: 0x06001BFD RID: 7165 RVA: 0x000C3F39 File Offset: 0x000C2139
	public static float Quantize(float position, float gridSize)
	{
		return Mathf.Round(position / gridSize) * gridSize;
	}

	// Token: 0x06001BFE RID: 7166 RVA: 0x000C3F45 File Offset: 0x000C2145
	public static float FlooredQuantize(float position, float gridSize)
	{
		return Mathf.Floor(position / gridSize) * gridSize;
	}
}
