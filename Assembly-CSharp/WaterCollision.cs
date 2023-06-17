using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006FA RID: 1786
public class WaterCollision : MonoBehaviour
{
	// Token: 0x040028F3 RID: 10483
	private ListDictionary<Collider, List<Collider>> ignoredColliders;

	// Token: 0x040028F4 RID: 10484
	private HashSet<Collider> waterColliders;

	// Token: 0x0600324E RID: 12878 RVA: 0x00135ECC File Offset: 0x001340CC
	private void Awake()
	{
		this.ignoredColliders = new ListDictionary<Collider, List<Collider>>();
		this.waterColliders = new HashSet<Collider>();
	}

	// Token: 0x0600324F RID: 12879 RVA: 0x00135EE4 File Offset: 0x001340E4
	public void Clear()
	{
		if (this.waterColliders.Count == 0)
		{
			return;
		}
		HashSet<Collider>.Enumerator enumerator = this.waterColliders.GetEnumerator();
		while (enumerator.MoveNext())
		{
			foreach (Collider collider in this.ignoredColliders.Keys)
			{
				Physics.IgnoreCollision(collider, enumerator.Current, false);
			}
		}
		this.ignoredColliders.Clear();
	}

	// Token: 0x06003250 RID: 12880 RVA: 0x00135F74 File Offset: 0x00134174
	public void Reset(Collider collider)
	{
		if (this.waterColliders.Count == 0 || !collider)
		{
			return;
		}
		foreach (Collider collider2 in this.waterColliders)
		{
			Physics.IgnoreCollision(collider, collider2, false);
		}
		this.ignoredColliders.Remove(collider);
	}

	// Token: 0x06003251 RID: 12881 RVA: 0x00135FC9 File Offset: 0x001341C9
	public bool GetIgnore(Vector3 pos, float radius = 0.01f)
	{
		return GamePhysics.CheckSphere<WaterVisibilityTrigger>(pos, radius, 262144, QueryTriggerInteraction.Collide);
	}

	// Token: 0x06003252 RID: 12882 RVA: 0x00135FD8 File Offset: 0x001341D8
	public bool GetIgnore(Bounds bounds)
	{
		return GamePhysics.CheckBounds<WaterVisibilityTrigger>(bounds, 262144, QueryTriggerInteraction.Collide);
	}

	// Token: 0x06003253 RID: 12883 RVA: 0x00135FE6 File Offset: 0x001341E6
	public bool GetIgnore(Vector3 start, Vector3 end, float radius)
	{
		return GamePhysics.CheckCapsule<WaterVisibilityTrigger>(start, end, radius, 262144, QueryTriggerInteraction.Collide);
	}

	// Token: 0x06003254 RID: 12884 RVA: 0x00135FF6 File Offset: 0x001341F6
	public bool GetIgnore(RaycastHit hit)
	{
		return this.waterColliders.Contains(hit.collider) && this.GetIgnore(hit.point, 0.01f);
	}

	// Token: 0x06003255 RID: 12885 RVA: 0x00136020 File Offset: 0x00134220
	public bool GetIgnore(Collider collider)
	{
		return this.waterColliders.Count != 0 && collider && this.ignoredColliders.Contains(collider);
	}

	// Token: 0x06003256 RID: 12886 RVA: 0x00136048 File Offset: 0x00134248
	public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
	{
		if (this.waterColliders.Count == 0 || !collider)
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
				foreach (Collider collider2 in this.waterColliders)
				{
					Physics.IgnoreCollision(collider, collider2, true);
				}
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

	// Token: 0x06003257 RID: 12887 RVA: 0x001360EC File Offset: 0x001342EC
	protected void LateUpdate()
	{
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
				foreach (Collider collider in this.waterColliders)
				{
					Physics.IgnoreCollision(key, collider, false);
				}
				this.ignoredColliders.RemoveAt(i--);
			}
		}
	}
}
