using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000057 RID: 87
public class CCTV_RC : PoweredRemoteControlEntity, IRemoteControllableClientCallbacks, IRemoteControllable
{
	// Token: 0x04000637 RID: 1591
	public Transform pivotOrigin;

	// Token: 0x04000638 RID: 1592
	public Transform yaw;

	// Token: 0x04000639 RID: 1593
	public Transform pitch;

	// Token: 0x0400063A RID: 1594
	public Vector2 pitchClamp = new Vector2(-50f, 50f);

	// Token: 0x0400063B RID: 1595
	public Vector2 yawClamp = new Vector2(-50f, 50f);

	// Token: 0x0400063C RID: 1596
	public float turnSpeed = 25f;

	// Token: 0x0400063D RID: 1597
	public float serverLerpSpeed = 15f;

	// Token: 0x0400063E RID: 1598
	public float clientLerpSpeed = 10f;

	// Token: 0x0400063F RID: 1599
	public float zoomLerpSpeed = 10f;

	// Token: 0x04000640 RID: 1600
	public float[] fovScales;

	// Token: 0x04000641 RID: 1601
	private float pitchAmount;

	// Token: 0x04000642 RID: 1602
	private float yawAmount;

	// Token: 0x04000643 RID: 1603
	private int fovScaleIndex;

	// Token: 0x04000644 RID: 1604
	private float fovScaleLerped = 1f;

	// Token: 0x04000645 RID: 1605
	public bool hasPTZ = true;

	// Token: 0x04000646 RID: 1606
	public AnimationCurve dofCurve = AnimationCurve.Constant(0f, 1f, 0f);

	// Token: 0x04000647 RID: 1607
	public float dofApertureMax = 10f;

	// Token: 0x04000648 RID: 1608
	public const global::BaseEntity.Flags Flag_HasViewer = global::BaseEntity.Flags.Reserved5;

	// Token: 0x04000649 RID: 1609
	public SoundDefinition movementLoopSoundDef;

	// Token: 0x0400064A RID: 1610
	public AnimationCurve movementLoopGainCurve;

	// Token: 0x0400064B RID: 1611
	public float movementLoopSmoothing = 1f;

	// Token: 0x0400064C RID: 1612
	public float movementLoopReference = 50f;

	// Token: 0x0400064D RID: 1613
	private Sound movementLoop;

	// Token: 0x0400064E RID: 1614
	private SoundModulation.Modulator movementLoopGainModulator;

	// Token: 0x0400064F RID: 1615
	public SoundDefinition zoomInSoundDef;

	// Token: 0x04000650 RID: 1616
	public SoundDefinition zoomOutSoundDef;

	// Token: 0x04000651 RID: 1617
	private RealTimeSinceEx timeSinceLastServerTick;

	// Token: 0x06000975 RID: 2421 RVA: 0x00059688 File Offset: 0x00057888
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CCTV_RC.OnRpcMessage", 0))
		{
			if (rpc == 3353964129U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_SetDir ");
				}
				using (TimeWarning.New("Server_SetDir", 0))
				{
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
							this.Server_SetDir(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_SetDir");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x000037BE File Offset: 0x000019BE
	public override int ConsumptionAmount()
	{
		return 3;
	}

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000977 RID: 2423 RVA: 0x000597AC File Offset: 0x000579AC
	public override bool RequiresMouse
	{
		get
		{
			return this.hasPTZ;
		}
	}

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000978 RID: 2424 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool EntityCanPing
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000979 RID: 2425 RVA: 0x000597AC File Offset: 0x000579AC
	public override bool CanAcceptInput
	{
		get
		{
			return this.hasPTZ;
		}
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x000597B4 File Offset: 0x000579B4
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		if (base.IsStatic())
		{
			this.pitchAmount = this.pitch.localEulerAngles.x;
			this.yawAmount = this.yaw.localEulerAngles.y;
			base.UpdateRCAccess(true);
		}
		this.timeSinceLastServerTick = 0.0;
		base.InvokeRandomized(new Action(this.ServerTick), UnityEngine.Random.Range(0f, 1f), 0.015f, 0.01f);
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x0005984A File Offset: 0x00057A4A
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.UpdateRotation(10000f);
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x0005985D File Offset: 0x00057A5D
	public override void UserInput(InputState inputState, CameraViewerId viewerID)
	{
		if (this.UpdateManualAim(inputState))
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x00059870 File Offset: 0x00057A70
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.rcEntity == null)
		{
			info.msg.rcEntity = Facepunch.Pool.Get<RCEntity>();
		}
		info.msg.rcEntity.aim.x = this.pitchAmount;
		info.msg.rcEntity.aim.y = this.yawAmount;
		info.msg.rcEntity.aim.z = 0f;
		info.msg.rcEntity.zoom = (float)this.fovScaleIndex;
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x00059908 File Offset: 0x00057B08
	[global::BaseEntity.RPC_Server]
	public void Server_SetDir(global::BaseEntity.RPCMessage msg)
	{
		if (base.IsStatic())
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (!player.CanBuild() || !player.IsBuildingAuthed())
		{
			return;
		}
		Vector3 vector = Vector3Ex.Direction(player.eyes.position, this.yaw.transform.position);
		vector = base.transform.InverseTransformDirection(vector);
		Vector3 vector2 = BaseMountable.ConvertVector(Quaternion.LookRotation(vector).eulerAngles);
		this.pitchAmount = Mathf.Clamp(vector2.x, this.pitchClamp.x, this.pitchClamp.y);
		this.yawAmount = Mathf.Clamp(vector2.y, this.yawClamp.x, this.yawClamp.y);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x000599CD File Offset: 0x00057BCD
	public override bool InitializeControl(CameraViewerId viewerID)
	{
		bool result = base.InitializeControl(viewerID);
		this.UpdateViewers();
		return result;
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x000599DC File Offset: 0x00057BDC
	public override void StopControl(CameraViewerId viewerID)
	{
		base.StopControl(viewerID);
		this.UpdateViewers();
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x000599EB File Offset: 0x00057BEB
	public void UpdateViewers()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved5, base.ViewerCount > 0, false, true);
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x00059A04 File Offset: 0x00057C04
	public void ServerTick()
	{
		if (base.isClient)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		float delta = (float)this.timeSinceLastServerTick;
		this.timeSinceLastServerTick = 0.0;
		this.UpdateRotation(delta);
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x00059A4C File Offset: 0x00057C4C
	private bool UpdateManualAim(InputState inputState)
	{
		if (!this.hasPTZ)
		{
			return false;
		}
		float num = -inputState.current.mouseDelta.y;
		float x = inputState.current.mouseDelta.x;
		bool flag = inputState.WasJustPressed(BUTTON.FIRE_PRIMARY);
		this.pitchAmount = Mathf.Clamp(this.pitchAmount + num * this.turnSpeed, this.pitchClamp.x, this.pitchClamp.y);
		this.yawAmount = Mathf.Clamp(this.yawAmount + x * this.turnSpeed, this.yawClamp.x, this.yawClamp.y) % 360f;
		if (flag)
		{
			this.fovScaleIndex = (this.fovScaleIndex + 1) % this.fovScales.Length;
		}
		return num != 0f || x != 0f || flag;
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x00059B2C File Offset: 0x00057D2C
	public void UpdateRotation(float delta)
	{
		Quaternion to = Quaternion.Euler(this.pitchAmount, 0f, 0f);
		Quaternion to2 = Quaternion.Euler(0f, this.yawAmount, 0f);
		float speed = (base.isServer && !base.IsBeingControlled) ? this.serverLerpSpeed : this.clientLerpSpeed;
		this.pitch.transform.localRotation = Mathx.Lerp(this.pitch.transform.localRotation, to, speed, delta);
		this.yaw.transform.localRotation = Mathx.Lerp(this.yaw.transform.localRotation, to2, speed, delta);
		if (this.fovScales == null || this.fovScales.Length == 0)
		{
			this.fovScaleLerped = 1f;
			return;
		}
		if (this.fovScales.Length > 1)
		{
			this.fovScaleLerped = Mathx.Lerp(this.fovScaleLerped, this.fovScales[this.fovScaleIndex], this.zoomLerpSpeed, delta);
			return;
		}
		this.fovScaleLerped = this.fovScales[0];
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x00059C30 File Offset: 0x00057E30
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.rcEntity != null)
		{
			int num = Mathf.Clamp((int)info.msg.rcEntity.zoom, 0, this.fovScales.Length - 1);
			if (base.isServer)
			{
				this.pitchAmount = info.msg.rcEntity.aim.x;
				this.yawAmount = info.msg.rcEntity.aim.y;
				this.fovScaleIndex = num;
			}
		}
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x00059CB8 File Offset: 0x00057EB8
	public override float GetFovScale()
	{
		return this.fovScaleLerped;
	}
}
