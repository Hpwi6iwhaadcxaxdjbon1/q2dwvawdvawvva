using System;
using UnityEngine;

// Token: 0x020001C6 RID: 454
public class EffectMount : EntityComponent<BaseEntity>, IClientComponent
{
	// Token: 0x040011CC RID: 4556
	public bool firstPerson;

	// Token: 0x040011CD RID: 4557
	public GameObject effectPrefab;

	// Token: 0x040011CE RID: 4558
	public GameObject spawnedEffect;

	// Token: 0x040011CF RID: 4559
	public GameObject mountBone;

	// Token: 0x040011D0 RID: 4560
	public SoundDefinition onSoundDef;

	// Token: 0x040011D1 RID: 4561
	public SoundDefinition offSoundDef;

	// Token: 0x040011D2 RID: 4562
	public bool blockOffSoundWhenGettingDisabled;
}
