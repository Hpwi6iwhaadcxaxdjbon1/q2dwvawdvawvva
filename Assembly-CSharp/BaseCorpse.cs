using System;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020003A7 RID: 935
public class BaseCorpse : BaseCombatEntity
{
	// Token: 0x040019CD RID: 6605
	public GameObjectRef prefabRagdoll;

	// Token: 0x040019CE RID: 6606
	public global::BaseEntity parentEnt;

	// Token: 0x040019CF RID: 6607
	[NonSerialized]
	internal ResourceDispenser resourceDispenser;

	// Token: 0x040019D0 RID: 6608
	[NonSerialized]
	public SpawnGroup spawnGroup;

	// Token: 0x060020DA RID: 8410 RVA: 0x000D8470 File Offset: 0x000D6670
	public override void ResetState()
	{
		this.spawnGroup = null;
		base.ResetState();
	}

	// Token: 0x060020DB RID: 8411 RVA: 0x000D847F File Offset: 0x000D667F
	public override void ServerInit()
	{
		this.SetupRigidBody();
		this.ResetRemovalTime();
		this.resourceDispenser = base.GetComponent<ResourceDispenser>();
		base.ServerInit();
	}

	// Token: 0x060020DC RID: 8412 RVA: 0x000D84A0 File Offset: 0x000D66A0
	public virtual void InitCorpse(global::BaseEntity pr)
	{
		this.parentEnt = pr;
		base.transform.SetPositionAndRotation(this.parentEnt.CenterPoint(), this.parentEnt.transform.rotation);
		SpawnPointInstance component = base.GetComponent<SpawnPointInstance>();
		if (component != null)
		{
			this.spawnGroup = (component.parentSpawnPointUser as SpawnGroup);
		}
	}

	// Token: 0x060020DD RID: 8413 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool CanRemove()
	{
		return true;
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x000D84FB File Offset: 0x000D66FB
	public void RemoveCorpse()
	{
		if (!this.CanRemove())
		{
			this.ResetRemovalTime();
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060020DF RID: 8415 RVA: 0x000D8514 File Offset: 0x000D6714
	public void ResetRemovalTime(float dur)
	{
		using (TimeWarning.New("ResetRemovalTime", 0))
		{
			if (base.IsInvoking(new Action(this.RemoveCorpse)))
			{
				base.CancelInvoke(new Action(this.RemoveCorpse));
			}
			base.Invoke(new Action(this.RemoveCorpse), dur);
		}
	}

	// Token: 0x060020E0 RID: 8416 RVA: 0x000D8584 File Offset: 0x000D6784
	public virtual float GetRemovalTime()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode != null)
		{
			return activeGameMode.CorpseRemovalTime(this);
		}
		return Server.corpsedespawn;
	}

	// Token: 0x060020E1 RID: 8417 RVA: 0x000D85AE File Offset: 0x000D67AE
	public void ResetRemovalTime()
	{
		this.ResetRemovalTime(this.GetRemovalTime());
	}

	// Token: 0x060020E2 RID: 8418 RVA: 0x000D85BC File Offset: 0x000D67BC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.corpse = Facepunch.Pool.Get<Corpse>();
		if (this.parentEnt.IsValid())
		{
			info.msg.corpse.parentID = this.parentEnt.net.ID;
		}
	}

	// Token: 0x060020E3 RID: 8419 RVA: 0x000D8610 File Offset: 0x000D6810
	public void TakeChildren(global::BaseEntity takeChildrenFrom)
	{
		if (takeChildrenFrom.children == null)
		{
			return;
		}
		using (TimeWarning.New("Corpse.TakeChildren", 0))
		{
			global::BaseEntity[] array = takeChildrenFrom.children.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SwitchParent(this);
			}
		}
	}

	// Token: 0x060020E4 RID: 8420 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
	}

	// Token: 0x060020E5 RID: 8421 RVA: 0x000D8674 File Offset: 0x000D6874
	private Rigidbody SetupRigidBody()
	{
		if (base.isServer)
		{
			GameObject gameObject = base.gameManager.FindPrefab(this.prefabRagdoll.resourcePath);
			if (gameObject == null)
			{
				return null;
			}
			Ragdoll component = gameObject.GetComponent<Ragdoll>();
			if (component == null)
			{
				return null;
			}
			if (component.primaryBody == null)
			{
				Debug.LogError("[BaseCorpse] ragdoll.primaryBody isn't set!" + component.gameObject.name);
				return null;
			}
			BoxCollider component2 = component.primaryBody.GetComponent<BoxCollider>();
			if (component2 == null)
			{
				Debug.LogError("Ragdoll has unsupported primary collider (make it supported) ", component);
				return null;
			}
			BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
			boxCollider.size = component2.size * 2f;
			boxCollider.center = component2.center;
			boxCollider.sharedMaterial = component2.sharedMaterial;
		}
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		if (rigidbody == null)
		{
			Debug.LogError("[BaseCorpse] already has a RigidBody defined - and it shouldn't!" + base.gameObject.name);
			return null;
		}
		rigidbody.mass = 10f;
		rigidbody.useGravity = true;
		rigidbody.drag = 0.5f;
		rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
		if (base.isServer)
		{
			Buoyancy component3 = base.GetComponent<Buoyancy>();
			if (component3 != null)
			{
				component3.rigidBody = rigidbody;
			}
			ConVar.Physics.ApplyDropped(rigidbody);
			Vector3 velocity = Vector3Ex.Range(-1f, 1f);
			velocity.y += 1f;
			rigidbody.velocity = velocity;
			rigidbody.angularVelocity = Vector3Ex.Range(-10f, 10f);
		}
		return rigidbody;
	}

	// Token: 0x060020E6 RID: 8422 RVA: 0x000D8800 File Offset: 0x000D6A00
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.corpse != null)
		{
			this.Load(info.msg.corpse);
		}
	}

	// Token: 0x060020E7 RID: 8423 RVA: 0x000D8827 File Offset: 0x000D6A27
	private void Load(Corpse corpse)
	{
		if (base.isServer)
		{
			this.parentEnt = (global::BaseNetworkable.serverEntities.Find(corpse.parentID) as global::BaseEntity);
		}
		bool isClient = base.isClient;
	}

	// Token: 0x060020E8 RID: 8424 RVA: 0x000D8853 File Offset: 0x000D6A53
	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer)
		{
			this.ResetRemovalTime();
			if (this.resourceDispenser)
			{
				this.resourceDispenser.OnAttacked(info);
			}
			if (!info.DidGather)
			{
				base.OnAttacked(info);
			}
		}
	}

	// Token: 0x060020E9 RID: 8425 RVA: 0x000D888B File Offset: 0x000D6A8B
	public override string Categorize()
	{
		return "corpse";
	}

	// Token: 0x170002BD RID: 701
	// (get) Token: 0x060020EA RID: 8426 RVA: 0x000D8892 File Offset: 0x000D6A92
	public override global::BaseEntity.TraitFlag Traits
	{
		get
		{
			return base.Traits | global::BaseEntity.TraitFlag.Food | global::BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x060020EB RID: 8427 RVA: 0x000D88A0 File Offset: 0x000D6AA0
	public override void Eat(BaseNpc baseNpc, float timeSpent)
	{
		this.ResetRemovalTime();
		base.Hurt(timeSpent * 5f);
		baseNpc.AddCalories(timeSpent * 2f);
	}

	// Token: 0x060020EC RID: 8428 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}
}
