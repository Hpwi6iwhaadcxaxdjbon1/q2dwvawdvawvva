using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E6 RID: 230
public class TreeManager : global::BaseEntity
{
	// Token: 0x04000CD4 RID: 3284
	public static ListHashSet<global::BaseEntity> entities = new ListHashSet<global::BaseEntity>(8);

	// Token: 0x04000CD5 RID: 3285
	public static TreeManager server;

	// Token: 0x04000CD6 RID: 3286
	private const int maxTreesPerPacket = 100;

	// Token: 0x0600145A RID: 5210 RVA: 0x000A0DB4 File Offset: 0x0009EFB4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TreeManager.OnRpcMessage", 0))
		{
			if (rpc == 1907121457U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_RequestTrees ");
				}
				using (TimeWarning.New("SERVER_RequestTrees", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1907121457U, "SERVER_RequestTrees", this, player, 0UL))
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
							this.SERVER_RequestTrees(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SERVER_RequestTrees");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600145B RID: 5211 RVA: 0x000A0F1C File Offset: 0x0009F11C
	public static Vector3 ProtoHalf3ToVec3(ProtoBuf.Half3 half3)
	{
		return new Vector3
		{
			x = Mathf.HalfToFloat((ushort)half3.x),
			y = Mathf.HalfToFloat((ushort)half3.y),
			z = Mathf.HalfToFloat((ushort)half3.z)
		};
	}

	// Token: 0x0600145C RID: 5212 RVA: 0x000A0F6C File Offset: 0x0009F16C
	public static ProtoBuf.Half3 Vec3ToProtoHalf3(Vector3 vec3)
	{
		return new ProtoBuf.Half3
		{
			x = (uint)Mathf.FloatToHalf(vec3.x),
			y = (uint)Mathf.FloatToHalf(vec3.y),
			z = (uint)Mathf.FloatToHalf(vec3.z)
		};
	}

	// Token: 0x0600145D RID: 5213 RVA: 0x000A0FB8 File Offset: 0x0009F1B8
	public override void ServerInit()
	{
		base.ServerInit();
		TreeManager.server = this;
	}

	// Token: 0x0600145E RID: 5214 RVA: 0x000A0FC6 File Offset: 0x0009F1C6
	public static void OnTreeDestroyed(global::BaseEntity billboardEntity)
	{
		TreeManager.entities.Remove(billboardEntity);
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isQuitting)
		{
			return;
		}
		TreeManager.server.ClientRPC<NetworkableId>(null, "CLIENT_TreeDestroyed", billboardEntity.net.ID);
	}

	// Token: 0x0600145F RID: 5215 RVA: 0x000A1000 File Offset: 0x0009F200
	public static void OnTreeSpawned(global::BaseEntity billboardEntity)
	{
		TreeManager.entities.Add(billboardEntity);
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isQuitting)
		{
			return;
		}
		using (ProtoBuf.Tree tree = Facepunch.Pool.Get<ProtoBuf.Tree>())
		{
			TreeManager.ExtractTreeNetworkData(billboardEntity, tree);
			TreeManager.server.ClientRPC<ProtoBuf.Tree>(null, "CLIENT_TreeSpawned", tree);
		}
	}

	// Token: 0x06001460 RID: 5216 RVA: 0x000A1064 File Offset: 0x0009F264
	private static void ExtractTreeNetworkData(global::BaseEntity billboardEntity, ProtoBuf.Tree tree)
	{
		tree.netId = billboardEntity.net.ID;
		tree.prefabId = billboardEntity.prefabID;
		tree.position = TreeManager.Vec3ToProtoHalf3(billboardEntity.transform.position);
		tree.scale = billboardEntity.transform.lossyScale.y;
	}

	// Token: 0x06001461 RID: 5217 RVA: 0x000A10BC File Offset: 0x0009F2BC
	public static void SendSnapshot(global::BasePlayer player)
	{
		BufferList<global::BaseEntity> values = TreeManager.entities.Values;
		TreeList treeList = null;
		for (int i = 0; i < values.Count; i++)
		{
			global::BaseEntity billboardEntity = values[i];
			ProtoBuf.Tree tree = Facepunch.Pool.Get<ProtoBuf.Tree>();
			TreeManager.ExtractTreeNetworkData(billboardEntity, tree);
			if (treeList == null)
			{
				treeList = Facepunch.Pool.Get<TreeList>();
				treeList.trees = Facepunch.Pool.GetList<ProtoBuf.Tree>();
			}
			treeList.trees.Add(tree);
			if (treeList.trees.Count >= 100)
			{
				TreeManager.server.ClientRPCPlayer<TreeList>(null, player, "CLIENT_ReceiveTrees", treeList);
				treeList.Dispose();
				treeList = null;
			}
		}
		if (treeList != null)
		{
			TreeManager.server.ClientRPCPlayer<TreeList>(null, player, "CLIENT_ReceiveTrees", treeList);
			treeList.Dispose();
		}
	}

	// Token: 0x06001462 RID: 5218 RVA: 0x000A1160 File Offset: 0x0009F360
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(0UL)]
	private void SERVER_RequestTrees(global::BaseEntity.RPCMessage msg)
	{
		TreeManager.SendSnapshot(msg.player);
	}
}
