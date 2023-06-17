using System;
using UnityEngine.Audio;

// Token: 0x0200022E RID: 558
public class MixerSnapshotManager : SingletonComponent<MixerSnapshotManager>, IClientComponent
{
	// Token: 0x04001416 RID: 5142
	public AudioMixerSnapshot defaultSnapshot;

	// Token: 0x04001417 RID: 5143
	public AudioMixerSnapshot underwaterSnapshot;

	// Token: 0x04001418 RID: 5144
	public AudioMixerSnapshot loadingSnapshot;

	// Token: 0x04001419 RID: 5145
	public AudioMixerSnapshot woundedSnapshot;

	// Token: 0x0400141A RID: 5146
	public AudioMixerSnapshot cctvSnapshot;

	// Token: 0x0400141B RID: 5147
	public SoundDefinition underwaterInSound;

	// Token: 0x0400141C RID: 5148
	public SoundDefinition underwaterOutSound;

	// Token: 0x0400141D RID: 5149
	public AudioMixerSnapshot recordingSnapshot;

	// Token: 0x0400141E RID: 5150
	public SoundDefinition woundedLoop;

	// Token: 0x0400141F RID: 5151
	private Sound woundedLoopSound;

	// Token: 0x04001420 RID: 5152
	public SoundDefinition cctvModeLoopDef;

	// Token: 0x04001421 RID: 5153
	private Sound cctvModeLoop;

	// Token: 0x04001422 RID: 5154
	public SoundDefinition cctvModeStartDef;

	// Token: 0x04001423 RID: 5155
	public SoundDefinition cctvModeStopDef;

	// Token: 0x04001424 RID: 5156
	public float deafness;
}
