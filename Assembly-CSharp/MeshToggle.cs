using System;
using UnityEngine;

// Token: 0x02000905 RID: 2309
public class MeshToggle : MonoBehaviour
{
	// Token: 0x040032F1 RID: 13041
	public Mesh[] RendererMeshes;

	// Token: 0x040032F2 RID: 13042
	public Mesh[] ColliderMeshes;

	// Token: 0x060037F4 RID: 14324 RVA: 0x0014EBA8 File Offset: 0x0014CDA8
	public void SwitchRenderer(int index)
	{
		if (this.RendererMeshes.Length == 0)
		{
			return;
		}
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (!component)
		{
			return;
		}
		component.sharedMesh = this.RendererMeshes[Mathf.Clamp(index, 0, this.RendererMeshes.Length - 1)];
	}

	// Token: 0x060037F5 RID: 14325 RVA: 0x0014EBF0 File Offset: 0x0014CDF0
	public void SwitchRenderer(float factor)
	{
		int index = Mathf.RoundToInt(factor * (float)this.RendererMeshes.Length);
		this.SwitchRenderer(index);
	}

	// Token: 0x060037F6 RID: 14326 RVA: 0x0014EC18 File Offset: 0x0014CE18
	public void SwitchCollider(int index)
	{
		if (this.ColliderMeshes.Length == 0)
		{
			return;
		}
		MeshCollider component = base.GetComponent<MeshCollider>();
		if (!component)
		{
			return;
		}
		component.sharedMesh = this.ColliderMeshes[Mathf.Clamp(index, 0, this.ColliderMeshes.Length - 1)];
	}

	// Token: 0x060037F7 RID: 14327 RVA: 0x0014EC60 File Offset: 0x0014CE60
	public void SwitchCollider(float factor)
	{
		int index = Mathf.RoundToInt(factor * (float)this.ColliderMeshes.Length);
		this.SwitchCollider(index);
	}

	// Token: 0x060037F8 RID: 14328 RVA: 0x0014EC85 File Offset: 0x0014CE85
	public void SwitchAll(int index)
	{
		this.SwitchRenderer(index);
		this.SwitchCollider(index);
	}

	// Token: 0x060037F9 RID: 14329 RVA: 0x0014EC95 File Offset: 0x0014CE95
	public void SwitchAll(float factor)
	{
		this.SwitchRenderer(factor);
		this.SwitchCollider(factor);
	}
}
