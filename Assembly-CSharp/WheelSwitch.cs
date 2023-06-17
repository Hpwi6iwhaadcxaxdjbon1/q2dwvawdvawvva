using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000EE RID: 238
public class WheelSwitch : global::IOEntity
{
	// Token: 0x04000D3E RID: 3390
	public Transform wheelObj;

	// Token: 0x04000D3F RID: 3391
	public float rotateSpeed = 90f;

	// Token: 0x04000D40 RID: 3392
	public global::BaseEntity.Flags BeingRotated = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000D41 RID: 3393
	public global::BaseEntity.Flags RotatingLeft = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000D42 RID: 3394
	public global::BaseEntity.Flags RotatingRight = global::BaseEntity.Flags.Reserved3;

	// Token: 0x04000D43 RID: 3395
	public float rotateProgress;

	// Token: 0x04000D44 RID: 3396
	public Animator animator;

	// Token: 0x04000D45 RID: 3397
	public float kineticEnergyPerSec = 1f;

	// Token: 0x04000D46 RID: 3398
	private global::BasePlayer rotatorPlayer;

	// Token: 0x04000D47 RID: 3399
	private float progressTickRate = 0.1f;

	// Token: 0x060014F7 RID: 5367 RVA: 0x000A598C File Offset: 0x000A3B8C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WheelSwitch.OnRpcMessage", 0))
		{
			if (rpc == 2223603322U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BeginRotate ");
				}
				using (TimeWarning.New("BeginRotate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2223603322U, "BeginRotate", this, player, 3f))
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
							this.BeginRotate(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in BeginRotate");
					}
				}
				return true;
			}
			if (rpc == 434251040U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - CancelRotate ");
				}
				using (TimeWarning.New("CancelRotate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(434251040U, "CancelRotate", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CancelRotate(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in CancelRotate");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060014F8 RID: 5368 RVA: 0x000A5C8C File Offset: 0x000A3E8C
	public override void ResetIOState()
	{
		this.CancelPlayerRotation();
	}

	// Token: 0x060014F9 RID: 5369 RVA: 0x000A5C94 File Offset: 0x000A3E94
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void BeginRotate(global::BaseEntity.RPCMessage msg)
	{
		if (this.IsBeingRotated())
		{
			return;
		}
		base.SetFlag(this.BeingRotated, true, false, true);
		this.rotatorPlayer = msg.player;
		base.InvokeRepeating(new Action(this.RotateProgress), 0f, this.progressTickRate);
	}

	// Token: 0x060014FA RID: 5370 RVA: 0x000A5CE4 File Offset: 0x000A3EE4
	public void CancelPlayerRotation()
	{
		base.CancelInvoke(new Action(this.RotateProgress));
		base.SetFlag(this.BeingRotated, false, false, true);
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				ioslot.connectedTo.Get(true).IOInput(this, this.ioType, 0f, ioslot.connectedToSlot);
			}
		}
		this.rotatorPlayer = null;
	}

	// Token: 0x060014FB RID: 5371 RVA: 0x000A5D6C File Offset: 0x000A3F6C
	public void RotateProgress()
	{
		if (!this.rotatorPlayer || this.rotatorPlayer.IsDead() || this.rotatorPlayer.IsSleeping() || Vector3Ex.Distance2D(this.rotatorPlayer.transform.position, base.transform.position) > 2f)
		{
			this.CancelPlayerRotation();
			return;
		}
		float num = this.kineticEnergyPerSec * this.progressTickRate;
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				num = ioslot.connectedTo.Get(true).IOInput(this, this.ioType, num, ioslot.connectedToSlot);
			}
		}
		if (num == 0f)
		{
			this.SetRotateProgress(this.rotateProgress + 0.1f);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060014FC RID: 5372 RVA: 0x000A5E4C File Offset: 0x000A404C
	public void SetRotateProgress(float newValue)
	{
		float num = this.rotateProgress;
		this.rotateProgress = newValue;
		base.SetFlag(global::BaseEntity.Flags.Reserved4, num != newValue, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		base.CancelInvoke(new Action(this.StoppedRotatingCheck));
		base.Invoke(new Action(this.StoppedRotatingCheck), 0.25f);
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x000A5EAB File Offset: 0x000A40AB
	public void StoppedRotatingCheck()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved4, false, false, true);
	}

	// Token: 0x060014FE RID: 5374 RVA: 0x000A5C8C File Offset: 0x000A3E8C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void CancelRotate(global::BaseEntity.RPCMessage msg)
	{
		this.CancelPlayerRotation();
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x000A5EBC File Offset: 0x000A40BC
	public void Powered()
	{
		float inputAmount = this.kineticEnergyPerSec * this.progressTickRate;
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				inputAmount = ioslot.connectedTo.Get(true).IOInput(this, this.ioType, inputAmount, ioslot.connectedToSlot);
			}
		}
		this.SetRotateProgress(this.rotateProgress + 0.1f);
	}

	// Token: 0x06001500 RID: 5376 RVA: 0x000A5F38 File Offset: 0x000A4138
	public override float IOInput(global::IOEntity from, global::IOEntity.IOType inputType, float inputAmount, int slot = 0)
	{
		if (inputAmount < 0f)
		{
			this.SetRotateProgress(this.rotateProgress + inputAmount);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		if (inputType == global::IOEntity.IOType.Electric && slot == 1)
		{
			if (inputAmount == 0f)
			{
				base.CancelInvoke(new Action(this.Powered));
			}
			else
			{
				base.InvokeRepeating(new Action(this.Powered), 0f, this.progressTickRate);
			}
		}
		return Mathf.Clamp(inputAmount - 1f, 0f, inputAmount);
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x000A5FB5 File Offset: 0x000A41B5
	public bool IsBeingRotated()
	{
		return base.HasFlag(this.BeingRotated);
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x000A5FC3 File Offset: 0x000A41C3
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sphereEntity == null)
		{
			return;
		}
		this.rotateProgress = info.msg.sphereEntity.radius;
	}

	// Token: 0x06001503 RID: 5379 RVA: 0x000A5FF0 File Offset: 0x000A41F0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Facepunch.Pool.Get<ProtoBuf.SphereEntity>();
		info.msg.sphereEntity.radius = this.rotateProgress;
	}
}
