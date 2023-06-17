using System;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000934 RID: 2356
public class FoliageGridBatch : MeshBatch
{
	// Token: 0x04003346 RID: 13126
	private Vector3 position;

	// Token: 0x04003347 RID: 13127
	private UnityEngine.Mesh meshBatch;

	// Token: 0x04003348 RID: 13128
	private MeshFilter meshFilter;

	// Token: 0x04003349 RID: 13129
	private MeshRenderer meshRenderer;

	// Token: 0x0400334A RID: 13130
	private FoliageGridMeshData meshData;

	// Token: 0x0400334B RID: 13131
	private MeshGroup meshGroup;

	// Token: 0x17000477 RID: 1143
	// (get) Token: 0x0600388D RID: 14477 RVA: 0x00151687 File Offset: 0x0014F887
	public override int VertexCapacity
	{
		get
		{
			return Batching.renderer_capacity;
		}
	}

	// Token: 0x17000478 RID: 1144
	// (get) Token: 0x0600388E RID: 14478 RVA: 0x0015168E File Offset: 0x0014F88E
	public override int VertexCutoff
	{
		get
		{
			return Batching.renderer_vertices;
		}
	}

	// Token: 0x0600388F RID: 14479 RVA: 0x00151695 File Offset: 0x0014F895
	protected void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshData = new FoliageGridMeshData();
		this.meshGroup = new MeshGroup();
	}

	// Token: 0x06003890 RID: 14480 RVA: 0x001516C8 File Offset: 0x0014F8C8
	public void Setup(Vector3 position, Material material, ShadowCastingMode shadows, int layer)
	{
		base.transform.position = position;
		this.position = position;
		base.gameObject.layer = layer;
		this.meshRenderer.sharedMaterial = material;
		this.meshRenderer.shadowCastingMode = shadows;
		if (shadows == ShadowCastingMode.ShadowsOnly)
		{
			this.meshRenderer.receiveShadows = false;
			this.meshRenderer.motionVectors = false;
			this.meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			this.meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			return;
		}
		this.meshRenderer.receiveShadows = true;
		this.meshRenderer.motionVectors = true;
		this.meshRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
		this.meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
	}

	// Token: 0x06003891 RID: 14481 RVA: 0x00151774 File Offset: 0x0014F974
	public void Add(MeshInstance instance)
	{
		instance.position -= this.position;
		this.meshGroup.data.Add(instance);
		base.AddVertices(instance.mesh.vertexCount);
	}

	// Token: 0x06003892 RID: 14482 RVA: 0x001517C1 File Offset: 0x0014F9C1
	protected override void AllocMemory()
	{
		this.meshGroup.Alloc();
		this.meshData.Alloc();
	}

	// Token: 0x06003893 RID: 14483 RVA: 0x001517D9 File Offset: 0x0014F9D9
	protected override void FreeMemory()
	{
		this.meshGroup.Free();
		this.meshData.Free();
	}

	// Token: 0x06003894 RID: 14484 RVA: 0x001517F1 File Offset: 0x0014F9F1
	protected override void RefreshMesh()
	{
		this.meshData.Clear();
		this.meshData.Combine(this.meshGroup);
	}

	// Token: 0x06003895 RID: 14485 RVA: 0x0015180F File Offset: 0x0014FA0F
	protected override void ApplyMesh()
	{
		if (!this.meshBatch)
		{
			this.meshBatch = AssetPool.Get<UnityEngine.Mesh>();
		}
		this.meshData.Apply(this.meshBatch);
		this.meshBatch.UploadMeshData(false);
	}

	// Token: 0x06003896 RID: 14486 RVA: 0x00151848 File Offset: 0x0014FA48
	protected override void ToggleMesh(bool state)
	{
		if (state)
		{
			if (this.meshFilter)
			{
				this.meshFilter.sharedMesh = this.meshBatch;
			}
			if (this.meshRenderer)
			{
				this.meshRenderer.enabled = true;
				return;
			}
		}
		else
		{
			if (this.meshFilter)
			{
				this.meshFilter.sharedMesh = null;
			}
			if (this.meshRenderer)
			{
				this.meshRenderer.enabled = false;
			}
		}
	}

	// Token: 0x06003897 RID: 14487 RVA: 0x001518C4 File Offset: 0x0014FAC4
	protected override void OnPooled()
	{
		if (this.meshFilter)
		{
			this.meshFilter.sharedMesh = null;
		}
		if (this.meshBatch)
		{
			AssetPool.Free(ref this.meshBatch);
		}
		this.meshData.Free();
		this.meshGroup.Free();
	}
}
