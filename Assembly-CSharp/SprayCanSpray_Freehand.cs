using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D6 RID: 214
public class SprayCanSpray_Freehand : SprayCanSpray
{
	// Token: 0x04000BED RID: 3053
	public AlignedLineDrawer LineDrawer;

	// Token: 0x04000BEE RID: 3054
	public List<AlignedLineDrawer.LinePoint> LinePoints = new List<AlignedLineDrawer.LinePoint>();

	// Token: 0x04000BEF RID: 3055
	private Color colour = Color.white;

	// Token: 0x04000BF0 RID: 3056
	private float width;

	// Token: 0x04000BF1 RID: 3057
	private EntityRef<global::BasePlayer> editingPlayer;

	// Token: 0x04000BF2 RID: 3058
	public GroundWatch groundWatch;

	// Token: 0x04000BF3 RID: 3059
	public MeshCollider meshCollider;

	// Token: 0x04000BF4 RID: 3060
	public const int MaxLinePointLength = 60;

	// Token: 0x04000BF5 RID: 3061
	public const float SimplifyTolerance = 0.008f;

	// Token: 0x060012FD RID: 4861 RVA: 0x00098EA8 File Offset: 0x000970A8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SprayCanSpray_Freehand.OnRpcMessage", 0))
		{
			if (rpc == 2020094435U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_AddPointMidSpray ");
				}
				using (TimeWarning.New("Server_AddPointMidSpray", 0))
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
							this.Server_AddPointMidSpray(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_AddPointMidSpray");
					}
				}
				return true;
			}
			if (rpc == 117883393U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_FinishEditing ");
				}
				using (TimeWarning.New("Server_FinishEditing", 0))
				{
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
							this.Server_FinishEditing(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Server_FinishEditing");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x060012FE RID: 4862 RVA: 0x00099108 File Offset: 0x00097308
	private bool AcceptingChanges
	{
		get
		{
			return this.editingPlayer.IsValid(true);
		}
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x00099116 File Offset: 0x00097316
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.LinePoints == null || this.LinePoints.Count == 0)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x0009913C File Offset: 0x0009733C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.sprayLine == null)
		{
			info.msg.sprayLine = Facepunch.Pool.Get<SprayLine>();
		}
		if (info.msg.sprayLine.linePoints == null)
		{
			info.msg.sprayLine.linePoints = Facepunch.Pool.GetList<LinePoint>();
		}
		bool flag = this.AcceptingChanges && info.forDisk;
		if (this.LinePoints != null && !flag)
		{
			this.CopyPoints(this.LinePoints, info.msg.sprayLine.linePoints);
		}
		info.msg.sprayLine.width = this.width;
		info.msg.sprayLine.colour = new Vector3(this.colour.r, this.colour.g, this.colour.b);
		if (!info.forDisk)
		{
			info.msg.sprayLine.editingPlayer = this.editingPlayer.uid;
		}
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x0009923C File Offset: 0x0009743C
	public void SetColour(Color newColour)
	{
		this.colour = newColour;
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x00099245 File Offset: 0x00097445
	public void SetWidth(float lineWidth)
	{
		this.width = lineWidth;
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x00099250 File Offset: 0x00097450
	[global::BaseEntity.RPC_Server]
	private void Server_AddPointMidSpray(global::BaseEntity.RPCMessage msg)
	{
		if (!this.AcceptingChanges || this.editingPlayer.Get(true) != msg.player)
		{
			return;
		}
		if (this.LinePoints.Count + 1 > 60)
		{
			return;
		}
		Vector3 vector = msg.read.Vector3();
		Vector3 worldNormal = msg.read.Vector3();
		if (Vector3.Distance(vector, this.LinePoints[0].LocalPosition) >= 10f)
		{
			return;
		}
		this.LinePoints.Add(new AlignedLineDrawer.LinePoint
		{
			LocalPosition = vector,
			WorldNormal = worldNormal
		});
		this.UpdateGroundWatch();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x000992F9 File Offset: 0x000974F9
	public void EnableChanges(global::BasePlayer byPlayer)
	{
		base.OwnerID = byPlayer.userID;
		this.editingPlayer.Set(byPlayer);
		base.Invoke(new Action(this.TimeoutEditing), 30f);
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x0009932A File Offset: 0x0009752A
	private void TimeoutEditing()
	{
		if (this.editingPlayer.IsSet)
		{
			this.editingPlayer.Set(null);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001307 RID: 4871 RVA: 0x00099354 File Offset: 0x00097554
	[global::BaseEntity.RPC_Server]
	private void Server_FinishEditing(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer basePlayer = this.editingPlayer.Get(true);
		if (msg.player != basePlayer)
		{
			return;
		}
		bool allowNewSprayImmediately = msg.read.Int32() == 1;
		SprayCan sprayCan;
		if (basePlayer != null && basePlayer.GetHeldEntity() != null && (sprayCan = (basePlayer.GetHeldEntity() as SprayCan)) != null)
		{
			sprayCan.ClearPaintingLine(allowNewSprayImmediately);
		}
		this.editingPlayer.Set(null);
		SprayList sprayList = SprayList.Deserialize(msg.read);
		int count = sprayList.linePoints.Count;
		if (count > 70)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			Facepunch.Pool.FreeList<LinePoint>(ref sprayList.linePoints);
			Facepunch.Pool.Free<SprayList>(ref sprayList);
			return;
		}
		if (this.LinePoints.Count <= 1)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			Facepunch.Pool.FreeList<LinePoint>(ref sprayList.linePoints);
			Facepunch.Pool.Free<SprayList>(ref sprayList);
			return;
		}
		base.CancelInvoke(new Action(this.TimeoutEditing));
		this.LinePoints.Clear();
		for (int i = 0; i < count; i++)
		{
			if (sprayList.linePoints[i].localPosition.sqrMagnitude < 100f)
			{
				this.LinePoints.Add(new AlignedLineDrawer.LinePoint
				{
					LocalPosition = sprayList.linePoints[i].localPosition,
					WorldNormal = sprayList.linePoints[i].worldNormal
				});
			}
		}
		this.OnDeployed(null, basePlayer, null);
		this.UpdateGroundWatch();
		Facepunch.Pool.FreeList<LinePoint>(ref sprayList.linePoints);
		Facepunch.Pool.Free<SprayList>(ref sprayList);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001308 RID: 4872 RVA: 0x000994E4 File Offset: 0x000976E4
	public void AddInitialPoint(Vector3 atNormal)
	{
		this.LinePoints = new List<AlignedLineDrawer.LinePoint>
		{
			new AlignedLineDrawer.LinePoint
			{
				LocalPosition = Vector3.zero,
				WorldNormal = atNormal
			}
		};
	}

	// Token: 0x06001309 RID: 4873 RVA: 0x00099520 File Offset: 0x00097720
	private void UpdateGroundWatch()
	{
		if (base.isServer && this.LinePoints.Count > 1)
		{
			Vector3 groundPosition = Vector3.Lerp(this.LinePoints[0].LocalPosition, this.LinePoints[this.LinePoints.Count - 1].LocalPosition, 0.5f);
			if (this.groundWatch != null)
			{
				this.groundWatch.groundPosition = groundPosition;
			}
		}
	}

	// Token: 0x0600130A RID: 4874 RVA: 0x00099598 File Offset: 0x00097798
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sprayLine != null)
		{
			if (info.msg.sprayLine.linePoints != null)
			{
				this.LinePoints.Clear();
				this.CopyPoints(info.msg.sprayLine.linePoints, this.LinePoints);
			}
			this.colour = new Color(info.msg.sprayLine.colour.x, info.msg.sprayLine.colour.y, info.msg.sprayLine.colour.z);
			this.width = info.msg.sprayLine.width;
			this.editingPlayer.uid = info.msg.sprayLine.editingPlayer;
			this.UpdateGroundWatch();
		}
	}

	// Token: 0x0600130B RID: 4875 RVA: 0x00099678 File Offset: 0x00097878
	private void CopyPoints(List<AlignedLineDrawer.LinePoint> from, List<LinePoint> to)
	{
		to.Clear();
		foreach (AlignedLineDrawer.LinePoint linePoint in from)
		{
			LinePoint linePoint2 = Facepunch.Pool.Get<LinePoint>();
			linePoint2.localPosition = linePoint.LocalPosition;
			linePoint2.worldNormal = linePoint.WorldNormal;
			to.Add(linePoint2);
		}
	}

	// Token: 0x0600130C RID: 4876 RVA: 0x000996EC File Offset: 0x000978EC
	private void CopyPoints(List<AlignedLineDrawer.LinePoint> from, List<Vector3> to)
	{
		to.Clear();
		foreach (AlignedLineDrawer.LinePoint linePoint in from)
		{
			to.Add(linePoint.LocalPosition);
			to.Add(linePoint.WorldNormal);
		}
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x00099754 File Offset: 0x00097954
	private void CopyPoints(List<LinePoint> from, List<AlignedLineDrawer.LinePoint> to)
	{
		to.Clear();
		foreach (LinePoint linePoint in from)
		{
			to.Add(new AlignedLineDrawer.LinePoint
			{
				LocalPosition = linePoint.localPosition,
				WorldNormal = linePoint.worldNormal
			});
		}
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x000997CC File Offset: 0x000979CC
	public static void CopyPoints(List<AlignedLineDrawer.LinePoint> from, List<AlignedLineDrawer.LinePoint> to)
	{
		to.Clear();
		foreach (AlignedLineDrawer.LinePoint item in from)
		{
			to.Add(item);
		}
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x00099820 File Offset: 0x00097A20
	public override void ResetState()
	{
		base.ResetState();
		this.editingPlayer.Set(null);
	}
}
