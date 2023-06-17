using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020002FF RID: 767
public class ArticulatedOccludee : BaseMonoBehaviour
{
	// Token: 0x04001784 RID: 6020
	private const float UpdateBoundsFadeStart = 20f;

	// Token: 0x04001785 RID: 6021
	private const float UpdateBoundsFadeLength = 1000f;

	// Token: 0x04001786 RID: 6022
	private const float UpdateBoundsMaxFrequency = 15f;

	// Token: 0x04001787 RID: 6023
	private const float UpdateBoundsMinFrequency = 0.5f;

	// Token: 0x04001788 RID: 6024
	private LODGroup lodGroup;

	// Token: 0x04001789 RID: 6025
	public List<Collider> colliders = new List<Collider>();

	// Token: 0x0400178A RID: 6026
	private OccludeeSphere localOccludee = new OccludeeSphere(-1);

	// Token: 0x0400178B RID: 6027
	private List<Renderer> renderers = new List<Renderer>();

	// Token: 0x0400178C RID: 6028
	private bool isVisible = true;

	// Token: 0x0400178D RID: 6029
	private Action TriggerUpdateVisibilityBoundsDelegate;

	// Token: 0x1700027C RID: 636
	// (get) Token: 0x06001E5C RID: 7772 RVA: 0x000CE6DA File Offset: 0x000CC8DA
	public bool IsVisible
	{
		get
		{
			return this.isVisible;
		}
	}

	// Token: 0x06001E5D RID: 7773 RVA: 0x000CE6E2 File Offset: 0x000CC8E2
	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.UnregisterFromCulling();
		this.ClearVisibility();
	}

	// Token: 0x06001E5E RID: 7774 RVA: 0x000CE6F8 File Offset: 0x000CC8F8
	private void ClearVisibility()
	{
		if (this.lodGroup != null)
		{
			this.lodGroup.localReferencePoint = Vector3.zero;
			this.lodGroup.RecalculateBounds();
			this.lodGroup = null;
		}
		if (this.renderers != null)
		{
			this.renderers.Clear();
		}
		this.localOccludee = new OccludeeSphere(-1);
	}

	// Token: 0x06001E5F RID: 7775 RVA: 0x000CE754 File Offset: 0x000CC954
	public void ProcessVisibility(LODGroup lod)
	{
		this.lodGroup = lod;
		if (lod != null)
		{
			this.renderers = new List<Renderer>(16);
			LOD[] lods = lod.GetLODs();
			for (int i = 0; i < lods.Length; i++)
			{
				foreach (Renderer renderer in lods[i].renderers)
				{
					if (renderer != null)
					{
						this.renderers.Add(renderer);
					}
				}
			}
		}
		this.UpdateCullingBounds();
	}

	// Token: 0x06001E60 RID: 7776 RVA: 0x000CE7D0 File Offset: 0x000CC9D0
	private void RegisterForCulling(OcclusionCulling.Sphere sphere, bool visible)
	{
		if (this.localOccludee.IsRegistered)
		{
			this.UnregisterFromCulling();
		}
		int num = OcclusionCulling.RegisterOccludee(sphere.position, sphere.radius, visible, 0.25f, false, base.gameObject.layer, new OcclusionCulling.OnVisibilityChanged(this.OnVisibilityChanged));
		if (num >= 0)
		{
			this.localOccludee = new OccludeeSphere(num, this.localOccludee.sphere);
			return;
		}
		this.localOccludee.Invalidate();
		Debug.LogWarning("[OcclusionCulling] Occludee registration failed for " + base.name + ". Too many registered.");
	}

	// Token: 0x06001E61 RID: 7777 RVA: 0x000CE862 File Offset: 0x000CCA62
	private void UnregisterFromCulling()
	{
		if (this.localOccludee.IsRegistered)
		{
			OcclusionCulling.UnregisterOccludee(this.localOccludee.id);
			this.localOccludee.Invalidate();
		}
	}

	// Token: 0x06001E62 RID: 7778 RVA: 0x000CE88C File Offset: 0x000CCA8C
	public void UpdateCullingBounds()
	{
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		bool flag = false;
		int num = (this.renderers != null) ? this.renderers.Count : 0;
		int num2 = (this.colliders != null) ? this.colliders.Count : 0;
		if (num > 0 && (num2 == 0 || num < num2))
		{
			for (int i = 0; i < this.renderers.Count; i++)
			{
				if (this.renderers[i].isVisible)
				{
					Bounds bounds = this.renderers[i].bounds;
					Vector3 min = bounds.min;
					Vector3 max = bounds.max;
					if (!flag)
					{
						vector = min;
						vector2 = max;
						flag = true;
					}
					else
					{
						vector.x = ((vector.x < min.x) ? vector.x : min.x);
						vector.y = ((vector.y < min.y) ? vector.y : min.y);
						vector.z = ((vector.z < min.z) ? vector.z : min.z);
						vector2.x = ((vector2.x > max.x) ? vector2.x : max.x);
						vector2.y = ((vector2.y > max.y) ? vector2.y : max.y);
						vector2.z = ((vector2.z > max.z) ? vector2.z : max.z);
					}
				}
			}
		}
		if (!flag && num2 > 0)
		{
			flag = true;
			vector = this.colliders[0].bounds.min;
			vector2 = this.colliders[0].bounds.max;
			for (int j = 1; j < this.colliders.Count; j++)
			{
				Bounds bounds2 = this.colliders[j].bounds;
				Vector3 min2 = bounds2.min;
				Vector3 max2 = bounds2.max;
				vector.x = ((vector.x < min2.x) ? vector.x : min2.x);
				vector.y = ((vector.y < min2.y) ? vector.y : min2.y);
				vector.z = ((vector.z < min2.z) ? vector.z : min2.z);
				vector2.x = ((vector2.x > max2.x) ? vector2.x : max2.x);
				vector2.y = ((vector2.y > max2.y) ? vector2.y : max2.y);
				vector2.z = ((vector2.z > max2.z) ? vector2.z : max2.z);
			}
		}
		if (flag)
		{
			Vector3 vector3 = vector2 - vector;
			Vector3 position = vector + vector3 * 0.5f;
			float radius = Mathf.Max(Mathf.Max(vector3.x, vector3.y), vector3.z) * 0.5f;
			OcclusionCulling.Sphere sphere = new OcclusionCulling.Sphere(position, radius);
			if (this.localOccludee.IsRegistered)
			{
				OcclusionCulling.UpdateDynamicOccludee(this.localOccludee.id, sphere.position, sphere.radius);
				this.localOccludee.sphere = sphere;
				return;
			}
			bool visible = true;
			if (this.lodGroup != null)
			{
				visible = this.lodGroup.enabled;
			}
			this.RegisterForCulling(sphere, visible);
		}
	}

	// Token: 0x06001E63 RID: 7779 RVA: 0x000CEC54 File Offset: 0x000CCE54
	protected virtual bool CheckVisibility()
	{
		return this.localOccludee.state == null || this.localOccludee.state.isVisible;
	}

	// Token: 0x06001E64 RID: 7780 RVA: 0x000CEC78 File Offset: 0x000CCE78
	private void ApplyVisibility(bool vis)
	{
		if (this.lodGroup != null)
		{
			float num = (float)(vis ? 0 : 100000);
			if (num != this.lodGroup.localReferencePoint.x)
			{
				this.lodGroup.localReferencePoint = new Vector3(num, num, num);
			}
		}
	}

	// Token: 0x06001E65 RID: 7781 RVA: 0x000CECC8 File Offset: 0x000CCEC8
	protected virtual void OnVisibilityChanged(bool visible)
	{
		if (MainCamera.mainCamera != null && this.localOccludee.IsRegistered)
		{
			float dist = Vector3.Distance(MainCamera.position, base.transform.position);
			this.VisUpdateUsingCulling(dist, visible);
			this.ApplyVisibility(this.isVisible);
		}
	}

	// Token: 0x06001E66 RID: 7782 RVA: 0x000063A5 File Offset: 0x000045A5
	private void UpdateVisibility(float delay)
	{
	}

	// Token: 0x06001E67 RID: 7783 RVA: 0x000063A5 File Offset: 0x000045A5
	private void VisUpdateUsingCulling(float dist, bool visibility)
	{
	}

	// Token: 0x06001E68 RID: 7784 RVA: 0x000CED1C File Offset: 0x000CCF1C
	public virtual void TriggerUpdateVisibilityBounds()
	{
		if (base.enabled)
		{
			float sqrMagnitude = (base.transform.position - MainCamera.position).sqrMagnitude;
			float num = 400f;
			float num2;
			if (sqrMagnitude < num)
			{
				num2 = 1f / UnityEngine.Random.Range(5f, 25f);
			}
			else
			{
				float t = Mathf.Clamp01((Mathf.Sqrt(sqrMagnitude) - 20f) * 0.001f);
				float num3 = Mathf.Lerp(0.06666667f, 2f, t);
				num2 = UnityEngine.Random.Range(num3, num3 + 0.06666667f);
			}
			this.UpdateVisibility(num2);
			this.ApplyVisibility(this.isVisible);
			if (this.TriggerUpdateVisibilityBoundsDelegate == null)
			{
				this.TriggerUpdateVisibilityBoundsDelegate = new Action(this.TriggerUpdateVisibilityBounds);
			}
			base.Invoke(this.TriggerUpdateVisibilityBoundsDelegate, num2);
		}
	}
}
