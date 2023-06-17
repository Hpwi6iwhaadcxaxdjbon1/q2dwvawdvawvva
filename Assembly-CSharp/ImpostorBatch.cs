using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000727 RID: 1831
public class ImpostorBatch
{
	// Token: 0x040029A8 RID: 10664
	public FPNativeList<Vector4> Positions;

	// Token: 0x040029AA RID: 10666
	private FPNativeList<uint> args;

	// Token: 0x040029AC RID: 10668
	private Queue<int> recycle = new Queue<int>(32);

	// Token: 0x17000439 RID: 1081
	// (get) Token: 0x06003339 RID: 13113 RVA: 0x0013AB77 File Offset: 0x00138D77
	// (set) Token: 0x06003338 RID: 13112 RVA: 0x0013AB6E File Offset: 0x00138D6E
	public Mesh Mesh { get; private set; }

	// Token: 0x1700043A RID: 1082
	// (get) Token: 0x0600333B RID: 13115 RVA: 0x0013AB88 File Offset: 0x00138D88
	// (set) Token: 0x0600333A RID: 13114 RVA: 0x0013AB7F File Offset: 0x00138D7F
	public Material Material { get; private set; }

	// Token: 0x1700043B RID: 1083
	// (get) Token: 0x0600333D RID: 13117 RVA: 0x0013AB99 File Offset: 0x00138D99
	// (set) Token: 0x0600333C RID: 13116 RVA: 0x0013AB90 File Offset: 0x00138D90
	public ComputeBuffer PositionBuffer { get; private set; }

	// Token: 0x1700043C RID: 1084
	// (get) Token: 0x0600333F RID: 13119 RVA: 0x0013ABAA File Offset: 0x00138DAA
	// (set) Token: 0x0600333E RID: 13118 RVA: 0x0013ABA1 File Offset: 0x00138DA1
	public ComputeBuffer ArgsBuffer { get; private set; }

	// Token: 0x1700043D RID: 1085
	// (get) Token: 0x06003340 RID: 13120 RVA: 0x0013ABB2 File Offset: 0x00138DB2
	// (set) Token: 0x06003341 RID: 13121 RVA: 0x0013ABBA File Offset: 0x00138DBA
	public bool IsDirty { get; set; }

	// Token: 0x1700043E RID: 1086
	// (get) Token: 0x06003342 RID: 13122 RVA: 0x0013ABC3 File Offset: 0x00138DC3
	public int Count
	{
		get
		{
			return this.Positions.Count;
		}
	}

	// Token: 0x1700043F RID: 1087
	// (get) Token: 0x06003343 RID: 13123 RVA: 0x0013ABD0 File Offset: 0x00138DD0
	public bool Visible
	{
		get
		{
			return this.Positions.Count - this.recycle.Count > 0;
		}
	}

	// Token: 0x06003344 RID: 13124 RVA: 0x0013ABEC File Offset: 0x00138DEC
	private ComputeBuffer SafeRelease(ComputeBuffer buffer)
	{
		if (buffer != null)
		{
			buffer.Release();
		}
		return null;
	}

	// Token: 0x06003345 RID: 13125 RVA: 0x0013ABF8 File Offset: 0x00138DF8
	public void Initialize(Mesh mesh, Material material)
	{
		this.Mesh = mesh;
		this.Material = material;
		this.Positions = Pool.Get<FPNativeList<Vector4>>();
		this.args = Pool.Get<FPNativeList<uint>>();
		this.args.Resize(5);
		this.ArgsBuffer = this.SafeRelease(this.ArgsBuffer);
		this.ArgsBuffer = new ComputeBuffer(1, this.args.Count * 4, ComputeBufferType.DrawIndirect);
		this.args[0] = this.Mesh.GetIndexCount(0);
		this.args[2] = this.Mesh.GetIndexStart(0);
		this.args[3] = this.Mesh.GetBaseVertex(0);
	}

	// Token: 0x06003346 RID: 13126 RVA: 0x0013ACB0 File Offset: 0x00138EB0
	public void Release()
	{
		this.recycle.Clear();
		Pool.Free<FPNativeList<Vector4>>(ref this.Positions);
		Pool.Free<FPNativeList<uint>>(ref this.args);
		this.PositionBuffer = this.SafeRelease(this.PositionBuffer);
		this.ArgsBuffer = this.SafeRelease(this.ArgsBuffer);
	}

	// Token: 0x06003347 RID: 13127 RVA: 0x0013AD04 File Offset: 0x00138F04
	public void AddInstance(ImpostorInstanceData data)
	{
		data.Batch = this;
		if (this.recycle.Count > 0)
		{
			data.BatchIndex = this.recycle.Dequeue();
			this.Positions[data.BatchIndex] = data.PositionAndScale();
		}
		else
		{
			data.BatchIndex = this.Positions.Count;
			this.Positions.Add(data.PositionAndScale());
		}
		this.IsDirty = true;
	}

	// Token: 0x06003348 RID: 13128 RVA: 0x0013AD7C File Offset: 0x00138F7C
	public void RemoveInstance(ImpostorInstanceData data)
	{
		this.Positions[data.BatchIndex] = new Vector4(0f, 0f, 0f, -1f);
		this.recycle.Enqueue(data.BatchIndex);
		data.BatchIndex = 0;
		data.Batch = null;
		this.IsDirty = true;
	}

	// Token: 0x06003349 RID: 13129 RVA: 0x0013ADDC File Offset: 0x00138FDC
	public void UpdateBuffers()
	{
		if (!this.IsDirty)
		{
			return;
		}
		bool flag = false;
		if (this.PositionBuffer == null || this.PositionBuffer.count != this.Positions.Count)
		{
			this.PositionBuffer = this.SafeRelease(this.PositionBuffer);
			this.PositionBuffer = new ComputeBuffer(this.Positions.Count, 16);
			flag = true;
		}
		if (this.PositionBuffer != null)
		{
			this.PositionBuffer.SetData<Vector4>(this.Positions.Array, 0, 0, this.Positions.Count);
		}
		if (this.ArgsBuffer != null && flag)
		{
			this.args[1] = (uint)this.Positions.Count;
			this.ArgsBuffer.SetData<uint>(this.args.Array, 0, 0, this.args.Count);
		}
		this.IsDirty = false;
	}
}
