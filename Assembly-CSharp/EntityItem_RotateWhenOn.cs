using System;
using UnityEngine;

// Token: 0x020003C9 RID: 969
public class EntityItem_RotateWhenOn : EntityComponent<BaseEntity>
{
	// Token: 0x04001A1F RID: 6687
	public EntityItem_RotateWhenOn.State on;

	// Token: 0x04001A20 RID: 6688
	public EntityItem_RotateWhenOn.State off;

	// Token: 0x04001A21 RID: 6689
	internal bool currentlyOn;

	// Token: 0x04001A22 RID: 6690
	internal bool stateInitialized;

	// Token: 0x04001A23 RID: 6691
	public BaseEntity.Flags targetFlag = BaseEntity.Flags.On;

	// Token: 0x02000CBE RID: 3262
	[Serializable]
	public class State
	{
		// Token: 0x040044A7 RID: 17575
		public Vector3 rotation;

		// Token: 0x040044A8 RID: 17576
		public float initialDelay;

		// Token: 0x040044A9 RID: 17577
		public float timeToTake = 2f;

		// Token: 0x040044AA RID: 17578
		public AnimationCurve animationCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		});

		// Token: 0x040044AB RID: 17579
		public string effectOnStart = "";

		// Token: 0x040044AC RID: 17580
		public string effectOnFinish = "";

		// Token: 0x040044AD RID: 17581
		public SoundDefinition movementLoop;

		// Token: 0x040044AE RID: 17582
		public float movementLoopFadeOutTime = 0.1f;

		// Token: 0x040044AF RID: 17583
		public SoundDefinition startSound;

		// Token: 0x040044B0 RID: 17584
		public SoundDefinition stopSound;
	}
}
