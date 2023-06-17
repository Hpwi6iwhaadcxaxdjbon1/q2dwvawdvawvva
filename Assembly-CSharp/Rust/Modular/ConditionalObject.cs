using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B23 RID: 2851
	[Serializable]
	public class ConditionalObject
	{
		// Token: 0x04003DA7 RID: 15783
		public GameObject gameObject;

		// Token: 0x04003DA8 RID: 15784
		public GameObject ownerGameObject;

		// Token: 0x04003DA9 RID: 15785
		public ConditionalSocketSettings[] socketSettings;

		// Token: 0x04003DAA RID: 15786
		public bool restrictOnHealth;

		// Token: 0x04003DAB RID: 15787
		public float healthRestrictionMin;

		// Token: 0x04003DAC RID: 15788
		public float healthRestrictionMax;

		// Token: 0x04003DAD RID: 15789
		public bool restrictOnAdjacent;

		// Token: 0x04003DAE RID: 15790
		public ConditionalObject.AdjacentCondition adjacentRestriction;

		// Token: 0x04003DAF RID: 15791
		public ConditionalObject.AdjacentMatchType adjacentMatch;

		// Token: 0x04003DB0 RID: 15792
		public bool restrictOnLockable;

		// Token: 0x04003DB1 RID: 15793
		public bool lockableRestriction;

		// Token: 0x17000640 RID: 1600
		// (get) Token: 0x06004520 RID: 17696 RVA: 0x00195113 File Offset: 0x00193313
		// (set) Token: 0x06004521 RID: 17697 RVA: 0x0019511B File Offset: 0x0019331B
		public bool? IsActive { get; private set; }

		// Token: 0x06004522 RID: 17698 RVA: 0x00195124 File Offset: 0x00193324
		public ConditionalObject(GameObject conditionalGO, GameObject ownerGO, int socketsTaken)
		{
			this.gameObject = conditionalGO;
			this.ownerGameObject = ownerGO;
			this.socketSettings = new ConditionalSocketSettings[socketsTaken];
		}

		// Token: 0x06004523 RID: 17699 RVA: 0x00195148 File Offset: 0x00193348
		public void SetActive(bool active)
		{
			if (this.IsActive != null && active == this.IsActive.Value)
			{
				return;
			}
			this.gameObject.SetActive(active);
			this.IsActive = new bool?(active);
		}

		// Token: 0x06004524 RID: 17700 RVA: 0x00195190 File Offset: 0x00193390
		public void RefreshActive()
		{
			if (this.IsActive == null)
			{
				return;
			}
			this.gameObject.SetActive(this.IsActive.Value);
		}

		// Token: 0x02000F91 RID: 3985
		public enum AdjacentCondition
		{
			// Token: 0x04005039 RID: 20537
			SameInFront,
			// Token: 0x0400503A RID: 20538
			SameBehind,
			// Token: 0x0400503B RID: 20539
			DifferentInFront,
			// Token: 0x0400503C RID: 20540
			DifferentBehind,
			// Token: 0x0400503D RID: 20541
			BothDifferent,
			// Token: 0x0400503E RID: 20542
			BothSame
		}

		// Token: 0x02000F92 RID: 3986
		public enum AdjacentMatchType
		{
			// Token: 0x04005040 RID: 20544
			GroupOrExact,
			// Token: 0x04005041 RID: 20545
			ExactOnly,
			// Token: 0x04005042 RID: 20546
			GroupNotExact
		}
	}
}
