using System;
using UnityEngine;

// Token: 0x020002C6 RID: 710
public class LightGroupAtTime : FacepunchBehaviour
{
	// Token: 0x04001672 RID: 5746
	public float IntensityOverride = 1f;

	// Token: 0x04001673 RID: 5747
	public AnimationCurve IntensityScaleOverTime = new AnimationCurve
	{
		keys = new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(8f, 0f),
			new Keyframe(12f, 0f),
			new Keyframe(19f, 1f),
			new Keyframe(24f, 1f)
		}
	};

	// Token: 0x04001674 RID: 5748
	public Transform SearchRoot;

	// Token: 0x04001675 RID: 5749
	[Header("Power Settings")]
	public bool requiresPower;

	// Token: 0x04001676 RID: 5750
	[Tooltip("Can NOT be entity, use new blank gameobject!")]
	public Transform powerOverrideTransform;

	// Token: 0x04001677 RID: 5751
	public LayerMask checkLayers = 1235288065;

	// Token: 0x04001678 RID: 5752
	public GameObject enableWhenLightsOn;

	// Token: 0x04001679 RID: 5753
	public float timeBetweenPowerLookup = 10f;
}
