using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000EF RID: 239
public class WireTool : HeldEntity
{
	// Token: 0x04000D48 RID: 3400
	public Sprite InputSprite;

	// Token: 0x04000D49 RID: 3401
	public Sprite OutputSprite;

	// Token: 0x04000D4A RID: 3402
	public Sprite ClearSprite;

	// Token: 0x04000D4B RID: 3403
	public static float maxWireLength = 30f;

	// Token: 0x04000D4C RID: 3404
	private const int maxLineNodes = 16;

	// Token: 0x04000D4D RID: 3405
	public GameObjectRef plugEffect;

	// Token: 0x04000D4E RID: 3406
	public SoundDefinition clearStartSoundDef;

	// Token: 0x04000D4F RID: 3407
	public SoundDefinition clearSoundDef;

	// Token: 0x04000D50 RID: 3408
	public GameObjectRef ioLine;

	// Token: 0x04000D51 RID: 3409
	public IOEntity.IOType wireType;

	// Token: 0x04000D52 RID: 3410
	public float RadialMenuHoldTime = 0.25f;

	// Token: 0x04000D53 RID: 3411
	private const float IndustrialWallOffset = 0.03f;

	// Token: 0x04000D54 RID: 3412
	public static Translate.Phrase Default = new Translate.Phrase("wiretoolcolour.default", "Default");

	// Token: 0x04000D55 RID: 3413
	public static Translate.Phrase DefaultDesc = new Translate.Phrase("wiretoolcolour.default.desc", "Default connection color");

	// Token: 0x04000D56 RID: 3414
	public static Translate.Phrase Red = new Translate.Phrase("wiretoolcolour.red", "Red");

	// Token: 0x04000D57 RID: 3415
	public static Translate.Phrase RedDesc = new Translate.Phrase("wiretoolcolour.red.desc", "Red connection color");

	// Token: 0x04000D58 RID: 3416
	public static Translate.Phrase Green = new Translate.Phrase("wiretoolcolour.green", "Green");

	// Token: 0x04000D59 RID: 3417
	public static Translate.Phrase GreenDesc = new Translate.Phrase("wiretoolcolour.green.desc", "Green connection color");

	// Token: 0x04000D5A RID: 3418
	public static Translate.Phrase Blue = new Translate.Phrase("wiretoolcolour.blue", "Blue");

	// Token: 0x04000D5B RID: 3419
	public static Translate.Phrase BlueDesc = new Translate.Phrase("wiretoolcolour.blue.desc", "Blue connection color");

	// Token: 0x04000D5C RID: 3420
	public static Translate.Phrase Yellow = new Translate.Phrase("wiretoolcolour.yellow", "Yellow");

	// Token: 0x04000D5D RID: 3421
	public static Translate.Phrase YellowDesc = new Translate.Phrase("wiretoolcolour.yellow.desc", "Yellow connection color");

	// Token: 0x04000D5E RID: 3422
	public static Translate.Phrase LightBlue = new Translate.Phrase("wiretoolcolour.light_blue", "Light Blue");

	// Token: 0x04000D5F RID: 3423
	public static Translate.Phrase LightBlueDesc = new Translate.Phrase("wiretoolcolour.light_blue.desc", "Light Blue connection color");

	// Token: 0x04000D60 RID: 3424
	public static Translate.Phrase Orange = new Translate.Phrase("wiretoolcolour.orange", "Orange");

	// Token: 0x04000D61 RID: 3425
	public static Translate.Phrase OrangeDesc = new Translate.Phrase("wiretoolcolour.orange.desc", "Orange connection color");

	// Token: 0x04000D62 RID: 3426
	public static Translate.Phrase Purple = new Translate.Phrase("wiretoolcolour.purple", "Purple");

	// Token: 0x04000D63 RID: 3427
	public static Translate.Phrase PurpleDesc = new Translate.Phrase("wiretoolcolour.purple.desc", "Purple connection color");

	// Token: 0x04000D64 RID: 3428
	public static Translate.Phrase White = new Translate.Phrase("wiretoolcolour.white", "White");

	// Token: 0x04000D65 RID: 3429
	public static Translate.Phrase WhiteDesc = new Translate.Phrase("wiretoolcolour.white.desc", "White connection color");

	// Token: 0x04000D66 RID: 3430
	public static Translate.Phrase Pink = new Translate.Phrase("wiretoolcolour.pink", "Pink");

	// Token: 0x04000D67 RID: 3431
	public static Translate.Phrase PinkDesc = new Translate.Phrase("wiretoolcolour.pink.desc", "Pink connection color");

	// Token: 0x04000D68 RID: 3432
	public WireTool.PendingPlug_t pending;

	// Token: 0x04000D69 RID: 3433
	private const float IndustrialThickness = 0.01f;

	// Token: 0x06001505 RID: 5381 RVA: 0x000A6078 File Offset: 0x000A4278
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WireTool.OnRpcMessage", 0))
		{
			if (rpc == 40328523U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - MakeConnection ");
				}
				using (TimeWarning.New("MakeConnection", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(40328523U, "MakeConnection", this, player))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(40328523U, "MakeConnection", this, player))
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
							this.MakeConnection(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in MakeConnection");
					}
				}
				return true;
			}
			if (rpc == 121409151U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestChangeColor ");
				}
				using (TimeWarning.New("RequestChangeColor", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(121409151U, "RequestChangeColor", this, player))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(121409151U, "RequestChangeColor", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RequestChangeColor(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RequestChangeColor");
					}
				}
				return true;
			}
			if (rpc == 2469840259U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestClear ");
				}
				using (TimeWarning.New("RequestClear", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(2469840259U, "RequestClear", this, player))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(2469840259U, "RequestClear", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg4 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RequestClear(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RequestClear");
					}
				}
				return true;
			}
			if (rpc == 2596458392U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetPlugged ");
				}
				using (TimeWarning.New("SetPlugged", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage plugged = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetPlugged(plugged);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in SetPlugged");
					}
				}
				return true;
			}
			if (rpc == 210386477U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - TryClear ");
				}
				using (TimeWarning.New("TryClear", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(210386477U, "TryClear", this, player))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(210386477U, "TryClear", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg5 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.TryClear(msg5);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in TryClear");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x06001506 RID: 5382 RVA: 0x000A678C File Offset: 0x000A498C
	public bool CanChangeColours
	{
		get
		{
			return this.wireType == IOEntity.IOType.Electric || this.wireType == IOEntity.IOType.Fluidic || this.wireType == IOEntity.IOType.Industrial;
		}
	}

	// Token: 0x06001507 RID: 5383 RVA: 0x000A67AA File Offset: 0x000A49AA
	public void ClearPendingPlug()
	{
		this.pending.ent = null;
		this.pending.index = -1;
	}

	// Token: 0x06001508 RID: 5384 RVA: 0x000A67C4 File Offset: 0x000A49C4
	public bool HasPendingPlug()
	{
		return this.pending.ent != null && this.pending.index != -1;
	}

	// Token: 0x06001509 RID: 5385 RVA: 0x000A67EC File Offset: 0x000A49EC
	public bool PendingPlugIsInput()
	{
		return this.pending.ent != null && this.pending.index != -1 && this.pending.input;
	}

	// Token: 0x0600150A RID: 5386 RVA: 0x000A681C File Offset: 0x000A4A1C
	public bool PendingPlugIsType(IOEntity.IOType type)
	{
		return this.pending.ent != null && this.pending.index != -1 && ((this.pending.input && this.pending.ent.inputs[this.pending.index].type == type) || (!this.pending.input && this.pending.ent.outputs[this.pending.index].type == type));
	}

	// Token: 0x0600150B RID: 5387 RVA: 0x000A68B2 File Offset: 0x000A4AB2
	public bool PendingPlugIsOutput()
	{
		return this.pending.ent != null && this.pending.index != -1 && !this.pending.input;
	}

	// Token: 0x0600150C RID: 5388 RVA: 0x000A68E8 File Offset: 0x000A4AE8
	public Vector3 PendingPlugWorldPos()
	{
		if (this.pending.ent == null || this.pending.index == -1)
		{
			return Vector3.zero;
		}
		if (this.pending.input)
		{
			return this.pending.ent.transform.TransformPoint(this.pending.ent.inputs[this.pending.index].handlePosition);
		}
		return this.pending.ent.transform.TransformPoint(this.pending.ent.outputs[this.pending.index].handlePosition);
	}

	// Token: 0x0600150D RID: 5389 RVA: 0x000A6998 File Offset: 0x000A4B98
	public static bool CanPlayerUseWires(BasePlayer player)
	{
		if (!player.CanBuild())
		{
			return false;
		}
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(player.eyes.position, 0.1f, list, 536870912, QueryTriggerInteraction.Collide);
		bool result = list.All((Collider collider) => collider.gameObject.CompareTag("IgnoreWireCheck"));
		Facepunch.Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x0600150E RID: 5390 RVA: 0x000A69FD File Offset: 0x000A4BFD
	public static bool CanModifyEntity(BasePlayer player, BaseEntity ent)
	{
		return player.CanBuild(ent.transform.position, ent.transform.rotation, ent.bounds);
	}

	// Token: 0x0600150F RID: 5391 RVA: 0x000A6A21 File Offset: 0x000A4C21
	public bool PendingPlugRoot()
	{
		return this.pending.ent != null && this.pending.ent.IsRootEntity();
	}

	// Token: 0x06001510 RID: 5392 RVA: 0x000A6A48 File Offset: 0x000A4C48
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.FromOwner]
	public void TryClear(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		NetworkableId uid = msg.read.EntityID();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		IOEntity ioentity = (baseNetworkable == null) ? null : baseNetworkable.GetComponent<IOEntity>();
		if (ioentity == null)
		{
			return;
		}
		if (!WireTool.CanPlayerUseWires(player))
		{
			return;
		}
		if (!WireTool.CanModifyEntity(player, ioentity))
		{
			return;
		}
		ioentity.ClearConnections();
		ioentity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001511 RID: 5393 RVA: 0x000A6AB4 File Offset: 0x000A4CB4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.FromOwner]
	public void MakeConnection(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!WireTool.CanPlayerUseWires(player))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num > 18)
		{
			return;
		}
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < num; i++)
		{
			Vector3 item = msg.read.Vector3();
			list.Add(item);
		}
		NetworkableId uid = msg.read.EntityID();
		int num2 = msg.read.Int32();
		NetworkableId uid2 = msg.read.EntityID();
		int num3 = msg.read.Int32();
		WireTool.WireColour wireColour = this.IntToColour(msg.read.Int32());
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		IOEntity ioentity = (baseNetworkable == null) ? null : baseNetworkable.GetComponent<IOEntity>();
		if (ioentity == null)
		{
			return;
		}
		BaseNetworkable baseNetworkable2 = BaseNetworkable.serverEntities.Find(uid2);
		IOEntity ioentity2 = (baseNetworkable2 == null) ? null : baseNetworkable2.GetComponent<IOEntity>();
		if (ioentity2 == null)
		{
			return;
		}
		if (!this.ValidateLine(list, ioentity, ioentity2, player, num3))
		{
			return;
		}
		if (Vector3.Distance(baseNetworkable2.transform.position, baseNetworkable.transform.position) > WireTool.maxWireLength)
		{
			return;
		}
		if (num2 >= ioentity.inputs.Length)
		{
			return;
		}
		if (num3 >= ioentity2.outputs.Length)
		{
			return;
		}
		if (ioentity.inputs[num2].connectedTo.Get(true) != null)
		{
			return;
		}
		if (ioentity2.outputs[num3].connectedTo.Get(true) != null)
		{
			return;
		}
		if (ioentity.inputs[num2].rootConnectionsOnly && !ioentity2.IsRootEntity())
		{
			return;
		}
		if (!WireTool.CanModifyEntity(player, ioentity))
		{
			return;
		}
		if (!WireTool.CanModifyEntity(player, ioentity2))
		{
			return;
		}
		ioentity.inputs[num2].connectedTo.Set(ioentity2);
		ioentity.inputs[num2].connectedToSlot = num3;
		ioentity.inputs[num2].wireColour = wireColour;
		ioentity.inputs[num2].connectedTo.Init();
		ioentity2.outputs[num3].connectedTo.Set(ioentity);
		ioentity2.outputs[num3].connectedToSlot = num2;
		ioentity2.outputs[num3].linePoints = list.ToArray();
		ioentity2.outputs[num3].wireColour = wireColour;
		ioentity2.outputs[num3].connectedTo.Init();
		ioentity2.outputs[num3].worldSpaceLineEndRotation = ioentity.transform.TransformDirection(ioentity.inputs[num2].handleDirection);
		ioentity2.MarkDirtyForceUpdateOutputs();
		ioentity2.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		ioentity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		ioentity2.SendChangedToRoot(true);
		ioentity2.RefreshIndustrialPreventBuilding();
		if (this.wireType == IOEntity.IOType.Industrial)
		{
			ioentity.NotifyIndustrialNetworkChanged();
			ioentity2.NotifyIndustrialNetworkChanged();
		}
	}

	// Token: 0x06001512 RID: 5394 RVA: 0x000063A5 File Offset: 0x000045A5
	[BaseEntity.RPC_Server]
	public void SetPlugged(BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x06001513 RID: 5395 RVA: 0x000A6D84 File Offset: 0x000A4F84
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.FromOwner]
	public void RequestClear(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!WireTool.CanPlayerUseWires(player))
		{
			return;
		}
		NetworkableId uid = msg.read.EntityID();
		int clearIndex = msg.read.Int32();
		bool isInput = msg.read.Bit();
		WireTool.AttemptClearSlot(BaseNetworkable.serverEntities.Find(uid), player, clearIndex, isInput);
	}

	// Token: 0x06001514 RID: 5396 RVA: 0x000A6DD8 File Offset: 0x000A4FD8
	public static void AttemptClearSlot(BaseNetworkable clearEnt, BasePlayer ply, int clearIndex, bool isInput)
	{
		IOEntity ioentity = (clearEnt == null) ? null : clearEnt.GetComponent<IOEntity>();
		if (ioentity == null)
		{
			return;
		}
		if (ply != null && !WireTool.CanModifyEntity(ply, ioentity))
		{
			return;
		}
		if (clearIndex >= (isInput ? ioentity.inputs.Length : ioentity.outputs.Length))
		{
			return;
		}
		IOEntity.IOSlot ioslot = isInput ? ioentity.inputs[clearIndex] : ioentity.outputs[clearIndex];
		if (ioslot.connectedTo.Get(true) == null)
		{
			return;
		}
		IOEntity ioentity2 = ioslot.connectedTo.Get(true);
		IOEntity.IOSlot ioslot2 = isInput ? ioentity2.outputs[ioslot.connectedToSlot] : ioentity2.inputs[ioslot.connectedToSlot];
		if (isInput)
		{
			ioentity.UpdateFromInput(0, clearIndex);
		}
		else if (ioentity2)
		{
			ioentity2.UpdateFromInput(0, ioslot.connectedToSlot);
		}
		ioslot.Clear();
		ioslot2.Clear();
		ioentity.MarkDirtyForceUpdateOutputs();
		ioentity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		ioentity.RefreshIndustrialPreventBuilding();
		if (ioentity2 != null)
		{
			ioentity2.RefreshIndustrialPreventBuilding();
		}
		if (isInput && ioentity2 != null)
		{
			ioentity2.SendChangedToRoot(true);
		}
		else if (!isInput)
		{
			foreach (IOEntity.IOSlot ioslot3 in ioentity.inputs)
			{
				if (ioslot3.mainPowerSlot && ioslot3.connectedTo.Get(true))
				{
					ioslot3.connectedTo.Get(true).SendChangedToRoot(true);
				}
			}
		}
		ioentity2.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		if (ioentity != null && ioentity.ioType == IOEntity.IOType.Industrial)
		{
			ioentity.NotifyIndustrialNetworkChanged();
		}
		if (ioentity2 != null && ioentity2.ioType == IOEntity.IOType.Industrial)
		{
			ioentity2.NotifyIndustrialNetworkChanged();
		}
	}

	// Token: 0x06001515 RID: 5397 RVA: 0x000A6F74 File Offset: 0x000A5174
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	[BaseEntity.RPC_Server.FromOwner]
	public void RequestChangeColor(BaseEntity.RPCMessage msg)
	{
		if (!WireTool.CanPlayerUseWires(msg.player))
		{
			return;
		}
		NetworkableId uid = msg.read.EntityID();
		int index = msg.read.Int32();
		bool flag = msg.read.Bit();
		WireTool.WireColour wireColour = this.IntToColour(msg.read.Int32());
		IOEntity ioentity = BaseNetworkable.serverEntities.Find(uid) as IOEntity;
		if (ioentity == null)
		{
			return;
		}
		IOEntity.IOSlot ioslot = flag ? ioentity.inputs.ElementAtOrDefault(index) : ioentity.outputs.ElementAtOrDefault(index);
		if (ioslot == null)
		{
			return;
		}
		IOEntity ioentity2 = ioslot.connectedTo.Get(true);
		if (ioentity2 == null)
		{
			return;
		}
		IOEntity.IOSlot ioslot2 = (flag ? ioentity2.outputs : ioentity2.inputs)[ioslot.connectedToSlot];
		ioslot.wireColour = wireColour;
		ioentity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		ioslot2.wireColour = wireColour;
		ioentity2.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001516 RID: 5398 RVA: 0x000A705C File Offset: 0x000A525C
	private WireTool.WireColour IntToColour(int i)
	{
		if (i < 0)
		{
			i = 0;
		}
		if (i >= 10)
		{
			i = 9;
		}
		WireTool.WireColour wireColour = (WireTool.WireColour)i;
		if (this.wireType == IOEntity.IOType.Fluidic && wireColour == WireTool.WireColour.Green)
		{
			wireColour = WireTool.WireColour.Default;
		}
		return wireColour;
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x000A708C File Offset: 0x000A528C
	private bool ValidateLine(List<Vector3> lineList, IOEntity inputEntity, IOEntity outputEntity, BasePlayer byPlayer, int outputIndex)
	{
		if (lineList.Count < 2)
		{
			return false;
		}
		if (inputEntity == null || outputEntity == null)
		{
			return false;
		}
		Vector3 a = lineList[0];
		float num = 0f;
		int count = lineList.Count;
		for (int i = 1; i < count; i++)
		{
			Vector3 vector = lineList[i];
			num += Vector3.Distance(a, vector);
			if (num > WireTool.maxWireLength)
			{
				return false;
			}
			a = vector;
		}
		Vector3 point = lineList[count - 1];
		Bounds bounds = outputEntity.bounds;
		bounds.Expand(0.5f);
		if (!bounds.Contains(point))
		{
			return false;
		}
		Vector3 position = outputEntity.transform.TransformPoint(lineList[0]);
		point = inputEntity.transform.InverseTransformPoint(position);
		Bounds bounds2 = inputEntity.bounds;
		bounds2.Expand(0.5f);
		if (!bounds2.Contains(point))
		{
			return false;
		}
		if (byPlayer == null)
		{
			return false;
		}
		Vector3 position2 = outputEntity.transform.TransformPoint(lineList[lineList.Count - 1]);
		return (byPlayer.Distance(position2) <= 5f || byPlayer.Distance(position) <= 5f) && (outputIndex < 0 || outputIndex >= outputEntity.outputs.Length || outputEntity.outputs[outputIndex].type != IOEntity.IOType.Industrial || this.VerifyLineOfSight(lineList, outputEntity.transform.localToWorldMatrix));
	}

	// Token: 0x06001518 RID: 5400 RVA: 0x000A71F0 File Offset: 0x000A53F0
	private bool VerifyLineOfSight(List<Vector3> positions, Matrix4x4 localToWorldSpace)
	{
		Vector3 worldSpaceA = localToWorldSpace.MultiplyPoint3x4(positions[0]);
		for (int i = 1; i < positions.Count; i++)
		{
			Vector3 vector = localToWorldSpace.MultiplyPoint3x4(positions[i]);
			if (!this.VerifyLineOfSight(worldSpaceA, vector))
			{
				return false;
			}
			worldSpaceA = vector;
		}
		return true;
	}

	// Token: 0x06001519 RID: 5401 RVA: 0x000A723C File Offset: 0x000A543C
	private bool VerifyLineOfSight(Vector3 worldSpaceA, Vector3 worldSpaceB)
	{
		float maxDistance = Vector3.Distance(worldSpaceA, worldSpaceB);
		Vector3 normalized = (worldSpaceA - worldSpaceB).normalized;
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(worldSpaceB, normalized), 0.01f, list, maxDistance, 2162944, QueryTriggerInteraction.UseGlobal, null);
		bool result = true;
		foreach (RaycastHit hit in list)
		{
			BaseEntity entity = hit.GetEntity();
			if (entity != null && hit.IsOnLayer(Rust.Layer.Deployed))
			{
				if (entity is VendingMachine)
				{
					result = false;
					break;
				}
			}
			else if (!(entity != null) || !(entity is Door))
			{
				result = false;
				break;
			}
		}
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
		return result;
	}

	// Token: 0x02000C19 RID: 3097
	public enum WireColour
	{
		// Token: 0x04004205 RID: 16901
		Default,
		// Token: 0x04004206 RID: 16902
		Red,
		// Token: 0x04004207 RID: 16903
		Green,
		// Token: 0x04004208 RID: 16904
		Blue,
		// Token: 0x04004209 RID: 16905
		Yellow,
		// Token: 0x0400420A RID: 16906
		Pink,
		// Token: 0x0400420B RID: 16907
		Purple,
		// Token: 0x0400420C RID: 16908
		Orange,
		// Token: 0x0400420D RID: 16909
		White,
		// Token: 0x0400420E RID: 16910
		LightBlue,
		// Token: 0x0400420F RID: 16911
		Count
	}

	// Token: 0x02000C1A RID: 3098
	public struct PendingPlug_t
	{
		// Token: 0x04004210 RID: 16912
		public IOEntity ent;

		// Token: 0x04004211 RID: 16913
		public bool input;

		// Token: 0x04004212 RID: 16914
		public int index;

		// Token: 0x04004213 RID: 16915
		public GameObject tempLine;
	}
}
