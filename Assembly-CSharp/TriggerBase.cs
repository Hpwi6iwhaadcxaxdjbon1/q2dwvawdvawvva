using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x0200057F RID: 1407
public class TriggerBase : BaseMonoBehaviour
{
	// Token: 0x040022FE RID: 8958
	public LayerMask interestLayers;

	// Token: 0x040022FF RID: 8959
	[NonSerialized]
	public HashSet<GameObject> contents;

	// Token: 0x04002300 RID: 8960
	[NonSerialized]
	public HashSet<BaseEntity> entityContents;

	// Token: 0x17000395 RID: 917
	// (get) Token: 0x06002B1D RID: 11037 RVA: 0x0010605D File Offset: 0x0010425D
	public bool HasAnyContents
	{
		get
		{
			return !this.contents.IsNullOrEmpty<GameObject>();
		}
	}

	// Token: 0x17000396 RID: 918
	// (get) Token: 0x06002B1E RID: 11038 RVA: 0x0010606D File Offset: 0x0010426D
	public bool HasAnyEntityContents
	{
		get
		{
			return !this.entityContents.IsNullOrEmpty<BaseEntity>();
		}
	}

	// Token: 0x06002B1F RID: 11039 RVA: 0x00106080 File Offset: 0x00104280
	internal virtual GameObject InterestedInObject(GameObject obj)
	{
		int num = 1 << obj.layer;
		if ((this.interestLayers.value & num) != num)
		{
			return null;
		}
		return obj;
	}

	// Token: 0x06002B20 RID: 11040 RVA: 0x001060AC File Offset: 0x001042AC
	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (this.contents == null)
		{
			return;
		}
		foreach (GameObject targetObj in this.contents.ToArray<GameObject>())
		{
			this.OnTriggerExit(targetObj);
		}
		this.contents = null;
	}

	// Token: 0x06002B21 RID: 11041 RVA: 0x001060F6 File Offset: 0x001042F6
	internal virtual void OnEntityEnter(BaseEntity ent)
	{
		if (ent == null)
		{
			return;
		}
		if (this.entityContents == null)
		{
			this.entityContents = new HashSet<BaseEntity>();
		}
		this.entityContents.Add(ent);
	}

	// Token: 0x06002B22 RID: 11042 RVA: 0x00106122 File Offset: 0x00104322
	internal virtual void OnEntityLeave(BaseEntity ent)
	{
		if (this.entityContents == null)
		{
			return;
		}
		this.entityContents.Remove(ent);
	}

	// Token: 0x06002B23 RID: 11043 RVA: 0x0010613C File Offset: 0x0010433C
	internal virtual void OnObjectAdded(GameObject obj, Collider col)
	{
		if (obj == null)
		{
			return;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity)
		{
			baseEntity.EnterTrigger(this);
			this.OnEntityEnter(baseEntity);
		}
	}

	// Token: 0x06002B24 RID: 11044 RVA: 0x00106174 File Offset: 0x00104374
	internal virtual void OnObjectRemoved(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity)
		{
			bool flag = false;
			foreach (GameObject gameObject in this.contents)
			{
				if (gameObject == null)
				{
					Debug.LogWarning("Trigger " + this.ToString() + " contains null object.");
				}
				else if (gameObject.ToBaseEntity() == baseEntity)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				baseEntity.LeaveTrigger(this);
				this.OnEntityLeave(baseEntity);
			}
		}
	}

	// Token: 0x06002B25 RID: 11045 RVA: 0x00106224 File Offset: 0x00104424
	internal void RemoveInvalidEntities()
	{
		if (this.entityContents.IsNullOrEmpty<BaseEntity>())
		{
			return;
		}
		Collider component = base.GetComponent<Collider>();
		if (component == null)
		{
			return;
		}
		Bounds bounds = component.bounds;
		bounds.Expand(1f);
		List<BaseEntity> list = null;
		foreach (BaseEntity baseEntity in this.entityContents)
		{
			if (baseEntity == null)
			{
				if (Debugging.checktriggers)
				{
					Debug.LogWarning("Trigger " + this.ToString() + " contains destroyed entity.");
				}
				if (list == null)
				{
					list = Facepunch.Pool.GetList<BaseEntity>();
				}
				list.Add(baseEntity);
			}
			else if (!bounds.Contains(baseEntity.ClosestPoint(base.transform.position)))
			{
				if (Debugging.checktriggers)
				{
					Debug.LogWarning("Trigger " + this.ToString() + " contains entity that is too far away: " + baseEntity.ToString());
				}
				if (list == null)
				{
					list = Facepunch.Pool.GetList<BaseEntity>();
				}
				list.Add(baseEntity);
			}
		}
		if (list != null)
		{
			foreach (BaseEntity ent in list)
			{
				this.RemoveEntity(ent);
			}
			Facepunch.Pool.FreeList<BaseEntity>(ref list);
		}
	}

	// Token: 0x06002B26 RID: 11046 RVA: 0x00106388 File Offset: 0x00104588
	internal bool CheckEntity(BaseEntity ent)
	{
		if (ent == null)
		{
			return true;
		}
		Collider component = base.GetComponent<Collider>();
		if (component == null)
		{
			return true;
		}
		Bounds bounds = component.bounds;
		bounds.Expand(1f);
		return bounds.Contains(ent.ClosestPoint(base.transform.position));
	}

	// Token: 0x06002B27 RID: 11047 RVA: 0x000063A5 File Offset: 0x000045A5
	internal virtual void OnObjects()
	{
	}

	// Token: 0x06002B28 RID: 11048 RVA: 0x001063DD File Offset: 0x001045DD
	internal virtual void OnEmpty()
	{
		this.contents = null;
		this.entityContents = null;
	}

	// Token: 0x06002B29 RID: 11049 RVA: 0x001063F0 File Offset: 0x001045F0
	public void RemoveObject(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		Collider component = obj.GetComponent<Collider>();
		if (component == null)
		{
			return;
		}
		this.OnTriggerExit(component);
	}

	// Token: 0x06002B2A RID: 11050 RVA: 0x00106420 File Offset: 0x00104620
	public void RemoveEntity(BaseEntity ent)
	{
		if (this == null || this.contents == null || ent == null)
		{
			return;
		}
		List<GameObject> list = Facepunch.Pool.GetList<GameObject>();
		foreach (GameObject gameObject in this.contents)
		{
			if (gameObject != null && gameObject.GetComponentInParent<BaseEntity>() == ent)
			{
				list.Add(gameObject);
			}
		}
		foreach (GameObject targetObj in list)
		{
			this.OnTriggerExit(targetObj);
		}
		Facepunch.Pool.FreeList<GameObject>(ref list);
	}

	// Token: 0x06002B2B RID: 11051 RVA: 0x001064F4 File Offset: 0x001046F4
	public void OnTriggerEnter(Collider collider)
	{
		if (this == null)
		{
			return;
		}
		if (!base.enabled)
		{
			return;
		}
		using (TimeWarning.New("TriggerBase.OnTriggerEnter", 0))
		{
			GameObject gameObject = this.InterestedInObject(collider.gameObject);
			if (gameObject == null)
			{
				return;
			}
			if (this.contents == null)
			{
				this.contents = new HashSet<GameObject>();
			}
			if (this.contents.Contains(gameObject))
			{
				return;
			}
			bool count = this.contents.Count != 0;
			this.contents.Add(gameObject);
			this.OnObjectAdded(gameObject, collider);
			if (!count && this.contents.Count == 1)
			{
				this.OnObjects();
			}
		}
		if (Debugging.checktriggers)
		{
			this.RemoveInvalidEntities();
		}
	}

	// Token: 0x06002B2C RID: 11052 RVA: 0x00007A3C File Offset: 0x00005C3C
	internal virtual bool SkipOnTriggerExit(Collider collider)
	{
		return false;
	}

	// Token: 0x06002B2D RID: 11053 RVA: 0x001065BC File Offset: 0x001047BC
	public void OnTriggerExit(Collider collider)
	{
		if (this == null)
		{
			return;
		}
		if (collider == null)
		{
			return;
		}
		if (this.SkipOnTriggerExit(collider))
		{
			return;
		}
		GameObject gameObject = this.InterestedInObject(collider.gameObject);
		if (gameObject == null)
		{
			return;
		}
		this.OnTriggerExit(gameObject);
		if (Debugging.checktriggers)
		{
			this.RemoveInvalidEntities();
		}
	}

	// Token: 0x06002B2E RID: 11054 RVA: 0x00106614 File Offset: 0x00104814
	private void OnTriggerExit(GameObject targetObj)
	{
		if (this.contents == null)
		{
			return;
		}
		if (!this.contents.Contains(targetObj))
		{
			return;
		}
		this.contents.Remove(targetObj);
		this.OnObjectRemoved(targetObj);
		if (this.contents == null || this.contents.Count == 0)
		{
			this.OnEmpty();
		}
	}
}
