using System;
using UnityEngine;

// Token: 0x02000448 RID: 1096
public class PlayerModelCinematicList : PrefabAttribute, IClientComponent
{
	// Token: 0x04001D04 RID: 7428
	public PlayerModelCinematicList.PlayerModelCinematicAnimation[] Animations;

	// Token: 0x060024AB RID: 9387 RVA: 0x000E89EE File Offset: 0x000E6BEE
	protected override Type GetIndexedType()
	{
		return typeof(PlayerModelCinematicList);
	}

	// Token: 0x060024AC RID: 9388 RVA: 0x000E89FA File Offset: 0x000E6BFA
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
	}

	// Token: 0x02000CE7 RID: 3303
	[Serializable]
	public struct PlayerModelCinematicAnimation
	{
		// Token: 0x04004583 RID: 17795
		public string StateName;

		// Token: 0x04004584 RID: 17796
		public string ClipName;

		// Token: 0x04004585 RID: 17797
		public float Length;
	}
}
