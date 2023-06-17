using System;

// Token: 0x02000650 RID: 1616
[Serializable]
public struct NoiseParameters
{
	// Token: 0x0400265F RID: 9823
	public int Octaves;

	// Token: 0x04002660 RID: 9824
	public float Frequency;

	// Token: 0x04002661 RID: 9825
	public float Amplitude;

	// Token: 0x04002662 RID: 9826
	public float Offset;

	// Token: 0x06002ED2 RID: 11986 RVA: 0x00119A8A File Offset: 0x00117C8A
	public NoiseParameters(int octaves, float frequency, float amplitude, float offset)
	{
		this.Octaves = octaves;
		this.Frequency = frequency;
		this.Amplitude = amplitude;
		this.Offset = offset;
	}
}
