using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200041F RID: 1055
public class ServerGib : BaseCombatEntity
{
	// Token: 0x04001BEF RID: 7151
	public GameObject _gibSource;

	// Token: 0x04001BF0 RID: 7152
	public string _gibName;

	// Token: 0x04001BF1 RID: 7153
	public PhysicMaterial physicsMaterial;

	// Token: 0x04001BF2 RID: 7154
	public bool useContinuousCollision;

	// Token: 0x04001BF3 RID: 7155
	private MeshCollider meshCollider;

	// Token: 0x04001BF4 RID: 7156
	private Rigidbody rigidBody;

	// Token: 0x060023C1 RID: 9153 RVA: 0x000ABEAB File Offset: 0x000AA0AB
	public override float BoundsPadding()
	{
		return 3f;
	}

	// Token: 0x060023C2 RID: 9154 RVA: 0x000E4BC0 File Offset: 0x000E2DC0
	public static List<global::ServerGib> CreateGibs(string entityToCreatePath, GameObject creator, GameObject gibSource, Vector3 inheritVelocity, float spreadVelocity)
	{
		List<global::ServerGib> list = new List<global::ServerGib>();
		foreach (MeshRenderer meshRenderer in gibSource.GetComponentsInChildren<MeshRenderer>(true))
		{
			MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
			Vector3 normalized = meshRenderer.transform.localPosition.normalized;
			Vector3 vector = creator.transform.localToWorldMatrix.MultiplyPoint(meshRenderer.transform.localPosition) + normalized * 0.5f;
			Quaternion quaternion = creator.transform.rotation * meshRenderer.transform.localRotation;
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(entityToCreatePath, vector, quaternion, true);
			if (baseEntity)
			{
				global::ServerGib component2 = baseEntity.GetComponent<global::ServerGib>();
				component2.transform.SetPositionAndRotation(vector, quaternion);
				component2._gibName = meshRenderer.name;
				MeshCollider component3 = meshRenderer.GetComponent<MeshCollider>();
				Mesh physicsMesh = (component3 != null) ? component3.sharedMesh : component.sharedMesh;
				component2.PhysicsInit(physicsMesh);
				Vector3 b = meshRenderer.transform.localPosition.normalized * spreadVelocity;
				component2.rigidBody.velocity = inheritVelocity + b;
				component2.rigidBody.angularVelocity = Vector3Ex.Range(-1f, 1f).normalized * 1f;
				component2.rigidBody.WakeUp();
				component2.Spawn();
				list.Add(component2);
			}
		}
		foreach (global::ServerGib serverGib in list)
		{
			foreach (global::ServerGib serverGib2 in list)
			{
				if (!(serverGib == serverGib2))
				{
					Physics.IgnoreCollision(serverGib2.GetCollider(), serverGib.GetCollider(), true);
				}
			}
		}
		return list;
	}

	// Token: 0x060023C3 RID: 9155 RVA: 0x000E4DE0 File Offset: 0x000E2FE0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk && this._gibName != "")
		{
			info.msg.servergib = Pool.Get<ProtoBuf.ServerGib>();
			info.msg.servergib.gibName = this._gibName;
		}
	}

	// Token: 0x060023C4 RID: 9156 RVA: 0x000E4E34 File Offset: 0x000E3034
	public MeshCollider GetCollider()
	{
		return this.meshCollider;
	}

	// Token: 0x060023C5 RID: 9157 RVA: 0x000E4E3C File Offset: 0x000E303C
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.RemoveMe), 1800f);
	}

	// Token: 0x060023C6 RID: 9158 RVA: 0x00003384 File Offset: 0x00001584
	public void RemoveMe()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060023C7 RID: 9159 RVA: 0x000E4E5C File Offset: 0x000E305C
	public virtual void PhysicsInit(Mesh physicsMesh)
	{
		Mesh sharedMesh = null;
		MeshFilter component = base.gameObject.GetComponent<MeshFilter>();
		if (component != null)
		{
			sharedMesh = component.sharedMesh;
			component.sharedMesh = physicsMesh;
		}
		this.meshCollider = base.gameObject.AddComponent<MeshCollider>();
		this.meshCollider.sharedMesh = physicsMesh;
		this.meshCollider.convex = true;
		this.meshCollider.material = this.physicsMaterial;
		if (component != null)
		{
			component.sharedMesh = sharedMesh;
		}
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.useGravity = true;
		rigidbody.mass = Mathf.Clamp(this.meshCollider.bounds.size.magnitude * this.meshCollider.bounds.size.magnitude * 20f, 10f, 2000f);
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		rigidbody.collisionDetectionMode = (this.useContinuousCollision ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete);
		if (base.isServer)
		{
			rigidbody.drag = 0.1f;
			rigidbody.angularDrag = 0.1f;
		}
		this.rigidBody = rigidbody;
		base.gameObject.layer = LayerMask.NameToLayer("Default");
		if (base.isClient)
		{
			rigidbody.isKinematic = true;
		}
	}
}
