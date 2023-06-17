using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E1 RID: 2017
public class ExpandedLifeStats : MonoBehaviour
{
	// Token: 0x04002D19 RID: 11545
	public GameObject DisplayRoot;

	// Token: 0x04002D1A RID: 11546
	public GameObjectRef GenericStatRow;

	// Token: 0x04002D1B RID: 11547
	[Header("Resources")]
	public Transform ResourcesStatRoot;

	// Token: 0x04002D1C RID: 11548
	public List<ExpandedLifeStats.GenericStatDisplay> ResourceStats;

	// Token: 0x04002D1D RID: 11549
	[Header("Weapons")]
	public GameObjectRef WeaponStatRow;

	// Token: 0x04002D1E RID: 11550
	public Transform WeaponsRoot;

	// Token: 0x04002D1F RID: 11551
	[Header("Misc")]
	public Transform MiscRoot;

	// Token: 0x04002D20 RID: 11552
	public List<ExpandedLifeStats.GenericStatDisplay> MiscStats;

	// Token: 0x04002D21 RID: 11553
	public LifeInfographic Infographic;

	// Token: 0x04002D22 RID: 11554
	public RectTransform MoveRoot;

	// Token: 0x04002D23 RID: 11555
	public Vector2 OpenPosition;

	// Token: 0x04002D24 RID: 11556
	public Vector2 ClosedPosition;

	// Token: 0x04002D25 RID: 11557
	public GameObject OpenButtonRoot;

	// Token: 0x04002D26 RID: 11558
	public GameObject CloseButtonRoot;

	// Token: 0x04002D27 RID: 11559
	public GameObject ScrollGradient;

	// Token: 0x04002D28 RID: 11560
	public ScrollRect Scroller;

	// Token: 0x02000E7A RID: 3706
	[Serializable]
	public struct GenericStatDisplay
	{
		// Token: 0x04004BD7 RID: 19415
		public string statKey;

		// Token: 0x04004BD8 RID: 19416
		public Sprite statSprite;

		// Token: 0x04004BD9 RID: 19417
		public Translate.Phrase displayPhrase;
	}
}
