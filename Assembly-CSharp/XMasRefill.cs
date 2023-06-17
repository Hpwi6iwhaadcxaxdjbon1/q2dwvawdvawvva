using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public class XMasRefill : BaseEntity
{
	// Token: 0x04000D7C RID: 3452
	public GameObjectRef[] giftPrefabs;

	// Token: 0x04000D7D RID: 3453
	public List<BasePlayer> goodKids;

	// Token: 0x04000D7E RID: 3454
	public List<Stocking> stockings;

	// Token: 0x04000D7F RID: 3455
	public AudioSource bells;

	// Token: 0x0600153F RID: 5439 RVA: 0x000A82F8 File Offset: 0x000A64F8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("XMasRefill.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001540 RID: 5440 RVA: 0x000A8338 File Offset: 0x000A6538
	public float GiftRadius()
	{
		return XMas.spawnRange;
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x000A833F File Offset: 0x000A653F
	public int GiftsPerPlayer()
	{
		return XMas.giftsPerPlayer;
	}

	// Token: 0x06001542 RID: 5442 RVA: 0x000A8346 File Offset: 0x000A6546
	public int GiftSpawnAttempts()
	{
		return XMas.giftsPerPlayer * XMas.spawnAttempts;
	}

	// Token: 0x06001543 RID: 5443 RVA: 0x000A8354 File Offset: 0x000A6554
	public override void ServerInit()
	{
		base.ServerInit();
		if (!XMas.enabled)
		{
			base.Invoke(new Action(this.RemoveMe), 0.1f);
			return;
		}
		this.goodKids = ((BasePlayer.activePlayerList != null) ? new List<BasePlayer>(BasePlayer.activePlayerList) : new List<BasePlayer>());
		this.stockings = ((Stocking.stockings != null) ? new List<Stocking>(Stocking.stockings.Values) : new List<Stocking>());
		base.Invoke(new Action(this.RemoveMe), 60f);
		base.InvokeRepeating(new Action(this.DistributeLoot), 3f, 0.02f);
		base.Invoke(new Action(this.SendBells), 0.5f);
	}

	// Token: 0x06001544 RID: 5444 RVA: 0x000A8411 File Offset: 0x000A6611
	public void SendBells()
	{
		base.ClientRPC(null, "PlayBells");
	}

	// Token: 0x06001545 RID: 5445 RVA: 0x000A841F File Offset: 0x000A661F
	public void RemoveMe()
	{
		if (this.goodKids.Count == 0 && this.stockings.Count == 0)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
			return;
		}
		base.Invoke(new Action(this.RemoveMe), 60f);
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x000A845C File Offset: 0x000A665C
	public void DistributeLoot()
	{
		if (this.goodKids.Count > 0)
		{
			BasePlayer basePlayer = null;
			foreach (BasePlayer basePlayer2 in this.goodKids)
			{
				if (!basePlayer2.IsSleeping() && !basePlayer2.IsWounded() && basePlayer2.IsAlive())
				{
					basePlayer = basePlayer2;
					break;
				}
			}
			if (basePlayer)
			{
				this.DistributeGiftsForPlayer(basePlayer);
				this.goodKids.Remove(basePlayer);
			}
		}
		if (this.stockings.Count > 0)
		{
			Stocking stocking = this.stockings[0];
			if (stocking != null)
			{
				stocking.SpawnLoot();
			}
			this.stockings.RemoveAt(0);
		}
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x000A8528 File Offset: 0x000A6728
	protected bool DropToGround(ref Vector3 pos)
	{
		int intVal = 1235288065;
		int num = 8454144;
		if (TerrainMeta.TopologyMap && (TerrainMeta.TopologyMap.GetTopology(pos) & 82048) != 0)
		{
			return false;
		}
		if (TerrainMeta.HeightMap && TerrainMeta.Collision && !TerrainMeta.Collision.GetIgnore(pos, 0.01f))
		{
			float height = TerrainMeta.HeightMap.GetHeight(pos);
			pos.y = Mathf.Max(pos.y, height);
		}
		RaycastHit raycastHit;
		if (!TransformUtil.GetGroundInfo(pos, out raycastHit, 80f, intVal, null))
		{
			return false;
		}
		if ((1 << raycastHit.transform.gameObject.layer & num) == 0)
		{
			return false;
		}
		pos = raycastHit.point;
		return true;
	}

	// Token: 0x06001548 RID: 5448 RVA: 0x000A8600 File Offset: 0x000A6800
	public bool DistributeGiftsForPlayer(BasePlayer player)
	{
		int num = this.GiftsPerPlayer();
		int num2 = this.GiftSpawnAttempts();
		int num3 = 0;
		while (num3 < num2 && num > 0)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * this.GiftRadius();
			Vector3 pos = player.transform.position + new Vector3(vector.x, 10f, vector.y);
			Quaternion rot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
			if (this.DropToGround(ref pos))
			{
				string resourcePath = this.giftPrefabs[UnityEngine.Random.Range(0, this.giftPrefabs.Length)].resourcePath;
				BaseEntity baseEntity = GameManager.server.CreateEntity(resourcePath, pos, rot, true);
				if (baseEntity)
				{
					baseEntity.Spawn();
					num--;
				}
			}
			num3++;
		}
		return true;
	}
}
