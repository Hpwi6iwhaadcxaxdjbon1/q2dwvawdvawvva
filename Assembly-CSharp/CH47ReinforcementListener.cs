using System;
using UnityEngine;

// Token: 0x02000482 RID: 1154
public class CH47ReinforcementListener : BaseEntity
{
	// Token: 0x04001E5E RID: 7774
	public string listenString;

	// Token: 0x04001E5F RID: 7775
	public GameObjectRef heliPrefab;

	// Token: 0x04001E60 RID: 7776
	public float startDist = 300f;

	// Token: 0x0600262A RID: 9770 RVA: 0x000F0D04 File Offset: 0x000EEF04
	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		if (msg == this.listenString)
		{
			this.Call();
		}
	}

	// Token: 0x0600262B RID: 9771 RVA: 0x000F0D1C File Offset: 0x000EEF1C
	public void Call()
	{
		CH47HelicopterAIController component = GameManager.server.CreateEntity(this.heliPrefab.resourcePath, default(Vector3), default(Quaternion), true).GetComponent<CH47HelicopterAIController>();
		if (component)
		{
			Vector3 size = TerrainMeta.Size;
			CH47LandingZone closest = CH47LandingZone.GetClosest(base.transform.position);
			Vector3 zero = Vector3.zero;
			zero.y = closest.transform.position.y;
			Vector3 a = Vector3Ex.Direction2D(closest.transform.position, zero);
			Vector3 position = closest.transform.position + a * this.startDist;
			position.y = closest.transform.position.y;
			component.transform.position = position;
			component.SetLandingTarget(closest.transform.position);
			component.Spawn();
		}
	}
}
