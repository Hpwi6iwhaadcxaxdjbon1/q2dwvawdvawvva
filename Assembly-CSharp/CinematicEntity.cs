using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000301 RID: 769
public class CinematicEntity : BaseEntity
{
	// Token: 0x0400178E RID: 6030
	private const BaseEntity.Flags HideMesh = BaseEntity.Flags.Reserved1;

	// Token: 0x0400178F RID: 6031
	public GameObject[] DisableObjects;

	// Token: 0x04001790 RID: 6032
	private static bool _hideObjects = false;

	// Token: 0x04001791 RID: 6033
	private static List<CinematicEntity> serverList = new List<CinematicEntity>();

	// Token: 0x1700027D RID: 637
	// (get) Token: 0x06001E6B RID: 7787 RVA: 0x000CEFBC File Offset: 0x000CD1BC
	// (set) Token: 0x06001E6C RID: 7788 RVA: 0x000CEFC4 File Offset: 0x000CD1C4
	[ServerVar(Help = "Hides cinematic light source meshes (keeps lights visible)")]
	public static bool HideObjects
	{
		get
		{
			return CinematicEntity._hideObjects;
		}
		set
		{
			if (value != CinematicEntity._hideObjects)
			{
				CinematicEntity._hideObjects = value;
				foreach (CinematicEntity cinematicEntity in CinematicEntity.serverList)
				{
					cinematicEntity.SetFlag(BaseEntity.Flags.Reserved1, CinematicEntity._hideObjects, false, true);
				}
			}
		}
	}

	// Token: 0x06001E6D RID: 7789 RVA: 0x000CF030 File Offset: 0x000CD230
	public override void ServerInit()
	{
		base.ServerInit();
		if (!CinematicEntity.serverList.Contains(this))
		{
			CinematicEntity.serverList.Add(this);
		}
		base.SetFlag(BaseEntity.Flags.Reserved1, CinematicEntity.HideObjects, false, true);
	}

	// Token: 0x06001E6E RID: 7790 RVA: 0x000CF062 File Offset: 0x000CD262
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer && CinematicEntity.serverList.Contains(this))
		{
			CinematicEntity.serverList.Remove(this);
		}
	}

	// Token: 0x06001E6F RID: 7791 RVA: 0x000CF08C File Offset: 0x000CD28C
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool state = !base.HasFlag(BaseEntity.Flags.Reserved1);
		this.ToggleObjects(state);
	}

	// Token: 0x06001E70 RID: 7792 RVA: 0x000CF0B8 File Offset: 0x000CD2B8
	private void ToggleObjects(bool state)
	{
		foreach (GameObject gameObject in this.DisableObjects)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(state);
			}
		}
	}
}
