using System;
using UnityEngine;

// Token: 0x02000278 RID: 632
public class SocketMod_InWater : SocketMod
{
	// Token: 0x04001577 RID: 5495
	public bool wantsInWater = true;

	// Token: 0x04001578 RID: 5496
	public static Translate.Phrase WantsWaterPhrase = new Translate.Phrase("error_inwater_wants", "Must be placed in water");

	// Token: 0x04001579 RID: 5497
	public static Translate.Phrase NoWaterPhrase = new Translate.Phrase("error_inwater", "Can't be placed in water");

	// Token: 0x06001CC2 RID: 7362 RVA: 0x000C7748 File Offset: 0x000C5948
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(Vector3.zero, 0.1f);
	}

	// Token: 0x06001CC3 RID: 7363 RVA: 0x000C7774 File Offset: 0x000C5974
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		bool flag = WaterLevel.Test(vector, true, null);
		if (!flag && this.wantsInWater && GamePhysics.CheckSphere(vector, 0.1f, 16, QueryTriggerInteraction.UseGlobal))
		{
			flag = true;
		}
		if (flag == this.wantsInWater)
		{
			return true;
		}
		if (this.wantsInWater)
		{
			Construction.lastPlacementError = SocketMod_InWater.WantsWaterPhrase.translated;
		}
		else
		{
			Construction.lastPlacementError = SocketMod_InWater.NoWaterPhrase.translated;
		}
		return false;
	}
}
