using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;

namespace Rust.AI
{
	// Token: 0x02000B4A RID: 2890
	public class SimpleAIMemory
	{
		// Token: 0x04003E7B RID: 15995
		public static HashSet<BasePlayer> PlayerIgnoreList = new HashSet<BasePlayer>();

		// Token: 0x04003E7C RID: 15996
		public List<SimpleAIMemory.SeenInfo> All = new List<SimpleAIMemory.SeenInfo>();

		// Token: 0x04003E7D RID: 15997
		public List<BaseEntity> Players = new List<BaseEntity>();

		// Token: 0x04003E7E RID: 15998
		public HashSet<BaseEntity> LOS = new HashSet<BaseEntity>();

		// Token: 0x04003E7F RID: 15999
		public List<BaseEntity> Targets = new List<BaseEntity>();

		// Token: 0x04003E80 RID: 16000
		public List<BaseEntity> Threats = new List<BaseEntity>();

		// Token: 0x04003E81 RID: 16001
		public List<BaseEntity> Friendlies = new List<BaseEntity>();

		// Token: 0x0600460A RID: 17930 RVA: 0x00198CE0 File Offset: 0x00196EE0
		public void SetKnown(BaseEntity ent, BaseEntity owner, AIBrainSenses brainSenses)
		{
			IAISenses iaisenses = owner as IAISenses;
			BasePlayer basePlayer = ent as BasePlayer;
			if (basePlayer != null && SimpleAIMemory.PlayerIgnoreList.Contains(basePlayer))
			{
				return;
			}
			bool flag = false;
			if (iaisenses != null && iaisenses.IsThreat(ent))
			{
				flag = true;
				if (brainSenses != null)
				{
					brainSenses.LastThreatTimestamp = UnityEngine.Time.realtimeSinceStartup;
				}
			}
			for (int i = 0; i < this.All.Count; i++)
			{
				if (this.All[i].Entity == ent)
				{
					SimpleAIMemory.SeenInfo seenInfo = this.All[i];
					seenInfo.Position = ent.transform.position;
					seenInfo.Timestamp = Mathf.Max(UnityEngine.Time.realtimeSinceStartup, seenInfo.Timestamp);
					this.All[i] = seenInfo;
					return;
				}
			}
			if (basePlayer != null)
			{
				if (AI.ignoreplayers && !basePlayer.IsNpc)
				{
					return;
				}
				this.Players.Add(ent);
			}
			if (iaisenses != null)
			{
				if (iaisenses.IsTarget(ent))
				{
					this.Targets.Add(ent);
				}
				if (iaisenses.IsFriendly(ent))
				{
					this.Friendlies.Add(ent);
				}
				if (flag)
				{
					this.Threats.Add(ent);
				}
			}
			this.All.Add(new SimpleAIMemory.SeenInfo
			{
				Entity = ent,
				Position = ent.transform.position,
				Timestamp = UnityEngine.Time.realtimeSinceStartup
			});
		}

		// Token: 0x0600460B RID: 17931 RVA: 0x00198E42 File Offset: 0x00197042
		public void SetLOS(BaseEntity ent, bool flag)
		{
			if (ent == null)
			{
				return;
			}
			if (flag)
			{
				this.LOS.Add(ent);
				return;
			}
			this.LOS.Remove(ent);
		}

		// Token: 0x0600460C RID: 17932 RVA: 0x00198E6C File Offset: 0x0019706C
		public bool IsLOS(BaseEntity ent)
		{
			return this.LOS.Contains(ent);
		}

		// Token: 0x0600460D RID: 17933 RVA: 0x00198E7A File Offset: 0x0019707A
		public bool IsPlayerKnown(BasePlayer player)
		{
			return this.Players.Contains(player);
		}

		// Token: 0x0600460E RID: 17934 RVA: 0x00198E88 File Offset: 0x00197088
		public void Forget(float secondsOld)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (UnityEngine.Time.realtimeSinceStartup - this.All[i].Timestamp >= secondsOld)
				{
					BaseEntity entity = this.All[i].Entity;
					if (entity != null)
					{
						if (entity is BasePlayer)
						{
							this.Players.Remove(entity);
						}
						this.Targets.Remove(entity);
						this.Threats.Remove(entity);
						this.Friendlies.Remove(entity);
						this.LOS.Remove(entity);
					}
					this.All.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x0600460F RID: 17935 RVA: 0x00198F3F File Offset: 0x0019713F
		public static void AddIgnorePlayer(BasePlayer player)
		{
			if (SimpleAIMemory.PlayerIgnoreList.Contains(player))
			{
				return;
			}
			SimpleAIMemory.PlayerIgnoreList.Add(player);
		}

		// Token: 0x06004610 RID: 17936 RVA: 0x00198F5B File Offset: 0x0019715B
		public static void RemoveIgnorePlayer(BasePlayer player)
		{
			SimpleAIMemory.PlayerIgnoreList.Remove(player);
		}

		// Token: 0x06004611 RID: 17937 RVA: 0x00198F69 File Offset: 0x00197169
		public static void ClearIgnoredPlayers()
		{
			SimpleAIMemory.PlayerIgnoreList.Clear();
		}

		// Token: 0x06004612 RID: 17938 RVA: 0x00198F78 File Offset: 0x00197178
		public static string GetIgnoredPlayers()
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[]
			{
				"Name",
				"Steam ID"
			});
			foreach (BasePlayer basePlayer in SimpleAIMemory.PlayerIgnoreList)
			{
				textTable.AddRow(new string[]
				{
					basePlayer.displayName,
					basePlayer.userID.ToString()
				});
			}
			return textTable.ToString();
		}

		// Token: 0x02000FA4 RID: 4004
		public struct SeenInfo
		{
			// Token: 0x0400508E RID: 20622
			public BaseEntity Entity;

			// Token: 0x0400508F RID: 20623
			public Vector3 Position;

			// Token: 0x04005090 RID: 20624
			public float Timestamp;

			// Token: 0x04005091 RID: 20625
			public float Danger;
		}
	}
}
