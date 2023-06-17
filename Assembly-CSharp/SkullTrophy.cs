using System;
using Facepunch;
using ntw.CurvedTextMeshPro;
using ProtoBuf;
using Rust.UI;
using UnityEngine;

// Token: 0x02000166 RID: 358
public class SkullTrophy : StorageContainer
{
	// Token: 0x04000FFF RID: 4095
	public RustText NameText;

	// Token: 0x04001000 RID: 4096
	public TextProOnACircle CircleModifier;

	// Token: 0x04001001 RID: 4097
	public int AngleModifierMinCharCount = 3;

	// Token: 0x04001002 RID: 4098
	public int AngleModifierMaxCharCount = 20;

	// Token: 0x04001003 RID: 4099
	public int AngleModifierMinArcAngle = 20;

	// Token: 0x04001004 RID: 4100
	public int AngleModifierMaxArcAngle = 45;

	// Token: 0x04001005 RID: 4101
	public float SunsetTime = 18f;

	// Token: 0x04001006 RID: 4102
	public float SunriseTime = 5f;

	// Token: 0x04001007 RID: 4103
	public MeshRenderer[] SkullRenderers;

	// Token: 0x04001008 RID: 4104
	public Material[] DaySkull;

	// Token: 0x04001009 RID: 4105
	public Material[] NightSkull;

	// Token: 0x0400100A RID: 4106
	public Material[] NoSkull;

	// Token: 0x0600174E RID: 5966 RVA: 0x000B14D8 File Offset: 0x000AF6D8
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x000B14EC File Offset: 0x000AF6EC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			if (base.inventory != null && base.inventory.itemList.Count == 1)
			{
				info.msg.skullTrophy = Pool.Get<ProtoBuf.SkullTrophy>();
				info.msg.skullTrophy.playerName = base.inventory.itemList[0].GetName(new bool?(false));
				info.msg.skullTrophy.streamerName = base.inventory.itemList[0].GetName(new bool?(true));
				return;
			}
			if (info.msg.skullTrophy != null)
			{
				info.msg.skullTrophy.playerName = string.Empty;
				info.msg.skullTrophy.streamerName = string.Empty;
			}
		}
	}
}
