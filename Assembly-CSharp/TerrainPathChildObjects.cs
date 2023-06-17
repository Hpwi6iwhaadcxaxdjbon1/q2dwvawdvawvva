using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200067E RID: 1662
public class TerrainPathChildObjects : MonoBehaviour
{
	// Token: 0x04002725 RID: 10021
	public bool Spline = true;

	// Token: 0x04002726 RID: 10022
	public float Width;

	// Token: 0x04002727 RID: 10023
	public float Offset;

	// Token: 0x04002728 RID: 10024
	public float Fade;

	// Token: 0x04002729 RID: 10025
	[InspectorFlags]
	public TerrainSplat.Enum Splat = TerrainSplat.Enum.Dirt;

	// Token: 0x0400272A RID: 10026
	[InspectorFlags]
	public TerrainTopology.Enum Topology = TerrainTopology.Enum.Road;

	// Token: 0x0400272B RID: 10027
	public InfrastructureType Type;

	// Token: 0x06002FAD RID: 12205 RVA: 0x0011E7DC File Offset: 0x0011C9DC
	protected void Awake()
	{
		if (!World.Cached && !World.Networked)
		{
			List<Vector3> list = new List<Vector3>();
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				list.Add(transform.position);
			}
			if (list.Count >= 2)
			{
				InfrastructureType type = this.Type;
				if (type != InfrastructureType.Road)
				{
					if (type == InfrastructureType.Power)
					{
						PathList pathList = new PathList("Powerline " + TerrainMeta.Path.Powerlines.Count, list.ToArray());
						pathList.Width = this.Width;
						pathList.InnerFade = this.Fade * 0.5f;
						pathList.OuterFade = this.Fade * 0.5f;
						pathList.MeshOffset = this.Offset * 0.3f;
						pathList.TerrainOffset = this.Offset;
						pathList.Topology = (int)this.Topology;
						pathList.Splat = (int)this.Splat;
						pathList.Spline = this.Spline;
						pathList.Path.RecalculateTangents();
						TerrainMeta.Path.Powerlines.Add(pathList);
					}
				}
				else
				{
					PathList pathList2 = new PathList("Road " + TerrainMeta.Path.Roads.Count, list.ToArray());
					pathList2.Width = this.Width;
					pathList2.InnerFade = this.Fade * 0.5f;
					pathList2.OuterFade = this.Fade * 0.5f;
					pathList2.MeshOffset = this.Offset * 0.3f;
					pathList2.TerrainOffset = this.Offset;
					pathList2.Topology = (int)this.Topology;
					pathList2.Splat = (int)this.Splat;
					pathList2.Spline = this.Spline;
					pathList2.Path.RecalculateTangents();
					TerrainMeta.Path.Roads.Add(pathList2);
				}
			}
		}
		GameManager.Destroy(base.gameObject, 0f);
	}

	// Token: 0x06002FAE RID: 12206 RVA: 0x0011EA14 File Offset: 0x0011CC14
	protected void OnDrawGizmos()
	{
		bool flag = false;
		Vector3 a = Vector3.zero;
		foreach (object obj in base.transform)
		{
			Vector3 position = ((Transform)obj).position;
			if (flag)
			{
				Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
				GizmosUtil.DrawWirePath(a, position, 0.5f * this.Width);
			}
			a = position;
			flag = true;
		}
	}
}
