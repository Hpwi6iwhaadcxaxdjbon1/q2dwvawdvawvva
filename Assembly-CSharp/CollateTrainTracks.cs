using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020006AF RID: 1711
public class CollateTrainTracks : ProceduralComponent
{
	// Token: 0x040027DD RID: 10205
	private const float MAX_NODE_DIST = 0.1f;

	// Token: 0x040027DE RID: 10206
	private const float MAX_NODE_DIST_SQR = 0.010000001f;

	// Token: 0x040027DF RID: 10207
	private const float MAX_NODE_ANGLE = 10f;

	// Token: 0x17000408 RID: 1032
	// (get) Token: 0x06003151 RID: 12625 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003152 RID: 12626 RVA: 0x00126DF4 File Offset: 0x00124FF4
	public override void Process(uint seed)
	{
		TrainTrackSpline[] array = UnityEngine.Object.FindObjectsOfType<TrainTrackSpline>();
		for (int i = array.Length - 1; i >= 0; i--)
		{
			CollateTrainTracks.<>c__DisplayClass5_0 CS$<>8__locals1;
			CS$<>8__locals1.ourSpline = array[i];
			if (CS$<>8__locals1.ourSpline.dataIndex < 0 && CS$<>8__locals1.ourSpline.points.Length > 3)
			{
				CollateTrainTracks.<>c__DisplayClass5_1 CS$<>8__locals2;
				CS$<>8__locals2.nodeIndex = CS$<>8__locals1.ourSpline.points.Length - 2;
				while (CS$<>8__locals2.nodeIndex >= 1)
				{
					CollateTrainTracks.<>c__DisplayClass5_2 CS$<>8__locals3;
					CS$<>8__locals3.ourPos = CS$<>8__locals1.ourSpline.points[CS$<>8__locals2.nodeIndex];
					CS$<>8__locals3.ourTangent = CS$<>8__locals1.ourSpline.tangents[CS$<>8__locals2.nodeIndex];
					foreach (TrainTrackSpline trainTrackSpline in array)
					{
						if (!(CS$<>8__locals1.ourSpline == trainTrackSpline))
						{
							Vector3 startPointWorld = trainTrackSpline.GetStartPointWorld();
							Vector3 endPointWorld = trainTrackSpline.GetEndPointWorld();
							Vector3 startTangentWorld = trainTrackSpline.GetStartTangentWorld();
							Vector3 endTangentWorld = trainTrackSpline.GetEndTangentWorld();
							if (!CollateTrainTracks.<Process>g__CompareNodes|5_1(startPointWorld, startTangentWorld, ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals3) && !CollateTrainTracks.<Process>g__CompareNodes|5_1(endPointWorld, endTangentWorld, ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals3) && !CollateTrainTracks.<Process>g__CompareNodes|5_1(startPointWorld, -startTangentWorld, ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals3))
							{
								CollateTrainTracks.<Process>g__CompareNodes|5_1(endPointWorld, -endTangentWorld, ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals3);
							}
						}
					}
					int j = CS$<>8__locals2.nodeIndex;
					CS$<>8__locals2.nodeIndex = j - 1;
				}
			}
		}
		array = UnityEngine.Object.FindObjectsOfType<TrainTrackSpline>();
		TrainTrackSpline[] array2 = array;
		for (int j = 0; j < array2.Length; j++)
		{
			CollateTrainTracks.<>c__DisplayClass5_3 CS$<>8__locals4;
			CS$<>8__locals4.ourSpline = array2[j];
			CollateTrainTracks.<>c__DisplayClass5_4 CS$<>8__locals5;
			CS$<>8__locals5.ourStartPos = CS$<>8__locals4.ourSpline.GetStartPointWorld();
			CS$<>8__locals5.ourEndPos = CS$<>8__locals4.ourSpline.GetEndPointWorld();
			CS$<>8__locals5.ourStartTangent = CS$<>8__locals4.ourSpline.GetStartTangentWorld();
			CS$<>8__locals5.ourEndTangent = CS$<>8__locals4.ourSpline.GetEndTangentWorld();
			if (CollateTrainTracks.<Process>g__NodesConnect|5_0(CS$<>8__locals5.ourStartPos, CS$<>8__locals5.ourEndPos, CS$<>8__locals5.ourStartTangent, CS$<>8__locals5.ourEndTangent))
			{
				CS$<>8__locals4.ourSpline.AddTrackConnection(CS$<>8__locals4.ourSpline, TrainTrackSpline.TrackPosition.Next, TrainTrackSpline.TrackOrientation.Same);
				CS$<>8__locals4.ourSpline.AddTrackConnection(CS$<>8__locals4.ourSpline, TrainTrackSpline.TrackPosition.Prev, TrainTrackSpline.TrackOrientation.Same);
			}
			else
			{
				TrainTrackSpline[] array3 = array;
				for (int k = 0; k < array3.Length; k++)
				{
					CollateTrainTracks.<>c__DisplayClass5_5 CS$<>8__locals6;
					CS$<>8__locals6.otherSpline = array3[k];
					if (!(CS$<>8__locals4.ourSpline == CS$<>8__locals6.otherSpline))
					{
						CollateTrainTracks.<>c__DisplayClass5_6 CS$<>8__locals7;
						CS$<>8__locals7.theirStartPos = CS$<>8__locals6.otherSpline.GetStartPointWorld();
						CS$<>8__locals7.theirEndPos = CS$<>8__locals6.otherSpline.GetEndPointWorld();
						CS$<>8__locals7.theirStartTangent = CS$<>8__locals6.otherSpline.GetStartTangentWorld();
						CS$<>8__locals7.theirEndTangent = CS$<>8__locals6.otherSpline.GetEndTangentWorld();
						if (!CollateTrainTracks.<Process>g__CompareNodes|5_2(false, true, ref CS$<>8__locals4, ref CS$<>8__locals5, ref CS$<>8__locals6, ref CS$<>8__locals7) && !CollateTrainTracks.<Process>g__CompareNodes|5_2(false, false, ref CS$<>8__locals4, ref CS$<>8__locals5, ref CS$<>8__locals6, ref CS$<>8__locals7) && !CollateTrainTracks.<Process>g__CompareNodes|5_2(true, true, ref CS$<>8__locals4, ref CS$<>8__locals5, ref CS$<>8__locals6, ref CS$<>8__locals7))
						{
							CollateTrainTracks.<Process>g__CompareNodes|5_2(true, false, ref CS$<>8__locals4, ref CS$<>8__locals5, ref CS$<>8__locals6, ref CS$<>8__locals7);
						}
					}
				}
			}
		}
	}

	// Token: 0x06003154 RID: 12628 RVA: 0x00127108 File Offset: 0x00125308
	[CompilerGenerated]
	internal static bool <Process>g__CompareNodes|5_1(Vector3 theirPos, Vector3 theirTangent, ref CollateTrainTracks.<>c__DisplayClass5_0 A_2, ref CollateTrainTracks.<>c__DisplayClass5_1 A_3, ref CollateTrainTracks.<>c__DisplayClass5_2 A_4)
	{
		if (CollateTrainTracks.<Process>g__NodesConnect|5_0(A_4.ourPos, theirPos, A_4.ourTangent, theirTangent))
		{
			TrainTrackSpline trainTrackSpline = A_2.ourSpline.gameObject.AddComponent<TrainTrackSpline>();
			Vector3[] array = new Vector3[A_2.ourSpline.points.Length - A_3.nodeIndex];
			Vector3[] array2 = new Vector3[A_2.ourSpline.points.Length - A_3.nodeIndex];
			Vector3[] array3 = new Vector3[A_3.nodeIndex + 1];
			Vector3[] array4 = new Vector3[A_3.nodeIndex + 1];
			for (int i = A_2.ourSpline.points.Length - 1; i >= 0; i--)
			{
				if (i >= A_3.nodeIndex)
				{
					array[i - A_3.nodeIndex] = A_2.ourSpline.points[i];
					array2[i - A_3.nodeIndex] = A_2.ourSpline.tangents[i];
				}
				if (i <= A_3.nodeIndex)
				{
					array3[i] = A_2.ourSpline.points[i];
					array4[i] = A_2.ourSpline.tangents[i];
				}
			}
			A_2.ourSpline.SetAll(array3, array4, A_2.ourSpline);
			trainTrackSpline.SetAll(array, array2, A_2.ourSpline);
			int nodeIndex = A_3.nodeIndex;
			A_3.nodeIndex = nodeIndex - 1;
			return true;
		}
		return false;
	}

	// Token: 0x06003155 RID: 12629 RVA: 0x00127278 File Offset: 0x00125478
	[CompilerGenerated]
	internal static bool <Process>g__CompareNodes|5_2(bool ourStart, bool theirStart, ref CollateTrainTracks.<>c__DisplayClass5_3 A_2, ref CollateTrainTracks.<>c__DisplayClass5_4 A_3, ref CollateTrainTracks.<>c__DisplayClass5_5 A_4, ref CollateTrainTracks.<>c__DisplayClass5_6 A_5)
	{
		Vector3 ourPos = ourStart ? A_3.ourStartPos : A_3.ourEndPos;
		Vector3 ourTangent = ourStart ? A_3.ourStartTangent : A_3.ourEndTangent;
		Vector3 theirPos = theirStart ? A_5.theirStartPos : A_5.theirEndPos;
		Vector3 vector = theirStart ? A_5.theirStartTangent : A_5.theirEndTangent;
		if (ourStart == theirStart)
		{
			vector *= -1f;
		}
		if (CollateTrainTracks.<Process>g__NodesConnect|5_0(ourPos, theirPos, ourTangent, vector))
		{
			if (ourStart)
			{
				A_2.ourSpline.AddTrackConnection(A_4.otherSpline, TrainTrackSpline.TrackPosition.Prev, theirStart ? TrainTrackSpline.TrackOrientation.Reverse : TrainTrackSpline.TrackOrientation.Same);
			}
			else
			{
				A_2.ourSpline.AddTrackConnection(A_4.otherSpline, TrainTrackSpline.TrackPosition.Next, theirStart ? TrainTrackSpline.TrackOrientation.Same : TrainTrackSpline.TrackOrientation.Reverse);
			}
			if (theirStart)
			{
				A_4.otherSpline.AddTrackConnection(A_2.ourSpline, TrainTrackSpline.TrackPosition.Prev, ourStart ? TrainTrackSpline.TrackOrientation.Reverse : TrainTrackSpline.TrackOrientation.Same);
			}
			else
			{
				A_4.otherSpline.AddTrackConnection(A_2.ourSpline, TrainTrackSpline.TrackPosition.Next, ourStart ? TrainTrackSpline.TrackOrientation.Same : TrainTrackSpline.TrackOrientation.Reverse);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06003156 RID: 12630 RVA: 0x0012735F File Offset: 0x0012555F
	[CompilerGenerated]
	internal static bool <Process>g__NodesConnect|5_0(Vector3 ourPos, Vector3 theirPos, Vector3 ourTangent, Vector3 theirTangent)
	{
		return Vector3.SqrMagnitude(ourPos - theirPos) < 0.010000001f && Vector3.Angle(ourTangent, theirTangent) < 10f;
	}
}
