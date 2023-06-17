using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x0200099C RID: 2460
public class OccludeeState : OcclusionCulling.SmartListValue
{
	// Token: 0x040034DC RID: 13532
	public int slot;

	// Token: 0x040034DD RID: 13533
	public bool isStatic;

	// Token: 0x040034DE RID: 13534
	public int layer;

	// Token: 0x040034DF RID: 13535
	public OcclusionCulling.OnVisibilityChanged onVisibilityChanged;

	// Token: 0x040034E0 RID: 13536
	public OcclusionCulling.Cell cell;

	// Token: 0x040034E1 RID: 13537
	public OcclusionCulling.SimpleList<OccludeeState.State> states;

	// Token: 0x170004A1 RID: 1185
	// (get) Token: 0x06003A7B RID: 14971 RVA: 0x00159D16 File Offset: 0x00157F16
	public bool isVisible
	{
		get
		{
			return this.states[this.slot].isVisible > 0;
		}
	}

	// Token: 0x06003A7C RID: 14972 RVA: 0x00159D34 File Offset: 0x00157F34
	public OccludeeState Initialize(OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.BufferSet set, int slot, Vector4 sphereBounds, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged)
	{
		states[slot] = new OccludeeState.State
		{
			sphereBounds = sphereBounds,
			minTimeVisible = minTimeVisible,
			waitTime = (isVisible ? (Time.time + minTimeVisible) : 0f),
			waitFrame = (uint)(Time.frameCount + 1),
			isVisible = (isVisible ? 1 : 0),
			active = 1,
			callback = ((onVisibilityChanged != null) ? 1 : 0)
		};
		this.slot = slot;
		this.isStatic = isStatic;
		this.layer = layer;
		this.onVisibilityChanged = onVisibilityChanged;
		this.cell = null;
		this.states = states;
		return this;
	}

	// Token: 0x06003A7D RID: 14973 RVA: 0x00159DDF File Offset: 0x00157FDF
	public void Invalidate()
	{
		this.states[this.slot] = OccludeeState.State.Unused;
		this.slot = -1;
		this.onVisibilityChanged = null;
		this.cell = null;
	}

	// Token: 0x06003A7E RID: 14974 RVA: 0x00159E0C File Offset: 0x0015800C
	public void MakeVisible()
	{
		this.states.array[this.slot].waitTime = Time.time + this.states[this.slot].minTimeVisible;
		this.states.array[this.slot].isVisible = 1;
		if (this.onVisibilityChanged != null)
		{
			this.onVisibilityChanged(true);
		}
	}

	// Token: 0x02000ED3 RID: 3795
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
	public struct State
	{
		// Token: 0x04004D22 RID: 19746
		[FieldOffset(0)]
		public Vector4 sphereBounds;

		// Token: 0x04004D23 RID: 19747
		[FieldOffset(16)]
		public float minTimeVisible;

		// Token: 0x04004D24 RID: 19748
		[FieldOffset(20)]
		public float waitTime;

		// Token: 0x04004D25 RID: 19749
		[FieldOffset(24)]
		public uint waitFrame;

		// Token: 0x04004D26 RID: 19750
		[FieldOffset(28)]
		public byte isVisible;

		// Token: 0x04004D27 RID: 19751
		[FieldOffset(29)]
		public byte active;

		// Token: 0x04004D28 RID: 19752
		[FieldOffset(30)]
		public byte callback;

		// Token: 0x04004D29 RID: 19753
		[FieldOffset(31)]
		public byte pad1;

		// Token: 0x04004D2A RID: 19754
		public static OccludeeState.State Unused = new OccludeeState.State
		{
			active = 0
		};
	}
}
