using System;
using UnityEngine;

// Token: 0x02000227 RID: 551
public class BlendedLoopEngineSound : MonoBehaviour, IClientComponent
{
	// Token: 0x040013D8 RID: 5080
	public BlendedEngineLoopDefinition loopDefinition;

	// Token: 0x040013D9 RID: 5081
	public bool engineOn;

	// Token: 0x040013DA RID: 5082
	[Range(0f, 1f)]
	public float RPMControl;

	// Token: 0x040013DB RID: 5083
	public float smoothedRPMControl;

	// Token: 0x040013DC RID: 5084
	private BlendedLoopEngineSound.EngineLoop[] engineLoops;

	// Token: 0x06001BC4 RID: 7108 RVA: 0x000C3126 File Offset: 0x000C1326
	public BlendedLoopEngineSound.EngineLoop[] GetEngineLoops()
	{
		return this.engineLoops;
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x000C312E File Offset: 0x000C132E
	public float GetLoopGain(int idx)
	{
		if (this.engineLoops != null && this.engineLoops[idx] != null && this.engineLoops[idx].gainMod != null)
		{
			return this.engineLoops[idx].gainMod.value;
		}
		return 0f;
	}

	// Token: 0x06001BC6 RID: 7110 RVA: 0x000C3169 File Offset: 0x000C1369
	public float GetLoopPitch(int idx)
	{
		if (this.engineLoops != null && this.engineLoops[idx] != null && this.engineLoops[idx].pitchMod != null)
		{
			return this.engineLoops[idx].pitchMod.value;
		}
		return 0f;
	}

	// Token: 0x17000257 RID: 599
	// (get) Token: 0x06001BC7 RID: 7111 RVA: 0x000C31A4 File Offset: 0x000C13A4
	public float maxDistance
	{
		get
		{
			return this.loopDefinition.engineLoops[0].soundDefinition.maxDistance;
		}
	}

	// Token: 0x02000C6E RID: 3182
	public class EngineLoop
	{
		// Token: 0x04004323 RID: 17187
		public BlendedEngineLoopDefinition.EngineLoopDefinition definition;

		// Token: 0x04004324 RID: 17188
		public BlendedLoopEngineSound parent;

		// Token: 0x04004325 RID: 17189
		public Sound sound;

		// Token: 0x04004326 RID: 17190
		public SoundModulation.Modulator gainMod;

		// Token: 0x04004327 RID: 17191
		public SoundModulation.Modulator pitchMod;
	}
}
