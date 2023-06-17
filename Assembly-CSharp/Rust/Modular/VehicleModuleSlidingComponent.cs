using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B2C RID: 2860
	[Serializable]
	public class VehicleModuleSlidingComponent
	{
		// Token: 0x04003DE0 RID: 15840
		public string interactionColliderName = "MyCollider";

		// Token: 0x04003DE1 RID: 15841
		public BaseEntity.Flags flag_SliderOpen = BaseEntity.Flags.Reserved3;

		// Token: 0x04003DE2 RID: 15842
		public float moveTime = 1f;

		// Token: 0x04003DE3 RID: 15843
		public VehicleModuleSlidingComponent.SlidingPart[] slidingParts;

		// Token: 0x04003DE4 RID: 15844
		public SoundDefinition openSoundDef;

		// Token: 0x04003DE5 RID: 15845
		public SoundDefinition closeSoundDef;

		// Token: 0x04003DE6 RID: 15846
		private float positionPercent;

		// Token: 0x06004566 RID: 17766 RVA: 0x00195EED File Offset: 0x001940ED
		public bool WantsOpenPos(BaseEntity parentEntity)
		{
			return parentEntity.HasFlag(this.flag_SliderOpen);
		}

		// Token: 0x06004567 RID: 17767 RVA: 0x00195EFB File Offset: 0x001940FB
		public void Use(BaseVehicleModule parentModule)
		{
			parentModule.SetFlag(this.flag_SliderOpen, !this.WantsOpenPos(parentModule), false, true);
		}

		// Token: 0x06004568 RID: 17768 RVA: 0x00195F15 File Offset: 0x00194115
		public void ServerUpdateTick(BaseVehicleModule parentModule)
		{
			this.CheckPosition(parentModule, Time.fixedDeltaTime);
		}

		// Token: 0x06004569 RID: 17769 RVA: 0x00195F24 File Offset: 0x00194124
		private void CheckPosition(BaseEntity parentEntity, float dt)
		{
			bool flag = this.WantsOpenPos(parentEntity);
			if (flag && this.positionPercent == 1f)
			{
				return;
			}
			if (!flag && this.positionPercent == 0f)
			{
				return;
			}
			float num = flag ? (dt / this.moveTime) : (-(dt / this.moveTime));
			this.positionPercent = Mathf.Clamp01(this.positionPercent + num);
			foreach (VehicleModuleSlidingComponent.SlidingPart slidingPart in this.slidingParts)
			{
				if (!(slidingPart.transform == null))
				{
					slidingPart.transform.localPosition = Vector3.Lerp(slidingPart.closedPosition, slidingPart.openPosition, this.positionPercent);
				}
			}
		}

		// Token: 0x02000F98 RID: 3992
		[Serializable]
		public class SlidingPart
		{
			// Token: 0x0400505A RID: 20570
			public Transform transform;

			// Token: 0x0400505B RID: 20571
			public Vector3 openPosition;

			// Token: 0x0400505C RID: 20572
			public Vector3 closedPosition;
		}
	}
}
