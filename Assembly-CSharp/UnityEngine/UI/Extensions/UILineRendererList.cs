using System;
using System.Collections.Generic;
using UnityEngine.Sprites;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A34 RID: 2612
	[AddComponentMenu("UI/Extensions/Primitives/UILineRendererList")]
	[RequireComponent(typeof(RectTransform))]
	public class UILineRendererList : UIPrimitiveBase
	{
		// Token: 0x040037CA RID: 14282
		private const float MIN_MITER_JOIN = 0.2617994f;

		// Token: 0x040037CB RID: 14283
		private const float MIN_BEVEL_NICE_JOIN = 0.5235988f;

		// Token: 0x040037CC RID: 14284
		private static Vector2 UV_TOP_LEFT;

		// Token: 0x040037CD RID: 14285
		private static Vector2 UV_BOTTOM_LEFT;

		// Token: 0x040037CE RID: 14286
		private static Vector2 UV_TOP_CENTER_LEFT;

		// Token: 0x040037CF RID: 14287
		private static Vector2 UV_TOP_CENTER_RIGHT;

		// Token: 0x040037D0 RID: 14288
		private static Vector2 UV_BOTTOM_CENTER_LEFT;

		// Token: 0x040037D1 RID: 14289
		private static Vector2 UV_BOTTOM_CENTER_RIGHT;

		// Token: 0x040037D2 RID: 14290
		private static Vector2 UV_TOP_RIGHT;

		// Token: 0x040037D3 RID: 14291
		private static Vector2 UV_BOTTOM_RIGHT;

		// Token: 0x040037D4 RID: 14292
		private static Vector2[] startUvs;

		// Token: 0x040037D5 RID: 14293
		private static Vector2[] middleUvs;

		// Token: 0x040037D6 RID: 14294
		private static Vector2[] endUvs;

		// Token: 0x040037D7 RID: 14295
		private static Vector2[] fullUvs;

		// Token: 0x040037D8 RID: 14296
		[SerializeField]
		[Tooltip("Points to draw lines between\n Can be improved using the Resolution Option")]
		internal List<Vector2> m_points;

		// Token: 0x040037D9 RID: 14297
		[SerializeField]
		[Tooltip("Thickness of the line")]
		internal float lineThickness = 2f;

		// Token: 0x040037DA RID: 14298
		[SerializeField]
		[Tooltip("Use the relative bounds of the Rect Transform (0,0 -> 0,1) or screen space coordinates")]
		internal bool relativeSize;

		// Token: 0x040037DB RID: 14299
		[SerializeField]
		[Tooltip("Do the points identify a single line or split pairs of lines")]
		internal bool lineList;

		// Token: 0x040037DC RID: 14300
		[SerializeField]
		[Tooltip("Add end caps to each line\nMultiple caps when used with Line List")]
		internal bool lineCaps;

		// Token: 0x040037DD RID: 14301
		[SerializeField]
		[Tooltip("Resolution of the Bezier curve, different to line Resolution")]
		internal int bezierSegmentsPerCurve = 10;

		// Token: 0x040037DE RID: 14302
		[Tooltip("The type of Join used between lines, Square/Mitre or Curved/Bevel")]
		public UILineRendererList.JoinType LineJoins;

		// Token: 0x040037DF RID: 14303
		[Tooltip("Bezier method to apply to line, see docs for options\nCan't be used in conjunction with Resolution as Bezier already changes the resolution")]
		public UILineRendererList.BezierType BezierMode;

		// Token: 0x040037E0 RID: 14304
		[HideInInspector]
		public bool drivenExternally;

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x06003E71 RID: 15985 RVA: 0x0016E922 File Offset: 0x0016CB22
		// (set) Token: 0x06003E72 RID: 15986 RVA: 0x0016E92A File Offset: 0x0016CB2A
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

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x06003E73 RID: 15987 RVA: 0x0016E939 File Offset: 0x0016CB39
		// (set) Token: 0x06003E74 RID: 15988 RVA: 0x0016E941 File Offset: 0x0016CB41
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

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x06003E75 RID: 15989 RVA: 0x0016E950 File Offset: 0x0016CB50
		// (set) Token: 0x06003E76 RID: 15990 RVA: 0x0016E958 File Offset: 0x0016CB58
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

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x06003E77 RID: 15991 RVA: 0x0016E967 File Offset: 0x0016CB67
		// (set) Token: 0x06003E78 RID: 15992 RVA: 0x0016E96F File Offset: 0x0016CB6F
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

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06003E79 RID: 15993 RVA: 0x0016E97E File Offset: 0x0016CB7E
		// (set) Token: 0x06003E7A RID: 15994 RVA: 0x0016E986 File Offset: 0x0016CB86
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

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x06003E7B RID: 15995 RVA: 0x0016E98F File Offset: 0x0016CB8F
		// (set) Token: 0x06003E7C RID: 15996 RVA: 0x0016E997 File Offset: 0x0016CB97
		public List<Vector2> Points
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

		// Token: 0x06003E7D RID: 15997 RVA: 0x0016E9B0 File Offset: 0x0016CBB0
		public void AddPoint(Vector2 pointToAdd)
		{
			this.m_points.Add(pointToAdd);
			this.SetAllDirty();
		}

		// Token: 0x06003E7E RID: 15998 RVA: 0x0016E9C4 File Offset: 0x0016CBC4
		public void RemovePoint(Vector2 pointToRemove)
		{
			this.m_points.Remove(pointToRemove);
			this.SetAllDirty();
		}

		// Token: 0x06003E7F RID: 15999 RVA: 0x0016E9D9 File Offset: 0x0016CBD9
		public void ClearPoints()
		{
			this.m_points.Clear();
			this.SetAllDirty();
		}

		// Token: 0x06003E80 RID: 16000 RVA: 0x0016E9EC File Offset: 0x0016CBEC
		private void PopulateMesh(VertexHelper vh, List<Vector2> pointsToDraw)
		{
			if (this.BezierMode != UILineRendererList.BezierType.None && this.BezierMode != UILineRendererList.BezierType.Catenary && pointsToDraw.Count > 3)
			{
				BezierPath bezierPath = new BezierPath();
				bezierPath.SetControlPoints(pointsToDraw);
				bezierPath.SegmentsPerCurve = this.bezierSegmentsPerCurve;
				UILineRendererList.BezierType bezierMode = this.BezierMode;
				List<Vector2> list;
				if (bezierMode != UILineRendererList.BezierType.Basic)
				{
					if (bezierMode != UILineRendererList.BezierType.Improved)
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
				pointsToDraw = list;
			}
			if (this.BezierMode == UILineRendererList.BezierType.Catenary && pointsToDraw.Count == 2)
			{
				CableCurve cableCurve = new CableCurve(pointsToDraw);
				cableCurve.slack = base.Resoloution;
				cableCurve.steps = this.BezierSegmentsPerCurve;
				pointsToDraw.Clear();
				pointsToDraw.AddRange(cableCurve.Points());
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
				for (int i = 1; i < pointsToDraw.Count; i += 2)
				{
					Vector2 vector = pointsToDraw[i - 1];
					Vector2 vector2 = pointsToDraw[i];
					vector = new Vector2(vector.x * num + num3, vector.y * num2 + num4);
					vector2 = new Vector2(vector2.x * num + num3, vector2.y * num2 + num4);
					if (this.lineCaps)
					{
						list2.Add(this.CreateLineCap(vector, vector2, UILineRendererList.SegmentType.Start));
					}
					list2.Add(this.CreateLineSegment(vector, vector2, UILineRendererList.SegmentType.Middle));
					if (this.lineCaps)
					{
						list2.Add(this.CreateLineCap(vector, vector2, UILineRendererList.SegmentType.End));
					}
				}
			}
			else
			{
				for (int j = 1; j < pointsToDraw.Count; j++)
				{
					Vector2 vector3 = pointsToDraw[j - 1];
					Vector2 vector4 = pointsToDraw[j];
					vector3 = new Vector2(vector3.x * num + num3, vector3.y * num2 + num4);
					vector4 = new Vector2(vector4.x * num + num3, vector4.y * num2 + num4);
					if (this.lineCaps && j == 1)
					{
						list2.Add(this.CreateLineCap(vector3, vector4, UILineRendererList.SegmentType.Start));
					}
					list2.Add(this.CreateLineSegment(vector3, vector4, UILineRendererList.SegmentType.Middle));
					if (this.lineCaps && j == pointsToDraw.Count - 1)
					{
						list2.Add(this.CreateLineCap(vector3, vector4, UILineRendererList.SegmentType.End));
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
					UILineRendererList.JoinType joinType = this.LineJoins;
					if (joinType == UILineRendererList.JoinType.Miter)
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
							joinType = UILineRendererList.JoinType.Bevel;
						}
					}
					if (joinType == UILineRendererList.JoinType.Bevel)
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

		// Token: 0x06003E81 RID: 16001 RVA: 0x0016EFED File Offset: 0x0016D1ED
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (this.m_points != null && this.m_points.Count > 0)
			{
				this.GeneratedUVs();
				vh.Clear();
				this.PopulateMesh(vh, this.m_points);
			}
		}

		// Token: 0x06003E82 RID: 16002 RVA: 0x0016F020 File Offset: 0x0016D220
		private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, UILineRendererList.SegmentType type)
		{
			if (type == UILineRendererList.SegmentType.Start)
			{
				Vector2 start2 = start - (end - start).normalized * this.lineThickness / 2f;
				return this.CreateLineSegment(start2, start, UILineRendererList.SegmentType.Start);
			}
			if (type == UILineRendererList.SegmentType.End)
			{
				Vector2 end2 = end + (end - start).normalized * this.lineThickness / 2f;
				return this.CreateLineSegment(end, end2, UILineRendererList.SegmentType.End);
			}
			Debug.LogError("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
			return null;
		}

		// Token: 0x06003E83 RID: 16003 RVA: 0x0016F0AC File Offset: 0x0016D2AC
		private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, UILineRendererList.SegmentType type)
		{
			Vector2 b = new Vector2(start.y - end.y, end.x - start.x).normalized * this.lineThickness / 2f;
			Vector2 vector = start - b;
			Vector2 vector2 = start + b;
			Vector2 vector3 = end + b;
			Vector2 vector4 = end - b;
			switch (type)
			{
			case UILineRendererList.SegmentType.Start:
				return base.SetVbo(new Vector2[]
				{
					vector,
					vector2,
					vector3,
					vector4
				}, UILineRendererList.startUvs);
			case UILineRendererList.SegmentType.End:
				return base.SetVbo(new Vector2[]
				{
					vector,
					vector2,
					vector3,
					vector4
				}, UILineRendererList.endUvs);
			case UILineRendererList.SegmentType.Full:
				return base.SetVbo(new Vector2[]
				{
					vector,
					vector2,
					vector3,
					vector4
				}, UILineRendererList.fullUvs);
			}
			return base.SetVbo(new Vector2[]
			{
				vector,
				vector2,
				vector3,
				vector4
			}, UILineRendererList.middleUvs);
		}

		// Token: 0x06003E84 RID: 16004 RVA: 0x0016F200 File Offset: 0x0016D400
		protected override void GeneratedUVs()
		{
			if (base.activeSprite != null)
			{
				Vector4 outerUV = DataUtility.GetOuterUV(base.activeSprite);
				Vector4 innerUV = DataUtility.GetInnerUV(base.activeSprite);
				UILineRendererList.UV_TOP_LEFT = new Vector2(outerUV.x, outerUV.y);
				UILineRendererList.UV_BOTTOM_LEFT = new Vector2(outerUV.x, outerUV.w);
				UILineRendererList.UV_TOP_CENTER_LEFT = new Vector2(innerUV.x, innerUV.y);
				UILineRendererList.UV_TOP_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.y);
				UILineRendererList.UV_BOTTOM_CENTER_LEFT = new Vector2(innerUV.x, innerUV.w);
				UILineRendererList.UV_BOTTOM_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.w);
				UILineRendererList.UV_TOP_RIGHT = new Vector2(outerUV.z, outerUV.y);
				UILineRendererList.UV_BOTTOM_RIGHT = new Vector2(outerUV.z, outerUV.w);
			}
			else
			{
				UILineRendererList.UV_TOP_LEFT = Vector2.zero;
				UILineRendererList.UV_BOTTOM_LEFT = new Vector2(0f, 1f);
				UILineRendererList.UV_TOP_CENTER_LEFT = new Vector2(0.5f, 0f);
				UILineRendererList.UV_TOP_CENTER_RIGHT = new Vector2(0.5f, 0f);
				UILineRendererList.UV_BOTTOM_CENTER_LEFT = new Vector2(0.5f, 1f);
				UILineRendererList.UV_BOTTOM_CENTER_RIGHT = new Vector2(0.5f, 1f);
				UILineRendererList.UV_TOP_RIGHT = new Vector2(1f, 0f);
				UILineRendererList.UV_BOTTOM_RIGHT = Vector2.one;
			}
			UILineRendererList.startUvs = new Vector2[]
			{
				UILineRendererList.UV_TOP_LEFT,
				UILineRendererList.UV_BOTTOM_LEFT,
				UILineRendererList.UV_BOTTOM_CENTER_LEFT,
				UILineRendererList.UV_TOP_CENTER_LEFT
			};
			UILineRendererList.middleUvs = new Vector2[]
			{
				UILineRendererList.UV_TOP_CENTER_LEFT,
				UILineRendererList.UV_BOTTOM_CENTER_LEFT,
				UILineRendererList.UV_BOTTOM_CENTER_RIGHT,
				UILineRendererList.UV_TOP_CENTER_RIGHT
			};
			UILineRendererList.endUvs = new Vector2[]
			{
				UILineRendererList.UV_TOP_CENTER_RIGHT,
				UILineRendererList.UV_BOTTOM_CENTER_RIGHT,
				UILineRendererList.UV_BOTTOM_RIGHT,
				UILineRendererList.UV_TOP_RIGHT
			};
			UILineRendererList.fullUvs = new Vector2[]
			{
				UILineRendererList.UV_TOP_LEFT,
				UILineRendererList.UV_BOTTOM_LEFT,
				UILineRendererList.UV_BOTTOM_RIGHT,
				UILineRendererList.UV_TOP_RIGHT
			};
		}

		// Token: 0x06003E85 RID: 16005 RVA: 0x0016F464 File Offset: 0x0016D664
		protected override void ResolutionToNativeSize(float distance)
		{
			if (base.UseNativeSize)
			{
				this.m_Resolution = distance / (base.activeSprite.rect.width / base.pixelsPerUnit);
				this.lineThickness = base.activeSprite.rect.height / base.pixelsPerUnit;
			}
		}

		// Token: 0x02000F12 RID: 3858
		private enum SegmentType
		{
			// Token: 0x04004E41 RID: 20033
			Start,
			// Token: 0x04004E42 RID: 20034
			Middle,
			// Token: 0x04004E43 RID: 20035
			End,
			// Token: 0x04004E44 RID: 20036
			Full
		}

		// Token: 0x02000F13 RID: 3859
		public enum JoinType
		{
			// Token: 0x04004E46 RID: 20038
			Bevel,
			// Token: 0x04004E47 RID: 20039
			Miter
		}

		// Token: 0x02000F14 RID: 3860
		public enum BezierType
		{
			// Token: 0x04004E49 RID: 20041
			None,
			// Token: 0x04004E4A RID: 20042
			Quick,
			// Token: 0x04004E4B RID: 20043
			Basic,
			// Token: 0x04004E4C RID: 20044
			Improved,
			// Token: 0x04004E4D RID: 20045
			Catenary
		}
	}
}
