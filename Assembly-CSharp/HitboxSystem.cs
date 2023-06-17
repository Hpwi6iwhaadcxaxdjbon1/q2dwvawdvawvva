using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x020002FB RID: 763
public class HitboxSystem : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x04001783 RID: 6019
	public List<HitboxSystem.HitboxShape> hitboxes = new List<HitboxSystem.HitboxShape>();

	// Token: 0x06001E50 RID: 7760 RVA: 0x000CE304 File Offset: 0x000CC504
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		List<HitboxDefinition> list = Pool.GetList<HitboxDefinition>();
		base.GetComponentsInChildren<HitboxDefinition>(list);
		if (serverside)
		{
			foreach (HitboxDefinition component in list)
			{
				if (preProcess != null)
				{
					preProcess.RemoveComponent(component);
				}
			}
			if (preProcess != null)
			{
				preProcess.RemoveComponent(this);
			}
		}
		if (clientside)
		{
			this.hitboxes.Clear();
			foreach (HitboxDefinition hitboxDefinition in from x in list
			orderby x.priority
			select x)
			{
				HitboxSystem.HitboxShape item = new HitboxSystem.HitboxShape
				{
					bone = hitboxDefinition.transform,
					localTransform = hitboxDefinition.LocalMatrix,
					colliderMaterial = hitboxDefinition.physicMaterial,
					type = hitboxDefinition.type
				};
				this.hitboxes.Add(item);
				if (preProcess != null)
				{
					preProcess.RemoveComponent(hitboxDefinition);
				}
			}
		}
		Pool.FreeList<HitboxDefinition>(ref list);
	}

	// Token: 0x02000C9F RID: 3231
	[Serializable]
	public class HitboxShape
	{
		// Token: 0x04004419 RID: 17433
		public Transform bone;

		// Token: 0x0400441A RID: 17434
		public HitboxDefinition.Type type;

		// Token: 0x0400441B RID: 17435
		public Matrix4x4 localTransform;

		// Token: 0x0400441C RID: 17436
		public PhysicMaterial colliderMaterial;

		// Token: 0x0400441D RID: 17437
		private Matrix4x4 transform;

		// Token: 0x0400441E RID: 17438
		private Matrix4x4 inverseTransform;

		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x06004F2F RID: 20271 RVA: 0x001A5CE7 File Offset: 0x001A3EE7
		public Matrix4x4 Transform
		{
			get
			{
				return this.transform;
			}
		}

		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x06004F30 RID: 20272 RVA: 0x001A5CEF File Offset: 0x001A3EEF
		public Vector3 Position
		{
			get
			{
				return this.transform.MultiplyPoint(Vector3.zero);
			}
		}

		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x06004F31 RID: 20273 RVA: 0x001A5D01 File Offset: 0x001A3F01
		public Quaternion Rotation
		{
			get
			{
				return this.transform.rotation;
			}
		}

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x06004F32 RID: 20274 RVA: 0x001A5D0E File Offset: 0x001A3F0E
		// (set) Token: 0x06004F33 RID: 20275 RVA: 0x001A5D16 File Offset: 0x001A3F16
		public Vector3 Size { get; private set; }

		// Token: 0x06004F34 RID: 20276 RVA: 0x001A5D20 File Offset: 0x001A3F20
		public void UpdateTransform()
		{
			using (TimeWarning.New("HitboxSystem.UpdateTransform", 0))
			{
				this.transform = this.bone.localToWorldMatrix * this.localTransform;
				this.Size = this.transform.lossyScale;
				this.transform = Matrix4x4.TRS(this.Position, this.Rotation, Vector3.one);
				this.inverseTransform = this.transform.inverse;
			}
		}

		// Token: 0x06004F35 RID: 20277 RVA: 0x001A5DB0 File Offset: 0x001A3FB0
		public Vector3 TransformPoint(Vector3 pt)
		{
			return this.transform.MultiplyPoint(pt);
		}

		// Token: 0x06004F36 RID: 20278 RVA: 0x001A5DBE File Offset: 0x001A3FBE
		public Vector3 InverseTransformPoint(Vector3 pt)
		{
			return this.inverseTransform.MultiplyPoint(pt);
		}

		// Token: 0x06004F37 RID: 20279 RVA: 0x001A5DCC File Offset: 0x001A3FCC
		public Vector3 TransformDirection(Vector3 pt)
		{
			return this.transform.MultiplyVector(pt);
		}

		// Token: 0x06004F38 RID: 20280 RVA: 0x001A5DDA File Offset: 0x001A3FDA
		public Vector3 InverseTransformDirection(Vector3 pt)
		{
			return this.inverseTransform.MultiplyVector(pt);
		}

		// Token: 0x06004F39 RID: 20281 RVA: 0x001A5DE8 File Offset: 0x001A3FE8
		public bool Trace(Ray ray, out RaycastHit hit, float forgivness = 0f, float maxDistance = float.PositiveInfinity)
		{
			bool result;
			using (TimeWarning.New("Hitbox.Trace", 0))
			{
				ray.origin = this.InverseTransformPoint(ray.origin);
				ray.direction = this.InverseTransformDirection(ray.direction);
				if (this.type == HitboxDefinition.Type.BOX)
				{
					AABB aabb = new AABB(Vector3.zero, this.Size);
					if (!aabb.Trace(ray, out hit, forgivness, maxDistance))
					{
						return false;
					}
				}
				else
				{
					Capsule capsule = new Capsule(Vector3.zero, this.Size.x, this.Size.y * 0.5f);
					if (!capsule.Trace(ray, out hit, forgivness, maxDistance))
					{
						return false;
					}
				}
				hit.point = this.TransformPoint(hit.point);
				hit.normal = this.TransformDirection(hit.normal);
				result = true;
			}
			return result;
		}

		// Token: 0x06004F3A RID: 20282 RVA: 0x001A5ED4 File Offset: 0x001A40D4
		public Bounds GetBounds()
		{
			Matrix4x4 matrix4x = this.Transform;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					matrix4x[i, j] = Mathf.Abs(matrix4x[i, j]);
				}
			}
			return new Bounds
			{
				center = this.Transform.MultiplyPoint(Vector3.zero),
				extents = matrix4x.MultiplyVector(this.Size)
			};
		}
	}
}
