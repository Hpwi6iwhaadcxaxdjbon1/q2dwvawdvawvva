using System;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000942 RID: 2370
public class MeshDataBatch : MeshBatch
{
	// Token: 0x04003352 RID: 13138
	private Vector3 position;

	// Token: 0x04003353 RID: 13139
	private UnityEngine.Mesh meshBatch;

	// Token: 0x04003354 RID: 13140
	private MeshFilter meshFilter;

	// Token: 0x04003355 RID: 13141
	private MeshRenderer meshRenderer;

	// Token: 0x04003356 RID: 13142
	private MeshData meshData;

	// Token: 0x04003357 RID: 13143
	private MeshGroup meshGroup;

	// Token: 0x17000480 RID: 1152
	// (get) Token: 0x060038D3 RID: 14547 RVA: 0x00151687 File Offset: 0x0014F887
	public override int VertexCapacity
	{
		get
		{
			return Batching.renderer_capacity;
		}
	}

	// Token: 0x17000481 RID: 1153
	// (get) Token: 0x060038D4 RID: 14548 RVA: 0x0015168E File Offset: 0x0014F88E
	public override int VertexCutoff
	{
		get
		{
			return Batching.renderer_vertices;
		}
	}

	// Token: 0x060038D5 RID: 14549 RVA: 0x001526F8 File Offset: 0x001508F8
	protected void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshData = new MeshData();
		this.meshGroup = new MeshGroup();
	}

	// Token: 0x060038D6 RID: 14550 RVA: 0x00152728 File Offset: 0x00150928
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

	// Token: 0x060038D7 RID: 14551 RVA: 0x001527D4 File Offset: 0x001509D4
	public void Add(MeshInstance instance)
	{
		instance.position -= this.position;
		this.meshGroup.data.Add(instance);
		base.AddVertices(instance.mesh.vertexCount);
	}

	// Token: 0x060038D8 RID: 14552 RVA: 0x00152821 File Offset: 0x00150A21
	protected override void AllocMemory()
	{
		this.meshGroup.Alloc();
		this.meshData.Alloc();
	}

	// Token: 0x060038D9 RID: 14553 RVA: 0x00152839 File Offset: 0x00150A39
	protected override void FreeMemory()
	{
		this.meshGroup.Free();
		this.meshData.Free();
	}

	// Token: 0x060038DA RID: 14554 RVA: 0x00152851 File Offset: 0x00150A51
	protected override void RefreshMesh()
	{
		this.meshData.Clear();
		this.meshData.Combine(this.meshGroup);
	}

	// Token: 0x060038DB RID: 14555 RVA: 0x0015286F File Offset: 0x00150A6F
	protected override void ApplyMesh()
	{
		if (!this.meshBatch)
		{
			this.meshBatch = AssetPool.Get<UnityEngine.Mesh>();
		}
		this.meshData.Apply(this.meshBatch);
		this.meshBatch.UploadMeshData(false);
	}

	// Token: 0x060038DC RID: 14556 RVA: 0x001528A8 File Offset: 0x00150AA8
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

	// Token: 0x060038DD RID: 14557 RVA: 0x00152924 File Offset: 0x00150B24
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
