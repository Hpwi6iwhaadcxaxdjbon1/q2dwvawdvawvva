using System;
using Rust.Interpolation;
using UnityEngine;

// Token: 0x020002DB RID: 731
public class PositionLerp : IDisposable
{
	// Token: 0x040016D9 RID: 5849
	private static ListHashSet<PositionLerp> InstanceList = new ListHashSet<PositionLerp>(8);

	// Token: 0x040016DA RID: 5850
	public static bool DebugLog = false;

	// Token: 0x040016DB RID: 5851
	public static bool DebugDraw = false;

	// Token: 0x040016DC RID: 5852
	public static int TimeOffsetInterval = 16;

	// Token: 0x040016DD RID: 5853
	public static float TimeOffset = 0f;

	// Token: 0x040016DE RID: 5854
	public const int TimeOffsetIntervalMin = 4;

	// Token: 0x040016DF RID: 5855
	public const int TimeOffsetIntervalMax = 64;

	// Token: 0x040016E0 RID: 5856
	private bool enabled = true;

	// Token: 0x040016E1 RID: 5857
	private Action idleDisable;

	// Token: 0x040016E2 RID: 5858
	private Interpolator<TransformSnapshot> interpolator = new Interpolator<TransformSnapshot>(32);

	// Token: 0x040016E3 RID: 5859
	private IPosLerpTarget target;

	// Token: 0x040016E4 RID: 5860
	private static TransformSnapshot snapshotPrototype = default(TransformSnapshot);

	// Token: 0x040016E5 RID: 5861
	private float timeOffset0 = float.MaxValue;

	// Token: 0x040016E6 RID: 5862
	private float timeOffset1 = float.MaxValue;

	// Token: 0x040016E7 RID: 5863
	private float timeOffset2 = float.MaxValue;

	// Token: 0x040016E8 RID: 5864
	private float timeOffset3 = float.MaxValue;

	// Token: 0x040016E9 RID: 5865
	private int timeOffsetCount;

	// Token: 0x040016EA RID: 5866
	private float lastClientTime;

	// Token: 0x040016EB RID: 5867
	private float lastServerTime;

	// Token: 0x040016EC RID: 5868
	private float extrapolatedTime;

	// Token: 0x040016ED RID: 5869
	private float enabledTime;

	// Token: 0x17000274 RID: 628
	// (get) Token: 0x06001DC4 RID: 7620 RVA: 0x000CB9C5 File Offset: 0x000C9BC5
	// (set) Token: 0x06001DC5 RID: 7621 RVA: 0x000CB9CD File Offset: 0x000C9BCD
	public bool Enabled
	{
		get
		{
			return this.enabled;
		}
		set
		{
			this.enabled = value;
			if (this.enabled)
			{
				this.OnEnable();
				return;
			}
			this.OnDisable();
		}
	}

	// Token: 0x17000275 RID: 629
	// (get) Token: 0x06001DC6 RID: 7622 RVA: 0x000CB9EB File Offset: 0x000C9BEB
	public static float LerpTime
	{
		get
		{
			return Time.time;
		}
	}

	// Token: 0x06001DC7 RID: 7623 RVA: 0x000CB9F2 File Offset: 0x000C9BF2
	private void OnEnable()
	{
		PositionLerp.InstanceList.Add(this);
		this.enabledTime = PositionLerp.LerpTime;
	}

	// Token: 0x06001DC8 RID: 7624 RVA: 0x000CBA0A File Offset: 0x000C9C0A
	private void OnDisable()
	{
		PositionLerp.InstanceList.Remove(this);
	}

	// Token: 0x06001DC9 RID: 7625 RVA: 0x000CBA18 File Offset: 0x000C9C18
	public void Initialize(IPosLerpTarget target)
	{
		this.target = target;
		this.Enabled = true;
	}

	// Token: 0x06001DCA RID: 7626 RVA: 0x000CBA28 File Offset: 0x000C9C28
	public void Snapshot(Vector3 position, Quaternion rotation, float serverTime)
	{
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		float num = interpolationDelay + interpolationSmoothing + 1f;
		float num2 = PositionLerp.LerpTime;
		this.timeOffset0 = Mathf.Min(this.timeOffset0, num2 - serverTime);
		this.timeOffsetCount++;
		if (this.timeOffsetCount >= PositionLerp.TimeOffsetInterval / 4)
		{
			this.timeOffset3 = this.timeOffset2;
			this.timeOffset2 = this.timeOffset1;
			this.timeOffset1 = this.timeOffset0;
			this.timeOffset0 = float.MaxValue;
			this.timeOffsetCount = 0;
		}
		PositionLerp.TimeOffset = Mathx.Min(this.timeOffset0, this.timeOffset1, this.timeOffset2, this.timeOffset3);
		num2 = serverTime + PositionLerp.TimeOffset;
		if (PositionLerp.DebugLog && this.interpolator.list.Count > 0 && serverTime < this.lastServerTime)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				this.target.ToString(),
				" adding tick from the past: server time ",
				serverTime,
				" < ",
				this.lastServerTime
			}));
		}
		else if (PositionLerp.DebugLog && this.interpolator.list.Count > 0 && num2 < this.lastClientTime)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				this.target.ToString(),
				" adding tick from the past: client time ",
				num2,
				" < ",
				this.lastClientTime
			}));
		}
		else
		{
			this.lastClientTime = num2;
			this.lastServerTime = serverTime;
			this.interpolator.Add(new TransformSnapshot(num2, position, rotation));
		}
		this.interpolator.Cull(num2 - num);
	}

	// Token: 0x06001DCB RID: 7627 RVA: 0x000CBBF2 File Offset: 0x000C9DF2
	public void Snapshot(Vector3 position, Quaternion rotation)
	{
		this.Snapshot(position, rotation, PositionLerp.LerpTime - PositionLerp.TimeOffset);
	}

	// Token: 0x06001DCC RID: 7628 RVA: 0x000CBC07 File Offset: 0x000C9E07
	public void SnapTo(Vector3 position, Quaternion rotation, float serverTime)
	{
		this.interpolator.Clear();
		this.Snapshot(position, rotation, serverTime);
		this.target.SetNetworkPosition(position);
		this.target.SetNetworkRotation(rotation);
	}

	// Token: 0x06001DCD RID: 7629 RVA: 0x000CBC35 File Offset: 0x000C9E35
	public void SnapTo(Vector3 position, Quaternion rotation)
	{
		this.interpolator.last = new TransformSnapshot(PositionLerp.LerpTime, position, rotation);
		this.Wipe();
	}

	// Token: 0x06001DCE RID: 7630 RVA: 0x000CBC54 File Offset: 0x000C9E54
	public void SnapToEnd()
	{
		float interpolationDelay = this.target.GetInterpolationDelay();
		Interpolator<TransformSnapshot>.Segment segment = this.interpolator.Query(PositionLerp.LerpTime, interpolationDelay, 0f, 0f, ref PositionLerp.snapshotPrototype);
		this.target.SetNetworkPosition(segment.tick.pos);
		this.target.SetNetworkRotation(segment.tick.rot);
		this.Wipe();
	}

	// Token: 0x06001DCF RID: 7631 RVA: 0x000CBCC0 File Offset: 0x000C9EC0
	public void Wipe()
	{
		this.interpolator.Clear();
		this.timeOffsetCount = 0;
		this.timeOffset0 = float.MaxValue;
		this.timeOffset1 = float.MaxValue;
		this.timeOffset2 = float.MaxValue;
		this.timeOffset3 = float.MaxValue;
	}

	// Token: 0x06001DD0 RID: 7632 RVA: 0x000CBD00 File Offset: 0x000C9F00
	public static void WipeAll()
	{
		foreach (PositionLerp positionLerp in PositionLerp.InstanceList)
		{
			positionLerp.Wipe();
		}
	}

	// Token: 0x06001DD1 RID: 7633 RVA: 0x000CBD50 File Offset: 0x000C9F50
	protected void DoCycle()
	{
		if (this.target == null)
		{
			return;
		}
		float interpolationInertia = this.target.GetInterpolationInertia();
		float num = (interpolationInertia > 0f) ? Mathf.InverseLerp(0f, interpolationInertia, PositionLerp.LerpTime - this.enabledTime) : 1f;
		float extrapolationTime = this.target.GetExtrapolationTime();
		float interpolation = this.target.GetInterpolationDelay() * num;
		float num2 = this.target.GetInterpolationSmoothing() * num;
		Interpolator<TransformSnapshot>.Segment segment = this.interpolator.Query(PositionLerp.LerpTime, interpolation, extrapolationTime, num2, ref PositionLerp.snapshotPrototype);
		if (segment.next.Time >= this.interpolator.last.Time)
		{
			this.extrapolatedTime = Mathf.Min(this.extrapolatedTime + Time.deltaTime, extrapolationTime);
		}
		else
		{
			this.extrapolatedTime = Mathf.Max(this.extrapolatedTime - Time.deltaTime, 0f);
		}
		if (this.extrapolatedTime > 0f && extrapolationTime > 0f && num2 > 0f)
		{
			float t = Time.deltaTime / (this.extrapolatedTime / extrapolationTime * num2);
			segment.tick.pos = Vector3.Lerp(this.target.GetNetworkPosition(), segment.tick.pos, t);
			segment.tick.rot = Quaternion.Slerp(this.target.GetNetworkRotation(), segment.tick.rot, t);
		}
		this.target.SetNetworkPosition(segment.tick.pos);
		this.target.SetNetworkRotation(segment.tick.rot);
		if (PositionLerp.DebugDraw)
		{
			this.target.DrawInterpolationState(segment, this.interpolator.list);
		}
		if (PositionLerp.LerpTime - this.lastClientTime > 10f)
		{
			if (this.idleDisable == null)
			{
				this.idleDisable = new Action(this.target.LerpIdleDisable);
			}
			InvokeHandler.Invoke(this.target as Behaviour, this.idleDisable, 0f);
		}
	}

	// Token: 0x06001DD2 RID: 7634 RVA: 0x000CBF54 File Offset: 0x000CA154
	public void TransformEntries(Matrix4x4 matrix)
	{
		Quaternion rotation = matrix.rotation;
		for (int i = 0; i < this.interpolator.list.Count; i++)
		{
			TransformSnapshot transformSnapshot = this.interpolator.list[i];
			transformSnapshot.pos = matrix.MultiplyPoint3x4(transformSnapshot.pos);
			transformSnapshot.rot = rotation * transformSnapshot.rot;
			this.interpolator.list[i] = transformSnapshot;
		}
		this.interpolator.last.pos = matrix.MultiplyPoint3x4(this.interpolator.last.pos);
		this.interpolator.last.rot = rotation * this.interpolator.last.rot;
	}

	// Token: 0x06001DD3 RID: 7635 RVA: 0x000CC01C File Offset: 0x000CA21C
	public Quaternion GetEstimatedAngularVelocity()
	{
		if (this.target == null)
		{
			return Quaternion.identity;
		}
		float extrapolationTime = this.target.GetExtrapolationTime();
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		Interpolator<TransformSnapshot>.Segment segment = this.interpolator.Query(PositionLerp.LerpTime, interpolationDelay, extrapolationTime, interpolationSmoothing, ref PositionLerp.snapshotPrototype);
		TransformSnapshot next = segment.next;
		TransformSnapshot prev = segment.prev;
		if (next.Time == prev.Time)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler((prev.rot.eulerAngles - next.rot.eulerAngles) / (prev.Time - next.Time));
	}

	// Token: 0x06001DD4 RID: 7636 RVA: 0x000CC0D0 File Offset: 0x000CA2D0
	public Vector3 GetEstimatedVelocity()
	{
		if (this.target == null)
		{
			return Vector3.zero;
		}
		float extrapolationTime = this.target.GetExtrapolationTime();
		float interpolationDelay = this.target.GetInterpolationDelay();
		float interpolationSmoothing = this.target.GetInterpolationSmoothing();
		Interpolator<TransformSnapshot>.Segment segment = this.interpolator.Query(PositionLerp.LerpTime, interpolationDelay, extrapolationTime, interpolationSmoothing, ref PositionLerp.snapshotPrototype);
		TransformSnapshot next = segment.next;
		TransformSnapshot prev = segment.prev;
		if (next.Time == prev.Time)
		{
			return Vector3.zero;
		}
		return (prev.pos - next.pos) / (prev.Time - next.Time);
	}

	// Token: 0x06001DD5 RID: 7637 RVA: 0x000CC174 File Offset: 0x000CA374
	public void Dispose()
	{
		this.target = null;
		this.idleDisable = null;
		this.interpolator.Clear();
		this.timeOffset0 = float.MaxValue;
		this.timeOffset1 = float.MaxValue;
		this.timeOffset2 = float.MaxValue;
		this.timeOffset3 = float.MaxValue;
		this.lastClientTime = 0f;
		this.lastServerTime = 0f;
		this.extrapolatedTime = 0f;
		this.timeOffsetCount = 0;
		this.Enabled = false;
	}

	// Token: 0x06001DD6 RID: 7638 RVA: 0x000CC1F5 File Offset: 0x000CA3F5
	public static void Clear()
	{
		PositionLerp.InstanceList.Clear();
	}

	// Token: 0x06001DD7 RID: 7639 RVA: 0x000CC204 File Offset: 0x000CA404
	public static void Cycle()
	{
		PositionLerp[] buffer = PositionLerp.InstanceList.Values.Buffer;
		int count = PositionLerp.InstanceList.Count;
		for (int i = 0; i < count; i++)
		{
			buffer[i].DoCycle();
		}
	}
}
