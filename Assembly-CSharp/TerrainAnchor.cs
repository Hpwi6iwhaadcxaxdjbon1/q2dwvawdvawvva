using System;
using UnityEngine;

// Token: 0x02000688 RID: 1672
public class TerrainAnchor : PrefabAttribute
{
	// Token: 0x04002742 RID: 10050
	public float Extents = 1f;

	// Token: 0x04002743 RID: 10051
	public float Offset;

	// Token: 0x04002744 RID: 10052
	public float Radius;

	// Token: 0x06002FC6 RID: 12230 RVA: 0x0011F274 File Offset: 0x0011D474
	public void Apply(out float height, out float min, out float max, Vector3 pos, Vector3 scale)
	{
		float num = this.Extents * scale.y;
		float num2 = this.Offset * scale.y;
		height = TerrainMeta.HeightMap.GetHeight(pos);
		min = height - num2 - num;
		max = height - num2 + num;
		if (this.Radius > 0f)
		{
			int num3 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(pos.x - this.Radius));
			int num4 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(pos.x + this.Radius));
			int num5 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(pos.z - this.Radius));
			int num6 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(pos.z + this.Radius));
			int num7 = num5;
			while (num7 <= num6 && max >= min)
			{
				int num8 = num3;
				while (num8 <= num4 && max >= min)
				{
					float height2 = TerrainMeta.HeightMap.GetHeight(num8, num7);
					min = Mathf.Max(min, height2 - num2 - num);
					max = Mathf.Min(max, height2 - num2 + num);
					num8++;
				}
				num7++;
			}
		}
	}

	// Token: 0x06002FC7 RID: 12231 RVA: 0x0011F39F File Offset: 0x0011D59F
	protected override Type GetIndexedType()
	{
		return typeof(TerrainAnchor);
	}
}
