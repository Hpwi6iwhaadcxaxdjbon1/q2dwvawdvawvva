using System;
using UnityEngine;

// Token: 0x02000612 RID: 1554
[CreateAssetMenu(menuName = "Rust/Missions/MoveMission")]
public class MoveMission : BaseMission
{
	// Token: 0x04002593 RID: 9619
	public float minDistForMovePoint = 20f;

	// Token: 0x04002594 RID: 9620
	public float maxDistForMovePoint = 25f;

	// Token: 0x04002595 RID: 9621
	private float minDistFromLocation = 3f;

	// Token: 0x06002DF5 RID: 11765 RVA: 0x00113F7C File Offset: 0x0011217C
	public override void MissionStart(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
		onUnitSphere.y = 0f;
		onUnitSphere.Normalize();
		Vector3 vector = assignee.transform.position + onUnitSphere * UnityEngine.Random.Range(this.minDistForMovePoint, this.maxDistForMovePoint);
		float b = vector.y;
		float a = vector.y;
		if (TerrainMeta.WaterMap != null)
		{
			a = TerrainMeta.WaterMap.GetHeight(vector);
		}
		if (TerrainMeta.HeightMap != null)
		{
			b = TerrainMeta.HeightMap.GetHeight(vector);
		}
		vector.y = Mathf.Max(a, b);
		instance.missionLocation = vector;
		base.MissionStart(instance, assignee);
	}

	// Token: 0x06002DF6 RID: 11766 RVA: 0x00114027 File Offset: 0x00112227
	public override void MissionEnded(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		base.MissionEnded(instance, assignee);
	}

	// Token: 0x06002DF7 RID: 11767 RVA: 0x00114031 File Offset: 0x00112231
	public override Sprite GetIcon(BaseMission.MissionInstance instance)
	{
		if (instance.status != BaseMission.MissionStatus.Accomplished)
		{
			return this.icon;
		}
		return this.providerIcon;
	}

	// Token: 0x06002DF8 RID: 11768 RVA: 0x0011404C File Offset: 0x0011224C
	public override void Think(BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		float num = Vector3.Distance(instance.missionLocation, assignee.transform.position);
		if (instance.status == BaseMission.MissionStatus.Active && num <= this.minDistFromLocation)
		{
			this.MissionSuccess(instance, assignee);
			BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(instance.providerID);
			if (baseNetworkable)
			{
				instance.missionLocation = baseNetworkable.transform.position;
			}
			return;
		}
		if (instance.status == BaseMission.MissionStatus.Accomplished)
		{
			float num2 = this.minDistFromLocation;
		}
		base.Think(instance, assignee, delta);
	}
}
