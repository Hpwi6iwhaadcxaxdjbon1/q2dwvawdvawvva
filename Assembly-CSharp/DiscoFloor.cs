using System;
using UnityEngine;

// Token: 0x02000399 RID: 921
public class DiscoFloor : AudioVisualisationEntity
{
	// Token: 0x04001977 RID: 6519
	public float GradientDuration = 3f;

	// Token: 0x04001978 RID: 6520
	public float VolumeSensitivityMultiplier = 3f;

	// Token: 0x04001979 RID: 6521
	public float BaseSpeed;

	// Token: 0x0400197A RID: 6522
	public Light[] LightSources;

	// Token: 0x0400197B RID: 6523
	public DiscoFloorMesh FloorMesh;
}
