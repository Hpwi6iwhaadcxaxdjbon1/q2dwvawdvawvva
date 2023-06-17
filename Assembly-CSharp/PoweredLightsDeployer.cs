using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B3 RID: 179
public class PoweredLightsDeployer : global::HeldEntity
{
	// Token: 0x04000A6E RID: 2670
	public GameObjectRef poweredLightsPrefab;

	// Token: 0x04000A6F RID: 2671
	public EntityRef activeLights;

	// Token: 0x04000A70 RID: 2672
	public MaterialReplacement guide;

	// Token: 0x04000A71 RID: 2673
	public GameObject guideObject;

	// Token: 0x04000A72 RID: 2674
	public float maxPlaceDistance = 5f;

	// Token: 0x04000A73 RID: 2675
	public float lengthPerAmount = 0.5f;

	// Token: 0x0600103F RID: 4159 RVA: 0x00087918 File Offset: 0x00085B18
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PoweredLightsDeployer.OnRpcMessage", 0))
		{
			if (rpc == 447739874U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AddPoint ");
				}
				using (TimeWarning.New("AddPoint", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(447739874U, "AddPoint", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.AddPoint(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in AddPoint");
					}
				}
				return true;
			}
			if (rpc == 1975273522U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Finish ");
				}
				using (TimeWarning.New("Finish", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(1975273522U, "Finish", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Finish(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Finish");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001040 RID: 4160 RVA: 0x00087C10 File Offset: 0x00085E10
	public static bool CanPlayerUse(global::BasePlayer player)
	{
		return player.CanBuild() && !GamePhysics.CheckSphere(player.eyes.position, 0.1f, 536870912, QueryTriggerInteraction.Collide);
	}

	// Token: 0x17000182 RID: 386
	// (get) Token: 0x06001041 RID: 4161 RVA: 0x00087C3C File Offset: 0x00085E3C
	// (set) Token: 0x06001042 RID: 4162 RVA: 0x00087C6B File Offset: 0x00085E6B
	public AdvancedChristmasLights active
	{
		get
		{
			global::BaseEntity baseEntity = this.activeLights.Get(base.isServer);
			if (baseEntity)
			{
				return baseEntity.GetComponent<AdvancedChristmasLights>();
			}
			return null;
		}
		set
		{
			this.activeLights.Set(value);
		}
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x00087C7C File Offset: 0x00085E7C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void AddPoint(global::BaseEntity.RPCMessage msg)
	{
		Vector3 vector = msg.read.Vector3();
		Vector3 vector2 = msg.read.Vector3();
		global::BasePlayer player = msg.player;
		if (this.GetItem() == null)
		{
			return;
		}
		if (this.GetItem().amount < 1)
		{
			return;
		}
		if (!base.IsVisible(vector, float.PositiveInfinity))
		{
			return;
		}
		if (!PoweredLightsDeployer.CanPlayerUse(player))
		{
			return;
		}
		if (Vector3.Distance(vector, player.eyes.position) > this.maxPlaceDistance)
		{
			return;
		}
		int num;
		if (this.active == null)
		{
			AdvancedChristmasLights component = GameManager.server.CreateEntity(this.poweredLightsPrefab.resourcePath, vector, Quaternion.LookRotation(vector2, player.eyes.HeadUp()), true).GetComponent<AdvancedChristmasLights>();
			component.Spawn();
			this.active = component;
			num = 1;
		}
		else
		{
			if (this.active.IsFinalized())
			{
				return;
			}
			float num2 = 0f;
			Vector3 vector3 = this.active.transform.position;
			if (this.active.points.Count > 0)
			{
				vector3 = this.active.points[this.active.points.Count - 1].point;
				num2 = Vector3.Distance(vector, vector3);
			}
			num2 = Mathf.Max(num2, this.lengthPerAmount);
			float num3 = (float)this.GetItem().amount * this.lengthPerAmount;
			if (num2 > num3)
			{
				num2 = num3;
				vector = vector3 + Vector3Ex.Direction(vector, vector3) * num2;
			}
			num2 = Mathf.Min(num3, num2);
			num = Mathf.CeilToInt(num2 / this.lengthPerAmount);
		}
		this.active.AddPoint(vector, vector2);
		base.SetFlag(global::BaseEntity.Flags.Reserved8, this.active != null, false, true);
		int iAmount = num;
		base.UseItemAmount(iAmount);
		this.active.AddLengthUsed(num);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001044 RID: 4164 RVA: 0x00087E58 File Offset: 0x00086058
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void Finish(global::BaseEntity.RPCMessage msg)
	{
		this.DoFinish();
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x00087E60 File Offset: 0x00086060
	public void DoFinish()
	{
		if (this.active)
		{
			this.active.FinishEditing();
		}
		this.active = null;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x00087E88 File Offset: 0x00086088
	public override void OnHeldChanged()
	{
		this.DoFinish();
		this.active = null;
		base.OnHeldChanged();
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x00087E9D File Offset: 0x0008609D
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.lightDeployer = Facepunch.Pool.Get<LightDeployer>();
			info.msg.lightDeployer.active = this.activeLights.uid;
		}
	}
}
