using System;
using System.Collections.Generic;
using Facepunch;

namespace CompanionServer
{
	// Token: 0x020009E0 RID: 2528
	public class ChatLog
	{
		// Token: 0x04003690 RID: 13968
		private const int MaxBacklog = 20;

		// Token: 0x04003691 RID: 13969
		private readonly Dictionary<ulong, ChatLog.ChatState> States = new Dictionary<ulong, ChatLog.ChatState>();

		// Token: 0x06003C67 RID: 15463 RVA: 0x001638A8 File Offset: 0x00161AA8
		public void Record(ulong teamId, ulong steamId, string name, string message, string color, uint time)
		{
			ChatLog.ChatState chatState;
			if (!this.States.TryGetValue(teamId, out chatState))
			{
				chatState = Pool.Get<ChatLog.ChatState>();
				chatState.History = Pool.GetList<ChatLog.Entry>();
				this.States.Add(teamId, chatState);
			}
			while (chatState.History.Count >= 20)
			{
				chatState.History.RemoveAt(0);
			}
			chatState.History.Add(new ChatLog.Entry
			{
				SteamId = steamId,
				Name = name,
				Message = message,
				Color = color,
				Time = time
			});
		}

		// Token: 0x06003C68 RID: 15464 RVA: 0x00163940 File Offset: 0x00161B40
		public void Remove(ulong teamId)
		{
			ChatLog.ChatState chatState;
			if (!this.States.TryGetValue(teamId, out chatState))
			{
				return;
			}
			this.States.Remove(teamId);
			Pool.Free<ChatLog.ChatState>(ref chatState);
		}

		// Token: 0x06003C69 RID: 15465 RVA: 0x00163974 File Offset: 0x00161B74
		public IReadOnlyList<ChatLog.Entry> GetHistory(ulong teamId)
		{
			ChatLog.ChatState chatState;
			if (!this.States.TryGetValue(teamId, out chatState))
			{
				return null;
			}
			return chatState.History;
		}

		// Token: 0x02000EE6 RID: 3814
		public struct Entry
		{
			// Token: 0x04004D8C RID: 19852
			public ulong SteamId;

			// Token: 0x04004D8D RID: 19853
			public string Name;

			// Token: 0x04004D8E RID: 19854
			public string Message;

			// Token: 0x04004D8F RID: 19855
			public string Color;

			// Token: 0x04004D90 RID: 19856
			public uint Time;
		}

		// Token: 0x02000EE7 RID: 3815
		private class ChatState : Pool.IPooled
		{
			// Token: 0x04004D91 RID: 19857
			public List<ChatLog.Entry> History;

			// Token: 0x060053B3 RID: 21427 RVA: 0x001B3569 File Offset: 0x001B1769
			public void EnterPool()
			{
				if (this.History != null)
				{
					Pool.FreeList<ChatLog.Entry>(ref this.History);
				}
			}

			// Token: 0x060053B4 RID: 21428 RVA: 0x000063A5 File Offset: 0x000045A5
			public void LeavePool()
			{
			}
		}
	}
}
