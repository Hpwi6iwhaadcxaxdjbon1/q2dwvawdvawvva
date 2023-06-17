using System;
using System.Collections;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B37 RID: 2871
	public class CoverPoint
	{
		// Token: 0x04003E08 RID: 15880
		public CoverPoint.CoverType NormalCoverType;

		// Token: 0x04003E09 RID: 15881
		public bool IsDynamic;

		// Token: 0x04003E0A RID: 15882
		public Transform SourceTransform;

		// Token: 0x04003E0B RID: 15883
		private Vector3 _staticPosition;

		// Token: 0x04003E0C RID: 15884
		private Vector3 _staticNormal;

		// Token: 0x17000655 RID: 1621
		// (get) Token: 0x06004595 RID: 17813 RVA: 0x00196982 File Offset: 0x00194B82
		// (set) Token: 0x06004596 RID: 17814 RVA: 0x0019698A File Offset: 0x00194B8A
		public CoverPointVolume Volume { get; private set; }

		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x06004597 RID: 17815 RVA: 0x00196993 File Offset: 0x00194B93
		// (set) Token: 0x06004598 RID: 17816 RVA: 0x001969BD File Offset: 0x00194BBD
		public Vector3 Position
		{
			get
			{
				if (this.IsDynamic && this.SourceTransform != null)
				{
					return this.SourceTransform.position;
				}
				return this._staticPosition;
			}
			set
			{
				this._staticPosition = value;
			}
		}

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x06004599 RID: 17817 RVA: 0x001969C6 File Offset: 0x00194BC6
		// (set) Token: 0x0600459A RID: 17818 RVA: 0x001969F0 File Offset: 0x00194BF0
		public Vector3 Normal
		{
			get
			{
				if (this.IsDynamic && this.SourceTransform != null)
				{
					return this.SourceTransform.forward;
				}
				return this._staticNormal;
			}
			set
			{
				this._staticNormal = value;
			}
		}

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x0600459B RID: 17819 RVA: 0x001969F9 File Offset: 0x00194BF9
		// (set) Token: 0x0600459C RID: 17820 RVA: 0x00196A01 File Offset: 0x00194C01
		public BaseEntity ReservedFor { get; set; }

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x0600459D RID: 17821 RVA: 0x00196A0A File Offset: 0x00194C0A
		public bool IsReserved
		{
			get
			{
				return this.ReservedFor != null;
			}
		}

		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x0600459E RID: 17822 RVA: 0x00196A18 File Offset: 0x00194C18
		// (set) Token: 0x0600459F RID: 17823 RVA: 0x00196A20 File Offset: 0x00194C20
		public bool IsCompromised { get; set; }

		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x060045A0 RID: 17824 RVA: 0x00196A29 File Offset: 0x00194C29
		// (set) Token: 0x060045A1 RID: 17825 RVA: 0x00196A31 File Offset: 0x00194C31
		public float Score { get; set; }

		// Token: 0x060045A2 RID: 17826 RVA: 0x00196A3A File Offset: 0x00194C3A
		public bool IsValidFor(BaseEntity entity)
		{
			return !this.IsCompromised && (this.ReservedFor == null || this.ReservedFor == entity);
		}

		// Token: 0x060045A3 RID: 17827 RVA: 0x00196A62 File Offset: 0x00194C62
		public CoverPoint(CoverPointVolume volume, float score)
		{
			this.Volume = volume;
			this.Score = score;
		}

		// Token: 0x060045A4 RID: 17828 RVA: 0x00196A78 File Offset: 0x00194C78
		public void CoverIsCompromised(float cooldown)
		{
			if (this.IsCompromised)
			{
				return;
			}
			if (this.Volume != null)
			{
				this.Volume.StartCoroutine(this.StartCooldown(cooldown));
			}
		}

		// Token: 0x060045A5 RID: 17829 RVA: 0x00196AA4 File Offset: 0x00194CA4
		private IEnumerator StartCooldown(float cooldown)
		{
			this.IsCompromised = true;
			yield return CoroutineEx.waitForSeconds(cooldown);
			this.IsCompromised = false;
			yield break;
		}

		// Token: 0x060045A6 RID: 17830 RVA: 0x00196ABC File Offset: 0x00194CBC
		public bool ProvidesCoverFromPoint(Vector3 point, float arcThreshold)
		{
			Vector3 normalized = (this.Position - point).normalized;
			return Vector3.Dot(this.Normal, normalized) < arcThreshold;
		}

		// Token: 0x02000F9A RID: 3994
		public enum CoverType
		{
			// Token: 0x04005061 RID: 20577
			Full,
			// Token: 0x04005062 RID: 20578
			Partial,
			// Token: 0x04005063 RID: 20579
			None
		}
	}
}
