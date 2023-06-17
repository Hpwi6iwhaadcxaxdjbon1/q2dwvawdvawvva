using System;
using UnityEngine;

// Token: 0x020001E1 RID: 481
public class AITraversalArea : TriggerBase
{
	// Token: 0x04001250 RID: 4688
	public Transform entryPoint1;

	// Token: 0x04001251 RID: 4689
	public Transform entryPoint2;

	// Token: 0x04001252 RID: 4690
	public AITraversalWaitPoint[] waitPoints;

	// Token: 0x04001253 RID: 4691
	public Bounds movementArea;

	// Token: 0x04001254 RID: 4692
	public Transform activeEntryPoint;

	// Token: 0x04001255 RID: 4693
	public float nextFreeTime;

	// Token: 0x0600198A RID: 6538 RVA: 0x000BB563 File Offset: 0x000B9763
	public void OnValidate()
	{
		this.movementArea.center = base.transform.position;
	}

	// Token: 0x0600198B RID: 6539 RVA: 0x000BB57C File Offset: 0x000B977C
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		if (!baseEntity.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x0600198C RID: 6540 RVA: 0x000BB5C9 File Offset: 0x000B97C9
	public bool CanTraverse(BaseEntity ent)
	{
		return Time.time > this.nextFreeTime;
	}

	// Token: 0x0600198D RID: 6541 RVA: 0x000BB5D8 File Offset: 0x000B97D8
	public Transform GetClosestEntry(Vector3 position)
	{
		float num = Vector3.Distance(position, this.entryPoint1.position);
		float num2 = Vector3.Distance(position, this.entryPoint2.position);
		if (num < num2)
		{
			return this.entryPoint1;
		}
		return this.entryPoint2;
	}

	// Token: 0x0600198E RID: 6542 RVA: 0x000BB618 File Offset: 0x000B9818
	public Transform GetFarthestEntry(Vector3 position)
	{
		float num = Vector3.Distance(position, this.entryPoint1.position);
		float num2 = Vector3.Distance(position, this.entryPoint2.position);
		if (num > num2)
		{
			return this.entryPoint1;
		}
		return this.entryPoint2;
	}

	// Token: 0x0600198F RID: 6543 RVA: 0x000BB658 File Offset: 0x000B9858
	public void SetBusyFor(float dur = 1f)
	{
		this.nextFreeTime = Time.time + dur;
	}

	// Token: 0x06001990 RID: 6544 RVA: 0x000BB5C9 File Offset: 0x000B97C9
	public bool CanUse(Vector3 dirFrom)
	{
		return Time.time > this.nextFreeTime;
	}

	// Token: 0x06001991 RID: 6545 RVA: 0x000BB667 File Offset: 0x000B9867
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
	}

	// Token: 0x06001992 RID: 6546 RVA: 0x000BB670 File Offset: 0x000B9870
	public AITraversalWaitPoint GetEntryPointNear(Vector3 pos)
	{
		Vector3 position = this.GetClosestEntry(pos).position;
		Vector3 position2 = this.GetFarthestEntry(pos).position;
		new BaseEntity[1];
		AITraversalWaitPoint result = null;
		float num = 0f;
		foreach (AITraversalWaitPoint aitraversalWaitPoint in this.waitPoints)
		{
			if (!aitraversalWaitPoint.Occupied())
			{
				Vector3 position3 = aitraversalWaitPoint.transform.position;
				float num2 = Vector3.Distance(position, position3);
				if (Vector3.Distance(position2, position3) >= num2)
				{
					float value = Vector3.Distance(position3, pos);
					float num3 = (1f - Mathf.InverseLerp(0f, 20f, value)) * 100f;
					if (num3 > num)
					{
						num = num3;
						result = aitraversalWaitPoint;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001993 RID: 6547 RVA: 0x000BB72A File Offset: 0x000B992A
	public bool EntityFilter(BaseEntity ent)
	{
		return ent.IsNpc && ent.isServer;
	}

	// Token: 0x06001994 RID: 6548 RVA: 0x000BB73C File Offset: 0x000B993C
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
	}

	// Token: 0x06001995 RID: 6549 RVA: 0x000BB748 File Offset: 0x000B9948
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawCube(this.entryPoint1.position + Vector3.up * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.DrawCube(this.entryPoint2.position + Vector3.up * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.5f);
		Gizmos.DrawCube(this.movementArea.center, this.movementArea.size);
		Gizmos.color = Color.magenta;
		AITraversalWaitPoint[] array = this.waitPoints;
		for (int i = 0; i < array.Length; i++)
		{
			GizmosUtil.DrawCircleY(array[i].transform.position, 0.5f);
		}
	}
}
