using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000135 RID: 309
public class ElectricWindmill : global::IOEntity
{
	// Token: 0x04000F06 RID: 3846
	public Animator animator;

	// Token: 0x04000F07 RID: 3847
	public int maxPowerGeneration = 100;

	// Token: 0x04000F08 RID: 3848
	public Transform vaneRot;

	// Token: 0x04000F09 RID: 3849
	public SoundDefinition wooshSound;

	// Token: 0x04000F0A RID: 3850
	public Transform wooshOrigin;

	// Token: 0x04000F0B RID: 3851
	public float targetSpeed;

	// Token: 0x04000F0C RID: 3852
	private float serverWindSpeed;

	// Token: 0x060016BD RID: 5821 RVA: 0x000AECBA File Offset: 0x000ACEBA
	public override int MaximalPowerOutput()
	{
		return this.maxPowerGeneration;
	}

	// Token: 0x060016BE RID: 5822 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x060016BF RID: 5823 RVA: 0x000AECC4 File Offset: 0x000ACEC4
	public float GetWindSpeedScale()
	{
		float num = Time.time / 600f;
		float num2 = base.transform.position.x / 512f;
		float num3 = base.transform.position.z / 512f;
		float num4 = Mathf.PerlinNoise(num2 + num, num3 + num * 0.1f);
		float height = TerrainMeta.HeightMap.GetHeight(base.transform.position);
		float num5 = base.transform.position.y - height;
		if (num5 < 0f)
		{
			num5 = 0f;
		}
		return Mathf.Clamp01(Mathf.InverseLerp(0f, 50f, num5) * 0.5f + num4);
	}

	// Token: 0x060016C0 RID: 5824 RVA: 0x000AED73 File Offset: 0x000ACF73
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x060016C1 RID: 5825 RVA: 0x000AED7C File Offset: 0x000ACF7C
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.WindUpdate), 1f, 20f, 2f);
	}

	// Token: 0x060016C2 RID: 5826 RVA: 0x000AEDA8 File Offset: 0x000ACFA8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			if (info.msg.ioEntity == null)
			{
				info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
			}
			info.msg.ioEntity.genericFloat1 = Time.time;
			info.msg.ioEntity.genericFloat2 = this.serverWindSpeed;
		}
	}

	// Token: 0x060016C3 RID: 5827 RVA: 0x000AEE0C File Offset: 0x000AD00C
	public bool AmIVisible()
	{
		int num = 15;
		Vector3 a = base.transform.position + Vector3.up * 6f;
		if (!base.IsVisible(a + base.transform.up * (float)num, (float)(num + 1)))
		{
			return false;
		}
		Vector3 windAimDir = this.GetWindAimDir(Time.time);
		return base.IsVisible(a + windAimDir * (float)num, (float)(num + 1));
	}

	// Token: 0x060016C4 RID: 5828 RVA: 0x000AEE8C File Offset: 0x000AD08C
	public void WindUpdate()
	{
		this.serverWindSpeed = this.GetWindSpeedScale();
		if (!this.AmIVisible())
		{
			this.serverWindSpeed = 0f;
		}
		int num = Mathf.FloorToInt((float)this.maxPowerGeneration * this.serverWindSpeed);
		bool flag = this.currentEnergy != num;
		this.currentEnergy = num;
		if (flag)
		{
			this.MarkDirty();
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060016C5 RID: 5829 RVA: 0x0006647B File Offset: 0x0006467B
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		return this.currentEnergy;
	}

	// Token: 0x060016C6 RID: 5830 RVA: 0x000AEEF0 File Offset: 0x000AD0F0
	public Vector3 GetWindAimDir(float time)
	{
		float num = time / 3600f * 360f;
		int num2 = 10;
		Vector3 vector = new Vector3(Mathf.Sin(num * 0.017453292f) * (float)num2, 0f, Mathf.Cos(num * 0.017453292f) * (float)num2);
		return vector.normalized;
	}
}
