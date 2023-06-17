using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B3D RID: 2877
	public class Memory
	{
		// Token: 0x04003E38 RID: 15928
		public List<BaseEntity> Visible = new List<BaseEntity>();

		// Token: 0x04003E39 RID: 15929
		public List<Memory.SeenInfo> All = new List<Memory.SeenInfo>();

		// Token: 0x04003E3A RID: 15930
		public List<Memory.ExtendedInfo> AllExtended = new List<Memory.ExtendedInfo>();

		// Token: 0x060045CD RID: 17869 RVA: 0x0019767C File Offset: 0x0019587C
		public Memory.SeenInfo Update(BaseEntity entity, float score, Vector3 direction, float dot, float distanceSqr, byte lineOfSight, bool updateLastHurtUsTime, float lastHurtUsTime, out Memory.ExtendedInfo extendedInfo)
		{
			return this.Update(entity, entity.ServerPosition, score, direction, dot, distanceSqr, lineOfSight, updateLastHurtUsTime, lastHurtUsTime, out extendedInfo);
		}

		// Token: 0x060045CE RID: 17870 RVA: 0x001976A4 File Offset: 0x001958A4
		public Memory.SeenInfo Update(BaseEntity entity, Vector3 position, float score, Vector3 direction, float dot, float distanceSqr, byte lineOfSight, bool updateLastHurtUsTime, float lastHurtUsTime, out Memory.ExtendedInfo extendedInfo)
		{
			extendedInfo = default(Memory.ExtendedInfo);
			bool flag = false;
			for (int i = 0; i < this.AllExtended.Count; i++)
			{
				if (this.AllExtended[i].Entity == entity)
				{
					Memory.ExtendedInfo extendedInfo2 = this.AllExtended[i];
					extendedInfo2.Direction = direction;
					extendedInfo2.Dot = dot;
					extendedInfo2.DistanceSqr = distanceSqr;
					extendedInfo2.LineOfSight = lineOfSight;
					if (updateLastHurtUsTime)
					{
						extendedInfo2.LastHurtUsTime = lastHurtUsTime;
					}
					this.AllExtended[i] = extendedInfo2;
					extendedInfo = extendedInfo2;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (updateLastHurtUsTime)
				{
					Memory.ExtendedInfo extendedInfo3 = new Memory.ExtendedInfo
					{
						Entity = entity,
						Direction = direction,
						Dot = dot,
						DistanceSqr = distanceSqr,
						LineOfSight = lineOfSight,
						LastHurtUsTime = lastHurtUsTime
					};
					this.AllExtended.Add(extendedInfo3);
					extendedInfo = extendedInfo3;
				}
				else
				{
					Memory.ExtendedInfo extendedInfo4 = new Memory.ExtendedInfo
					{
						Entity = entity,
						Direction = direction,
						Dot = dot,
						DistanceSqr = distanceSqr,
						LineOfSight = lineOfSight
					};
					this.AllExtended.Add(extendedInfo4);
					extendedInfo = extendedInfo4;
				}
			}
			return this.Update(entity, position, score);
		}

		// Token: 0x060045CF RID: 17871 RVA: 0x001977FB File Offset: 0x001959FB
		public Memory.SeenInfo Update(BaseEntity ent, float danger = 0f)
		{
			return this.Update(ent, ent.ServerPosition, danger);
		}

		// Token: 0x060045D0 RID: 17872 RVA: 0x0019780C File Offset: 0x00195A0C
		public Memory.SeenInfo Update(BaseEntity ent, Vector3 position, float danger = 0f)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (this.All[i].Entity == ent)
				{
					Memory.SeenInfo seenInfo = this.All[i];
					seenInfo.Position = position;
					seenInfo.Timestamp = Time.realtimeSinceStartup;
					seenInfo.Danger += danger;
					this.All[i] = seenInfo;
					return seenInfo;
				}
			}
			Memory.SeenInfo seenInfo2 = new Memory.SeenInfo
			{
				Entity = ent,
				Position = position,
				Timestamp = Time.realtimeSinceStartup,
				Danger = danger
			};
			this.All.Add(seenInfo2);
			this.Visible.Add(ent);
			return seenInfo2;
		}

		// Token: 0x060045D1 RID: 17873 RVA: 0x001978CC File Offset: 0x00195ACC
		public void AddDanger(Vector3 position, float amount)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (Mathf.Approximately(this.All[i].Position.x, position.x) && Mathf.Approximately(this.All[i].Position.y, position.y) && Mathf.Approximately(this.All[i].Position.z, position.z))
				{
					Memory.SeenInfo value = this.All[i];
					value.Danger = amount;
					this.All[i] = value;
					return;
				}
			}
			this.All.Add(new Memory.SeenInfo
			{
				Position = position,
				Timestamp = Time.realtimeSinceStartup,
				Danger = amount
			});
		}

		// Token: 0x060045D2 RID: 17874 RVA: 0x001979B4 File Offset: 0x00195BB4
		public Memory.SeenInfo GetInfo(BaseEntity entity)
		{
			foreach (Memory.SeenInfo seenInfo in this.All)
			{
				if (seenInfo.Entity == entity)
				{
					return seenInfo;
				}
			}
			return default(Memory.SeenInfo);
		}

		// Token: 0x060045D3 RID: 17875 RVA: 0x00197A20 File Offset: 0x00195C20
		public Memory.SeenInfo GetInfo(Vector3 position)
		{
			foreach (Memory.SeenInfo seenInfo in this.All)
			{
				if ((seenInfo.Position - position).sqrMagnitude < 1f)
				{
					return seenInfo;
				}
			}
			return default(Memory.SeenInfo);
		}

		// Token: 0x060045D4 RID: 17876 RVA: 0x00197A98 File Offset: 0x00195C98
		public Memory.ExtendedInfo GetExtendedInfo(BaseEntity entity)
		{
			foreach (Memory.ExtendedInfo extendedInfo in this.AllExtended)
			{
				if (extendedInfo.Entity == entity)
				{
					return extendedInfo;
				}
			}
			return default(Memory.ExtendedInfo);
		}

		// Token: 0x060045D5 RID: 17877 RVA: 0x00197B04 File Offset: 0x00195D04
		internal void Forget(float maxSecondsOld)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				float num = Time.realtimeSinceStartup - this.All[i].Timestamp;
				if (num > maxSecondsOld)
				{
					if (this.All[i].Entity != null)
					{
						this.Visible.Remove(this.All[i].Entity);
						for (int j = 0; j < this.AllExtended.Count; j++)
						{
							if (this.AllExtended[j].Entity == this.All[i].Entity)
							{
								this.AllExtended.RemoveAt(j);
								break;
							}
						}
					}
					this.All.RemoveAt(i);
					i--;
				}
				else if (num > 0f)
				{
					float num2 = num / maxSecondsOld;
					if (this.All[i].Danger > 0f)
					{
						Memory.SeenInfo value = this.All[i];
						value.Danger -= num2;
						this.All[i] = value;
					}
					if (num >= 1f)
					{
						for (int k = 0; k < this.AllExtended.Count; k++)
						{
							if (this.AllExtended[k].Entity == this.All[i].Entity)
							{
								Memory.ExtendedInfo value2 = this.AllExtended[k];
								value2.LineOfSight = 0;
								this.AllExtended[k] = value2;
								break;
							}
						}
					}
				}
			}
			for (int l = 0; l < this.Visible.Count; l++)
			{
				if (this.Visible[l] == null)
				{
					this.Visible.RemoveAt(l);
					l--;
				}
			}
			for (int m = 0; m < this.AllExtended.Count; m++)
			{
				if (this.AllExtended[m].Entity == null)
				{
					this.AllExtended.RemoveAt(m);
					m--;
				}
			}
		}

		// Token: 0x02000F9D RID: 3997
		public struct SeenInfo
		{
			// Token: 0x0400506C RID: 20588
			public BaseEntity Entity;

			// Token: 0x0400506D RID: 20589
			public Vector3 Position;

			// Token: 0x0400506E RID: 20590
			public float Timestamp;

			// Token: 0x0400506F RID: 20591
			public float Danger;
		}

		// Token: 0x02000F9E RID: 3998
		public struct ExtendedInfo
		{
			// Token: 0x04005070 RID: 20592
			public BaseEntity Entity;

			// Token: 0x04005071 RID: 20593
			public Vector3 Direction;

			// Token: 0x04005072 RID: 20594
			public float Dot;

			// Token: 0x04005073 RID: 20595
			public float DistanceSqr;

			// Token: 0x04005074 RID: 20596
			public byte LineOfSight;

			// Token: 0x04005075 RID: 20597
			public float LastHurtUsTime;
		}
	}
}
