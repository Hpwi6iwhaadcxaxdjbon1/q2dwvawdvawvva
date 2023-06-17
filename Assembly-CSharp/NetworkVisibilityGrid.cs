using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConVar;
using Network;
using Network.Visibility;
using Rust;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000621 RID: 1569
public class NetworkVisibilityGrid : MonoBehaviour, Provider
{
	// Token: 0x040025C3 RID: 9667
	public const int overworldLayer = 0;

	// Token: 0x040025C4 RID: 9668
	public const int cavesLayer = 1;

	// Token: 0x040025C5 RID: 9669
	public const int tunnelsLayer = 2;

	// Token: 0x040025C6 RID: 9670
	public const int dynamicDungeonsFirstLayer = 10;

	// Token: 0x040025C7 RID: 9671
	public int startID = 1024;

	// Token: 0x040025C8 RID: 9672
	public int gridSize = 100;

	// Token: 0x040025C9 RID: 9673
	public int cellCount = 32;

	// Token: 0x040025CA RID: 9674
	[FormerlySerializedAs("visibilityRadius")]
	public int visibilityRadiusFar = 2;

	// Token: 0x040025CB RID: 9675
	public int visibilityRadiusNear = 1;

	// Token: 0x040025CC RID: 9676
	public float switchTolerance = 20f;

	// Token: 0x040025CD RID: 9677
	public float cavesThreshold = -5f;

	// Token: 0x040025CE RID: 9678
	public float tunnelsThreshold = -50f;

	// Token: 0x040025CF RID: 9679
	public float dynamicDungeonsThreshold = 1000f;

	// Token: 0x040025D0 RID: 9680
	public float dynamicDungeonsInterval = 100f;

	// Token: 0x040025D1 RID: 9681
	private float halfGridSize;

	// Token: 0x040025D2 RID: 9682
	private float cellSize;

	// Token: 0x040025D3 RID: 9683
	private float halfCellSize;

	// Token: 0x040025D4 RID: 9684
	private int numIDsPerLayer;

	// Token: 0x06002E2F RID: 11823 RVA: 0x0011506D File Offset: 0x0011326D
	private void Awake()
	{
		Debug.Assert(Network.Net.sv != null, "Network.Net.sv is NULL when creating Visibility Grid");
		Debug.Assert(Network.Net.sv.visibility == null, "Network.Net.sv.visibility is being set multiple times");
		Network.Net.sv.visibility = new Manager(this);
	}

	// Token: 0x06002E30 RID: 11824 RVA: 0x001150A8 File Offset: 0x001132A8
	private void OnEnable()
	{
		this.halfGridSize = (float)this.gridSize / 2f;
		this.cellSize = (float)this.gridSize / (float)this.cellCount;
		this.halfCellSize = this.cellSize / 2f;
		this.numIDsPerLayer = this.cellCount * this.cellCount;
	}

	// Token: 0x06002E31 RID: 11825 RVA: 0x00115102 File Offset: 0x00113302
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (Network.Net.sv != null && Network.Net.sv.visibility != null)
		{
			Network.Net.sv.visibility.Dispose();
			Network.Net.sv.visibility = null;
		}
	}

	// Token: 0x06002E32 RID: 11826 RVA: 0x0011513C File Offset: 0x0011333C
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Vector3 position = base.transform.position;
		for (int i = 0; i <= this.cellCount; i++)
		{
			float num = -this.halfGridSize + (float)i * this.cellSize - this.halfCellSize;
			Gizmos.DrawLine(new Vector3(this.halfGridSize, position.y, num), new Vector3(-this.halfGridSize, position.y, num));
			Gizmos.DrawLine(new Vector3(num, position.y, this.halfGridSize), new Vector3(num, position.y, -this.halfGridSize));
		}
	}

	// Token: 0x06002E33 RID: 11827 RVA: 0x001151DD File Offset: 0x001133DD
	private int PositionToGrid(float value)
	{
		return Mathf.RoundToInt((value + this.halfGridSize) / this.cellSize);
	}

	// Token: 0x06002E34 RID: 11828 RVA: 0x001151F3 File Offset: 0x001133F3
	private float GridToPosition(int value)
	{
		return (float)value * this.cellSize - this.halfGridSize;
	}

	// Token: 0x06002E35 RID: 11829 RVA: 0x00115205 File Offset: 0x00113405
	private int PositionToLayer(float y)
	{
		if (y < this.tunnelsThreshold)
		{
			return 2;
		}
		if (y < this.cavesThreshold)
		{
			return 1;
		}
		if (y >= this.dynamicDungeonsThreshold)
		{
			return 10 + Mathf.FloorToInt((y - this.dynamicDungeonsThreshold) / this.dynamicDungeonsInterval);
		}
		return 0;
	}

	// Token: 0x06002E36 RID: 11830 RVA: 0x0011523F File Offset: 0x0011343F
	private uint CoordToID(int x, int y, int layer)
	{
		return (uint)(layer * this.numIDsPerLayer + (x * this.cellCount + y) + this.startID);
	}

	// Token: 0x06002E37 RID: 11831 RVA: 0x0011525C File Offset: 0x0011345C
	private uint GetID(Vector3 vPos)
	{
		int num = this.PositionToGrid(vPos.x);
		int num2 = this.PositionToGrid(vPos.z);
		int num3 = this.PositionToLayer(vPos.y);
		if (num < 0)
		{
			return 0U;
		}
		if (num >= this.cellCount)
		{
			return 0U;
		}
		if (num2 < 0)
		{
			return 0U;
		}
		if (num2 >= this.cellCount)
		{
			return 0U;
		}
		uint num4 = this.CoordToID(num, num2, num3);
		if ((ulong)num4 < (ulong)((long)this.startID))
		{
			Debug.LogError(string.Format("NetworkVisibilityGrid.GetID - group is below range {0} {1} {2} {3} {4}", new object[]
			{
				num,
				num2,
				num3,
				num4,
				this.cellCount
			}));
		}
		return num4;
	}

	// Token: 0x06002E38 RID: 11832 RVA: 0x00115310 File Offset: 0x00113510
	[return: TupleElementNames(new string[]
	{
		"x",
		"y",
		"layer"
	})]
	private ValueTuple<int, int, int> DeconstructGroupId(int groupId)
	{
		groupId -= this.startID;
		int a;
		int item = Math.DivRem(groupId, this.numIDsPerLayer, out a);
		int item2;
		return new ValueTuple<int, int, int>(Math.DivRem(a, this.cellCount, out item2), item2, item);
	}

	// Token: 0x06002E39 RID: 11833 RVA: 0x0011534C File Offset: 0x0011354C
	private Bounds GetBounds(uint uid)
	{
		ValueTuple<int, int, int> valueTuple = this.DeconstructGroupId((int)uid);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		int item3 = valueTuple.Item3;
		Vector3 vector = new Vector3(this.GridToPosition(item) - this.halfCellSize, 0f, this.GridToPosition(item2) - this.halfCellSize);
		Vector3 max = new Vector3(vector.x + this.cellSize, 0f, vector.z + this.cellSize);
		if (item3 == 0)
		{
			vector.y = this.cavesThreshold;
			max.y = this.dynamicDungeonsThreshold;
		}
		else if (item3 == 1)
		{
			vector.y = this.tunnelsThreshold;
			max.y = this.cavesThreshold - float.Epsilon;
		}
		else if (item3 == 2)
		{
			vector.y = -10000f;
			max.y = this.tunnelsThreshold - float.Epsilon;
		}
		else if (item3 >= 10)
		{
			int num = item3 - 10;
			vector.y = this.dynamicDungeonsThreshold + (float)num * this.dynamicDungeonsInterval + float.Epsilon;
			max.y = vector.y + this.dynamicDungeonsInterval;
		}
		else
		{
			Debug.LogError(string.Format("Cannot get bounds for unknown layer {0}!", item3), this);
		}
		return new Bounds
		{
			min = vector,
			max = max
		};
	}

	// Token: 0x06002E3A RID: 11834 RVA: 0x0011549F File Offset: 0x0011369F
	public void OnGroupAdded(Group group)
	{
		group.bounds = this.GetBounds(group.ID);
	}

	// Token: 0x06002E3B RID: 11835 RVA: 0x001154B3 File Offset: 0x001136B3
	public bool IsInside(Group group, Vector3 vPos)
	{
		return false || group.ID == 0U || group.bounds.Contains(vPos) || group.bounds.SqrDistance(vPos) < this.switchTolerance;
	}

	// Token: 0x06002E3C RID: 11836 RVA: 0x001154F0 File Offset: 0x001136F0
	public Group GetGroup(Vector3 vPos)
	{
		uint id = this.GetID(vPos);
		if (id == 0U)
		{
			return null;
		}
		Group group = Network.Net.sv.visibility.Get(id);
		if (!this.IsInside(group, vPos))
		{
			float num = group.bounds.SqrDistance(vPos);
			Debug.Log(string.Concat(new object[]
			{
				"Group is inside is all fucked ",
				id,
				"/",
				num,
				"/",
				vPos
			}));
		}
		return group;
	}

	// Token: 0x06002E3D RID: 11837 RVA: 0x00115578 File Offset: 0x00113778
	public void GetVisibleFromFar(Group group, List<Group> groups)
	{
		int visibilityRadiusFarOverride = ConVar.Net.visibilityRadiusFarOverride;
		int radius = (visibilityRadiusFarOverride > 0) ? visibilityRadiusFarOverride : this.visibilityRadiusFar;
		this.GetVisibleFrom(group, groups, radius);
	}

	// Token: 0x06002E3E RID: 11838 RVA: 0x001155A4 File Offset: 0x001137A4
	public void GetVisibleFromNear(Group group, List<Group> groups)
	{
		int visibilityRadiusNearOverride = ConVar.Net.visibilityRadiusNearOverride;
		int radius = (visibilityRadiusNearOverride > 0) ? visibilityRadiusNearOverride : this.visibilityRadiusNear;
		this.GetVisibleFrom(group, groups, radius);
	}

	// Token: 0x06002E3F RID: 11839 RVA: 0x001155D0 File Offset: 0x001137D0
	private void GetVisibleFrom(Group group, List<Group> groups, int radius)
	{
		NetworkVisibilityGrid.<>c__DisplayClass34_0 CS$<>8__locals1;
		CS$<>8__locals1.groups = groups;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.groups.Add(Network.Net.sv.visibility.Get(0U));
		int id = (int)group.ID;
		if (id < this.startID)
		{
			return;
		}
		ValueTuple<int, int, int> valueTuple = this.DeconstructGroupId(id);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		int item3 = valueTuple.Item3;
		this.<GetVisibleFrom>g__AddLayers|34_0(item, item2, item3, ref CS$<>8__locals1);
		for (int i = 1; i <= radius; i++)
		{
			this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item, item2 - i, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item, item2 + i, item3, ref CS$<>8__locals1);
			for (int j = 1; j < i; j++)
			{
				this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2 - j, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2 + j, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2 - j, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2 + j, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item - j, item2 - i, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item + j, item2 - i, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item - j, item2 + i, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item + j, item2 + i, item3, ref CS$<>8__locals1);
			}
			this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2 - i, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2 + i, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2 - i, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2 + i, item3, ref CS$<>8__locals1);
		}
	}

	// Token: 0x06002E41 RID: 11841 RVA: 0x001157F7 File Offset: 0x001139F7
	[CompilerGenerated]
	private void <GetVisibleFrom>g__AddLayers|34_0(int groupX, int groupY, int groupLayer, ref NetworkVisibilityGrid.<>c__DisplayClass34_0 A_4)
	{
		this.<GetVisibleFrom>g__Add|34_1(groupX, groupY, groupLayer, ref A_4);
		if (groupLayer == 0)
		{
			this.<GetVisibleFrom>g__Add|34_1(groupX, groupY, 1, ref A_4);
		}
		if (groupLayer == 1)
		{
			this.<GetVisibleFrom>g__Add|34_1(groupX, groupY, 2, ref A_4);
			this.<GetVisibleFrom>g__Add|34_1(groupX, groupY, 0, ref A_4);
		}
	}

	// Token: 0x06002E42 RID: 11842 RVA: 0x0011582C File Offset: 0x00113A2C
	[CompilerGenerated]
	private void <GetVisibleFrom>g__Add|34_1(int groupX, int groupY, int groupLayer, ref NetworkVisibilityGrid.<>c__DisplayClass34_0 A_4)
	{
		A_4.groups.Add(Network.Net.sv.visibility.Get(this.CoordToID(groupX, groupY, groupLayer)));
	}
}
