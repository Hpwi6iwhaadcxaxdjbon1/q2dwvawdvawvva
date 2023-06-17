using System;
using UnityEngine;

// Token: 0x020005B6 RID: 1462
[CreateAssetMenu(menuName = "Rust/Gestures/Gesture Collection")]
public class GestureCollection : ScriptableObject
{
	// Token: 0x040023B0 RID: 9136
	public GestureConfig[] AllGestures;

	// Token: 0x040023B1 RID: 9137
	public float GestureVmInDuration = 0.25f;

	// Token: 0x040023B2 RID: 9138
	public AnimationCurve GestureInCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040023B3 RID: 9139
	public float GestureVmOutDuration = 0.25f;

	// Token: 0x040023B4 RID: 9140
	public AnimationCurve GestureOutCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040023B5 RID: 9141
	public float GestureViewmodelDeployDelay = 0.25f;

	// Token: 0x06002C12 RID: 11282 RVA: 0x0010B0CC File Offset: 0x001092CC
	public GestureConfig IdToGesture(uint id)
	{
		foreach (GestureConfig gestureConfig in this.AllGestures)
		{
			if (gestureConfig.gestureId == id)
			{
				return gestureConfig;
			}
		}
		return null;
	}

	// Token: 0x06002C13 RID: 11283 RVA: 0x0010B100 File Offset: 0x00109300
	public GestureConfig StringToGesture(string gestureName)
	{
		foreach (GestureConfig gestureConfig in this.AllGestures)
		{
			if (gestureConfig.convarName == gestureName)
			{
				return gestureConfig;
			}
		}
		return null;
	}
}
