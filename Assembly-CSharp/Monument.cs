﻿using System;
using UnityEngine;

// Token: 0x020006F4 RID: 1780
public class Monument : TerrainPlacement
{
	// Token: 0x040028D9 RID: 10457
	public float Radius;

	// Token: 0x040028DA RID: 10458
	public float Fade = 10f;

	// Token: 0x06003225 RID: 12837 RVA: 0x00134E30 File Offset: 0x00133030
	protected void OnDrawGizmosSelected()
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		GizmosUtil.DrawWireCircleY(base.transform.position, this.Radius);
		GizmosUtil.DrawWireCircleY(base.transform.position, this.Radius - this.Fade);
	}

	// Token: 0x06003226 RID: 12838 RVA: 0x00134EAC File Offset: 0x001330AC
	protected override void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		bool useBlendMap = this.blendmap.isValid;
		Vector3 position = localToWorld.MultiplyPoint3x4(Vector3.zero);
		TextureData heightdata = new TextureData(this.heightmap.Get());
		TextureData blenddata = new TextureData(useBlendMap ? this.blendmap.Get() : null);
		float num = useBlendMap ? this.extents.x : this.Radius;
		float num2 = useBlendMap ? this.extents.z : this.Radius;
		Vector3 v = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-num, 0f, -num2));
		Vector3 v2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(num, 0f, -num2));
		Vector3 v3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-num, 0f, num2));
		Vector3 v4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(num, 0f, num2));
		TerrainMeta.HeightMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			float normZ = TerrainMeta.HeightMap.Coordinate(z);
			float normX = TerrainMeta.HeightMap.Coordinate(x);
			Vector3 point = new Vector3(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 vector = worldToLocal.MultiplyPoint3x4(point) - this.offset;
			float num3;
			if (useBlendMap)
			{
				num3 = blenddata.GetInterpolatedVector((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z).w;
			}
			else
			{
				num3 = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade, vector.Magnitude2D());
			}
			if (num3 == 0f)
			{
				return;
			}
			float num4 = TerrainMeta.NormalizeY(position.y + this.offset.y + heightdata.GetInterpolatedHalf((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z) * this.size.y);
			num4 = Mathf.SmoothStep(TerrainMeta.HeightMap.GetHeight01(x, z), num4, num3);
			TerrainMeta.HeightMap.SetHeight(x, z, num4);
		});
	}

	// Token: 0x06003227 RID: 12839 RVA: 0x00135020 File Offset: 0x00133220
	protected override void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		bool should0 = base.ShouldSplat(1);
		bool should1 = base.ShouldSplat(2);
		bool should2 = base.ShouldSplat(4);
		bool should3 = base.ShouldSplat(8);
		bool should4 = base.ShouldSplat(16);
		bool should5 = base.ShouldSplat(32);
		bool should6 = base.ShouldSplat(64);
		bool should7 = base.ShouldSplat(128);
		if (!should0 && !should1 && !should2 && !should3 && !should4 && !should5 && !should6 && !should7)
		{
			return;
		}
		TextureData splat0data = new TextureData(this.splatmap0.Get());
		TextureData splat1data = new TextureData(this.splatmap1.Get());
		Vector3 v = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 v2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 v3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 v4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.SplatMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			GenerateCliffSplat.Process(x, z);
			float normZ = TerrainMeta.SplatMap.Coordinate(z);
			float normX = TerrainMeta.SplatMap.Coordinate(x);
			Vector3 point = new Vector3(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 vector = worldToLocal.MultiplyPoint3x4(point) - this.offset;
			float num = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade, vector.Magnitude2D());
			if (num == 0f)
			{
				return;
			}
			Vector4 interpolatedVector = splat0data.GetInterpolatedVector((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z);
			Vector4 interpolatedVector2 = splat1data.GetInterpolatedVector((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z);
			if (!should0)
			{
				interpolatedVector.x = 0f;
			}
			if (!should1)
			{
				interpolatedVector.y = 0f;
			}
			if (!should2)
			{
				interpolatedVector.z = 0f;
			}
			if (!should3)
			{
				interpolatedVector.w = 0f;
			}
			if (!should4)
			{
				interpolatedVector2.x = 0f;
			}
			if (!should5)
			{
				interpolatedVector2.y = 0f;
			}
			if (!should6)
			{
				interpolatedVector2.z = 0f;
			}
			if (!should7)
			{
				interpolatedVector2.w = 0f;
			}
			TerrainMeta.SplatMap.SetSplatRaw(x, z, interpolatedVector, interpolatedVector2, num);
		});
	}

	// Token: 0x06003228 RID: 12840 RVA: 0x00135200 File Offset: 0x00133400
	protected override void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		TextureData alphadata = new TextureData(this.alphamap.Get());
		Vector3 v = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 v2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 v3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 v4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.AlphaMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			float normZ = TerrainMeta.AlphaMap.Coordinate(z);
			float normX = TerrainMeta.AlphaMap.Coordinate(x);
			Vector3 point = new Vector3(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 vector = worldToLocal.MultiplyPoint3x4(point) - this.offset;
			float num = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade, vector.Magnitude2D());
			if (num == 0f)
			{
				return;
			}
			float w = alphadata.GetInterpolatedVector((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z).w;
			TerrainMeta.AlphaMap.SetAlpha(x, z, w, num);
		});
	}

	// Token: 0x06003229 RID: 12841 RVA: 0x0013531C File Offset: 0x0013351C
	protected override void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		bool should0 = base.ShouldBiome(1);
		bool should1 = base.ShouldBiome(2);
		bool should2 = base.ShouldBiome(4);
		bool should3 = base.ShouldBiome(8);
		if (!should0 && !should1 && !should2 && !should3)
		{
			return;
		}
		TextureData biomedata = new TextureData(this.biomemap.Get());
		Vector3 v = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 v2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 v3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 v4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.BiomeMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			float normZ = TerrainMeta.BiomeMap.Coordinate(z);
			float normX = TerrainMeta.BiomeMap.Coordinate(x);
			Vector3 point = new Vector3(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 vector = worldToLocal.MultiplyPoint3x4(point) - this.offset;
			float num = Mathf.InverseLerp(this.Radius, this.Radius - this.Fade, vector.Magnitude2D());
			if (num == 0f)
			{
				return;
			}
			Vector4 interpolatedVector = biomedata.GetInterpolatedVector((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z);
			if (!should0)
			{
				interpolatedVector.x = 0f;
			}
			if (!should1)
			{
				interpolatedVector.y = 0f;
			}
			if (!should2)
			{
				interpolatedVector.z = 0f;
			}
			if (!should3)
			{
				interpolatedVector.w = 0f;
			}
			TerrainMeta.BiomeMap.SetBiomeRaw(x, z, interpolatedVector, num);
		});
	}

	// Token: 0x0600322A RID: 12842 RVA: 0x0013548C File Offset: 0x0013368C
	protected override void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		if (this.Radius == 0f)
		{
			this.Radius = this.extents.x;
		}
		TextureData topologydata = new TextureData(this.topologymap.Get());
		Vector3 v = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, -this.Radius));
		Vector3 v2 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, -this.Radius));
		Vector3 v3 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(-this.Radius, 0f, this.Radius));
		Vector3 v4 = localToWorld.MultiplyPoint3x4(this.offset + new Vector3(this.Radius, 0f, this.Radius));
		TerrainMeta.TopologyMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			GenerateCliffTopology.Process(x, z);
			float normZ = TerrainMeta.TopologyMap.Coordinate(z);
			float normX = TerrainMeta.TopologyMap.Coordinate(x);
			Vector3 point = new Vector3(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 vector = worldToLocal.MultiplyPoint3x4(point) - this.offset;
			int interpolatedInt = topologydata.GetInterpolatedInt((vector.x + this.extents.x) / this.size.x, (vector.z + this.extents.z) / this.size.z);
			if (this.ShouldTopology(interpolatedInt))
			{
				TerrainMeta.TopologyMap.AddTopology(x, z, interpolatedInt & (int)this.TopologyMask);
			}
		});
	}

	// Token: 0x0600322B RID: 12843 RVA: 0x000063A5 File Offset: 0x000045A5
	protected override void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
	}
}
