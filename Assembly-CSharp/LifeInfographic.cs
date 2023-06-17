using System;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000860 RID: 2144
public class LifeInfographic : MonoBehaviour
{
	// Token: 0x04003023 RID: 12323
	[NonSerialized]
	public PlayerLifeStory life;

	// Token: 0x04003024 RID: 12324
	public GameObject container;

	// Token: 0x04003025 RID: 12325
	public RawImage AttackerAvatarImage;

	// Token: 0x04003026 RID: 12326
	public Image DamageSourceImage;

	// Token: 0x04003027 RID: 12327
	public LifeInfographicStat[] Stats;

	// Token: 0x04003028 RID: 12328
	public Animator[] AllAnimators;

	// Token: 0x04003029 RID: 12329
	public GameObject WeaponRoot;

	// Token: 0x0400302A RID: 12330
	public GameObject DistanceRoot;

	// Token: 0x0400302B RID: 12331
	public GameObject DistanceDivider;

	// Token: 0x0400302C RID: 12332
	public Image WeaponImage;

	// Token: 0x0400302D RID: 12333
	public LifeInfographic.DamageSetting[] DamageDisplays;

	// Token: 0x0400302E RID: 12334
	public Texture2D defaultAvatarTexture;

	// Token: 0x0400302F RID: 12335
	public bool ShowDebugData;

	// Token: 0x02000E8B RID: 3723
	[Serializable]
	public struct DamageSetting
	{
		// Token: 0x04004BFA RID: 19450
		public DamageType ForType;

		// Token: 0x04004BFB RID: 19451
		public string Display;

		// Token: 0x04004BFC RID: 19452
		public Sprite DamageSprite;
	}
}
