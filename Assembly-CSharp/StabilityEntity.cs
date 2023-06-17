using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000461 RID: 1121
public class StabilityEntity : global::DecayEntity
{
	// Token: 0x04001D5D RID: 7517
	public static global::StabilityEntity.StabilityCheckWorkQueue stabilityCheckQueue = new global::StabilityEntity.StabilityCheckWorkQueue();

	// Token: 0x04001D5E RID: 7518
	public static global::StabilityEntity.UpdateSurroundingsQueue updateSurroundingsQueue = new global::StabilityEntity.UpdateSurroundingsQueue();

	// Token: 0x04001D5F RID: 7519
	public bool grounded;

	// Token: 0x04001D60 RID: 7520
	[NonSerialized]
	public float cachedStability;

	// Token: 0x04001D61 RID: 7521
	[NonSerialized]
	public int cachedDistanceFromGround = int.MaxValue;

	// Token: 0x04001D62 RID: 7522
	private List<global::StabilityEntity.Support> supports;

	// Token: 0x04001D63 RID: 7523
	private int stabilityStrikes;

	// Token: 0x04001D64 RID: 7524
	private bool dirty;

	// Token: 0x060024FF RID: 9471 RVA: 0x000EA008 File Offset: 0x000E8208
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.stabilityEntity = Facepunch.Pool.Get<ProtoBuf.StabilityEntity>();
		info.msg.stabilityEntity.stability = this.cachedStability;
		info.msg.stabilityEntity.distanceFromGround = this.cachedDistanceFromGround;
	}

	// Token: 0x06002500 RID: 9472 RVA: 0x000EA058 File Offset: 0x000E8258
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.stabilityEntity != null)
		{
			this.cachedStability = info.msg.stabilityEntity.stability;
			this.cachedDistanceFromGround = info.msg.stabilityEntity.distanceFromGround;
			if (this.cachedStability <= 0f)
			{
				this.cachedStability = 0f;
			}
			if (this.cachedDistanceFromGround <= 0)
			{
				this.cachedDistanceFromGround = int.MaxValue;
			}
		}
	}

	// Token: 0x06002501 RID: 9473 RVA: 0x000EA0D1 File Offset: 0x000E82D1
	public override void ResetState()
	{
		base.ResetState();
		this.cachedStability = 0f;
		this.cachedDistanceFromGround = int.MaxValue;
		if (base.isServer)
		{
			this.supports = null;
			this.stabilityStrikes = 0;
			this.dirty = false;
		}
	}

	// Token: 0x06002502 RID: 9474 RVA: 0x000EA10C File Offset: 0x000E830C
	public void InitializeSupports()
	{
		this.supports = new List<global::StabilityEntity.Support>();
		if (this.grounded)
		{
			return;
		}
		List<EntityLink> entityLinks = base.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			if (entityLink.IsMale())
			{
				if (entityLink.socket is StabilitySocket)
				{
					this.supports.Add(new global::StabilityEntity.Support(this, entityLink, (entityLink.socket as StabilitySocket).support));
				}
				if (entityLink.socket is ConstructionSocket)
				{
					this.supports.Add(new global::StabilityEntity.Support(this, entityLink, (entityLink.socket as ConstructionSocket).support));
				}
			}
		}
	}

	// Token: 0x06002503 RID: 9475 RVA: 0x000EA1B4 File Offset: 0x000E83B4
	public int DistanceFromGround(global::StabilityEntity ignoreEntity = null)
	{
		if (this.grounded)
		{
			return 1;
		}
		if (this.supports == null)
		{
			return 1;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		int num = int.MaxValue;
		for (int i = 0; i < this.supports.Count; i++)
		{
			global::StabilityEntity stabilityEntity = this.supports[i].SupportEntity(ignoreEntity);
			if (!(stabilityEntity == null))
			{
				int num2 = stabilityEntity.CachedDistanceFromGround(ignoreEntity);
				if (num2 != 2147483647)
				{
					num = Mathf.Min(num, num2 + 1);
				}
			}
		}
		return num;
	}

	// Token: 0x06002504 RID: 9476 RVA: 0x000EA234 File Offset: 0x000E8434
	public float SupportValue(global::StabilityEntity ignoreEntity = null)
	{
		if (this.grounded)
		{
			return 1f;
		}
		if (this.supports == null)
		{
			return 1f;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		float num = 0f;
		for (int i = 0; i < this.supports.Count; i++)
		{
			global::StabilityEntity.Support support = this.supports[i];
			global::StabilityEntity stabilityEntity = support.SupportEntity(ignoreEntity);
			if (!(stabilityEntity == null))
			{
				float num2 = stabilityEntity.CachedSupportValue(ignoreEntity);
				if (num2 != 0f)
				{
					num += num2 * support.factor;
				}
			}
		}
		return Mathf.Clamp01(num);
	}

	// Token: 0x06002505 RID: 9477 RVA: 0x000EA2C8 File Offset: 0x000E84C8
	public int CachedDistanceFromGround(global::StabilityEntity ignoreEntity = null)
	{
		if (this.grounded)
		{
			return 1;
		}
		if (this.supports == null)
		{
			return 1;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		int num = int.MaxValue;
		for (int i = 0; i < this.supports.Count; i++)
		{
			global::StabilityEntity stabilityEntity = this.supports[i].SupportEntity(ignoreEntity);
			if (!(stabilityEntity == null))
			{
				int num2 = stabilityEntity.cachedDistanceFromGround;
				if (num2 != 2147483647)
				{
					num = Mathf.Min(num, num2 + 1);
				}
			}
		}
		return num;
	}

	// Token: 0x06002506 RID: 9478 RVA: 0x000EA348 File Offset: 0x000E8548
	public float CachedSupportValue(global::StabilityEntity ignoreEntity = null)
	{
		if (this.grounded)
		{
			return 1f;
		}
		if (this.supports == null)
		{
			return 1f;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		float num = 0f;
		for (int i = 0; i < this.supports.Count; i++)
		{
			global::StabilityEntity.Support support = this.supports[i];
			global::StabilityEntity stabilityEntity = support.SupportEntity(ignoreEntity);
			if (!(stabilityEntity == null))
			{
				float num2 = stabilityEntity.cachedStability;
				if (num2 != 0f)
				{
					num += num2 * support.factor;
				}
			}
		}
		return Mathf.Clamp01(num);
	}

	// Token: 0x06002507 RID: 9479 RVA: 0x000EA3DC File Offset: 0x000E85DC
	public void StabilityCheck()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (this.supports == null)
		{
			this.InitializeSupports();
		}
		bool flag = false;
		int num = this.DistanceFromGround(null);
		if (num != this.cachedDistanceFromGround)
		{
			this.cachedDistanceFromGround = num;
			flag = true;
		}
		float num2 = this.SupportValue(null);
		if (Mathf.Abs(this.cachedStability - num2) > Stability.accuracy)
		{
			this.cachedStability = num2;
			flag = true;
		}
		if (flag)
		{
			this.dirty = true;
			this.UpdateConnectedEntities();
			this.UpdateStability();
		}
		else if (this.dirty)
		{
			this.dirty = false;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		if (num2 >= Stability.collapse)
		{
			this.stabilityStrikes = 0;
			return;
		}
		if (this.stabilityStrikes < Stability.strikes)
		{
			this.UpdateStability();
			this.stabilityStrikes++;
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06002508 RID: 9480 RVA: 0x000EA4A8 File Offset: 0x000E86A8
	public void UpdateStability()
	{
		global::StabilityEntity.stabilityCheckQueue.Add(this);
	}

	// Token: 0x06002509 RID: 9481 RVA: 0x000EA4B8 File Offset: 0x000E86B8
	public void UpdateSurroundingEntities()
	{
		global::StabilityEntity.updateSurroundingsQueue.Add(this.WorldSpaceBounds().ToBounds());
	}

	// Token: 0x0600250A RID: 9482 RVA: 0x000EA4E0 File Offset: 0x000E86E0
	public void UpdateConnectedEntities()
	{
		List<EntityLink> entityLinks = base.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			if (entityLink.IsFemale())
			{
				for (int j = 0; j < entityLink.connections.Count; j++)
				{
					global::StabilityEntity stabilityEntity = entityLink.connections[j].owner as global::StabilityEntity;
					if (!(stabilityEntity == null) && !stabilityEntity.isClient && !stabilityEntity.IsDestroyed)
					{
						stabilityEntity.UpdateStability();
					}
				}
			}
		}
	}

	// Token: 0x0600250B RID: 9483 RVA: 0x000EA567 File Offset: 0x000E8767
	protected void OnPhysicsNeighbourChanged()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		this.StabilityCheck();
	}

	// Token: 0x0600250C RID: 9484 RVA: 0x000EA578 File Offset: 0x000E8778
	protected void DebugNudge()
	{
		this.StabilityCheck();
	}

	// Token: 0x0600250D RID: 9485 RVA: 0x000EA580 File Offset: 0x000E8780
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.UpdateStability();
		}
	}

	// Token: 0x0600250E RID: 9486 RVA: 0x000EA595 File Offset: 0x000E8795
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.UpdateSurroundingEntities();
	}

	// Token: 0x02000CEE RID: 3310
	public class StabilityCheckWorkQueue : ObjectWorkQueue<global::StabilityEntity>
	{
		// Token: 0x06004FD4 RID: 20436 RVA: 0x001A7205 File Offset: 0x001A5405
		protected override void RunJob(global::StabilityEntity entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.StabilityCheck();
		}

		// Token: 0x06004FD5 RID: 20437 RVA: 0x001A7217 File Offset: 0x001A5417
		protected override bool ShouldAdd(global::StabilityEntity entity)
		{
			return ConVar.Server.stability && entity.IsValid() && entity.isServer;
		}
	}

	// Token: 0x02000CEF RID: 3311
	public class UpdateSurroundingsQueue : ObjectWorkQueue<Bounds>
	{
		// Token: 0x06004FD7 RID: 20439 RVA: 0x001A7240 File Offset: 0x001A5440
		protected override void RunJob(Bounds bounds)
		{
			if (!ConVar.Server.stability)
			{
				return;
			}
			List<global::BaseEntity> list = Facepunch.Pool.GetList<global::BaseEntity>();
			global::Vis.Entities<global::BaseEntity>(bounds.center, bounds.extents.magnitude + 1f, list, 69372162, QueryTriggerInteraction.Collide);
			foreach (global::BaseEntity baseEntity in list)
			{
				if (!baseEntity.IsDestroyed && !baseEntity.isClient)
				{
					if (baseEntity is global::StabilityEntity)
					{
						(baseEntity as global::StabilityEntity).OnPhysicsNeighbourChanged();
					}
					else
					{
						baseEntity.BroadcastMessage("OnPhysicsNeighbourChanged", SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			Facepunch.Pool.FreeList<global::BaseEntity>(ref list);
		}
	}

	// Token: 0x02000CF0 RID: 3312
	private class Support
	{
		// Token: 0x040045A3 RID: 17827
		public global::StabilityEntity parent;

		// Token: 0x040045A4 RID: 17828
		public EntityLink link;

		// Token: 0x040045A5 RID: 17829
		public float factor = 1f;

		// Token: 0x06004FD9 RID: 20441 RVA: 0x001A7300 File Offset: 0x001A5500
		public Support(global::StabilityEntity parent, EntityLink link, float factor)
		{
			this.parent = parent;
			this.link = link;
			this.factor = factor;
		}

		// Token: 0x06004FDA RID: 20442 RVA: 0x001A7328 File Offset: 0x001A5528
		public global::StabilityEntity SupportEntity(global::StabilityEntity ignoreEntity = null)
		{
			global::StabilityEntity stabilityEntity = null;
			for (int i = 0; i < this.link.connections.Count; i++)
			{
				global::StabilityEntity stabilityEntity2 = this.link.connections[i].owner as global::StabilityEntity;
				Socket_Base socket = this.link.connections[i].socket;
				ConstructionSocket constructionSocket;
				if (!(stabilityEntity2 == null) && !(stabilityEntity2 == this.parent) && !(stabilityEntity2 == ignoreEntity) && !stabilityEntity2.isClient && !stabilityEntity2.IsDestroyed && ((constructionSocket = (socket as ConstructionSocket)) == null || !constructionSocket.femaleNoStability) && (stabilityEntity == null || stabilityEntity2.cachedDistanceFromGround < stabilityEntity.cachedDistanceFromGround))
				{
					stabilityEntity = stabilityEntity2;
				}
			}
			return stabilityEntity;
		}
	}
}
