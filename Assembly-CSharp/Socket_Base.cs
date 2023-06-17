using System;
using UnityEngine;

// Token: 0x0200027E RID: 638
public class Socket_Base : PrefabAttribute
{
	// Token: 0x0400158B RID: 5515
	public bool male = true;

	// Token: 0x0400158C RID: 5516
	public bool maleDummy;

	// Token: 0x0400158D RID: 5517
	public bool female;

	// Token: 0x0400158E RID: 5518
	public bool femaleDummy;

	// Token: 0x0400158F RID: 5519
	public bool femaleNoStability;

	// Token: 0x04001590 RID: 5520
	public bool monogamous;

	// Token: 0x04001591 RID: 5521
	[NonSerialized]
	public Vector3 position;

	// Token: 0x04001592 RID: 5522
	[NonSerialized]
	public Quaternion rotation;

	// Token: 0x04001593 RID: 5523
	private Type cachedType;

	// Token: 0x04001594 RID: 5524
	public Vector3 selectSize = new Vector3(2f, 0.1f, 2f);

	// Token: 0x04001595 RID: 5525
	public Vector3 selectCenter = new Vector3(0f, 0f, 1f);

	// Token: 0x04001596 RID: 5526
	[ReadOnly]
	public string socketName;

	// Token: 0x04001597 RID: 5527
	[NonSerialized]
	public SocketMod[] socketMods;

	// Token: 0x04001598 RID: 5528
	public Socket_Base.OccupiedSocketCheck[] checkOccupiedSockets;

	// Token: 0x06001CD6 RID: 7382 RVA: 0x000C7E90 File Offset: 0x000C6090
	public Socket_Base()
	{
		this.cachedType = base.GetType();
	}

	// Token: 0x06001CD7 RID: 7383 RVA: 0x000C4763 File Offset: 0x000C2963
	public Vector3 GetSelectPivot(Vector3 position, Quaternion rotation)
	{
		return position + rotation * this.worldPosition;
	}

	// Token: 0x06001CD8 RID: 7384 RVA: 0x000C7EEA File Offset: 0x000C60EA
	public OBB GetSelectBounds(Vector3 position, Quaternion rotation)
	{
		return new OBB(position + rotation * this.worldPosition, Vector3.one, rotation * this.worldRotation, new Bounds(this.selectCenter, this.selectSize));
	}

	// Token: 0x06001CD9 RID: 7385 RVA: 0x000C7F25 File Offset: 0x000C6125
	protected override Type GetIndexedType()
	{
		return typeof(Socket_Base);
	}

	// Token: 0x06001CDA RID: 7386 RVA: 0x000C7F34 File Offset: 0x000C6134
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.position = base.transform.position;
		this.rotation = base.transform.rotation;
		this.socketMods = base.GetComponentsInChildren<SocketMod>(true);
		SocketMod[] array = this.socketMods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].baseSocket = this;
		}
	}

	// Token: 0x06001CDB RID: 7387 RVA: 0x000C7F9B File Offset: 0x000C619B
	public virtual bool TestTarget(Construction.Target target)
	{
		return target.socket != null;
	}

	// Token: 0x06001CDC RID: 7388 RVA: 0x000C7FAC File Offset: 0x000C61AC
	public virtual bool IsCompatible(Socket_Base socket)
	{
		return !(socket == null) && (socket.male || this.male) && (socket.female || this.female) && socket.cachedType == this.cachedType;
	}

	// Token: 0x06001CDD RID: 7389 RVA: 0x000C7FF9 File Offset: 0x000C61F9
	public virtual bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		return this.IsCompatible(socket);
	}

	// Token: 0x06001CDE RID: 7390 RVA: 0x000C8004 File Offset: 0x000C6204
	public virtual Construction.Placement DoPlacement(Construction.Target target)
	{
		Quaternion quaternion = Quaternion.LookRotation(target.normal, Vector3.up) * Quaternion.Euler(target.rotation);
		Vector3 a = target.position;
		a -= quaternion * this.position;
		return new Construction.Placement
		{
			rotation = quaternion,
			position = a
		};
	}

	// Token: 0x06001CDF RID: 7391 RVA: 0x000C8060 File Offset: 0x000C6260
	public virtual bool CheckSocketMods(Construction.Placement placement)
	{
		SocketMod[] array = this.socketMods;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ModifyPlacement(placement);
		}
		foreach (SocketMod socketMod in this.socketMods)
		{
			if (!socketMod.DoCheck(placement))
			{
				if (socketMod.FailedPhrase.IsValid())
				{
					Construction.lastPlacementError = "Failed Check: (" + socketMod.FailedPhrase.translated + ")";
				}
				return false;
			}
		}
		return true;
	}

	// Token: 0x02000C8B RID: 3211
	[Serializable]
	public class OccupiedSocketCheck
	{
		// Token: 0x040043CE RID: 17358
		public Socket_Base Socket;

		// Token: 0x040043CF RID: 17359
		public bool FemaleDummy;
	}
}
