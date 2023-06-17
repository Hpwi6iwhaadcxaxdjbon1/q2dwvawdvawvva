using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000943 RID: 2371
public class MeshRendererBatch : MeshBatch
{
	// Token: 0x04003358 RID: 13144
	private Vector3 position;

	// Token: 0x04003359 RID: 13145
	private UnityEngine.Mesh meshBatch;

	// Token: 0x0400335A RID: 13146
	private MeshFilter meshFilter;

	// Token: 0x0400335B RID: 13147
	private MeshRenderer meshRenderer;

	// Token: 0x0400335C RID: 13148
	private MeshRendererData meshData;

	// Token: 0x0400335D RID: 13149
	private MeshRendererGroup meshGroup;

	// Token: 0x0400335E RID: 13150
	private MeshRendererLookup meshLookup;

	// Token: 0x17000482 RID: 1154
	// (get) Token: 0x060038DF RID: 14559 RVA: 0x00151687 File Offset: 0x0014F887
	public override int VertexCapacity
	{
		get
		{
			return Batching.renderer_capacity;
		}
	}

	// Token: 0x17000483 RID: 1155
	// (get) Token: 0x060038E0 RID: 14560 RVA: 0x0015168E File Offset: 0x0014F88E
	public override int VertexCutoff
	{
		get
		{
			return Batching.renderer_vertices;
		}
	}

	// Token: 0x060038E1 RID: 14561 RVA: 0x00152978 File Offset: 0x00150B78
	protected void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshData = new MeshRendererData();
		this.meshGroup = new MeshRendererGroup();
		this.meshLookup = new MeshRendererLookup();
	}

	// Token: 0x060038E2 RID: 14562 RVA: 0x001529B4 File Offset: 0x00150BB4
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

	// Token: 0x060038E3 RID: 14563 RVA: 0x00152A60 File Offset: 0x00150C60
	public void Add(MeshRendererInstance instance)
	{
		instance.position -= this.position;
		this.meshGroup.data.Add(instance);
		base.AddVertices(instance.mesh.vertexCount);
	}

	// Token: 0x060038E4 RID: 14564 RVA: 0x00152AAD File Offset: 0x00150CAD
	protected override void AllocMemory()
	{
		this.meshGroup.Alloc();
		this.meshData.Alloc();
	}

	// Token: 0x060038E5 RID: 14565 RVA: 0x00152AC5 File Offset: 0x00150CC5
	protected override void FreeMemory()
	{
		this.meshGroup.Free();
		this.meshData.Free();
	}

	// Token: 0x060038E6 RID: 14566 RVA: 0x00152ADD File Offset: 0x00150CDD
	protected override void RefreshMesh()
	{
		this.meshLookup.dst.Clear();
		this.meshData.Clear();
		this.meshData.Combine(this.meshGroup, this.meshLookup);
	}

	// Token: 0x060038E7 RID: 14567 RVA: 0x00152B14 File Offset: 0x00150D14
	protected override void ApplyMesh()
	{
		if (!this.meshBatch)
		{
			this.meshBatch = AssetPool.Get<UnityEngine.Mesh>();
		}
		this.meshLookup.Apply();
		this.meshData.Apply(this.meshBatch);
		this.meshBatch.UploadMeshData(false);
	}

	// Token: 0x060038E8 RID: 14568 RVA: 0x00152B64 File Offset: 0x00150D64
	protected override void ToggleMesh(bool state)
	{
		List<MeshRendererLookup.LookupEntry> data = this.meshLookup.src.data;
		for (int i = 0; i < data.Count; i++)
		{
			Renderer renderer = data[i].renderer;
			if (renderer)
			{
				renderer.enabled = !state;
			}
		}
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

	// Token: 0x060038E9 RID: 14569 RVA: 0x00152C20 File Offset: 0x00150E20
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
		this.meshLookup.src.Clear();
		this.meshLookup.dst.Clear();
	}
}
