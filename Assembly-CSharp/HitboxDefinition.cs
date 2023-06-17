using System;
using UnityEngine;

// Token: 0x020002FA RID: 762
public class HitboxDefinition : MonoBehaviour
{
	// Token: 0x0400177D RID: 6013
	public Vector3 center;

	// Token: 0x0400177E RID: 6014
	public Vector3 rotation;

	// Token: 0x0400177F RID: 6015
	public HitboxDefinition.Type type;

	// Token: 0x04001780 RID: 6016
	public int priority;

	// Token: 0x04001781 RID: 6017
	public PhysicMaterial physicMaterial;

	// Token: 0x04001782 RID: 6018
	[SerializeField]
	private Vector3 scale = Vector3.one;

	// Token: 0x1700027A RID: 634
	// (get) Token: 0x06001E49 RID: 7753 RVA: 0x000CE011 File Offset: 0x000CC211
	// (set) Token: 0x06001E4A RID: 7754 RVA: 0x000CE019 File Offset: 0x000CC219
	public Vector3 Scale
	{
		get
		{
			return this.scale;
		}
		set
		{
			this.scale = new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));
		}
	}

	// Token: 0x1700027B RID: 635
	// (get) Token: 0x06001E4B RID: 7755 RVA: 0x000CE047 File Offset: 0x000CC247
	public Matrix4x4 LocalMatrix
	{
		get
		{
			return Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), this.scale);
		}
	}

	// Token: 0x06001E4C RID: 7756 RVA: 0x000CE065 File Offset: 0x000CC265
	private void OnValidate()
	{
		this.Scale = this.Scale;
	}

	// Token: 0x06001E4D RID: 7757 RVA: 0x000CE074 File Offset: 0x000CC274
	protected virtual void OnDrawGizmosSelected()
	{
		HitboxDefinition.Type type = this.type;
		if (type == HitboxDefinition.Type.BOX)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.matrix *= Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), this.scale);
			Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			Gizmos.color = Color.white;
			Gizmos.matrix = Matrix4x4.identity;
			return;
		}
		if (type != HitboxDefinition.Type.CAPSULE)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.matrix *= Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), Vector3.one);
		Gizmos.color = Color.green;
		GizmosUtil.DrawWireCapsuleY(Vector3.zero, this.scale.x, this.scale.y);
		Gizmos.color = Color.white;
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x06001E4E RID: 7758 RVA: 0x000CE1A4 File Offset: 0x000CC3A4
	protected virtual void OnDrawGizmos()
	{
		HitboxDefinition.Type type = this.type;
		if (type == HitboxDefinition.Type.BOX)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.matrix *= Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), this.scale);
			Gizmos.color = Color.black;
			Gizmos.DrawSphere(Vector3.zero, 0.005f);
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
			Gizmos.color = Color.white;
			Gizmos.matrix = Matrix4x4.identity;
			return;
		}
		if (type != HitboxDefinition.Type.CAPSULE)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.matrix *= Matrix4x4.TRS(this.center, Quaternion.Euler(this.rotation), Vector3.one);
		Gizmos.color = Color.black;
		Gizmos.DrawSphere(Vector3.zero, 0.005f);
		GizmosUtil.DrawWireCapsuleY(Vector3.zero, this.scale.x, this.scale.y);
		Gizmos.color = Color.white;
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x02000C9E RID: 3230
	public enum Type
	{
		// Token: 0x04004417 RID: 17431
		BOX,
		// Token: 0x04004418 RID: 17432
		CAPSULE
	}
}
