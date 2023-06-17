using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020004DA RID: 1242
public class SlidingProgressDoor : ProgressDoor
{
	// Token: 0x04002072 RID: 8306
	public Vector3 openPosition;

	// Token: 0x04002073 RID: 8307
	public Vector3 closedPosition;

	// Token: 0x04002074 RID: 8308
	public GameObject doorObject;

	// Token: 0x04002075 RID: 8309
	public TriggerVehiclePush vehiclePhysBox;

	// Token: 0x04002076 RID: 8310
	private float lastEnergyTime;

	// Token: 0x04002077 RID: 8311
	private float lastServerUpdateTime;

	// Token: 0x0600283B RID: 10299 RVA: 0x000F8C24 File Offset: 0x000F6E24
	public override void Spawn()
	{
		base.Spawn();
		base.InvokeRepeating(new Action(this.ServerUpdate), 0f, 0.1f);
		if (this.vehiclePhysBox != null)
		{
			this.vehiclePhysBox.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600283C RID: 10300 RVA: 0x000F8C72 File Offset: 0x000F6E72
	public override void NoEnergy()
	{
		base.NoEnergy();
	}

	// Token: 0x0600283D RID: 10301 RVA: 0x000F8C7A File Offset: 0x000F6E7A
	public override void AddEnergy(float amount)
	{
		this.lastEnergyTime = Time.time;
		base.AddEnergy(amount);
	}

	// Token: 0x0600283E RID: 10302 RVA: 0x000F8C90 File Offset: 0x000F6E90
	public void ServerUpdate()
	{
		if (base.isServer)
		{
			if (this.lastServerUpdateTime == 0f)
			{
				this.lastServerUpdateTime = Time.realtimeSinceStartup;
			}
			float num = Time.realtimeSinceStartup - this.lastServerUpdateTime;
			this.lastServerUpdateTime = Time.realtimeSinceStartup;
			if (Time.time > this.lastEnergyTime + 0.333f)
			{
				float b = this.energyForOpen * num / this.secondsToClose;
				float num2 = Mathf.Min(this.storedEnergy, b);
				if (this.vehiclePhysBox != null)
				{
					this.vehiclePhysBox.gameObject.SetActive(num2 > 0f && this.storedEnergy > 0f);
					if (this.vehiclePhysBox.gameObject.activeSelf && this.vehiclePhysBox.ContentsCount > 0)
					{
						num2 = 0f;
					}
				}
				this.storedEnergy -= num2;
				this.storedEnergy = Mathf.Clamp(this.storedEnergy, 0f, this.energyForOpen);
				if (num2 > 0f)
				{
					foreach (global::IOEntity.IOSlot ioslot in this.outputs)
					{
						if (ioslot.connectedTo.Get(true) != null)
						{
							ioslot.connectedTo.Get(true).IOInput(this, this.ioType, -num2, ioslot.connectedToSlot);
						}
					}
				}
			}
			this.UpdateProgress();
		}
	}

	// Token: 0x0600283F RID: 10303 RVA: 0x000F8DF8 File Offset: 0x000F6FF8
	public override void UpdateProgress()
	{
		Vector3 localPosition = this.doorObject.transform.localPosition;
		float t = this.storedEnergy / this.energyForOpen;
		Vector3 vector = Vector3.Lerp(this.closedPosition, this.openPosition, t);
		this.doorObject.transform.localPosition = vector;
		if (base.isServer)
		{
			bool flag = Vector3.Distance(localPosition, vector) > 0.01f;
			base.SetFlag(global::BaseEntity.Flags.Reserved1, flag, false, true);
			if (flag)
			{
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
	}

	// Token: 0x06002840 RID: 10304 RVA: 0x000F8E77 File Offset: 0x000F7077
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		ProtoBuf.SphereEntity sphereEntity = info.msg.sphereEntity;
	}

	// Token: 0x06002841 RID: 10305 RVA: 0x000F8E8C File Offset: 0x000F708C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Pool.Get<ProtoBuf.SphereEntity>();
		info.msg.sphereEntity.radius = this.storedEnergy;
	}
}
