using System;
using System.Runtime.CompilerServices;
using ConVar;
using ProtoBuf;
using UnityEngine;

// Token: 0x020001EF RID: 495
public class GingerbreadNPC : HumanNPC, IClientBrainStateListener
{
	// Token: 0x04001298 RID: 4760
	public GameObjectRef OverrideCorpseMale;

	// Token: 0x04001299 RID: 4761
	public GameObjectRef OverrideCorpseFemale;

	// Token: 0x0400129A RID: 4762
	public PhysicMaterial HitMaterial;

	// Token: 0x0400129B RID: 4763
	public bool RoamAroundHomePoint;

	// Token: 0x060019F8 RID: 6648 RVA: 0x000BCC0F File Offset: 0x000BAE0F
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		info.HitMaterial = Global.GingerbreadMaterialID();
	}

	// Token: 0x060019F9 RID: 6649 RVA: 0x000BCC23 File Offset: 0x000BAE23
	public override string Categorize()
	{
		return "Gingerbread";
	}

	// Token: 0x060019FA RID: 6650 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	// Token: 0x060019FB RID: 6651 RVA: 0x000BCC2C File Offset: 0x000BAE2C
	public override BaseCorpse CreateCorpse()
	{
		BaseCorpse result;
		using (TimeWarning.New("Create corpse", 0))
		{
			string corpseResourcePath = this.CorpseResourcePath;
			NPCPlayerCorpse npcplayerCorpse = base.DropCorpse(corpseResourcePath) as NPCPlayerCorpse;
			if (npcplayerCorpse)
			{
				npcplayerCorpse.transform.position = npcplayerCorpse.transform.position + Vector3.down * this.NavAgent.baseOffset;
				npcplayerCorpse.SetLootableIn(2f);
				npcplayerCorpse.SetFlag(global::BaseEntity.Flags.Reserved5, base.HasPlayerFlag(global::BasePlayer.PlayerFlags.DisplaySash), false, true);
				npcplayerCorpse.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
				npcplayerCorpse.TakeFrom(new global::ItemContainer[]
				{
					this.inventory.containerMain
				});
				npcplayerCorpse.playerName = "Gingerbread";
				npcplayerCorpse.playerSteamID = this.userID;
				npcplayerCorpse.Spawn();
				global::ItemContainer[] containers = npcplayerCorpse.containers;
				for (int i = 0; i < containers.Length; i++)
				{
					containers[i].Clear();
				}
				if (this.LootSpawnSlots.Length != 0)
				{
					foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
					{
						for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
						{
							if (UnityEngine.Random.Range(0f, 1f) <= lootSpawnSlot.probability)
							{
								lootSpawnSlot.definition.SpawnIntoContainer(npcplayerCorpse.containers[0]);
							}
						}
					}
				}
			}
			result = npcplayerCorpse;
		}
		return result;
	}

	// Token: 0x060019FC RID: 6652 RVA: 0x000BCDBC File Offset: 0x000BAFBC
	public override void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		base.AttackerInfo(info);
		info.inflictorName = this.inventory.containerBelt.GetSlot(0).info.shortname;
		info.attackerName = base.ShortPrefabName;
	}

	// Token: 0x17000231 RID: 561
	// (get) Token: 0x060019FD RID: 6653 RVA: 0x000BCDF4 File Offset: 0x000BAFF4
	protected string CorpseResourcePath
	{
		get
		{
			bool flag = GingerbreadNPC.<get_CorpseResourcePath>g__GetFloatBasedOnUserID|10_0(this.userID, 4332UL) > 0.5f;
			if (this.OverrideCorpseMale.isValid && !flag)
			{
				return this.OverrideCorpseMale.resourcePath;
			}
			if (this.OverrideCorpseFemale.isValid && flag)
			{
				return this.OverrideCorpseFemale.resourcePath;
			}
			return "assets/prefabs/npc/murderer/murderer_corpse.prefab";
		}
	}

	// Token: 0x060019FE RID: 6654 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnClientStateChanged(AIState state)
	{
	}

	// Token: 0x06001A00 RID: 6656 RVA: 0x000BCE64 File Offset: 0x000BB064
	[CompilerGenerated]
	internal static float <get_CorpseResourcePath>g__GetFloatBasedOnUserID|10_0(ulong steamid, ulong seed)
	{
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState((int)(seed + steamid));
		float result = UnityEngine.Random.Range(0f, 1f);
		UnityEngine.Random.state = state;
		return result;
	}
}
