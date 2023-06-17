using System;
using UnityEngine;

// Token: 0x02000441 RID: 1089
public class PlatformEntity : BaseEntity
{
	// Token: 0x04001CA3 RID: 7331
	private const float movementSpeed = 1f;

	// Token: 0x04001CA4 RID: 7332
	private const float rotationSpeed = 10f;

	// Token: 0x04001CA5 RID: 7333
	private const float radius = 10f;

	// Token: 0x04001CA6 RID: 7334
	private Vector3 targetPosition = Vector3.zero;

	// Token: 0x04001CA7 RID: 7335
	private Quaternion targetRotation = Quaternion.identity;

	// Token: 0x06002470 RID: 9328 RVA: 0x000E7C3C File Offset: 0x000E5E3C
	protected void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (this.targetPosition == Vector3.zero || Vector3.Distance(base.transform.position, this.targetPosition) < 0.01f)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * 10f;
			this.targetPosition = base.transform.position + new Vector3(vector.x, 0f, vector.y);
			if (TerrainMeta.HeightMap != null && TerrainMeta.WaterMap != null)
			{
				float height = TerrainMeta.HeightMap.GetHeight(this.targetPosition);
				float height2 = TerrainMeta.WaterMap.GetHeight(this.targetPosition);
				this.targetPosition.y = Mathf.Max(height, height2) + 1f;
			}
			this.targetRotation = Quaternion.LookRotation(this.targetPosition - base.transform.position);
		}
		base.transform.SetPositionAndRotation(Vector3.MoveTowards(base.transform.position, this.targetPosition, Time.fixedDeltaTime * 1f), Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, Time.fixedDeltaTime * 10f));
	}

	// Token: 0x06002471 RID: 9329 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}
}
