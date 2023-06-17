using System;
using Rust;
using UnityEngine;

// Token: 0x02000941 RID: 2369
public abstract class MeshBatch : MonoBehaviour
{
	// Token: 0x17000479 RID: 1145
	// (get) Token: 0x060038B8 RID: 14520 RVA: 0x001525CD File Offset: 0x001507CD
	// (set) Token: 0x060038B9 RID: 14521 RVA: 0x001525D5 File Offset: 0x001507D5
	public bool NeedsRefresh { get; private set; }

	// Token: 0x1700047A RID: 1146
	// (get) Token: 0x060038BA RID: 14522 RVA: 0x001525DE File Offset: 0x001507DE
	// (set) Token: 0x060038BB RID: 14523 RVA: 0x001525E6 File Offset: 0x001507E6
	public int Count { get; private set; }

	// Token: 0x1700047B RID: 1147
	// (get) Token: 0x060038BC RID: 14524 RVA: 0x001525EF File Offset: 0x001507EF
	// (set) Token: 0x060038BD RID: 14525 RVA: 0x001525F7 File Offset: 0x001507F7
	public int BatchedCount { get; private set; }

	// Token: 0x1700047C RID: 1148
	// (get) Token: 0x060038BE RID: 14526 RVA: 0x00152600 File Offset: 0x00150800
	// (set) Token: 0x060038BF RID: 14527 RVA: 0x00152608 File Offset: 0x00150808
	public int VertexCount { get; private set; }

	// Token: 0x060038C0 RID: 14528
	protected abstract void AllocMemory();

	// Token: 0x060038C1 RID: 14529
	protected abstract void FreeMemory();

	// Token: 0x060038C2 RID: 14530
	protected abstract void RefreshMesh();

	// Token: 0x060038C3 RID: 14531
	protected abstract void ApplyMesh();

	// Token: 0x060038C4 RID: 14532
	protected abstract void ToggleMesh(bool state);

	// Token: 0x060038C5 RID: 14533
	protected abstract void OnPooled();

	// Token: 0x1700047D RID: 1149
	// (get) Token: 0x060038C6 RID: 14534
	public abstract int VertexCapacity { get; }

	// Token: 0x1700047E RID: 1150
	// (get) Token: 0x060038C7 RID: 14535
	public abstract int VertexCutoff { get; }

	// Token: 0x1700047F RID: 1151
	// (get) Token: 0x060038C8 RID: 14536 RVA: 0x00152611 File Offset: 0x00150811
	public int AvailableVertices
	{
		get
		{
			return Mathf.Clamp(this.VertexCapacity, this.VertexCutoff, 65534) - this.VertexCount;
		}
	}

	// Token: 0x060038C9 RID: 14537 RVA: 0x00152630 File Offset: 0x00150830
	public void Alloc()
	{
		this.AllocMemory();
	}

	// Token: 0x060038CA RID: 14538 RVA: 0x00152638 File Offset: 0x00150838
	public void Free()
	{
		this.FreeMemory();
	}

	// Token: 0x060038CB RID: 14539 RVA: 0x00152640 File Offset: 0x00150840
	public void Refresh()
	{
		this.RefreshMesh();
	}

	// Token: 0x060038CC RID: 14540 RVA: 0x00152648 File Offset: 0x00150848
	public void Apply()
	{
		this.NeedsRefresh = false;
		this.ApplyMesh();
	}

	// Token: 0x060038CD RID: 14541 RVA: 0x00152657 File Offset: 0x00150857
	public void Display()
	{
		this.ToggleMesh(true);
		this.BatchedCount = this.Count;
	}

	// Token: 0x060038CE RID: 14542 RVA: 0x0015266C File Offset: 0x0015086C
	public void Invalidate()
	{
		this.ToggleMesh(false);
		this.BatchedCount = 0;
	}

	// Token: 0x060038CF RID: 14543 RVA: 0x0015267C File Offset: 0x0015087C
	protected void AddVertices(int vertices)
	{
		this.NeedsRefresh = true;
		int count = this.Count;
		this.Count = count + 1;
		this.VertexCount += vertices;
	}

	// Token: 0x060038D0 RID: 14544 RVA: 0x001526AE File Offset: 0x001508AE
	protected void OnEnable()
	{
		this.NeedsRefresh = false;
		this.Count = 0;
		this.BatchedCount = 0;
		this.VertexCount = 0;
	}

	// Token: 0x060038D1 RID: 14545 RVA: 0x001526CC File Offset: 0x001508CC
	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.NeedsRefresh = false;
		this.Count = 0;
		this.BatchedCount = 0;
		this.VertexCount = 0;
		this.OnPooled();
	}
}
