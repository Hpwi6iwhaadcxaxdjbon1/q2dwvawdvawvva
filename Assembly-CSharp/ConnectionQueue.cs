using System;
using System.Collections.Generic;
using Network;
using UnityEngine;

// Token: 0x02000740 RID: 1856
public class ConnectionQueue
{
	// Token: 0x04002A10 RID: 10768
	private List<Connection> queue = new List<Connection>();

	// Token: 0x04002A11 RID: 10769
	private List<Connection> joining = new List<Connection>();

	// Token: 0x04002A12 RID: 10770
	private float nextMessageTime;

	// Token: 0x17000443 RID: 1091
	// (get) Token: 0x060033A9 RID: 13225 RVA: 0x0013DBB5 File Offset: 0x0013BDB5
	public int Queued
	{
		get
		{
			return this.queue.Count;
		}
	}

	// Token: 0x17000444 RID: 1092
	// (get) Token: 0x060033AA RID: 13226 RVA: 0x0013DBC2 File Offset: 0x0013BDC2
	public int Joining
	{
		get
		{
			return this.joining.Count;
		}
	}

	// Token: 0x060033AB RID: 13227 RVA: 0x0013DBD0 File Offset: 0x0013BDD0
	public void SkipQueue(ulong userid)
	{
		for (int i = 0; i < this.queue.Count; i++)
		{
			Connection connection = this.queue[i];
			if (connection.userid == userid)
			{
				this.JoinGame(connection);
				return;
			}
		}
	}

	// Token: 0x060033AC RID: 13228 RVA: 0x0013DC11 File Offset: 0x0013BE11
	internal void Join(Connection connection)
	{
		connection.state = Connection.State.InQueue;
		this.queue.Add(connection);
		this.nextMessageTime = 0f;
		if (this.CanJumpQueue(connection))
		{
			this.JoinGame(connection);
		}
	}

	// Token: 0x060033AD RID: 13229 RVA: 0x0013DC41 File Offset: 0x0013BE41
	public void Cycle(int availableSlots)
	{
		if (this.queue.Count == 0)
		{
			return;
		}
		if (availableSlots - this.Joining > 0)
		{
			this.JoinGame(this.queue[0]);
		}
		this.SendMessages();
	}

	// Token: 0x060033AE RID: 13230 RVA: 0x0013DC74 File Offset: 0x0013BE74
	private void SendMessages()
	{
		if (this.nextMessageTime > Time.realtimeSinceStartup)
		{
			return;
		}
		this.nextMessageTime = Time.realtimeSinceStartup + 10f;
		for (int i = 0; i < this.queue.Count; i++)
		{
			this.SendMessage(this.queue[i], i);
		}
	}

	// Token: 0x060033AF RID: 13231 RVA: 0x0013DCCC File Offset: 0x0013BECC
	private void SendMessage(Connection c, int position)
	{
		string val = string.Empty;
		if (position > 0)
		{
			val = string.Format("{0:N0} PLAYERS AHEAD OF YOU, {1:N0} PLAYERS BEHIND", position, this.queue.Count - position - 1);
		}
		else
		{
			val = string.Format("YOU'RE NEXT - {1:N0} PLAYERS BEHIND YOU", position, this.queue.Count - position - 1);
		}
		NetWrite netWrite = Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.Message);
		netWrite.String("QUEUE");
		netWrite.String(val);
		netWrite.Send(new SendInfo(c));
	}

	// Token: 0x060033B0 RID: 13232 RVA: 0x0013DD5E File Offset: 0x0013BF5E
	public void RemoveConnection(Connection connection)
	{
		if (this.queue.Remove(connection))
		{
			this.nextMessageTime = 0f;
		}
		this.joining.Remove(connection);
	}

	// Token: 0x060033B1 RID: 13233 RVA: 0x0013DD86 File Offset: 0x0013BF86
	private void JoinGame(Connection connection)
	{
		this.queue.Remove(connection);
		connection.state = Connection.State.Welcoming;
		this.nextMessageTime = 0f;
		this.joining.Add(connection);
		SingletonComponent<ServerMgr>.Instance.JoinGame(connection);
	}

	// Token: 0x060033B2 RID: 13234 RVA: 0x0013DDBE File Offset: 0x0013BFBE
	public void JoinedGame(Connection connection)
	{
		this.RemoveConnection(connection);
	}

	// Token: 0x060033B3 RID: 13235 RVA: 0x0013DDC8 File Offset: 0x0013BFC8
	private bool CanJumpQueue(Connection connection)
	{
		if (DeveloperList.Contains(connection.userid))
		{
			return true;
		}
		ServerUsers.User user = ServerUsers.Get(connection.userid);
		return (user != null && user.group == ServerUsers.UserGroup.Moderator) || (user != null && user.group == ServerUsers.UserGroup.Owner) || (user != null && user.group == ServerUsers.UserGroup.SkipQueue);
	}

	// Token: 0x060033B4 RID: 13236 RVA: 0x0013DE1C File Offset: 0x0013C01C
	public bool IsQueued(ulong userid)
	{
		for (int i = 0; i < this.queue.Count; i++)
		{
			if (this.queue[i].userid == userid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060033B5 RID: 13237 RVA: 0x0013DE58 File Offset: 0x0013C058
	public bool IsJoining(ulong userid)
	{
		for (int i = 0; i < this.joining.Count; i++)
		{
			if (this.joining[i].userid == userid)
			{
				return true;
			}
		}
		return false;
	}
}
