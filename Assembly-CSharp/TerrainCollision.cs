using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000691 RID: 1681
public class TerrainCollision : TerrainExtension
{
	// Token: 0x04002774 RID: 10100
	private ListDictionary<Collider, List<Collider>> ignoredColliders;

	// Token: 0x04002775 RID: 10101
	private TerrainCollider terrainCollider;

	// Token: 0x06002FD7 RID: 12247 RVA: 0x0011F95E File Offset: 0x0011DB5E
	public override void Setup()
	{
		this.ignoredColliders = new ListDictionary<Collider, List<Collider>>();
		this.terrainCollider = this.terrain.GetComponent<TerrainCollider>();
	}

	// Token: 0x06002FD8 RID: 12248 RVA: 0x0011F97C File Offset: 0x0011DB7C
	public void Clear()
	{
		if (!this.terrainCollider)
		{
			return;
		}
		foreach (Collider collider in this.ignoredColliders.Keys)
		{
			Physics.IgnoreCollision(collider, this.terrainCollider, false);
		}
		this.ignoredColliders.Clear();
	}

	// Token: 0x06002FD9 RID: 12249 RVA: 0x0011F9F4 File Offset: 0x0011DBF4
	public void Reset(Collider collider)
	{
		if (!this.terrainCollider || !collider)
		{
			return;
		}
		Physics.IgnoreCollision(collider, this.terrainCollider, false);
		this.ignoredColliders.Remove(collider);
	}

	// Token: 0x06002FDA RID: 12250 RVA: 0x0011FA26 File Offset: 0x0011DC26
	public bool GetIgnore(Vector3 pos, float radius = 0.01f)
	{
		return GamePhysics.CheckSphere<TerrainCollisionTrigger>(pos, radius, 262144, QueryTriggerInteraction.Collide);
	}

	// Token: 0x06002FDB RID: 12251 RVA: 0x0011FA35 File Offset: 0x0011DC35
	public bool GetIgnore(RaycastHit hit)
	{
		return hit.collider is TerrainCollider && this.GetIgnore(hit.point, 0.01f);
	}

	// Token: 0x06002FDC RID: 12252 RVA: 0x0011FA59 File Offset: 0x0011DC59
	public bool GetIgnore(Collider collider)
	{
		return this.terrainCollider && collider && this.ignoredColliders.Contains(collider);
	}

	// Token: 0x06002FDD RID: 12253 RVA: 0x0011FA80 File Offset: 0x0011DC80
	public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
	{
		if (!this.terrainCollider || !collider)
		{
			return;
		}
		if (!this.GetIgnore(collider))
		{
			if (ignore)
			{
				List<Collider> val = new List<Collider>
				{
					trigger
				};
				Physics.IgnoreCollision(collider, this.terrainCollider, true);
				this.ignoredColliders.Add(collider, val);
				return;
			}
		}
		else
		{
			List<Collider> list = this.ignoredColliders[collider];
			if (ignore)
			{
				if (!list.Contains(trigger))
				{
					list.Add(trigger);
					return;
				}
			}
			else if (list.Contains(trigger))
			{
				list.Remove(trigger);
			}
		}
	}

	// Token: 0x06002FDE RID: 12254 RVA: 0x0011FB0C File Offset: 0x0011DD0C
	protected void LateUpdate()
	{
		if (this.ignoredColliders == null)
		{
			return;
		}
		for (int i = 0; i < this.ignoredColliders.Count; i++)
		{
			KeyValuePair<Collider, List<Collider>> byIndex = this.ignoredColliders.GetByIndex(i);
			Collider key = byIndex.Key;
			List<Collider> value = byIndex.Value;
			if (key == null)
			{
				this.ignoredColliders.RemoveAt(i--);
			}
			else if (value.Count == 0)
			{
				Physics.IgnoreCollision(key, this.terrainCollider, false);
				this.ignoredColliders.RemoveAt(i--);
			}
		}
	}
}
