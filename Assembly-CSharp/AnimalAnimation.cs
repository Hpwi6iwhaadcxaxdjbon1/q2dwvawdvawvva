using System;
using UnityEngine;

// Token: 0x02000288 RID: 648
public class AnimalAnimation : MonoBehaviour, IClientComponent
{
	// Token: 0x040015AA RID: 5546
	public BaseEntity Entity;

	// Token: 0x040015AB RID: 5547
	public BaseNpc Target;

	// Token: 0x040015AC RID: 5548
	public Animator Animator;

	// Token: 0x040015AD RID: 5549
	public MaterialEffect FootstepEffects;

	// Token: 0x040015AE RID: 5550
	public Transform[] Feet;

	// Token: 0x040015AF RID: 5551
	public SoundDefinition saddleMovementSoundDef;

	// Token: 0x040015B0 RID: 5552
	public SoundDefinition saddleMovementSoundDefWood;

	// Token: 0x040015B1 RID: 5553
	public SoundDefinition saddleMovementSoundDefRoadsign;

	// Token: 0x040015B2 RID: 5554
	public AnimationCurve saddleMovementGainCurve;

	// Token: 0x040015B3 RID: 5555
	[Tooltip("Ensure there is a float param called idleOffset if this is enabled")]
	public bool hasIdleOffset;

	// Token: 0x040015B4 RID: 5556
	[ReadOnly]
	public string BaseFolder;

	// Token: 0x040015B5 RID: 5557
	public const BaseEntity.Flags Flag_WoodArmor = BaseEntity.Flags.Reserved5;

	// Token: 0x040015B6 RID: 5558
	public const BaseEntity.Flags Flag_RoadsignArmor = BaseEntity.Flags.Reserved6;
}
