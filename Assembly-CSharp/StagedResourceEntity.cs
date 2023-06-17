using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000D7 RID: 215
public class StagedResourceEntity : ResourceEntity
{
	// Token: 0x04000BF6 RID: 3062
	public List<StagedResourceEntity.ResourceStage> stages = new List<StagedResourceEntity.ResourceStage>();

	// Token: 0x04000BF7 RID: 3063
	public int stage;

	// Token: 0x04000BF8 RID: 3064
	public GameObjectRef changeStageEffect;

	// Token: 0x04000BF9 RID: 3065
	public GameObject gibSourceTest;

	// Token: 0x06001311 RID: 4881 RVA: 0x00099854 File Offset: 0x00097A54
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StagedResourceEntity.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x00099894 File Offset: 0x00097A94
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource == null)
		{
			return;
		}
		int num = info.msg.resource.stage;
		if (info.fromDisk && base.isServer)
		{
			this.health = this.startHealth;
			num = 0;
		}
		if (num != this.stage)
		{
			this.stage = num;
			this.UpdateStage();
		}
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x00099900 File Offset: 0x00097B00
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.resource == null)
		{
			info.msg.resource = Pool.Get<BaseResource>();
		}
		info.msg.resource.health = this.Health();
		info.msg.resource.stage = this.stage;
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x0009995D File Offset: 0x00097B5D
	protected override void OnHealthChanged()
	{
		base.Invoke(new Action(this.UpdateNetworkStage), 0.1f);
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x00099977 File Offset: 0x00097B77
	protected virtual void UpdateNetworkStage()
	{
		if (this.FindBestStage() != this.stage)
		{
			this.stage = this.FindBestStage();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.UpdateStage();
		}
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x000999A0 File Offset: 0x00097BA0
	private int FindBestStage()
	{
		float num = Mathf.InverseLerp(0f, this.MaxHealth(), this.Health());
		for (int i = 0; i < this.stages.Count; i++)
		{
			if (num >= this.stages[i].health)
			{
				return i;
			}
		}
		return this.stages.Count - 1;
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x000999FD File Offset: 0x00097BFD
	public T GetStageComponent<T>() where T : Component
	{
		return this.stages[this.stage].instance.GetComponentInChildren<T>();
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x00099A1C File Offset: 0x00097C1C
	private void UpdateStage()
	{
		if (this.stages.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.stages.Count; i++)
		{
			this.stages[i].instance.SetActive(i == this.stage);
		}
		GroundWatch.PhysicsChanged(base.gameObject);
	}

	// Token: 0x02000C08 RID: 3080
	[Serializable]
	public class ResourceStage
	{
		// Token: 0x040041B3 RID: 16819
		public float health;

		// Token: 0x040041B4 RID: 16820
		public GameObject instance;
	}
}
