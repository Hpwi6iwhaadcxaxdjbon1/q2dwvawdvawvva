using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000031 RID: 49
public class AdvancedChristmasLights : global::IOEntity
{
	// Token: 0x04000188 RID: 392
	public GameObjectRef bulbPrefab;

	// Token: 0x04000189 RID: 393
	public LineRenderer lineRenderer;

	// Token: 0x0400018A RID: 394
	public List<AdvancedChristmasLights.pointEntry> points = new List<AdvancedChristmasLights.pointEntry>();

	// Token: 0x0400018B RID: 395
	public List<BaseBulb> bulbs = new List<BaseBulb>();

	// Token: 0x0400018C RID: 396
	public float bulbSpacing = 0.25f;

	// Token: 0x0400018D RID: 397
	public float wireThickness = 0.02f;

	// Token: 0x0400018E RID: 398
	public Transform wireEmission;

	// Token: 0x0400018F RID: 399
	public AdvancedChristmasLights.AnimationType animationStyle = AdvancedChristmasLights.AnimationType.ON;

	// Token: 0x04000190 RID: 400
	public RendererLOD _lod;

	// Token: 0x04000191 RID: 401
	[Tooltip("This many units used will result in +1 power usage")]
	public float lengthToPowerRatio = 5f;

	// Token: 0x04000192 RID: 402
	private bool finalized;

	// Token: 0x04000193 RID: 403
	private int lengthUsed;

	// Token: 0x0600013A RID: 314 RVA: 0x000215B0 File Offset: 0x0001F7B0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("AdvancedChristmasLights.OnRpcMessage", 0))
		{
			if (rpc == 1435781224U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetAnimationStyle ");
				}
				using (TimeWarning.New("SetAnimationStyle", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1435781224U, "SetAnimationStyle", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetAnimationStyle(rpcmessage);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetAnimationStyle");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600013B RID: 315 RVA: 0x00021718 File Offset: 0x0001F918
	public void ClearPoints()
	{
		this.points.Clear();
	}

	// Token: 0x0600013C RID: 316 RVA: 0x00021725 File Offset: 0x0001F925
	public void FinishEditing()
	{
		this.finalized = true;
	}

	// Token: 0x0600013D RID: 317 RVA: 0x0002172E File Offset: 0x0001F92E
	public bool IsFinalized()
	{
		return this.finalized;
	}

	// Token: 0x0600013E RID: 318 RVA: 0x00021738 File Offset: 0x0001F938
	public void AddPoint(Vector3 newPoint, Vector3 newNormal)
	{
		if (base.isServer && this.points.Count == 0)
		{
			newPoint = this.wireEmission.position;
		}
		AdvancedChristmasLights.pointEntry item = default(AdvancedChristmasLights.pointEntry);
		item.point = newPoint;
		item.normal = newNormal;
		this.points.Add(item);
		if (base.isServer)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600013F RID: 319 RVA: 0x0002179A File Offset: 0x0001F99A
	public override int ConsumptionAmount()
	{
		return 5;
	}

	// Token: 0x06000140 RID: 320 RVA: 0x0002179D File Offset: 0x0001F99D
	protected override int GetPickupCount()
	{
		return Mathf.Max(this.lengthUsed, 1);
	}

	// Token: 0x06000141 RID: 321 RVA: 0x000217AB File Offset: 0x0001F9AB
	public void AddLengthUsed(int addLength)
	{
		this.lengthUsed += addLength;
	}

	// Token: 0x06000142 RID: 322 RVA: 0x000217BB File Offset: 0x0001F9BB
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x06000143 RID: 323 RVA: 0x000217C4 File Offset: 0x0001F9C4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lightString = Facepunch.Pool.Get<LightString>();
		info.msg.lightString.points = Facepunch.Pool.GetList<LightString.StringPoint>();
		info.msg.lightString.lengthUsed = this.lengthUsed;
		info.msg.lightString.animationStyle = (int)this.animationStyle;
		foreach (AdvancedChristmasLights.pointEntry pointEntry in this.points)
		{
			LightString.StringPoint stringPoint = Facepunch.Pool.Get<LightString.StringPoint>();
			stringPoint.point = pointEntry.point;
			stringPoint.normal = pointEntry.normal;
			info.msg.lightString.points.Add(stringPoint);
		}
	}

	// Token: 0x06000144 RID: 324 RVA: 0x0002189C File Offset: 0x0001FA9C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lightString != null)
		{
			this.ClearPoints();
			foreach (LightString.StringPoint stringPoint in info.msg.lightString.points)
			{
				this.AddPoint(stringPoint.point, stringPoint.normal);
			}
			this.lengthUsed = info.msg.lightString.lengthUsed;
			this.animationStyle = (AdvancedChristmasLights.AnimationType)info.msg.lightString.animationStyle;
			if (info.fromDisk)
			{
				this.FinishEditing();
			}
		}
	}

	// Token: 0x06000145 RID: 325 RVA: 0x0002195C File Offset: 0x0001FB5C
	public bool IsStyle(AdvancedChristmasLights.AnimationType testType)
	{
		return testType == this.animationStyle;
	}

	// Token: 0x06000146 RID: 326 RVA: 0x0000441C File Offset: 0x0000261C
	public bool CanPlayerManipulate(global::BasePlayer player)
	{
		return true;
	}

	// Token: 0x06000147 RID: 327 RVA: 0x00021968 File Offset: 0x0001FB68
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetAnimationStyle(global::BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		num = Mathf.Clamp(num, 1, 7);
		if (Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Set animation style to :",
				num,
				" old was : ",
				(int)this.animationStyle
			}));
		}
		AdvancedChristmasLights.AnimationType animationType = (AdvancedChristmasLights.AnimationType)num;
		if (animationType == this.animationStyle)
		{
			return;
		}
		this.animationStyle = animationType;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x02000B50 RID: 2896
	public struct pointEntry
	{
		// Token: 0x04003EA5 RID: 16037
		public Vector3 point;

		// Token: 0x04003EA6 RID: 16038
		public Vector3 normal;
	}

	// Token: 0x02000B51 RID: 2897
	public enum AnimationType
	{
		// Token: 0x04003EA8 RID: 16040
		ON = 1,
		// Token: 0x04003EA9 RID: 16041
		FLASHING,
		// Token: 0x04003EAA RID: 16042
		CHASING,
		// Token: 0x04003EAB RID: 16043
		FADE,
		// Token: 0x04003EAC RID: 16044
		SLOWGLOW = 6
	}
}
