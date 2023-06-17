using System;
using UnityEngine;

// Token: 0x02000146 RID: 326
public class ArcadeEntityController : BaseMonoBehaviour
{
	// Token: 0x04000F78 RID: 3960
	public BaseArcadeGame parentGame;

	// Token: 0x04000F79 RID: 3961
	public ArcadeEntity arcadeEntity;

	// Token: 0x04000F7A RID: 3962
	public ArcadeEntity sourceEntity;

	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x060016FE RID: 5886 RVA: 0x000AFECA File Offset: 0x000AE0CA
	// (set) Token: 0x060016FF RID: 5887 RVA: 0x000AFED7 File Offset: 0x000AE0D7
	public Vector3 heading
	{
		get
		{
			return this.arcadeEntity.heading;
		}
		set
		{
			this.arcadeEntity.heading = value;
		}
	}

	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x06001700 RID: 5888 RVA: 0x000AFEE5 File Offset: 0x000AE0E5
	// (set) Token: 0x06001701 RID: 5889 RVA: 0x000AFEF7 File Offset: 0x000AE0F7
	public Vector3 positionLocal
	{
		get
		{
			return this.arcadeEntity.transform.localPosition;
		}
		set
		{
			this.arcadeEntity.transform.localPosition = value;
		}
	}

	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x06001702 RID: 5890 RVA: 0x000AFF0A File Offset: 0x000AE10A
	// (set) Token: 0x06001703 RID: 5891 RVA: 0x000AFF1C File Offset: 0x000AE11C
	public Vector3 positionWorld
	{
		get
		{
			return this.arcadeEntity.transform.position;
		}
		set
		{
			this.arcadeEntity.transform.position = value;
		}
	}
}
