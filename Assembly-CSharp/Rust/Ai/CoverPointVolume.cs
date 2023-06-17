using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	// Token: 0x02000B39 RID: 2873
	public class CoverPointVolume : MonoBehaviour, IServerComponent
	{
		// Token: 0x04003E10 RID: 15888
		public float DefaultCoverPointScore = 1f;

		// Token: 0x04003E11 RID: 15889
		public float CoverPointRayLength = 1f;

		// Token: 0x04003E12 RID: 15890
		public LayerMask CoverLayerMask;

		// Token: 0x04003E13 RID: 15891
		public Transform BlockerGroup;

		// Token: 0x04003E14 RID: 15892
		public Transform ManualCoverPointGroup;

		// Token: 0x04003E15 RID: 15893
		[ServerVar(Help = "cover_point_sample_step_size defines the size of the steps we do horizontally for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 6.0)")]
		public static float cover_point_sample_step_size = 6f;

		// Token: 0x04003E16 RID: 15894
		[ServerVar(Help = "cover_point_sample_step_height defines the height of the steps we do vertically for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 2.0)")]
		public static float cover_point_sample_step_height = 2f;

		// Token: 0x04003E17 RID: 15895
		public readonly List<CoverPoint> CoverPoints = new List<CoverPoint>();

		// Token: 0x04003E18 RID: 15896
		private readonly List<CoverPointBlockerVolume> _coverPointBlockers = new List<CoverPointBlockerVolume>();

		// Token: 0x04003E19 RID: 15897
		private float _dynNavMeshBuildCompletionTime = -1f;

		// Token: 0x04003E1A RID: 15898
		private int _genAttempts;

		// Token: 0x04003E1B RID: 15899
		private Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x060045A8 RID: 17832 RVA: 0x0000441C File Offset: 0x0000261C
		public bool repeat
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060045A9 RID: 17833 RVA: 0x00196AF0 File Offset: 0x00194CF0
		public float? ExecuteUpdate(float deltaTime, float nextInterval)
		{
			if (this.CoverPoints.Count == 0)
			{
				if (this._dynNavMeshBuildCompletionTime < 0f)
				{
					if (SingletonComponent<DynamicNavMesh>.Instance == null || !SingletonComponent<DynamicNavMesh>.Instance.enabled || !SingletonComponent<DynamicNavMesh>.Instance.IsBuilding)
					{
						this._dynNavMeshBuildCompletionTime = Time.realtimeSinceStartup;
					}
				}
				else if (this._genAttempts < 4 && Time.realtimeSinceStartup - this._dynNavMeshBuildCompletionTime > 0.25f)
				{
					this.GenerateCoverPoints(null);
					if (this.CoverPoints.Count != 0)
					{
						return null;
					}
					this._dynNavMeshBuildCompletionTime = Time.realtimeSinceStartup;
					this._genAttempts++;
					if (this._genAttempts >= 4)
					{
						UnityEngine.Object.Destroy(base.gameObject);
						return null;
					}
				}
			}
			return new float?(1f + UnityEngine.Random.value * 2f);
		}

		// Token: 0x060045AA RID: 17834 RVA: 0x00196BD3 File Offset: 0x00194DD3
		[ContextMenu("Clear Cover Points")]
		private void ClearCoverPoints()
		{
			this.CoverPoints.Clear();
			this._coverPointBlockers.Clear();
		}

		// Token: 0x060045AB RID: 17835 RVA: 0x00196BEC File Offset: 0x00194DEC
		public Bounds GetBounds()
		{
			if (Mathf.Approximately(this.bounds.center.sqrMagnitude, 0f))
			{
				this.bounds = new Bounds(base.transform.position, base.transform.localScale);
			}
			return this.bounds;
		}

		// Token: 0x060045AC RID: 17836 RVA: 0x00196C3F File Offset: 0x00194E3F
		[ContextMenu("Pre-Generate Cover Points")]
		public void PreGenerateCoverPoints()
		{
			this.GenerateCoverPoints(null);
		}

		// Token: 0x060045AD RID: 17837 RVA: 0x00196C48 File Offset: 0x00194E48
		[ContextMenu("Convert to Manual Cover Points")]
		public void ConvertToManualCoverPoints()
		{
			foreach (CoverPoint coverPoint in this.CoverPoints)
			{
				ManualCoverPoint manualCoverPoint = new GameObject("MCP").AddComponent<ManualCoverPoint>();
				manualCoverPoint.transform.localPosition = Vector3.zero;
				manualCoverPoint.transform.position = coverPoint.Position;
				manualCoverPoint.Normal = coverPoint.Normal;
				manualCoverPoint.NormalCoverType = coverPoint.NormalCoverType;
				manualCoverPoint.Volume = this;
			}
		}

		// Token: 0x060045AE RID: 17838 RVA: 0x00196CE4 File Offset: 0x00194EE4
		public void GenerateCoverPoints(Transform coverPointGroup)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			this.ClearCoverPoints();
			if (this.ManualCoverPointGroup == null)
			{
				this.ManualCoverPointGroup = coverPointGroup;
			}
			if (this.ManualCoverPointGroup == null)
			{
				this.ManualCoverPointGroup = base.transform;
			}
			if (this.ManualCoverPointGroup.childCount > 0)
			{
				ManualCoverPoint[] componentsInChildren = this.ManualCoverPointGroup.GetComponentsInChildren<ManualCoverPoint>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					CoverPoint item = componentsInChildren[i].ToCoverPoint(this);
					this.CoverPoints.Add(item);
				}
			}
			if (this._coverPointBlockers.Count == 0 && this.BlockerGroup != null)
			{
				CoverPointBlockerVolume[] componentsInChildren2 = this.BlockerGroup.GetComponentsInChildren<CoverPointBlockerVolume>();
				if (componentsInChildren2 != null && componentsInChildren2.Length != 0)
				{
					this._coverPointBlockers.AddRange(componentsInChildren2);
				}
			}
			NavMeshHit navMeshHit;
			if (this.CoverPoints.Count == 0 && NavMesh.SamplePosition(base.transform.position, out navMeshHit, base.transform.localScale.y * CoverPointVolume.cover_point_sample_step_height, -1))
			{
				Vector3 position = base.transform.position;
				Vector3 vector = base.transform.lossyScale * 0.5f;
				for (float num = position.x - vector.x + 1f; num < position.x + vector.x - 1f; num += CoverPointVolume.cover_point_sample_step_size)
				{
					for (float num2 = position.z - vector.z + 1f; num2 < position.z + vector.z - 1f; num2 += CoverPointVolume.cover_point_sample_step_size)
					{
						for (float num3 = position.y - vector.y; num3 < position.y + vector.y; num3 += CoverPointVolume.cover_point_sample_step_height)
						{
							NavMeshHit info;
							if (NavMesh.FindClosestEdge(new Vector3(num, num3, num2), out info, navMeshHit.mask))
							{
								info.position = new Vector3(info.position.x, info.position.y + 0.5f, info.position.z);
								bool flag = true;
								using (List<CoverPoint>.Enumerator enumerator = this.CoverPoints.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										if ((enumerator.Current.Position - info.position).sqrMagnitude < CoverPointVolume.cover_point_sample_step_size * CoverPointVolume.cover_point_sample_step_size)
										{
											flag = false;
											break;
										}
									}
								}
								if (flag)
								{
									CoverPoint coverPoint = this.CalculateCoverPoint(info);
									if (coverPoint != null)
									{
										this.CoverPoints.Add(coverPoint);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060045AF RID: 17839 RVA: 0x00196FA4 File Offset: 0x001951A4
		private CoverPoint CalculateCoverPoint(NavMeshHit info)
		{
			RaycastHit raycastHit;
			CoverPointVolume.CoverType coverType = this.ProvidesCoverInDir(new Ray(info.position, -info.normal), this.CoverPointRayLength, out raycastHit);
			if (coverType == CoverPointVolume.CoverType.None)
			{
				return null;
			}
			CoverPoint coverPoint = new CoverPoint(this, this.DefaultCoverPointScore)
			{
				Position = info.position,
				Normal = -info.normal
			};
			if (coverType == CoverPointVolume.CoverType.Full)
			{
				coverPoint.NormalCoverType = CoverPoint.CoverType.Full;
			}
			else if (coverType == CoverPointVolume.CoverType.Partial)
			{
				coverPoint.NormalCoverType = CoverPoint.CoverType.Partial;
			}
			return coverPoint;
		}

		// Token: 0x060045B0 RID: 17840 RVA: 0x00197024 File Offset: 0x00195224
		internal CoverPointVolume.CoverType ProvidesCoverInDir(Ray ray, float maxDistance, out RaycastHit rayHit)
		{
			rayHit = default(RaycastHit);
			if (ray.origin.IsNaNOrInfinity())
			{
				return CoverPointVolume.CoverType.None;
			}
			if (ray.direction.IsNaNOrInfinity())
			{
				return CoverPointVolume.CoverType.None;
			}
			if (ray.direction == Vector3.zero)
			{
				return CoverPointVolume.CoverType.None;
			}
			ray.origin += PlayerEyes.EyeOffset;
			if (Physics.Raycast(ray.origin, ray.direction, out rayHit, maxDistance, this.CoverLayerMask))
			{
				return CoverPointVolume.CoverType.Full;
			}
			ray.origin += PlayerEyes.DuckOffset;
			if (Physics.Raycast(ray.origin, ray.direction, out rayHit, maxDistance, this.CoverLayerMask))
			{
				return CoverPointVolume.CoverType.Partial;
			}
			return CoverPointVolume.CoverType.None;
		}

		// Token: 0x060045B1 RID: 17841 RVA: 0x001970E8 File Offset: 0x001952E8
		public bool Contains(Vector3 point)
		{
			Bounds bounds = new Bounds(base.transform.position, base.transform.localScale);
			return bounds.Contains(point);
		}

		// Token: 0x02000F9C RID: 3996
		internal enum CoverType
		{
			// Token: 0x04005069 RID: 20585
			None,
			// Token: 0x0400506A RID: 20586
			Partial,
			// Token: 0x0400506B RID: 20587
			Full
		}
	}
}
