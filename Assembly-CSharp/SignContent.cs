using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;

// Token: 0x020003D9 RID: 985
public class SignContent : ImageStorageEntity, IUGCBrowserEntity
{
	// Token: 0x04001A45 RID: 6725
	private uint[] textureIDs = new uint[1];

	// Token: 0x04001A46 RID: 6726
	private List<ulong> editHistory = new List<ulong>();

	// Token: 0x170002CF RID: 719
	// (get) Token: 0x060021DC RID: 8668 RVA: 0x000DC1CE File Offset: 0x000DA3CE
	protected override uint CrcToLoad
	{
		get
		{
			return this.textureIDs[0];
		}
	}

	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x060021DD RID: 8669 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected override FileStorage.Type StorageType
	{
		get
		{
			return FileStorage.Type.png;
		}
	}

	// Token: 0x170002D1 RID: 721
	// (get) Token: 0x060021DE RID: 8670 RVA: 0x0000441C File Offset: 0x0000261C
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImagePng;
		}
	}

	// Token: 0x060021DF RID: 8671 RVA: 0x000DC1D8 File Offset: 0x000DA3D8
	public void CopyInfoFromSign(ISignage s, IUGCBrowserEntity b)
	{
		uint[] textureCRCs = s.GetTextureCRCs();
		this.textureIDs = new uint[textureCRCs.Length];
		textureCRCs.CopyTo(this.textureIDs, 0);
		this.editHistory.Clear();
		foreach (ulong item in b.EditingHistory)
		{
			this.editHistory.Add(item);
		}
		FileStorage.server.ReassignEntityId(s.NetworkID, this.net.ID);
	}

	// Token: 0x060021E0 RID: 8672 RVA: 0x000DC278 File Offset: 0x000DA478
	public void CopyInfoToSign(ISignage s, IUGCBrowserEntity b)
	{
		FileStorage.server.ReassignEntityId(this.net.ID, s.NetworkID);
		s.SetTextureCRCs(this.textureIDs);
		b.EditingHistory.Clear();
		foreach (ulong item in this.editHistory)
		{
			b.EditingHistory.Add(item);
		}
	}

	// Token: 0x060021E1 RID: 8673 RVA: 0x000DC304 File Offset: 0x000DA504
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.paintableSign == null)
		{
			info.msg.paintableSign = Pool.Get<PaintableSign>();
		}
		info.msg.paintableSign.crcs = Pool.GetList<uint>();
		foreach (uint item in this.textureIDs)
		{
			info.msg.paintableSign.crcs.Add(item);
		}
	}

	// Token: 0x060021E2 RID: 8674 RVA: 0x000DC379 File Offset: 0x000DA579
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		FileStorage.server.RemoveAllByEntity(this.net.ID);
	}

	// Token: 0x060021E3 RID: 8675 RVA: 0x000DC398 File Offset: 0x000DA598
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.paintableSign != null)
		{
			this.textureIDs = new uint[info.msg.paintableSign.crcs.Count];
			for (int i = 0; i < info.msg.paintableSign.crcs.Count; i++)
			{
				this.textureIDs[i] = info.msg.paintableSign.crcs[i];
			}
		}
	}

	// Token: 0x170002D2 RID: 722
	// (get) Token: 0x060021E4 RID: 8676 RVA: 0x000DC417 File Offset: 0x000DA617
	public uint[] GetContentCRCs
	{
		get
		{
			return this.textureIDs;
		}
	}

	// Token: 0x060021E5 RID: 8677 RVA: 0x00003384 File Offset: 0x00001584
	public void ClearContent()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x170002D3 RID: 723
	// (get) Token: 0x060021E6 RID: 8678 RVA: 0x000DC41F File Offset: 0x000DA61F
	public FileStorage.Type FileType
	{
		get
		{
			return this.StorageType;
		}
	}

	// Token: 0x170002D4 RID: 724
	// (get) Token: 0x060021E7 RID: 8679 RVA: 0x000DC427 File Offset: 0x000DA627
	public List<ulong> EditingHistory
	{
		get
		{
			return this.editHistory;
		}
	}

	// Token: 0x170002D5 RID: 725
	// (get) Token: 0x060021E8 RID: 8680 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}
}
