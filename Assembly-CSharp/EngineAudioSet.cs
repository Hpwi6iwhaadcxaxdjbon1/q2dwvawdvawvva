using System;
using UnityEngine;

// Token: 0x02000493 RID: 1171
[CreateAssetMenu(fileName = "Engine Audio Preset", menuName = "Rust/Vehicles/Engine Audio Preset")]
public class EngineAudioSet : ScriptableObject
{
	// Token: 0x04001ECC RID: 7884
	public BlendedEngineLoopDefinition[] engineAudioLoops;

	// Token: 0x04001ECD RID: 7885
	public int priority;

	// Token: 0x04001ECE RID: 7886
	public float idleVolume = 0.4f;

	// Token: 0x04001ECF RID: 7887
	public float maxVolume = 0.6f;

	// Token: 0x04001ED0 RID: 7888
	public float volumeChangeRateUp = 48f;

	// Token: 0x04001ED1 RID: 7889
	public float volumeChangeRateDown = 16f;

	// Token: 0x04001ED2 RID: 7890
	public float idlePitch = 0.25f;

	// Token: 0x04001ED3 RID: 7891
	public float maxPitch = 1.5f;

	// Token: 0x04001ED4 RID: 7892
	public float idleRpm = 600f;

	// Token: 0x04001ED5 RID: 7893
	public float gearUpRpm = 5000f;

	// Token: 0x04001ED6 RID: 7894
	public float gearDownRpm = 2500f;

	// Token: 0x04001ED7 RID: 7895
	public int numGears = 5;

	// Token: 0x04001ED8 RID: 7896
	public float maxRpm = 6000f;

	// Token: 0x04001ED9 RID: 7897
	public float gearUpRpmRate = 5f;

	// Token: 0x04001EDA RID: 7898
	public float gearDownRpmRate = 6f;

	// Token: 0x04001EDB RID: 7899
	public SoundDefinition badPerformanceLoop;

	// Token: 0x06002690 RID: 9872 RVA: 0x000F2018 File Offset: 0x000F0218
	public BlendedEngineLoopDefinition GetEngineLoopDef(int numEngines)
	{
		int num = (numEngines - 1) % this.engineAudioLoops.Length;
		return this.engineAudioLoops[num];
	}
}
