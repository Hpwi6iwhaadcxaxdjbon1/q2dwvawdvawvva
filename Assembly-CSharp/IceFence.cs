using System;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class IceFence : GraveyardFence
{
	// Token: 0x04001091 RID: 4241
	public GameObject[] styles;

	// Token: 0x04001092 RID: 4242
	private bool init;

	// Token: 0x04001093 RID: 4243
	public AdaptMeshToTerrain snowMesh;

	// Token: 0x060017B6 RID: 6070 RVA: 0x000B3308 File Offset: 0x000B1508
	public int GetStyleFromID()
	{
		uint num = (uint)this.net.ID.Value;
		return SeedRandom.Range(ref num, 0, this.styles.Length);
	}

	// Token: 0x060017B7 RID: 6071 RVA: 0x000B3337 File Offset: 0x000B1537
	public override void ServerInit()
	{
		base.ServerInit();
		this.InitStyle();
		this.UpdatePillars();
	}

	// Token: 0x060017B8 RID: 6072 RVA: 0x000B334B File Offset: 0x000B154B
	public void InitStyle()
	{
		if (this.init)
		{
			return;
		}
		this.SetStyle(this.GetStyleFromID());
	}

	// Token: 0x060017B9 RID: 6073 RVA: 0x000B3364 File Offset: 0x000B1564
	public void SetStyle(int style)
	{
		GameObject[] array = this.styles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
		this.styles[style].gameObject.SetActive(true);
	}

	// Token: 0x060017BA RID: 6074 RVA: 0x000B33A7 File Offset: 0x000B15A7
	public override void UpdatePillars()
	{
		base.UpdatePillars();
	}
}
