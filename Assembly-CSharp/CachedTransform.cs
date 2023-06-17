using System;
using UnityEngine;

// Token: 0x020008F3 RID: 2291
public struct CachedTransform<T> where T : Component
{
	// Token: 0x040032B9 RID: 12985
	public T component;

	// Token: 0x040032BA RID: 12986
	public Vector3 position;

	// Token: 0x040032BB RID: 12987
	public Quaternion rotation;

	// Token: 0x040032BC RID: 12988
	public Vector3 localScale;

	// Token: 0x060037C3 RID: 14275 RVA: 0x0014DFFC File Offset: 0x0014C1FC
	public CachedTransform(T instance)
	{
		this.component = instance;
		if (this.component)
		{
			this.position = this.component.transform.position;
			this.rotation = this.component.transform.rotation;
			this.localScale = this.component.transform.localScale;
			return;
		}
		this.position = Vector3.zero;
		this.rotation = Quaternion.identity;
		this.localScale = Vector3.one;
	}

	// Token: 0x060037C4 RID: 14276 RVA: 0x0014E098 File Offset: 0x0014C298
	public void Apply()
	{
		if (this.component)
		{
			this.component.transform.SetPositionAndRotation(this.position, this.rotation);
			this.component.transform.localScale = this.localScale;
		}
	}

	// Token: 0x060037C5 RID: 14277 RVA: 0x0014E0F4 File Offset: 0x0014C2F4
	public void RotateAround(Vector3 center, Vector3 axis, float angle)
	{
		Quaternion rhs = Quaternion.AngleAxis(angle, axis);
		Vector3 b = rhs * (this.position - center);
		this.position = center + b;
		this.rotation *= Quaternion.Inverse(this.rotation) * rhs * this.rotation;
	}

	// Token: 0x1700046F RID: 1135
	// (get) Token: 0x060037C6 RID: 14278 RVA: 0x0014E156 File Offset: 0x0014C356
	public Matrix4x4 localToWorldMatrix
	{
		get
		{
			return Matrix4x4.TRS(this.position, this.rotation, this.localScale);
		}
	}

	// Token: 0x17000470 RID: 1136
	// (get) Token: 0x060037C7 RID: 14279 RVA: 0x0014E170 File Offset: 0x0014C370
	public Matrix4x4 worldToLocalMatrix
	{
		get
		{
			return this.localToWorldMatrix.inverse;
		}
	}

	// Token: 0x17000471 RID: 1137
	// (get) Token: 0x060037C8 RID: 14280 RVA: 0x0014E18B File Offset: 0x0014C38B
	public Vector3 forward
	{
		get
		{
			return this.rotation * Vector3.forward;
		}
	}

	// Token: 0x17000472 RID: 1138
	// (get) Token: 0x060037C9 RID: 14281 RVA: 0x0014E19D File Offset: 0x0014C39D
	public Vector3 up
	{
		get
		{
			return this.rotation * Vector3.up;
		}
	}

	// Token: 0x17000473 RID: 1139
	// (get) Token: 0x060037CA RID: 14282 RVA: 0x0014E1AF File Offset: 0x0014C3AF
	public Vector3 right
	{
		get
		{
			return this.rotation * Vector3.right;
		}
	}

	// Token: 0x060037CB RID: 14283 RVA: 0x0014E1C1 File Offset: 0x0014C3C1
	public static implicit operator bool(CachedTransform<T> instance)
	{
		return instance.component != null;
	}
}
