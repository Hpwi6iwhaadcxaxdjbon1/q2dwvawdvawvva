using System;
using EZhex1991.EZSoftBone;
using UnityEngine;

// Token: 0x020000FB RID: 251
public class GhostSheetSystemSpaceUpdater : MonoBehaviour, IClientComponent
{
	// Token: 0x04000DC9 RID: 3529
	private EZSoftBone[] ezSoftBones;

	// Token: 0x04000DCA RID: 3530
	private BasePlayer player;

	// Token: 0x06001579 RID: 5497 RVA: 0x000AA1A7 File Offset: 0x000A83A7
	public void Awake()
	{
		this.ezSoftBones = base.GetComponents<EZSoftBone>();
		this.player = (base.gameObject.ToBaseEntity() as BasePlayer);
	}

	// Token: 0x0600157A RID: 5498 RVA: 0x000AA1CC File Offset: 0x000A83CC
	public void Update()
	{
		if (this.ezSoftBones == null || this.ezSoftBones.Length == 0 || this.player == null)
		{
			return;
		}
		BaseMountable mounted = this.player.GetMounted();
		if (mounted != null)
		{
			this.SetSimulateSpace(mounted.transform, false);
			return;
		}
		BaseEntity parentEntity = this.player.GetParentEntity();
		if (parentEntity != null)
		{
			this.SetSimulateSpace(parentEntity.transform, true);
			return;
		}
		this.SetSimulateSpace(null, true);
	}

	// Token: 0x0600157B RID: 5499 RVA: 0x000AA248 File Offset: 0x000A8448
	private void SetSimulateSpace(Transform transform, bool collisionEnabled)
	{
		for (int i = 0; i < this.ezSoftBones.Length; i++)
		{
			EZSoftBone ezsoftBone = this.ezSoftBones[i];
			ezsoftBone.simulateSpace = transform;
			ezsoftBone.collisionEnabled = collisionEnabled;
		}
	}
}
