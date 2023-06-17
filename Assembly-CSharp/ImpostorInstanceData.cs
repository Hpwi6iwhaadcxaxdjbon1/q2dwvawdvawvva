using System;
using UnityEngine;

// Token: 0x02000726 RID: 1830
public class ImpostorInstanceData
{
	// Token: 0x040029A1 RID: 10657
	public ImpostorBatch Batch;

	// Token: 0x040029A2 RID: 10658
	public int BatchIndex;

	// Token: 0x040029A3 RID: 10659
	private int hash;

	// Token: 0x040029A4 RID: 10660
	private Vector4 positionAndScale = Vector4.zero;

	// Token: 0x17000436 RID: 1078
	// (get) Token: 0x0600332C RID: 13100 RVA: 0x0013A999 File Offset: 0x00138B99
	// (set) Token: 0x0600332B RID: 13099 RVA: 0x0013A990 File Offset: 0x00138B90
	public Renderer Renderer { get; private set; }

	// Token: 0x17000437 RID: 1079
	// (get) Token: 0x0600332E RID: 13102 RVA: 0x0013A9AA File Offset: 0x00138BAA
	// (set) Token: 0x0600332D RID: 13101 RVA: 0x0013A9A1 File Offset: 0x00138BA1
	public Mesh Mesh { get; private set; }

	// Token: 0x17000438 RID: 1080
	// (get) Token: 0x06003330 RID: 13104 RVA: 0x0013A9BB File Offset: 0x00138BBB
	// (set) Token: 0x0600332F RID: 13103 RVA: 0x0013A9B2 File Offset: 0x00138BB2
	public Material Material { get; private set; }

	// Token: 0x06003331 RID: 13105 RVA: 0x0013A9C3 File Offset: 0x00138BC3
	public ImpostorInstanceData(Renderer renderer, Mesh mesh, Material material)
	{
		this.Renderer = renderer;
		this.Mesh = mesh;
		this.Material = material;
		this.hash = this.GenerateHashCode();
		this.Update();
	}

	// Token: 0x06003332 RID: 13106 RVA: 0x0013AA00 File Offset: 0x00138C00
	public ImpostorInstanceData(Vector3 position, Vector3 scale, Mesh mesh, Material material)
	{
		this.positionAndScale = new Vector4(position.x, position.y, position.z, scale.x);
		this.Mesh = mesh;
		this.Material = material;
		this.hash = this.GenerateHashCode();
		this.Update();
	}

	// Token: 0x06003333 RID: 13107 RVA: 0x0013AA62 File Offset: 0x00138C62
	private int GenerateHashCode()
	{
		return (17 * 31 + this.Material.GetHashCode()) * 31 + this.Mesh.GetHashCode();
	}

	// Token: 0x06003334 RID: 13108 RVA: 0x0013AA84 File Offset: 0x00138C84
	public override bool Equals(object obj)
	{
		ImpostorInstanceData impostorInstanceData = obj as ImpostorInstanceData;
		return impostorInstanceData.Material == this.Material && impostorInstanceData.Mesh == this.Mesh;
	}

	// Token: 0x06003335 RID: 13109 RVA: 0x0013AABE File Offset: 0x00138CBE
	public override int GetHashCode()
	{
		return this.hash;
	}

	// Token: 0x06003336 RID: 13110 RVA: 0x0013AAC8 File Offset: 0x00138CC8
	public Vector4 PositionAndScale()
	{
		if (this.Renderer != null)
		{
			Transform transform = this.Renderer.transform;
			Vector3 position = transform.position;
			Vector3 lossyScale = transform.lossyScale;
			float w = this.Renderer.enabled ? lossyScale.x : (-lossyScale.x);
			this.positionAndScale = new Vector4(position.x, position.y, position.z, w);
		}
		return this.positionAndScale;
	}

	// Token: 0x06003337 RID: 13111 RVA: 0x0013AB3C File Offset: 0x00138D3C
	public void Update()
	{
		if (this.Batch != null)
		{
			this.Batch.Positions[this.BatchIndex] = this.PositionAndScale();
			this.Batch.IsDirty = true;
		}
	}
}
