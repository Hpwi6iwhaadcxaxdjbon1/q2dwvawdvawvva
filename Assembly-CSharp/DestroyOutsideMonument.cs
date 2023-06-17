using System;
using UnityEngine;

// Token: 0x020003E8 RID: 1000
public class DestroyOutsideMonument : FacepunchBehaviour
{
	// Token: 0x04001A6F RID: 6767
	[SerializeField]
	private BaseCombatEntity baseCombatEntity;

	// Token: 0x04001A70 RID: 6768
	[SerializeField]
	private float checkEvery = 10f;

	// Token: 0x04001A71 RID: 6769
	private MonumentInfo ourMonument;

	// Token: 0x170002E0 RID: 736
	// (get) Token: 0x0600224A RID: 8778 RVA: 0x000DD828 File Offset: 0x000DBA28
	private Vector3 OurPos
	{
		get
		{
			return this.baseCombatEntity.transform.position;
		}
	}

	// Token: 0x0600224B RID: 8779 RVA: 0x000DD83C File Offset: 0x000DBA3C
	protected void OnEnable()
	{
		if (this.ourMonument == null)
		{
			this.ourMonument = this.GetOurMonument();
		}
		if (this.ourMonument == null)
		{
			this.DoOutsideMonument();
			return;
		}
		base.InvokeRandomized(new Action(this.CheckPosition), this.checkEvery, this.checkEvery, this.checkEvery * 0.1f);
	}

	// Token: 0x0600224C RID: 8780 RVA: 0x000DD8A2 File Offset: 0x000DBAA2
	protected void OnDisable()
	{
		base.CancelInvoke(new Action(this.CheckPosition));
	}

	// Token: 0x0600224D RID: 8781 RVA: 0x000DD8B8 File Offset: 0x000DBAB8
	private MonumentInfo GetOurMonument()
	{
		foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
		{
			if (monumentInfo.IsInBounds(this.OurPos))
			{
				return monumentInfo;
			}
		}
		return null;
	}

	// Token: 0x0600224E RID: 8782 RVA: 0x000DD920 File Offset: 0x000DBB20
	private void CheckPosition()
	{
		if (this.ourMonument == null)
		{
			this.DoOutsideMonument();
		}
		if (!this.ourMonument.IsInBounds(this.OurPos))
		{
			this.DoOutsideMonument();
		}
	}

	// Token: 0x0600224F RID: 8783 RVA: 0x000DD94F File Offset: 0x000DBB4F
	private void DoOutsideMonument()
	{
		this.baseCombatEntity.Kill(BaseNetworkable.DestroyMode.Gib);
	}
}
