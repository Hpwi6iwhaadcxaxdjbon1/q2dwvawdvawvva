using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020007F1 RID: 2033
public class MapLayerRenderer : SingletonComponent<MapLayerRenderer>
{
	// Token: 0x04002D70 RID: 11632
	private NetworkableId? _currentlyRenderedDungeon;

	// Token: 0x04002D71 RID: 11633
	private int? _underwaterLabFloorCount;

	// Token: 0x04002D72 RID: 11634
	public Camera renderCamera;

	// Token: 0x04002D73 RID: 11635
	public CameraEvent cameraEvent;

	// Token: 0x04002D74 RID: 11636
	public Material renderMaterial;

	// Token: 0x04002D75 RID: 11637
	private MapLayer? _currentlyRenderedLayer;

	// Token: 0x0600358F RID: 13711 RVA: 0x001473F8 File Offset: 0x001455F8
	private void RenderDungeonsLayer()
	{
		ProceduralDynamicDungeon proceduralDynamicDungeon = MapLayerRenderer.FindDungeon(MainCamera.isValid ? MainCamera.position : Vector3.zero, 200f);
		MapLayer? currentlyRenderedLayer = this._currentlyRenderedLayer;
		MapLayer mapLayer = MapLayer.Dungeons;
		if (currentlyRenderedLayer.GetValueOrDefault() == mapLayer & currentlyRenderedLayer != null)
		{
			NetworkableId? currentlyRenderedDungeon = this._currentlyRenderedDungeon;
			NetworkableId? networkableId;
			if (proceduralDynamicDungeon == null)
			{
				networkableId = null;
			}
			else
			{
				Networkable net = proceduralDynamicDungeon.net;
				networkableId = ((net != null) ? new NetworkableId?(net.ID) : null);
			}
			if (currentlyRenderedDungeon == networkableId)
			{
				return;
			}
		}
		this._currentlyRenderedLayer = new MapLayer?(MapLayer.Dungeons);
		NetworkableId? currentlyRenderedDungeon2;
		if (proceduralDynamicDungeon == null)
		{
			currentlyRenderedDungeon2 = null;
		}
		else
		{
			Networkable net2 = proceduralDynamicDungeon.net;
			currentlyRenderedDungeon2 = ((net2 != null) ? new NetworkableId?(net2.ID) : null);
		}
		this._currentlyRenderedDungeon = currentlyRenderedDungeon2;
		using (CommandBuffer commandBuffer = this.BuildCommandBufferDungeons(proceduralDynamicDungeon))
		{
			this.RenderImpl(commandBuffer);
		}
	}

	// Token: 0x06003590 RID: 13712 RVA: 0x00147520 File Offset: 0x00145720
	private CommandBuffer BuildCommandBufferDungeons(ProceduralDynamicDungeon closest)
	{
		CommandBuffer commandBuffer = new CommandBuffer
		{
			name = "DungeonsLayer Render"
		};
		if (closest != null && closest.spawnedCells != null)
		{
			Matrix4x4 lhs = Matrix4x4.Translate(closest.mapOffset);
			foreach (ProceduralDungeonCell proceduralDungeonCell in closest.spawnedCells)
			{
				if (!(proceduralDungeonCell == null) && proceduralDungeonCell.mapRenderers != null && proceduralDungeonCell.mapRenderers.Length != 0)
				{
					foreach (MeshRenderer meshRenderer in proceduralDungeonCell.mapRenderers)
					{
						MeshFilter meshFilter;
						if (!(meshRenderer == null) && meshRenderer.TryGetComponent<MeshFilter>(out meshFilter))
						{
							Mesh sharedMesh = meshFilter.sharedMesh;
							int subMeshCount = sharedMesh.subMeshCount;
							Matrix4x4 matrix = lhs * meshRenderer.transform.localToWorldMatrix;
							for (int j = 0; j < subMeshCount; j++)
							{
								commandBuffer.DrawMesh(sharedMesh, matrix, this.renderMaterial, j);
							}
						}
					}
				}
			}
		}
		return commandBuffer;
	}

	// Token: 0x06003591 RID: 13713 RVA: 0x0014764C File Offset: 0x0014584C
	public static ProceduralDynamicDungeon FindDungeon(Vector3 position, float maxDist = 200f)
	{
		ProceduralDynamicDungeon result = null;
		float num = 100000f;
		foreach (ProceduralDynamicDungeon proceduralDynamicDungeon in ProceduralDynamicDungeon.dungeons)
		{
			if (!(proceduralDynamicDungeon == null) && proceduralDynamicDungeon.isClient)
			{
				float num2 = Vector3.Distance(position, proceduralDynamicDungeon.transform.position);
				if (num2 <= maxDist && num2 <= num)
				{
					result = proceduralDynamicDungeon;
					num = num2;
				}
			}
		}
		return result;
	}

	// Token: 0x06003592 RID: 13714 RVA: 0x001476D4 File Offset: 0x001458D4
	private void RenderTrainLayer()
	{
		using (CommandBuffer commandBuffer = this.BuildCommandBufferTrainTunnels())
		{
			this.RenderImpl(commandBuffer);
		}
	}

	// Token: 0x06003593 RID: 13715 RVA: 0x0014770C File Offset: 0x0014590C
	private CommandBuffer BuildCommandBufferTrainTunnels()
	{
		CommandBuffer commandBuffer = new CommandBuffer
		{
			name = "TrainLayer Render"
		};
		foreach (DungeonGridCell dungeonGridCell in TerrainMeta.Path.DungeonGridCells)
		{
			if (dungeonGridCell.MapRenderers != null && dungeonGridCell.MapRenderers.Length != 0)
			{
				foreach (MeshRenderer meshRenderer in dungeonGridCell.MapRenderers)
				{
					MeshFilter meshFilter;
					if (!(meshRenderer == null) && meshRenderer.TryGetComponent<MeshFilter>(out meshFilter))
					{
						Mesh sharedMesh = meshFilter.sharedMesh;
						int subMeshCount = sharedMesh.subMeshCount;
						Matrix4x4 localToWorldMatrix = meshRenderer.transform.localToWorldMatrix;
						for (int j = 0; j < subMeshCount; j++)
						{
							commandBuffer.DrawMesh(sharedMesh, localToWorldMatrix, this.renderMaterial, j);
						}
					}
				}
			}
		}
		return commandBuffer;
	}

	// Token: 0x06003594 RID: 13716 RVA: 0x00147800 File Offset: 0x00145A00
	private void RenderUnderwaterLabs(int floor)
	{
		using (CommandBuffer commandBuffer = this.BuildCommandBufferUnderwaterLabs(floor))
		{
			this.RenderImpl(commandBuffer);
		}
	}

	// Token: 0x06003595 RID: 13717 RVA: 0x00147838 File Offset: 0x00145A38
	public int GetUnderwaterLabFloorCount()
	{
		if (this._underwaterLabFloorCount != null)
		{
			return this._underwaterLabFloorCount.Value;
		}
		List<DungeonBaseInfo> dungeonBaseEntrances = TerrainMeta.Path.DungeonBaseEntrances;
		int value;
		if (dungeonBaseEntrances == null || dungeonBaseEntrances.Count <= 0)
		{
			value = 0;
		}
		else
		{
			value = dungeonBaseEntrances.Max((DungeonBaseInfo l) => l.Floors.Count);
		}
		this._underwaterLabFloorCount = new int?(value);
		return this._underwaterLabFloorCount.Value;
	}

	// Token: 0x06003596 RID: 13718 RVA: 0x001478B4 File Offset: 0x00145AB4
	private CommandBuffer BuildCommandBufferUnderwaterLabs(int floor)
	{
		CommandBuffer commandBuffer = new CommandBuffer
		{
			name = "UnderwaterLabLayer Render"
		};
		foreach (DungeonBaseInfo dungeonBaseInfo in TerrainMeta.Path.DungeonBaseEntrances)
		{
			if (dungeonBaseInfo.Floors.Count > floor)
			{
				foreach (DungeonBaseLink dungeonBaseLink in dungeonBaseInfo.Floors[floor].Links)
				{
					if (dungeonBaseLink.MapRenderers != null && dungeonBaseLink.MapRenderers.Length != 0)
					{
						foreach (MeshRenderer meshRenderer in dungeonBaseLink.MapRenderers)
						{
							MeshFilter meshFilter;
							if (!(meshRenderer == null) && meshRenderer.TryGetComponent<MeshFilter>(out meshFilter))
							{
								Mesh sharedMesh = meshFilter.sharedMesh;
								int subMeshCount = sharedMesh.subMeshCount;
								Matrix4x4 localToWorldMatrix = meshRenderer.transform.localToWorldMatrix;
								for (int j = 0; j < subMeshCount; j++)
								{
									commandBuffer.DrawMesh(sharedMesh, localToWorldMatrix, this.renderMaterial, j);
								}
							}
						}
					}
				}
			}
		}
		return commandBuffer;
	}

	// Token: 0x06003597 RID: 13719 RVA: 0x00147A0C File Offset: 0x00145C0C
	public void Render(MapLayer layer)
	{
		if (layer < MapLayer.TrainTunnels)
		{
			return;
		}
		if (layer == MapLayer.Dungeons)
		{
			this.RenderDungeonsLayer();
			return;
		}
		MapLayer? currentlyRenderedLayer = this._currentlyRenderedLayer;
		if (layer == currentlyRenderedLayer.GetValueOrDefault() & currentlyRenderedLayer != null)
		{
			return;
		}
		this._currentlyRenderedLayer = new MapLayer?(layer);
		if (layer == MapLayer.TrainTunnels)
		{
			this.RenderTrainLayer();
			return;
		}
		if (layer >= MapLayer.Underwater1 && layer <= MapLayer.Underwater8)
		{
			this.RenderUnderwaterLabs(layer - MapLayer.Underwater1);
		}
	}

	// Token: 0x06003598 RID: 13720 RVA: 0x00147A70 File Offset: 0x00145C70
	private void RenderImpl(CommandBuffer cb)
	{
		double num = World.Size * 1.5;
		this.renderCamera.orthographicSize = (float)num / 2f;
		this.renderCamera.RemoveAllCommandBuffers();
		this.renderCamera.AddCommandBuffer(this.cameraEvent, cb);
		this.renderCamera.Render();
		this.renderCamera.RemoveAllCommandBuffers();
	}

	// Token: 0x06003599 RID: 13721 RVA: 0x00147AD5 File Offset: 0x00145CD5
	public static MapLayerRenderer GetOrCreate()
	{
		if (SingletonComponent<MapLayerRenderer>.Instance != null)
		{
			return SingletonComponent<MapLayerRenderer>.Instance;
		}
		return GameManager.server.CreatePrefab("assets/prefabs/engine/maplayerrenderer.prefab", Vector3.zero, Quaternion.identity, true).GetComponent<MapLayerRenderer>();
	}
}
