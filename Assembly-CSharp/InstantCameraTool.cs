using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000086 RID: 134
public class InstantCameraTool : HeldEntity
{
	// Token: 0x0400082C RID: 2092
	public ItemDefinition photoItem;

	// Token: 0x0400082D RID: 2093
	public GameObjectRef screenshotEffect;

	// Token: 0x0400082E RID: 2094
	public SoundDefinition startPhotoSoundDef;

	// Token: 0x0400082F RID: 2095
	public SoundDefinition finishPhotoSoundDef;

	// Token: 0x04000830 RID: 2096
	[Range(640f, 1920f)]
	public int resolutionX = 640;

	// Token: 0x04000831 RID: 2097
	[Range(480f, 1080f)]
	public int resolutionY = 480;

	// Token: 0x04000832 RID: 2098
	[Range(10f, 100f)]
	public int quality = 75;

	// Token: 0x04000833 RID: 2099
	[Range(0f, 5f)]
	public float cooldownSeconds = 3f;

	// Token: 0x04000834 RID: 2100
	private TimeSince _sinceLastPhoto;

	// Token: 0x04000835 RID: 2101
	private bool hasSentAchievement;

	// Token: 0x04000836 RID: 2102
	public const string PhotographPlayerAchievement = "SUMMER_PAPARAZZI";

	// Token: 0x06000CAE RID: 3246 RVA: 0x0006DB2C File Offset: 0x0006BD2C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("InstantCameraTool.OnRpcMessage", 0))
		{
			if (rpc == 3122234259U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - TakePhoto ");
				}
				using (TimeWarning.New("TakePhoto", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(3122234259U, "TakePhoto", this, player, 3UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.FromOwner.Test(3122234259U, "TakePhoto", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.TakePhoto(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in TakePhoto");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x0006DCE8 File Offset: 0x0006BEE8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	[BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	private void TakePhoto(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		Item item = this.GetItem();
		if (player == null || item == null || item.condition <= 0f)
		{
			return;
		}
		byte[] array = msg.read.BytesWithSize(10485760U);
		if (array.Length > 102400 || !ImageProcessing.IsValidJPG(array, this.resolutionX, this.resolutionY))
		{
			return;
		}
		Item item2 = ItemManager.Create(this.photoItem, 1, 0UL);
		if (item2 == null)
		{
			Debug.LogError("Failed to create photo item");
			return;
		}
		if (!item2.instanceData.subEntity.IsValid)
		{
			item2.Remove(0f);
			Debug.LogError("Photo has no sub-entity");
			return;
		}
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(item2.instanceData.subEntity);
		if (baseNetworkable == null)
		{
			item2.Remove(0f);
			Debug.LogError("Sub-entity was not found");
			return;
		}
		PhotoEntity photoEntity;
		if ((photoEntity = (baseNetworkable as PhotoEntity)) == null)
		{
			item2.Remove(0f);
			Debug.LogError("Sub-entity is not a photo");
			return;
		}
		photoEntity.SetImageData(player.userID, array);
		if (!player.inventory.GiveItem(item2, null, false))
		{
			item2.Drop(player.GetDropPosition(), player.GetDropVelocity(), default(Quaternion));
		}
		EffectNetwork.Send(new Effect(this.screenshotEffect.resourcePath, base.transform.position, base.transform.forward, msg.connection));
		if (!this.hasSentAchievement && !string.IsNullOrEmpty("SUMMER_PAPARAZZI"))
		{
			Vector3 position = base.GetOwnerPlayer().eyes.position;
			Vector3 a = base.GetOwnerPlayer().eyes.HeadForward();
			List<BasePlayer> list = Facepunch.Pool.GetList<BasePlayer>();
			global::Vis.Entities<BasePlayer>(position + a * 5f, 5f, list, 131072, QueryTriggerInteraction.Collide);
			foreach (BasePlayer basePlayer in list)
			{
				if (basePlayer.isServer && basePlayer != base.GetOwnerPlayer() && basePlayer.IsVisible(base.GetOwnerPlayer().eyes.position, float.PositiveInfinity))
				{
					this.hasSentAchievement = true;
					base.GetOwnerPlayer().GiveAchievement("SUMMER_PAPARAZZI");
					break;
				}
			}
			Facepunch.Pool.FreeList<BasePlayer>(ref list);
		}
		item.LoseCondition(1f);
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x0006DF5C File Offset: 0x0006C15C
	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		this.hasSentAchievement = false;
	}
}
