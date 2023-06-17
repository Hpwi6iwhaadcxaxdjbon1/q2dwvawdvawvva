using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020009AD RID: 2477
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(VolumetricLightBeam))]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-dynocclusion/")]
	public class DynamicOcclusion : MonoBehaviour
	{
		// Token: 0x040035C0 RID: 13760
		public LayerMask layerMask = -1;

		// Token: 0x040035C1 RID: 13761
		public float minOccluderArea;

		// Token: 0x040035C2 RID: 13762
		public int waitFrameCount = 3;

		// Token: 0x040035C3 RID: 13763
		public float minSurfaceRatio = 0.5f;

		// Token: 0x040035C4 RID: 13764
		public float maxSurfaceDot = 0.25f;

		// Token: 0x040035C5 RID: 13765
		public PlaneAlignment planeAlignment;

		// Token: 0x040035C6 RID: 13766
		public float planeOffset = 0.1f;

		// Token: 0x040035C7 RID: 13767
		private VolumetricLightBeam m_Master;

		// Token: 0x040035C8 RID: 13768
		private int m_FrameCountToWait;

		// Token: 0x040035C9 RID: 13769
		private float m_RangeMultiplier = 1f;

		// Token: 0x040035CA RID: 13770
		private uint m_PrevNonSubHitDirectionId;

		// Token: 0x06003B2D RID: 15149 RVA: 0x0015EDC9 File Offset: 0x0015CFC9
		private void OnValidate()
		{
			this.minOccluderArea = Mathf.Max(this.minOccluderArea, 0f);
			this.waitFrameCount = Mathf.Clamp(this.waitFrameCount, 1, 60);
		}

		// Token: 0x06003B2E RID: 15150 RVA: 0x0015EDF5 File Offset: 0x0015CFF5
		private void OnEnable()
		{
			this.m_Master = base.GetComponent<VolumetricLightBeam>();
			Debug.Assert(this.m_Master);
		}

		// Token: 0x06003B2F RID: 15151 RVA: 0x0015EE13 File Offset: 0x0015D013
		private void OnDisable()
		{
			this.SetHitNull();
		}

		// Token: 0x06003B30 RID: 15152 RVA: 0x0015EE1C File Offset: 0x0015D01C
		private void Start()
		{
			if (Application.isPlaying)
			{
				TriggerZone component = base.GetComponent<TriggerZone>();
				if (component)
				{
					this.m_RangeMultiplier = Mathf.Max(1f, component.rangeMultiplier);
				}
			}
		}

		// Token: 0x06003B31 RID: 15153 RVA: 0x0015EE55 File Offset: 0x0015D055
		private void LateUpdate()
		{
			if (this.m_FrameCountToWait <= 0)
			{
				this.ProcessRaycasts();
				this.m_FrameCountToWait = this.waitFrameCount;
			}
			this.m_FrameCountToWait--;
		}

		// Token: 0x06003B32 RID: 15154 RVA: 0x0015EE80 File Offset: 0x0015D080
		private Vector3 GetRandomVectorAround(Vector3 direction, float angleDiff)
		{
			float num = angleDiff * 0.5f;
			return Quaternion.Euler(UnityEngine.Random.Range(-num, num), UnityEngine.Random.Range(-num, num), UnityEngine.Random.Range(-num, num)) * direction;
		}

		// Token: 0x06003B33 RID: 15155 RVA: 0x0015EEB8 File Offset: 0x0015D0B8
		private RaycastHit GetBestHit(Vector3 rayPos, Vector3 rayDir)
		{
			RaycastHit[] array = Physics.RaycastAll(rayPos, rayDir, this.m_Master.fadeEnd * this.m_RangeMultiplier, this.layerMask.value);
			int num = -1;
			float num2 = float.MaxValue;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].collider.isTrigger && array[i].collider.bounds.GetMaxArea2D() >= this.minOccluderArea && array[i].distance < num2)
				{
					num2 = array[i].distance;
					num = i;
				}
			}
			if (num != -1)
			{
				return array[num];
			}
			return default(RaycastHit);
		}

		// Token: 0x06003B34 RID: 15156 RVA: 0x0015EF68 File Offset: 0x0015D168
		private Vector3 GetDirection(uint dirInt)
		{
			dirInt %= (uint)Enum.GetValues(typeof(DynamicOcclusion.Direction)).Length;
			switch (dirInt)
			{
			case 0U:
				return base.transform.up;
			case 1U:
				return base.transform.right;
			case 2U:
				return -base.transform.up;
			case 3U:
				return -base.transform.right;
			default:
				return Vector3.zero;
			}
		}

		// Token: 0x06003B35 RID: 15157 RVA: 0x0015EFE4 File Offset: 0x0015D1E4
		private bool IsHitValid(RaycastHit hit)
		{
			return hit.collider && Vector3.Dot(hit.normal, -base.transform.forward) >= this.maxSurfaceDot;
		}

		// Token: 0x06003B36 RID: 15158 RVA: 0x0015F020 File Offset: 0x0015D220
		private void ProcessRaycasts()
		{
			RaycastHit hit = this.GetBestHit(base.transform.position, base.transform.forward);
			if (this.IsHitValid(hit))
			{
				if (this.minSurfaceRatio > 0.5f)
				{
					for (uint num = 0U; num < (uint)Enum.GetValues(typeof(DynamicOcclusion.Direction)).Length; num += 1U)
					{
						Vector3 direction = this.GetDirection(num + this.m_PrevNonSubHitDirectionId);
						Vector3 vector = base.transform.position + direction * this.m_Master.coneRadiusStart * (this.minSurfaceRatio * 2f - 1f);
						Vector3 a = base.transform.position + base.transform.forward * this.m_Master.fadeEnd + direction * this.m_Master.coneRadiusEnd * (this.minSurfaceRatio * 2f - 1f);
						RaycastHit bestHit = this.GetBestHit(vector, a - vector);
						if (!this.IsHitValid(bestHit))
						{
							this.m_PrevNonSubHitDirectionId = num;
							this.SetHitNull();
							return;
						}
						if (bestHit.distance > hit.distance)
						{
							hit = bestHit;
						}
					}
				}
				this.SetHit(hit);
				return;
			}
			this.SetHitNull();
		}

		// Token: 0x06003B37 RID: 15159 RVA: 0x0015F17C File Offset: 0x0015D37C
		private void SetHit(RaycastHit hit)
		{
			PlaneAlignment planeAlignment = this.planeAlignment;
			if (planeAlignment != PlaneAlignment.Surface && planeAlignment == PlaneAlignment.Beam)
			{
				this.SetClippingPlane(new Plane(-base.transform.forward, hit.point));
				return;
			}
			this.SetClippingPlane(new Plane(hit.normal, hit.point));
		}

		// Token: 0x06003B38 RID: 15160 RVA: 0x0015F1D3 File Offset: 0x0015D3D3
		private void SetHitNull()
		{
			this.SetClippingPlaneOff();
		}

		// Token: 0x06003B39 RID: 15161 RVA: 0x0015F1DB File Offset: 0x0015D3DB
		private void SetClippingPlane(Plane planeWS)
		{
			planeWS = planeWS.TranslateCustom(planeWS.normal * this.planeOffset);
			this.m_Master.SetClippingPlane(planeWS);
		}

		// Token: 0x06003B3A RID: 15162 RVA: 0x0015F203 File Offset: 0x0015D403
		private void SetClippingPlaneOff()
		{
			this.m_Master.SetClippingPlaneOff();
		}

		// Token: 0x02000EE0 RID: 3808
		private enum Direction
		{
			// Token: 0x04004D6E RID: 19822
			Up,
			// Token: 0x04004D6F RID: 19823
			Right,
			// Token: 0x04004D70 RID: 19824
			Down,
			// Token: 0x04004D71 RID: 19825
			Left
		}
	}
}
