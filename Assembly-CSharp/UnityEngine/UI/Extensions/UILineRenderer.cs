using System;
using System.Collections.Generic;
using UnityEngine.Sprites;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A33 RID: 2611
	[AddComponentMenu("UI/Extensions/Primitives/UILineRenderer")]
	[RequireComponent(typeof(RectTransform))]
	public class UILineRenderer : UIPrimitiveBase
	{
		// Token: 0x040037B2 RID: 14258
		private const float MIN_MITER_JOIN = 0.2617994f;

		// Token: 0x040037B3 RID: 14259
		private const float MIN_BEVEL_NICE_JOIN = 0.5235988f;

		// Token: 0x040037B4 RID: 14260
		private static Vector2 UV_TOP_LEFT;

		// Token: 0x040037B5 RID: 14261
		private static Vector2 UV_BOTTOM_LEFT;

		// Token: 0x040037B6 RID: 14262
		private static Vector2 UV_TOP_CENTER_LEFT;

		// Token: 0x040037B7 RID: 14263
		private static Vector2 UV_TOP_CENTER_RIGHT;

		// Token: 0x040037B8 RID: 14264
		private static Vector2 UV_BOTTOM_CENTER_LEFT;

		// Token: 0x040037B9 RID: 14265
		private static Vector2 UV_BOTTOM_CENTER_RIGHT;

		// Token: 0x040037BA RID: 14266
		private static Vector2 UV_TOP_RIGHT;

		// Token: 0x040037BB RID: 14267
		private static Vector2 UV_BOTTOM_RIGHT;

		// Token: 0x040037BC RID: 14268
		private static Vector2[] startUvs;

		// Token: 0x040037BD RID: 14269
		private static Vector2[] middleUvs;

		// Token: 0x040037BE RID: 14270
		private static Vector2[] endUvs;

		// Token: 0x040037BF RID: 14271
		private static Vector2[] fullUvs;

		// Token: 0x040037C0 RID: 14272
		[SerializeField]
		[Tooltip("Points to draw lines between\n Can be improved using the Resolution Option")]
		internal Vector2[] m_points;

		// Token: 0x040037C1 RID: 14273
		[SerializeField]
		[Tooltip("Segments to be drawn\n This is a list of arrays of points")]
		internal List<Vector2[]> m_segments;

		// Token: 0x040037C2 RID: 14274
		[SerializeField]
		[Tooltip("Thickness of the line")]
		internal float lineThickness = 2f;

		// Token: 0x040037C3 RID: 14275
		[SerializeField]
		[Tooltip("Use the relative bounds of the Rect Transform (0,0 -> 0,1) or screen space coordinates")]
		internal bool relativeSize;

		// Token: 0x040037C4 RID: 14276
		[SerializeField]
		[Tooltip("Do the points identify a single line or split pairs of lines")]
		internal bool lineList;

		// Token: 0x040037C5 RID: 14277
		[SerializeField]
		[Tooltip("Add end caps to each line\nMultiple caps when used with Line List")]
		internal bool lineCaps;

		// Token: 0x040037C6 RID: 14278
		[SerializeField]
		[Tooltip("Resolution of the Bezier curve, different to line Resolution")]
		internal int bezierSegmentsPerCurve = 10;

		// Token: 0x040037C7 RID: 14279
		[Tooltip("The type of Join used between lines, Square/Mitre or Curved/Bevel")]
		public UILineRenderer.JoinType LineJoins;

		// Token: 0x040037C8 RID: 14280
		[Tooltip("Bezier method to apply to line, see docs for options\nCan't be used in conjunction with Resolution as Bezier already changes the resolution")]
		public UILineRenderer.BezierType BezierMode;

		// Token: 0x040037C9 RID: 14281
		[HideInInspector]
		public bool drivenExternally;

		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x06003E58 RID: 15960 RVA: 0x0016DB49 File Offset: 0x0016BD49
		// (set) Token: 0x06003E59 RID: 15961 RVA: 0x0016DB51 File Offset: 0x0016BD51
		public float LineThickness
		{
			get
			{
				return this.lineThickness;
			}
			set
			{
				this.lineThickness = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x06003E5A RID: 15962 RVA: 0x0016DB60 File Offset: 0x0016BD60
		// (set) Token: 0x06003E5B RID: 15963 RVA: 0x0016DB68 File Offset: 0x0016BD68
		public bool RelativeSize
		{
			get
			{
				return this.relativeSize;
			}
			set
			{
				this.relativeSize = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x06003E5C RID: 15964 RVA: 0x0016DB77 File Offset: 0x0016BD77
		// (set) Token: 0x06003E5D RID: 15965 RVA: 0x0016DB7F File Offset: 0x0016BD7F
		public bool LineList
		{
			get
			{
				return this.lineList;
			}
			set
			{
				this.lineList = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x06003E5E RID: 15966 RVA: 0x0016DB8E File Offset: 0x0016BD8E
		// (set) Token: 0x06003E5F RID: 15967 RVA: 0x0016DB96 File Offset: 0x0016BD96
		public bool LineCaps
		{
			get
			{
				return this.lineCaps;
			}
			set
			{
				this.lineCaps = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x06003E60 RID: 15968 RVA: 0x0016DBA5 File Offset: 0x0016BDA5
		// (set) Token: 0x06003E61 RID: 15969 RVA: 0x0016DBAD File Offset: 0x0016BDAD
		public int BezierSegmentsPerCurve
		{
			get
			{
				return this.bezierSegmentsPerCurve;
			}
			set
			{
				this.bezierSegmentsPerCurve = value;
			}
		}

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x06003E62 RID: 15970 RVA: 0x0016DBB6 File Offset: 0x0016BDB6
		// (set) Token: 0x06003E63 RID: 15971 RVA: 0x0016DBBE File Offset: 0x0016BDBE
		public Vector2[] Points
		{
			get
			{
				return this.m_points;
			}
			set
			{
				if (this.m_points == value)
				{
					return;
				}
				this.m_points = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x06003E64 RID: 15972 RVA: 0x0016DBD7 File Offset: 0x0016BDD7
		// (set) Token: 0x06003E65 RID: 15973 RVA: 0x0016DBDF File Offset: 0x0016BDDF
		public List<Vector2[]> Segments
		{
			get
			{
				return this.m_segments;
			}
			set
			{
				this.m_segments = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x06003E66 RID: 15974 RVA: 0x0016DBF0 File Offset: 0x0016BDF0
		private void PopulateMesh(VertexHelper vh, Vector2[] pointsToDraw)
		{
			if (this.BezierMode != UILineRenderer.BezierType.None && this.BezierMode != UILineRenderer.BezierType.Catenary && pointsToDraw.Length > 3)
			{
				BezierPath bezierPath = new BezierPath();
				bezierPath.SetControlPoints(pointsToDraw);
				bezierPath.SegmentsPerCurve = this.bezierSegmentsPerCurve;
				UILineRenderer.BezierType bezierMode = this.BezierMode;
				List<Vector2> list;
				if (bezierMode != UILineRenderer.BezierType.Basic)
				{
					if (bezierMode != UILineRenderer.BezierType.Improved)
					{
						list = bezierPath.GetDrawingPoints2();
					}
					else
					{
						list = bezierPath.GetDrawingPoints1();
					}
				}
				else
				{
					list = bezierPath.GetDrawingPoints0();
				}
				pointsToDraw = list.ToArray();
			}
			if (this.BezierMode == UILineRenderer.BezierType.Catenary && pointsToDraw.Length == 2)
			{
				pointsToDraw = new CableCurve(pointsToDraw)
				{
					slack = base.Resoloution,
					steps = this.BezierSegmentsPerCurve
				}.Points();
			}
			if (base.ImproveResolution != ResolutionMode.None)
			{
				pointsToDraw = base.IncreaseResolution(pointsToDraw);
			}
			float num = (!this.relativeSize) ? 1f : base.rectTransform.rect.width;
			float num2 = (!this.relativeSize) ? 1f : base.rectTransform.rect.height;
			float num3 = -base.rectTransform.pivot.x * num;
			float num4 = -base.rectTransform.pivot.y * num2;
			List<UIVertex[]> list2 = new List<UIVertex[]>();
			if (this.lineList)
			{
				for (int i = 1; i < pointsToDraw.Length; i += 2)
				{
					Vector2 vector = pointsToDraw[i - 1];
					Vector2 vector2 = pointsToDraw[i];
					vector = new Vector2(vector.x * num + num3, vector.y * num2 + num4);
					vector2 = new Vector2(vector2.x * num + num3, vector2.y * num2 + num4);
					if (this.lineCaps)
					{
						list2.Add(this.CreateLineCap(vector, vector2, UILineRenderer.SegmentType.Start));
					}
					list2.Add(this.CreateLineSegment(vector, vector2, UILineRenderer.SegmentType.Middle, (list2.Count > 1) ? list2[list2.Count - 2] : null));
					if (this.lineCaps)
					{
						list2.Add(this.CreateLineCap(vector, vector2, UILineRenderer.SegmentType.End));
					}
				}
			}
			else
			{
				for (int j = 1; j < pointsToDraw.Length; j++)
				{
					Vector2 vector3 = pointsToDraw[j - 1];
					Vector2 vector4 = pointsToDraw[j];
					vector3 = new Vector2(vector3.x * num + num3, vector3.y * num2 + num4);
					vector4 = new Vector2(vector4.x * num + num3, vector4.y * num2 + num4);
					if (this.lineCaps && j == 1)
					{
						list2.Add(this.CreateLineCap(vector3, vector4, UILineRenderer.SegmentType.Start));
					}
					list2.Add(this.CreateLineSegment(vector3, vector4, UILineRenderer.SegmentType.Middle, null));
					if (this.lineCaps && j == pointsToDraw.Length - 1)
					{
						list2.Add(this.CreateLineCap(vector3, vector4, UILineRenderer.SegmentType.End));
					}
				}
			}
			for (int k = 0; k < list2.Count; k++)
			{
				if (!this.lineList && k < list2.Count - 1)
				{
					Vector3 v = list2[k][1].position - list2[k][2].position;
					Vector3 v2 = list2[k + 1][2].position - list2[k + 1][1].position;
					float num5 = Vector2.Angle(v, v2) * 0.017453292f;
					float num6 = Mathf.Sign(Vector3.Cross(v.normalized, v2.normalized).z);
					float num7 = this.lineThickness / (2f * Mathf.Tan(num5 / 2f));
					Vector3 position = list2[k][2].position - v.normalized * num7 * num6;
					Vector3 position2 = list2[k][3].position + v.normalized * num7 * num6;
					UILineRenderer.JoinType joinType = this.LineJoins;
					if (joinType == UILineRenderer.JoinType.Miter)
					{
						if (num7 < v.magnitude / 2f && num7 < v2.magnitude / 2f && num5 > 0.2617994f)
						{
							list2[k][2].position = position;
							list2[k][3].position = position2;
							list2[k + 1][0].position = position2;
							list2[k + 1][1].position = position;
						}
						else
						{
							joinType = UILineRenderer.JoinType.Bevel;
						}
					}
					if (joinType == UILineRenderer.JoinType.Bevel)
					{
						if (num7 < v.magnitude / 2f && num7 < v2.magnitude / 2f && num5 > 0.5235988f)
						{
							if (num6 < 0f)
							{
								list2[k][2].position = position;
								list2[k + 1][1].position = position;
							}
							else
							{
								list2[k][3].position = position2;
								list2[k + 1][0].position = position2;
							}
						}
						UIVertex[] verts = new UIVertex[]
						{
							list2[k][2],
							list2[k][3],
							list2[k + 1][0],
							list2[k + 1][1]
						};
						vh.AddUIVertexQuad(verts);
					}
				}
				vh.AddUIVertexQuad(list2[k]);
			}
			if (vh.currentVertCount > 64000)
			{
				Debug.LogError("Max Verticies size is 64000, current mesh vertcies count is [" + vh.currentVertCount + "] - Cannot Draw");
				vh.Clear();
				return;
			}
		}

		// Token: 0x06003E67 RID: 15975 RVA: 0x0016E1F8 File Offset: 0x0016C3F8
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (this.m_points != null && this.m_points.Length != 0)
			{
				this.GeneratedUVs();
				vh.Clear();
				this.PopulateMesh(vh, this.m_points);
				return;
			}
			if (this.m_segments != null && this.m_segments.Count > 0)
			{
				this.GeneratedUVs();
				vh.Clear();
				for (int i = 0; i < this.m_segments.Count; i++)
				{
					Vector2[] pointsToDraw = this.m_segments[i];
					this.PopulateMesh(vh, pointsToDraw);
				}
			}
		}

		// Token: 0x06003E68 RID: 15976 RVA: 0x0016E280 File Offset: 0x0016C480
		private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, UILineRenderer.SegmentType type)
		{
			if (type == UILineRenderer.SegmentType.Start)
			{
				Vector2 start2 = start - (end - start).normalized * this.lineThickness / 2f;
				return this.CreateLineSegment(start2, start, UILineRenderer.SegmentType.Start, null);
			}
			if (type == UILineRenderer.SegmentType.End)
			{
				Vector2 end2 = end + (end - start).normalized * this.lineThickness / 2f;
				return this.CreateLineSegment(end, end2, UILineRenderer.SegmentType.End, null);
			}
			Debug.LogError("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
			return null;
		}

		// Token: 0x06003E69 RID: 15977 RVA: 0x0016E30C File Offset: 0x0016C50C
		private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, UILineRenderer.SegmentType type, UIVertex[] previousVert = null)
		{
			Vector2 b = new Vector2(start.y - end.y, end.x - start.x).normalized * this.lineThickness / 2f;
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			if (previousVert != null)
			{
				vector = new Vector2(previousVert[3].position.x, previousVert[3].position.y);
				vector2 = new Vector2(previousVert[2].position.x, previousVert[2].position.y);
			}
			else
			{
				vector = start - b;
				vector2 = start + b;
			}
			Vector2 vector3 = end + b;
			Vector2 vector4 = end - b;
			switch (type)
			{
			case UILineRenderer.SegmentType.Start:
				return base.SetVbo(new Vector2[]
				{
					vector,
					vector2,
					vector3,
					vector4
				}, UILineRenderer.startUvs);
			case UILineRenderer.SegmentType.End:
				return base.SetVbo(new Vector2[]
				{
					vector,
					vector2,
					vector3,
					vector4
				}, UILineRenderer.endUvs);
			case UILineRenderer.SegmentType.Full:
				return base.SetVbo(new Vector2[]
				{
					vector,
					vector2,
					vector3,
					vector4
				}, UILineRenderer.fullUvs);
			}
			return base.SetVbo(new Vector2[]
			{
				vector,
				vector2,
				vector3,
				vector4
			}, UILineRenderer.middleUvs);
		}

		// Token: 0x06003E6A RID: 15978 RVA: 0x0016E4C8 File Offset: 0x0016C6C8
		protected override void GeneratedUVs()
		{
			if (base.activeSprite != null)
			{
				Vector4 outerUV = DataUtility.GetOuterUV(base.activeSprite);
				Vector4 innerUV = DataUtility.GetInnerUV(base.activeSprite);
				UILineRenderer.UV_TOP_LEFT = new Vector2(outerUV.x, outerUV.y);
				UILineRenderer.UV_BOTTOM_LEFT = new Vector2(outerUV.x, outerUV.w);
				UILineRenderer.UV_TOP_CENTER_LEFT = new Vector2(innerUV.x, innerUV.y);
				UILineRenderer.UV_TOP_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.y);
				UILineRenderer.UV_BOTTOM_CENTER_LEFT = new Vector2(innerUV.x, innerUV.w);
				UILineRenderer.UV_BOTTOM_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.w);
				UILineRenderer.UV_TOP_RIGHT = new Vector2(outerUV.z, outerUV.y);
				UILineRenderer.UV_BOTTOM_RIGHT = new Vector2(outerUV.z, outerUV.w);
			}
			else
			{
				UILineRenderer.UV_TOP_LEFT = Vector2.zero;
				UILineRenderer.UV_BOTTOM_LEFT = new Vector2(0f, 1f);
				UILineRenderer.UV_TOP_CENTER_LEFT = new Vector2(0.5f, 0f);
				UILineRenderer.UV_TOP_CENTER_RIGHT = new Vector2(0.5f, 0f);
				UILineRenderer.UV_BOTTOM_CENTER_LEFT = new Vector2(0.5f, 1f);
				UILineRenderer.UV_BOTTOM_CENTER_RIGHT = new Vector2(0.5f, 1f);
				UILineRenderer.UV_TOP_RIGHT = new Vector2(1f, 0f);
				UILineRenderer.UV_BOTTOM_RIGHT = Vector2.one;
			}
			UILineRenderer.startUvs = new Vector2[]
			{
				UILineRenderer.UV_TOP_LEFT,
				UILineRenderer.UV_BOTTOM_LEFT,
				UILineRenderer.UV_BOTTOM_CENTER_LEFT,
				UILineRenderer.UV_TOP_CENTER_LEFT
			};
			UILineRenderer.middleUvs = new Vector2[]
			{
				UILineRenderer.UV_TOP_CENTER_LEFT,
				UILineRenderer.UV_BOTTOM_CENTER_LEFT,
				UILineRenderer.UV_BOTTOM_CENTER_RIGHT,
				UILineRenderer.UV_TOP_CENTER_RIGHT
			};
			UILineRenderer.endUvs = new Vector2[]
			{
				UILineRenderer.UV_TOP_CENTER_RIGHT,
				UILineRenderer.UV_BOTTOM_CENTER_RIGHT,
				UILineRenderer.UV_BOTTOM_RIGHT,
				UILineRenderer.UV_TOP_RIGHT
			};
			UILineRenderer.fullUvs = new Vector2[]
			{
				UILineRenderer.UV_TOP_LEFT,
				UILineRenderer.UV_BOTTOM_LEFT,
				UILineRenderer.UV_BOTTOM_RIGHT,
				UILineRenderer.UV_TOP_RIGHT
			};
		}

		// Token: 0x06003E6B RID: 15979 RVA: 0x0016E72C File Offset: 0x0016C92C
		protected override void ResolutionToNativeSize(float distance)
		{
			if (base.UseNativeSize)
			{
				this.m_Resolution = distance / (base.activeSprite.rect.width / base.pixelsPerUnit);
				this.lineThickness = base.activeSprite.rect.height / base.pixelsPerUnit;
			}
		}

		// Token: 0x06003E6C RID: 15980 RVA: 0x0016E784 File Offset: 0x0016C984
		private int GetSegmentPointCount()
		{
			List<Vector2[]> segments = this.Segments;
			if (segments != null && segments.Count > 0)
			{
				int num = 0;
				foreach (Vector2[] array in this.Segments)
				{
					num += array.Length;
				}
				return num;
			}
			return this.Points.Length;
		}

		// Token: 0x06003E6D RID: 15981 RVA: 0x0016E7FC File Offset: 0x0016C9FC
		public Vector2 GetPosition(int index, int segmentIndex = 0)
		{
			if (segmentIndex > 0)
			{
				return this.Segments[segmentIndex - 1][index - 1];
			}
			if (this.Segments.Count > 0)
			{
				int num = 0;
				int num2 = index;
				foreach (Vector2[] array in this.Segments)
				{
					if (num2 - array.Length <= 0)
					{
						break;
					}
					num2 -= array.Length;
					num++;
				}
				return this.Segments[num][num2 - 1];
			}
			return this.Points[index - 1];
		}

		// Token: 0x06003E6E RID: 15982 RVA: 0x0016E8AC File Offset: 0x0016CAAC
		public Vector2 GetPositionBySegment(int index, int segment)
		{
			return this.Segments[segment][index - 1];
		}

		// Token: 0x06003E6F RID: 15983 RVA: 0x0016E8C4 File Offset: 0x0016CAC4
		public Vector2 GetClosestPoint(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			Vector2 lhs = p3 - p1;
			Vector2 a = p2 - p1;
			float d = Mathf.Clamp01(Vector2.Dot(lhs, a.normalized) / a.magnitude);
			return p1 + a * d;
		}

		// Token: 0x02000F0F RID: 3855
		private enum SegmentType
		{
			// Token: 0x04004E33 RID: 20019
			Start,
			// Token: 0x04004E34 RID: 20020
			Middle,
			// Token: 0x04004E35 RID: 20021
			End,
			// Token: 0x04004E36 RID: 20022
			Full
		}

		// Token: 0x02000F10 RID: 3856
		public enum JoinType
		{
			// Token: 0x04004E38 RID: 20024
			Bevel,
			// Token: 0x04004E39 RID: 20025
			Miter
		}

		// Token: 0x02000F11 RID: 3857
		public enum BezierType
		{
			// Token: 0x04004E3B RID: 20027
			None,
			// Token: 0x04004E3C RID: 20028
			Quick,
			// Token: 0x04004E3D RID: 20029
			Basic,
			// Token: 0x04004E3E RID: 20030
			Improved,
			// Token: 0x04004E3F RID: 20031
			Catenary
		}
	}
}
