using System;
using UnityEngine;

// Token: 0x0200099D RID: 2461
public class Occludee : MonoBehaviour
{
	// Token: 0x040034E2 RID: 13538
	public float minTimeVisible = 0.1f;

	// Token: 0x040034E3 RID: 13539
	public bool isStatic = true;

	// Token: 0x040034E4 RID: 13540
	public bool autoRegister;

	// Token: 0x040034E5 RID: 13541
	public bool stickyGizmos;

	// Token: 0x040034E6 RID: 13542
	public OccludeeState state;

	// Token: 0x040034E7 RID: 13543
	protected int occludeeId = -1;

	// Token: 0x040034E8 RID: 13544
	protected Vector3 center;

	// Token: 0x040034E9 RID: 13545
	protected float radius;

	// Token: 0x040034EA RID: 13546
	protected Renderer renderer;

	// Token: 0x040034EB RID: 13547
	protected Collider collider;

	// Token: 0x06003A80 RID: 14976 RVA: 0x00159E88 File Offset: 0x00158088
	protected virtual void Awake()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.collider = base.GetComponent<Collider>();
	}

	// Token: 0x06003A81 RID: 14977 RVA: 0x00159EA2 File Offset: 0x001580A2
	public void OnEnable()
	{
		if (this.autoRegister && this.collider != null)
		{
			this.Register();
		}
	}

	// Token: 0x06003A82 RID: 14978 RVA: 0x00159EC0 File Offset: 0x001580C0
	public void OnDisable()
	{
		if (this.autoRegister && this.occludeeId >= 0)
		{
			this.Unregister();
		}
	}

	// Token: 0x06003A83 RID: 14979 RVA: 0x00159EDC File Offset: 0x001580DC
	public void Register()
	{
		this.center = this.collider.bounds.center;
		this.radius = Mathf.Max(Mathf.Max(this.collider.bounds.extents.x, this.collider.bounds.extents.y), this.collider.bounds.extents.z);
		this.occludeeId = OcclusionCulling.RegisterOccludee(this.center, this.radius, this.renderer.enabled, this.minTimeVisible, this.isStatic, base.gameObject.layer, new OcclusionCulling.OnVisibilityChanged(this.OnVisibilityChanged));
		if (this.occludeeId < 0)
		{
			Debug.LogWarning("[OcclusionCulling] Occludee registration failed for " + base.name + ". Too many registered.");
		}
		this.state = OcclusionCulling.GetStateById(this.occludeeId);
	}

	// Token: 0x06003A84 RID: 14980 RVA: 0x00159FD4 File Offset: 0x001581D4
	public void Unregister()
	{
		OcclusionCulling.UnregisterOccludee(this.occludeeId);
	}

	// Token: 0x06003A85 RID: 14981 RVA: 0x00159FE1 File Offset: 0x001581E1
	protected virtual void OnVisibilityChanged(bool visible)
	{
		if (this.renderer != null)
		{
			this.renderer.enabled = visible;
		}
	}
}
