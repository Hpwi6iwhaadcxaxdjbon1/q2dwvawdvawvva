using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B6 RID: 182
public class ProceduralLift : global::BaseEntity
{
	// Token: 0x04000A81 RID: 2689
	public float movementSpeed = 1f;

	// Token: 0x04000A82 RID: 2690
	public float resetDelay = 5f;

	// Token: 0x04000A83 RID: 2691
	public ProceduralLiftCabin cabin;

	// Token: 0x04000A84 RID: 2692
	public ProceduralLiftStop[] stops;

	// Token: 0x04000A85 RID: 2693
	public GameObjectRef triggerPrefab;

	// Token: 0x04000A86 RID: 2694
	public string triggerBone;

	// Token: 0x04000A87 RID: 2695
	private int floorIndex = -1;

	// Token: 0x04000A88 RID: 2696
	public SoundDefinition startSoundDef;

	// Token: 0x04000A89 RID: 2697
	public SoundDefinition stopSoundDef;

	// Token: 0x04000A8A RID: 2698
	public SoundDefinition movementLoopSoundDef;

	// Token: 0x04000A8B RID: 2699
	private Sound movementLoopSound;

	// Token: 0x06001076 RID: 4214 RVA: 0x00088704 File Offset: 0x00086904
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ProceduralLift.OnRpcMessage", 0))
		{
			if (rpc == 2657791441U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_UseLift ");
				}
				using (TimeWarning.New("RPC_UseLift", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2657791441U, "RPC_UseLift", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_UseLift(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_UseLift");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001077 RID: 4215 RVA: 0x0008886C File Offset: 0x00086A6C
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.triggerPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
			baseEntity.Spawn();
			baseEntity.SetParent(this, this.triggerBone, false, false);
		}
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x000888BA File Offset: 0x00086ABA
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_UseLift(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		this.MoveToFloor((this.floorIndex + 1) % this.stops.Length);
	}

	// Token: 0x06001079 RID: 4217 RVA: 0x000888EA File Offset: 0x00086AEA
	public override void ServerInit()
	{
		base.ServerInit();
		this.SnapToFloor(0);
	}

	// Token: 0x0600107A RID: 4218 RVA: 0x000888F9 File Offset: 0x00086AF9
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lift = Facepunch.Pool.Get<ProtoBuf.Lift>();
		info.msg.lift.floor = this.floorIndex;
	}

	// Token: 0x0600107B RID: 4219 RVA: 0x00088928 File Offset: 0x00086B28
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.msg.lift != null)
		{
			if (this.floorIndex == -1)
			{
				this.SnapToFloor(info.msg.lift.floor);
			}
			else
			{
				this.MoveToFloor(info.msg.lift.floor);
			}
		}
		base.Load(info);
	}

	// Token: 0x0600107C RID: 4220 RVA: 0x00088980 File Offset: 0x00086B80
	private void ResetLift()
	{
		this.MoveToFloor(0);
	}

	// Token: 0x0600107D RID: 4221 RVA: 0x0008898C File Offset: 0x00086B8C
	private void MoveToFloor(int floor)
	{
		this.floorIndex = Mathf.Clamp(floor, 0, this.stops.Length - 1);
		if (base.isServer)
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
			base.SendNetworkUpdateImmediate(false);
			base.CancelInvoke(new Action(this.ResetLift));
		}
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x000889E0 File Offset: 0x00086BE0
	private void SnapToFloor(int floor)
	{
		this.floorIndex = Mathf.Clamp(floor, 0, this.stops.Length - 1);
		ProceduralLiftStop proceduralLiftStop = this.stops[this.floorIndex];
		this.cabin.transform.position = proceduralLiftStop.transform.position;
		if (base.isServer)
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			base.SendNetworkUpdateImmediate(false);
			base.CancelInvoke(new Action(this.ResetLift));
		}
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x00088A5C File Offset: 0x00086C5C
	private void OnFinishedMoving()
	{
		if (base.isServer)
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			base.SendNetworkUpdateImmediate(false);
			if (this.floorIndex != 0)
			{
				base.Invoke(new Action(this.ResetLift), this.resetDelay);
			}
		}
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x00088A9C File Offset: 0x00086C9C
	protected void Update()
	{
		if (this.floorIndex < 0 || this.floorIndex > this.stops.Length - 1)
		{
			return;
		}
		ProceduralLiftStop proceduralLiftStop = this.stops[this.floorIndex];
		if (this.cabin.transform.position == proceduralLiftStop.transform.position)
		{
			return;
		}
		this.cabin.transform.position = Vector3.MoveTowards(this.cabin.transform.position, proceduralLiftStop.transform.position, this.movementSpeed * UnityEngine.Time.deltaTime);
		if (this.cabin.transform.position == proceduralLiftStop.transform.position)
		{
			this.OnFinishedMoving();
		}
	}

	// Token: 0x06001081 RID: 4225 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StartMovementSounds()
	{
	}

	// Token: 0x06001082 RID: 4226 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopMovementSounds()
	{
	}
}
