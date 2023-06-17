using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;

// Token: 0x0200017A RID: 378
public class PhotoEntity : ImageStorageEntity, IUGCBrowserEntity
{
	// Token: 0x170001FA RID: 506
	// (get) Token: 0x06001790 RID: 6032 RVA: 0x000B2C05 File Offset: 0x000B0E05
	// (set) Token: 0x06001791 RID: 6033 RVA: 0x000B2C0D File Offset: 0x000B0E0D
	public ulong PhotographerSteamId { get; private set; }

	// Token: 0x170001FB RID: 507
	// (get) Token: 0x06001792 RID: 6034 RVA: 0x000B2C16 File Offset: 0x000B0E16
	// (set) Token: 0x06001793 RID: 6035 RVA: 0x000B2C1E File Offset: 0x000B0E1E
	public uint ImageCrc { get; private set; }

	// Token: 0x170001FC RID: 508
	// (get) Token: 0x06001794 RID: 6036 RVA: 0x000B2C27 File Offset: 0x000B0E27
	protected override uint CrcToLoad
	{
		get
		{
			return this.ImageCrc;
		}
	}

	// Token: 0x06001795 RID: 6037 RVA: 0x000B2C30 File Offset: 0x000B0E30
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.photo != null)
		{
			this.PhotographerSteamId = info.msg.photo.photographerSteamId;
			this.ImageCrc = info.msg.photo.imageCrc;
		}
	}

	// Token: 0x06001796 RID: 6038 RVA: 0x000B2C80 File Offset: 0x000B0E80
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.photo = Pool.Get<Photo>();
		info.msg.photo.photographerSteamId = this.PhotographerSteamId;
		info.msg.photo.imageCrc = this.ImageCrc;
	}

	// Token: 0x06001797 RID: 6039 RVA: 0x000B2CD0 File Offset: 0x000B0ED0
	public void SetImageData(ulong steamId, byte[] data)
	{
		this.ImageCrc = FileStorage.server.Store(data, FileStorage.Type.jpg, this.net.ID, 0U);
		this.PhotographerSteamId = steamId;
	}

	// Token: 0x06001798 RID: 6040 RVA: 0x000820DC File Offset: 0x000802DC
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (!Rust.Application.isQuitting && this.net != null)
		{
			FileStorage.server.RemoveAllByEntity(this.net.ID);
		}
	}

	// Token: 0x170001FD RID: 509
	// (get) Token: 0x06001799 RID: 6041 RVA: 0x000B2CF7 File Offset: 0x000B0EF7
	public uint[] GetContentCRCs
	{
		get
		{
			if (this.ImageCrc <= 0U)
			{
				return Array.Empty<uint>();
			}
			return new uint[]
			{
				this.ImageCrc
			};
		}
	}

	// Token: 0x0600179A RID: 6042 RVA: 0x000B2D17 File Offset: 0x000B0F17
	public void ClearContent()
	{
		this.ImageCrc = 0U;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x0600179B RID: 6043 RVA: 0x00007A3C File Offset: 0x00005C3C
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImageJpg;
		}
	}

	// Token: 0x170001FF RID: 511
	// (get) Token: 0x0600179C RID: 6044 RVA: 0x000B2D27 File Offset: 0x000B0F27
	public List<ulong> EditingHistory
	{
		get
		{
			if (this.PhotographerSteamId <= 0UL)
			{
				return new List<ulong>();
			}
			return new List<ulong>
			{
				this.PhotographerSteamId
			};
		}
	}

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x0600179D RID: 6045 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}
}
