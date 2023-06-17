using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x02000611 RID: 1553
public class MissionPoint : MonoBehaviour
{
	// Token: 0x0400257F RID: 9599
	public bool dropToGround = true;

	// Token: 0x04002580 RID: 9600
	public const int COUNT = 8;

	// Token: 0x04002581 RID: 9601
	public const int EVERYTHING = -1;

	// Token: 0x04002582 RID: 9602
	public const int NOTHING = 0;

	// Token: 0x04002583 RID: 9603
	public const int EASY_MONUMENT = 1;

	// Token: 0x04002584 RID: 9604
	public const int MED_MONUMENT = 2;

	// Token: 0x04002585 RID: 9605
	public const int HARD_MONUMENT = 4;

	// Token: 0x04002586 RID: 9606
	public const int ITEM_HIDESPOT = 8;

	// Token: 0x04002587 RID: 9607
	public const int UNDERWATER = 128;

	// Token: 0x04002588 RID: 9608
	public const int EASY_MONUMENT_IDX = 0;

	// Token: 0x04002589 RID: 9609
	public const int MED_MONUMENT_IDX = 1;

	// Token: 0x0400258A RID: 9610
	public const int HARD_MONUMENT_IDX = 2;

	// Token: 0x0400258B RID: 9611
	public const int ITEM_HIDESPOT_IDX = 3;

	// Token: 0x0400258C RID: 9612
	public const int FOREST_IDX = 4;

	// Token: 0x0400258D RID: 9613
	public const int ROADSIDE_IDX = 5;

	// Token: 0x0400258E RID: 9614
	public const int BEACH = 6;

	// Token: 0x0400258F RID: 9615
	public const int UNDERWATER_IDX = 7;

	// Token: 0x04002590 RID: 9616
	private static Dictionary<int, int> type2index = new Dictionary<int, int>
	{
		{
			1,
			0
		},
		{
			2,
			1
		},
		{
			4,
			2
		},
		{
			8,
			3
		},
		{
			128,
			7
		}
	};

	// Token: 0x04002591 RID: 9617
	public static List<MissionPoint> all = new List<MissionPoint>();

	// Token: 0x04002592 RID: 9618
	[global::InspectorFlags]
	public MissionPoint.MissionPointEnum Flags = (MissionPoint.MissionPointEnum)(-1);

	// Token: 0x06002DEA RID: 11754 RVA: 0x00113D00 File Offset: 0x00111F00
	public static int TypeToIndex(int id)
	{
		return MissionPoint.type2index[id];
	}

	// Token: 0x06002DEB RID: 11755 RVA: 0x00113D0D File Offset: 0x00111F0D
	public static int IndexToType(int idx)
	{
		return 1 << idx;
	}

	// Token: 0x06002DEC RID: 11756 RVA: 0x00113D15 File Offset: 0x00111F15
	public void Awake()
	{
		MissionPoint.all.Add(this);
	}

	// Token: 0x06002DED RID: 11757 RVA: 0x00113D22 File Offset: 0x00111F22
	private void Start()
	{
		if (this.dropToGround)
		{
			SingletonComponent<InvokeHandler>.Instance.Invoke(new Action(this.DropToGround), 0.5f);
		}
	}

	// Token: 0x06002DEE RID: 11758 RVA: 0x00113D48 File Offset: 0x00111F48
	private void DropToGround()
	{
		if (Rust.Application.isLoading)
		{
			SingletonComponent<InvokeHandler>.Instance.Invoke(new Action(this.DropToGround), 0.5f);
			return;
		}
		Vector3 position = base.transform.position;
		base.transform.DropToGround(false, 100f);
	}

	// Token: 0x06002DEF RID: 11759 RVA: 0x00113D96 File Offset: 0x00111F96
	public void OnDisable()
	{
		if (MissionPoint.all.Contains(this))
		{
			MissionPoint.all.Remove(this);
		}
	}

	// Token: 0x06002DF0 RID: 11760 RVA: 0x0002C673 File Offset: 0x0002A873
	public virtual Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06002DF1 RID: 11761 RVA: 0x00113DB1 File Offset: 0x00111FB1
	public virtual Quaternion GetRotation()
	{
		return base.transform.rotation;
	}

	// Token: 0x06002DF2 RID: 11762 RVA: 0x00113DC0 File Offset: 0x00111FC0
	public static bool GetMissionPoints(ref List<MissionPoint> points, Vector3 near, float minDistance, float maxDistance, int flags, int exclusionFlags)
	{
		List<MissionPoint> list = Pool.GetList<MissionPoint>();
		foreach (MissionPoint missionPoint in MissionPoint.all)
		{
			if ((missionPoint.Flags & (MissionPoint.MissionPointEnum)flags) == (MissionPoint.MissionPointEnum)flags && (exclusionFlags == 0 || (missionPoint.Flags & (MissionPoint.MissionPointEnum)exclusionFlags) == (MissionPoint.MissionPointEnum)0))
			{
				float num = Vector3.Distance(missionPoint.transform.position, near);
				if (num <= maxDistance && num > minDistance)
				{
					if (BaseMission.blockedPoints.Count > 0)
					{
						bool flag = false;
						using (List<Vector3>.Enumerator enumerator2 = BaseMission.blockedPoints.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (Vector3.Distance(enumerator2.Current, missionPoint.transform.position) < 5f)
								{
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							continue;
						}
					}
					list.Add(missionPoint);
				}
			}
		}
		if (list.Count == 0)
		{
			return false;
		}
		foreach (MissionPoint item in list)
		{
			points.Add(item);
		}
		Pool.FreeList<MissionPoint>(ref list);
		return true;
	}

	// Token: 0x02000D91 RID: 3473
	public enum MissionPointEnum
	{
		// Token: 0x04004808 RID: 18440
		EasyMonument = 1,
		// Token: 0x04004809 RID: 18441
		MediumMonument,
		// Token: 0x0400480A RID: 18442
		HardMonument = 4,
		// Token: 0x0400480B RID: 18443
		Item_Hidespot = 8,
		// Token: 0x0400480C RID: 18444
		Underwater = 128
	}
}
