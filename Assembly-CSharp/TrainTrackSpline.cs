using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020004B6 RID: 1206
public class TrainTrackSpline : WorldSpline
{
	// Token: 0x04001FF5 RID: 8181
	[Tooltip("Is this track spline part of a train station?")]
	public bool isStation;

	// Token: 0x04001FF6 RID: 8182
	[Tooltip("Can above-ground trains spawn here?")]
	public bool aboveGroundSpawn;

	// Token: 0x04001FF7 RID: 8183
	public int hierarchy;

	// Token: 0x04001FF8 RID: 8184
	public static List<TrainTrackSpline> SidingSplines = new List<TrainTrackSpline>();

	// Token: 0x04001FF9 RID: 8185
	private List<TrainTrackSpline.ConnectedTrackInfo> nextTracks = new List<TrainTrackSpline.ConnectedTrackInfo>();

	// Token: 0x04001FFA RID: 8186
	private int straightestNextIndex;

	// Token: 0x04001FFB RID: 8187
	private List<TrainTrackSpline.ConnectedTrackInfo> prevTracks = new List<TrainTrackSpline.ConnectedTrackInfo>();

	// Token: 0x04001FFC RID: 8188
	private int straightestPrevIndex;

	// Token: 0x04001FFD RID: 8189
	private HashSet<TrainTrackSpline.ITrainTrackUser> trackUsers = new HashSet<TrainTrackSpline.ITrainTrackUser>();

	// Token: 0x17000356 RID: 854
	// (get) Token: 0x0600275D RID: 10077 RVA: 0x000F5A99 File Offset: 0x000F3C99
	private bool HasNextTrack
	{
		get
		{
			return this.nextTracks.Count > 0;
		}
	}

	// Token: 0x17000357 RID: 855
	// (get) Token: 0x0600275E RID: 10078 RVA: 0x000F5AA9 File Offset: 0x000F3CA9
	private bool HasPrevTrack
	{
		get
		{
			return this.prevTracks.Count > 0;
		}
	}

	// Token: 0x0600275F RID: 10079 RVA: 0x000F5AB9 File Offset: 0x000F3CB9
	public void SetAll(Vector3[] points, Vector3[] tangents, TrainTrackSpline sourceSpline)
	{
		this.points = points;
		this.tangents = tangents;
		this.lutInterval = sourceSpline.lutInterval;
		this.isStation = sourceSpline.isStation;
		this.aboveGroundSpawn = sourceSpline.aboveGroundSpawn;
		this.hierarchy = sourceSpline.hierarchy;
	}

	// Token: 0x06002760 RID: 10080 RVA: 0x000F5AFC File Offset: 0x000F3CFC
	public float GetSplineDistAfterMove(float prevSplineDist, Vector3 askerForward, float distMoved, TrainTrackSpline.TrackSelection trackSelection, out TrainTrackSpline onSpline, out bool atEndOfLine, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		bool facingForward = this.IsForward(askerForward, prevSplineDist);
		return this.GetSplineDistAfterMove(prevSplineDist, distMoved, trackSelection, facingForward, out onSpline, out atEndOfLine, preferredAltA, preferredAltB);
	}

	// Token: 0x06002761 RID: 10081 RVA: 0x000F5B28 File Offset: 0x000F3D28
	private float GetSplineDistAfterMove(float prevSplineDist, float distMoved, TrainTrackSpline.TrackSelection trackSelection, bool facingForward, out TrainTrackSpline onSpline, out bool atEndOfLine, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		WorldSplineData data = base.GetData();
		float num = facingForward ? (prevSplineDist + distMoved) : (prevSplineDist - distMoved);
		atEndOfLine = false;
		onSpline = this;
		if (num < 0f)
		{
			if (this.HasPrevTrack)
			{
				TrainTrackSpline.ConnectedTrackInfo trackSelection2 = this.GetTrackSelection(this.prevTracks, this.straightestPrevIndex, trackSelection, false, facingForward, preferredAltA, preferredAltB);
				float distMoved2 = facingForward ? num : (-num);
				if (trackSelection2.orientation == TrainTrackSpline.TrackOrientation.Same)
				{
					prevSplineDist = trackSelection2.track.GetLength();
				}
				else
				{
					prevSplineDist = 0f;
					facingForward = !facingForward;
				}
				return trackSelection2.track.GetSplineDistAfterMove(prevSplineDist, distMoved2, trackSelection, facingForward, out onSpline, out atEndOfLine, preferredAltA, preferredAltB);
			}
			atEndOfLine = true;
			num = 0f;
		}
		else if (num > data.Length)
		{
			if (this.HasNextTrack)
			{
				TrainTrackSpline.ConnectedTrackInfo trackSelection3 = this.GetTrackSelection(this.nextTracks, this.straightestNextIndex, trackSelection, true, facingForward, preferredAltA, preferredAltB);
				float distMoved3 = facingForward ? (num - data.Length) : (-(num - data.Length));
				if (trackSelection3.orientation == TrainTrackSpline.TrackOrientation.Same)
				{
					prevSplineDist = 0f;
				}
				else
				{
					prevSplineDist = trackSelection3.track.GetLength();
					facingForward = !facingForward;
				}
				return trackSelection3.track.GetSplineDistAfterMove(prevSplineDist, distMoved3, trackSelection, facingForward, out onSpline, out atEndOfLine, preferredAltA, preferredAltB);
			}
			atEndOfLine = true;
			num = data.Length;
		}
		return num;
	}

	// Token: 0x06002762 RID: 10082 RVA: 0x000F5C6C File Offset: 0x000F3E6C
	public float GetDistance(Vector3 position, float maxError, out float minSplineDist)
	{
		WorldSplineData data = base.GetData();
		float num = maxError * maxError;
		Vector3 b = base.transform.InverseTransformPoint(position);
		float num2 = float.MaxValue;
		minSplineDist = 0f;
		int num3 = 0;
		int num4 = data.LUTValues.Count;
		if (data.Length > 40f)
		{
			int num5 = 0;
			while ((float)num5 < data.Length + 10f)
			{
				float num6 = Vector3.SqrMagnitude(data.GetPointCubicHermite((float)num5) - b);
				if (num6 < num2)
				{
					num2 = num6;
					minSplineDist = (float)num5;
				}
				num5 += 10;
			}
			num3 = Mathf.FloorToInt(Mathf.Max(0f, minSplineDist - 10f + 1f));
			num4 = Mathf.CeilToInt(Mathf.Min((float)data.LUTValues.Count, minSplineDist + 10f - 1f));
		}
		for (int i = num3; i < num4; i++)
		{
			WorldSplineData.LUTEntry lutentry = data.LUTValues[i];
			for (int j = 0; j < lutentry.points.Count; j++)
			{
				WorldSplineData.LUTEntry.LUTPoint lutpoint = lutentry.points[j];
				float num7 = Vector3.SqrMagnitude(lutpoint.pos - b);
				if (num7 < num2)
				{
					num2 = num7;
					minSplineDist = lutpoint.distance;
					if (num7 < num)
					{
						break;
					}
				}
			}
		}
		return Mathf.Sqrt(num2);
	}

	// Token: 0x06002763 RID: 10083 RVA: 0x000F5DBF File Offset: 0x000F3FBF
	public float GetLength()
	{
		return base.GetData().Length;
	}

	// Token: 0x06002764 RID: 10084 RVA: 0x000F5DCC File Offset: 0x000F3FCC
	public Vector3 GetPosition(float distance)
	{
		return base.GetPointCubicHermiteWorld(distance);
	}

	// Token: 0x06002765 RID: 10085 RVA: 0x000F5DD5 File Offset: 0x000F3FD5
	public Vector3 GetPositionAndTangent(float distance, Vector3 askerForward, out Vector3 tangent)
	{
		Vector3 pointAndTangentCubicHermiteWorld = base.GetPointAndTangentCubicHermiteWorld(distance, out tangent);
		if (Vector3.Dot(askerForward, tangent) < 0f)
		{
			tangent = -tangent;
		}
		return pointAndTangentCubicHermiteWorld;
	}

	// Token: 0x06002766 RID: 10086 RVA: 0x000F5E04 File Offset: 0x000F4004
	public void AddTrackConnection(TrainTrackSpline track, TrainTrackSpline.TrackPosition p, TrainTrackSpline.TrackOrientation o)
	{
		List<TrainTrackSpline.ConnectedTrackInfo> list = (p == TrainTrackSpline.TrackPosition.Next) ? this.nextTracks : this.prevTracks;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].track == track)
			{
				return;
			}
		}
		Vector3 position = (p == TrainTrackSpline.TrackPosition.Next) ? this.points[this.points.Length - 2] : this.points[0];
		Vector3 position2 = (p == TrainTrackSpline.TrackPosition.Next) ? this.points[this.points.Length - 1] : this.points[1];
		Vector3 from = base.transform.TransformPoint(position2) - base.transform.TransformPoint(position);
		Vector3 initialVector = TrainTrackSpline.GetInitialVector(track, p, o);
		float num = Vector3.SignedAngle(from, initialVector, Vector3.up);
		int num2 = 0;
		while (num2 < list.Count && list[num2].angle <= num)
		{
			num2++;
		}
		list.Insert(num2, new TrainTrackSpline.ConnectedTrackInfo(track, o, num));
		int num3 = int.MaxValue;
		for (int j = 0; j < list.Count; j++)
		{
			num3 = Mathf.Min(num3, list[j].track.hierarchy);
		}
		float num4 = float.MaxValue;
		int num5 = 0;
		for (int k = 0; k < list.Count; k++)
		{
			TrainTrackSpline.ConnectedTrackInfo connectedTrackInfo = list[k];
			if (connectedTrackInfo.track.hierarchy <= num3)
			{
				float num6 = Mathf.Abs(connectedTrackInfo.angle);
				if (num6 < num4)
				{
					num4 = num6;
					num5 = k;
					if (num4 == 0f)
					{
						break;
					}
				}
			}
		}
		if (p == TrainTrackSpline.TrackPosition.Next)
		{
			this.straightestNextIndex = num5;
			return;
		}
		this.straightestPrevIndex = num5;
	}

	// Token: 0x06002767 RID: 10087 RVA: 0x000F5FAC File Offset: 0x000F41AC
	public void RegisterTrackUser(TrainTrackSpline.ITrainTrackUser user)
	{
		this.trackUsers.Add(user);
	}

	// Token: 0x06002768 RID: 10088 RVA: 0x000F5FBB File Offset: 0x000F41BB
	public void DeregisterTrackUser(TrainTrackSpline.ITrainTrackUser user)
	{
		if (user == null)
		{
			return;
		}
		this.trackUsers.Remove(user);
	}

	// Token: 0x06002769 RID: 10089 RVA: 0x000F5FD0 File Offset: 0x000F41D0
	public bool IsForward(Vector3 askerForward, float askerSplineDist)
	{
		WorldSplineData data = base.GetData();
		Vector3 tangentCubicHermiteWorld = base.GetTangentCubicHermiteWorld(askerSplineDist, data);
		return Vector3.Dot(askerForward, tangentCubicHermiteWorld) >= 0f;
	}

	// Token: 0x0600276A RID: 10090 RVA: 0x000F6000 File Offset: 0x000F4200
	public bool HasValidHazardWithin(TrainCar asker, float askerSplineDist, float minHazardDist, float maxHazardDist, TrainTrackSpline.TrackSelection trackSelection, float trackSpeed, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		Vector3 askerForward = (trackSpeed >= 0f) ? asker.transform.forward : (-asker.transform.forward);
		bool movingForward = this.IsForward(askerForward, askerSplineDist);
		return this.HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist, maxHazardDist, trackSelection, movingForward, preferredAltA, preferredAltB);
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x000F6050 File Offset: 0x000F4250
	public bool HasValidHazardWithin(TrainTrackSpline.ITrainTrackUser asker, Vector3 askerForward, float askerSplineDist, float minHazardDist, float maxHazardDist, TrainTrackSpline.TrackSelection trackSelection, bool movingForward, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		WorldSplineData data = base.GetData();
		foreach (TrainTrackSpline.ITrainTrackUser trainTrackUser in this.trackUsers)
		{
			if (trainTrackUser != asker)
			{
				Vector3 rhs = trainTrackUser.Position - asker.Position;
				if (Vector3.Dot(askerForward, rhs) >= 0f)
				{
					float magnitude = rhs.magnitude;
					if (magnitude > minHazardDist && magnitude < maxHazardDist)
					{
						Vector3 worldVelocity = trainTrackUser.GetWorldVelocity();
						if (worldVelocity.sqrMagnitude < 4f || Vector3.Dot(worldVelocity, rhs) < 0f)
						{
							return true;
						}
					}
				}
			}
		}
		float num = movingForward ? (askerSplineDist + minHazardDist) : (askerSplineDist - minHazardDist);
		float num2 = movingForward ? (askerSplineDist + maxHazardDist) : (askerSplineDist - maxHazardDist);
		if (num2 < 0f)
		{
			if (this.HasPrevTrack)
			{
				TrainTrackSpline.ConnectedTrackInfo trackSelection2 = this.GetTrackSelection(this.prevTracks, this.straightestPrevIndex, trackSelection, false, movingForward, preferredAltA, preferredAltB);
				if (trackSelection2.orientation == TrainTrackSpline.TrackOrientation.Same)
				{
					askerSplineDist = trackSelection2.track.GetLength();
				}
				else
				{
					askerSplineDist = 0f;
					movingForward = !movingForward;
				}
				float minHazardDist2 = Mathf.Max(-num, 0f);
				float maxHazardDist2 = -num2;
				return trackSelection2.track.HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist2, maxHazardDist2, trackSelection, movingForward, preferredAltA, preferredAltB);
			}
		}
		else if (num2 > data.Length && this.HasNextTrack)
		{
			TrainTrackSpline.ConnectedTrackInfo trackSelection3 = this.GetTrackSelection(this.nextTracks, this.straightestNextIndex, trackSelection, true, movingForward, preferredAltA, preferredAltB);
			if (trackSelection3.orientation == TrainTrackSpline.TrackOrientation.Same)
			{
				askerSplineDist = 0f;
			}
			else
			{
				askerSplineDist = trackSelection3.track.GetLength();
				movingForward = !movingForward;
			}
			float minHazardDist3 = Mathf.Max(num - data.Length, 0f);
			float maxHazardDist3 = num2 - data.Length;
			return trackSelection3.track.HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist3, maxHazardDist3, trackSelection, movingForward, preferredAltA, preferredAltB);
		}
		return false;
	}

	// Token: 0x0600276C RID: 10092 RVA: 0x000F6254 File Offset: 0x000F4454
	public bool HasAnyUsers()
	{
		return this.trackUsers.Count > 0;
	}

	// Token: 0x0600276D RID: 10093 RVA: 0x000F6264 File Offset: 0x000F4464
	public bool HasAnyUsersOfType(TrainCar.TrainCarType carType)
	{
		using (HashSet<TrainTrackSpline.ITrainTrackUser>.Enumerator enumerator = this.trackUsers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.CarType == carType)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600276E RID: 10094 RVA: 0x000F62C0 File Offset: 0x000F44C0
	public bool HasConnectedTrack(TrainTrackSpline tts)
	{
		return this.HasConnectedNextTrack(tts) || this.HasConnectedPrevTrack(tts);
	}

	// Token: 0x0600276F RID: 10095 RVA: 0x000F62D4 File Offset: 0x000F44D4
	public bool HasConnectedNextTrack(TrainTrackSpline tts)
	{
		using (List<TrainTrackSpline.ConnectedTrackInfo>.Enumerator enumerator = this.nextTracks.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.track == tts)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002770 RID: 10096 RVA: 0x000F6334 File Offset: 0x000F4534
	public bool HasConnectedPrevTrack(TrainTrackSpline tts)
	{
		using (List<TrainTrackSpline.ConnectedTrackInfo>.Enumerator enumerator = this.prevTracks.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.track == tts)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002771 RID: 10097 RVA: 0x000F6394 File Offset: 0x000F4594
	private static Vector3 GetInitialVector(TrainTrackSpline track, TrainTrackSpline.TrackPosition p, TrainTrackSpline.TrackOrientation o)
	{
		Vector3 position;
		Vector3 position2;
		if (p == TrainTrackSpline.TrackPosition.Next)
		{
			if (o == TrainTrackSpline.TrackOrientation.Reverse)
			{
				position = track.points[track.points.Length - 1];
				position2 = track.points[track.points.Length - 2];
			}
			else
			{
				position = track.points[0];
				position2 = track.points[1];
			}
		}
		else if (o == TrainTrackSpline.TrackOrientation.Reverse)
		{
			position = track.points[1];
			position2 = track.points[0];
		}
		else
		{
			position = track.points[track.points.Length - 2];
			position2 = track.points[track.points.Length - 1];
		}
		return track.transform.TransformPoint(position2) - track.transform.TransformPoint(position);
	}

	// Token: 0x06002772 RID: 10098 RVA: 0x000F645C File Offset: 0x000F465C
	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		for (int i = 0; i < this.nextTracks.Count; i++)
		{
			Color splineColour = Color.white;
			if (this.straightestNextIndex != i && this.nextTracks.Count > 1)
			{
				if (i == 0)
				{
					splineColour = Color.green;
				}
				else if (i == this.nextTracks.Count - 1)
				{
					splineColour = Color.yellow;
				}
			}
			WorldSpline.DrawSplineGizmo(this.nextTracks[i].track, splineColour);
		}
		for (int j = 0; j < this.prevTracks.Count; j++)
		{
			Color splineColour2 = Color.white;
			if (this.straightestPrevIndex != j && this.prevTracks.Count > 1)
			{
				if (j == 0)
				{
					splineColour2 = Color.green;
				}
				else if (j == this.nextTracks.Count - 1)
				{
					splineColour2 = Color.yellow;
				}
			}
			WorldSpline.DrawSplineGizmo(this.prevTracks[j].track, splineColour2);
		}
	}

	// Token: 0x06002773 RID: 10099 RVA: 0x000F6548 File Offset: 0x000F4748
	private TrainTrackSpline.ConnectedTrackInfo GetTrackSelection(List<TrainTrackSpline.ConnectedTrackInfo> trackOptions, int straightestIndex, TrainTrackSpline.TrackSelection trackSelection, bool nextTrack, bool trainForward, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		if (trackOptions.Count == 1)
		{
			return trackOptions[0];
		}
		foreach (TrainTrackSpline.ConnectedTrackInfo connectedTrackInfo in trackOptions)
		{
			if (connectedTrackInfo.track == preferredAltA || connectedTrackInfo.track == preferredAltB)
			{
				return connectedTrackInfo;
			}
		}
		bool flag = nextTrack ^ trainForward;
		if (trackSelection != TrainTrackSpline.TrackSelection.Left)
		{
			if (trackSelection != TrainTrackSpline.TrackSelection.Right)
			{
				return trackOptions[straightestIndex];
			}
			if (!flag)
			{
				return trackOptions[trackOptions.Count - 1];
			}
			return trackOptions[0];
		}
		else
		{
			if (!flag)
			{
				return trackOptions[0];
			}
			return trackOptions[trackOptions.Count - 1];
		}
		TrainTrackSpline.ConnectedTrackInfo result;
		return result;
	}

	// Token: 0x06002774 RID: 10100 RVA: 0x000F6610 File Offset: 0x000F4810
	public static bool TryFindTrackNear(Vector3 pos, float maxDist, out TrainTrackSpline splineResult, out float distResult)
	{
		splineResult = null;
		distResult = 0f;
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, maxDist, list, 65536, QueryTriggerInteraction.Ignore);
		if (list.Count > 0)
		{
			List<TrainTrackSpline> list2 = Pool.GetList<TrainTrackSpline>();
			float num = float.MaxValue;
			foreach (Collider collider in list)
			{
				collider.GetComponentsInParent<TrainTrackSpline>(false, list2);
				if (list2.Count > 0)
				{
					foreach (TrainTrackSpline trainTrackSpline in list2)
					{
						float num2;
						float distance = trainTrackSpline.GetDistance(pos, 1f, out num2);
						if (distance < num)
						{
							num = distance;
							distResult = num2;
							splineResult = trainTrackSpline;
						}
					}
				}
			}
			Pool.FreeList<TrainTrackSpline>(ref list2);
		}
		Pool.FreeList<Collider>(ref list);
		return splineResult != null;
	}

	// Token: 0x02000D0F RID: 3343
	public enum TrackSelection
	{
		// Token: 0x0400460B RID: 17931
		Default,
		// Token: 0x0400460C RID: 17932
		Left,
		// Token: 0x0400460D RID: 17933
		Right
	}

	// Token: 0x02000D10 RID: 3344
	public enum TrackPosition
	{
		// Token: 0x0400460F RID: 17935
		Next,
		// Token: 0x04004610 RID: 17936
		Prev
	}

	// Token: 0x02000D11 RID: 3345
	public enum TrackOrientation
	{
		// Token: 0x04004612 RID: 17938
		Same,
		// Token: 0x04004613 RID: 17939
		Reverse
	}

	// Token: 0x02000D12 RID: 3346
	private class ConnectedTrackInfo
	{
		// Token: 0x04004614 RID: 17940
		public TrainTrackSpline track;

		// Token: 0x04004615 RID: 17941
		public TrainTrackSpline.TrackOrientation orientation;

		// Token: 0x04004616 RID: 17942
		public float angle;

		// Token: 0x0600501D RID: 20509 RVA: 0x001A7FA1 File Offset: 0x001A61A1
		public ConnectedTrackInfo(TrainTrackSpline track, TrainTrackSpline.TrackOrientation orientation, float angle)
		{
			this.track = track;
			this.orientation = orientation;
			this.angle = angle;
		}
	}

	// Token: 0x02000D13 RID: 3347
	public enum DistanceType
	{
		// Token: 0x04004618 RID: 17944
		SplineDistance,
		// Token: 0x04004619 RID: 17945
		WorldDistance
	}

	// Token: 0x02000D14 RID: 3348
	public interface ITrainTrackUser
	{
		// Token: 0x170006A6 RID: 1702
		// (get) Token: 0x0600501E RID: 20510
		Vector3 Position { get; }

		// Token: 0x170006A7 RID: 1703
		// (get) Token: 0x0600501F RID: 20511
		float FrontWheelSplineDist { get; }

		// Token: 0x06005020 RID: 20512
		Vector3 GetWorldVelocity();

		// Token: 0x170006A8 RID: 1704
		// (get) Token: 0x06005021 RID: 20513
		TrainCar.TrainCarType CarType { get; }
	}
}
