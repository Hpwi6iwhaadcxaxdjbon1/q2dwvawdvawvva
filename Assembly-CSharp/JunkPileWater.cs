using System;
using UnityEngine;

// Token: 0x02000168 RID: 360
public class JunkPileWater : JunkPile
{
	// Token: 0x0400100B RID: 4107
	public static JunkPileWater.JunkpileWaterWorkQueue junkpileWaterWorkQueue = new JunkPileWater.JunkpileWaterWorkQueue();

	// Token: 0x0400100C RID: 4108
	[ServerVar]
	[Help("How many milliseconds to budget for processing life story updates per frame")]
	public static float framebudgetms = 0.25f;

	// Token: 0x0400100D RID: 4109
	public Transform[] buoyancyPoints;

	// Token: 0x0400100E RID: 4110
	public bool debugDraw;

	// Token: 0x0400100F RID: 4111
	private Quaternion baseRotation = Quaternion.identity;

	// Token: 0x04001010 RID: 4112
	private bool first = true;

	// Token: 0x04001011 RID: 4113
	private TimeUntil nextPlayerCheck;

	// Token: 0x04001012 RID: 4114
	private bool hasPlayersNearby;

	// Token: 0x06001752 RID: 5970 RVA: 0x000B1604 File Offset: 0x000AF804
	public override void Spawn()
	{
		Vector3 position = base.transform.position;
		position.y = TerrainMeta.WaterMap.GetHeight(base.transform.position);
		base.transform.position = position;
		base.Spawn();
		this.baseRotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
	}

	// Token: 0x06001753 RID: 5971 RVA: 0x000B1678 File Offset: 0x000AF878
	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		this.UpdateMovement();
	}

	// Token: 0x06001754 RID: 5972 RVA: 0x000B168C File Offset: 0x000AF88C
	public void UpdateMovement()
	{
		if (this.nextPlayerCheck <= 0f)
		{
			this.nextPlayerCheck = UnityEngine.Random.Range(0.5f, 1f);
			JunkPileWater.junkpileWaterWorkQueue.Add(this);
		}
		if (!this.isSinking && this.hasPlayersNearby)
		{
			float height = WaterSystem.GetHeight(base.transform.position);
			base.transform.position = new Vector3(base.transform.position.x, height, base.transform.position.z);
			if (this.buoyancyPoints != null && this.buoyancyPoints.Length >= 3)
			{
				Vector3 position = base.transform.position;
				Vector3 localPosition = this.buoyancyPoints[0].localPosition;
				Vector3 localPosition2 = this.buoyancyPoints[1].localPosition;
				Vector3 localPosition3 = this.buoyancyPoints[2].localPosition;
				Vector3 vector = localPosition + position;
				Vector3 vector2 = localPosition2 + position;
				Vector3 vector3 = localPosition3 + position;
				vector.y = WaterSystem.GetHeight(vector);
				vector2.y = WaterSystem.GetHeight(vector2);
				vector3.y = WaterSystem.GetHeight(vector3);
				Vector3 position2 = new Vector3(position.x, vector.y - localPosition.y, position.z);
				Vector3 rhs = vector2 - vector;
				Vector3 vector4 = Vector3.Cross(vector3 - vector, rhs);
				Vector3 eulerAngles = Quaternion.LookRotation(new Vector3(vector4.x, vector4.z, vector4.y)).eulerAngles;
				Quaternion lhs = Quaternion.Euler(-eulerAngles.x, 0f, -eulerAngles.y);
				if (this.first)
				{
					this.baseRotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
					this.first = false;
				}
				base.transform.SetPositionAndRotation(position2, lhs * this.baseRotation);
			}
		}
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x000B1895 File Offset: 0x000AFA95
	public void UpdateNearbyPlayers()
	{
		this.hasPlayersNearby = BaseNetworkable.HasCloseConnections(base.transform.position, 16f);
	}

	// Token: 0x02000C2B RID: 3115
	public class JunkpileWaterWorkQueue : ObjectWorkQueue<JunkPileWater>
	{
		// Token: 0x06004E12 RID: 19986 RVA: 0x001A1E85 File Offset: 0x001A0085
		protected override void RunJob(JunkPileWater entity)
		{
			if (this.ShouldAdd(entity))
			{
				entity.UpdateNearbyPlayers();
			}
		}

		// Token: 0x06004E13 RID: 19987 RVA: 0x001A1E96 File Offset: 0x001A0096
		protected override bool ShouldAdd(JunkPileWater entity)
		{
			return base.ShouldAdd(entity) && entity.IsValid();
		}
	}
}
