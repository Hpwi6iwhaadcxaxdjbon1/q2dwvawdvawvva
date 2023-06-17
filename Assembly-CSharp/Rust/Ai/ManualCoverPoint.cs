using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B3A RID: 2874
	public class ManualCoverPoint : FacepunchBehaviour
	{
		// Token: 0x04003E1C RID: 15900
		public bool IsDynamic;

		// Token: 0x04003E1D RID: 15901
		public float Score = 2f;

		// Token: 0x04003E1E RID: 15902
		public CoverPointVolume Volume;

		// Token: 0x04003E1F RID: 15903
		public Vector3 Normal;

		// Token: 0x04003E20 RID: 15904
		public CoverPoint.CoverType NormalCoverType;

		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x060045B4 RID: 17844 RVA: 0x0002C673 File Offset: 0x0002A873
		public Vector3 Position
		{
			get
			{
				return base.transform.position;
			}
		}

		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x060045B5 RID: 17845 RVA: 0x00197191 File Offset: 0x00195391
		public float DirectionMagnitude
		{
			get
			{
				if (this.Volume != null)
				{
					return this.Volume.CoverPointRayLength;
				}
				return 1f;
			}
		}

		// Token: 0x060045B6 RID: 17846 RVA: 0x001971B2 File Offset: 0x001953B2
		private void Awake()
		{
			if (base.transform.parent != null)
			{
				this.Volume = base.transform.parent.GetComponent<CoverPointVolume>();
			}
		}

		// Token: 0x060045B7 RID: 17847 RVA: 0x001971E0 File Offset: 0x001953E0
		public CoverPoint ToCoverPoint(CoverPointVolume volume)
		{
			this.Volume = volume;
			if (this.IsDynamic)
			{
				CoverPoint coverPoint = new CoverPoint(this.Volume, this.Score);
				coverPoint.IsDynamic = true;
				coverPoint.SourceTransform = base.transform;
				coverPoint.NormalCoverType = this.NormalCoverType;
				Transform transform = base.transform;
				coverPoint.Position = ((transform != null) ? transform.position : Vector3.zero);
				return coverPoint;
			}
			Vector3 normalized = (base.transform.rotation * this.Normal).normalized;
			return new CoverPoint(this.Volume, this.Score)
			{
				IsDynamic = false,
				Position = base.transform.position,
				Normal = normalized,
				NormalCoverType = this.NormalCoverType
			};
		}
	}
}
