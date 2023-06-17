using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A39 RID: 2617
	public class UIPrimitiveBase : MaskableGraphic, ILayoutElement, ICanvasRaycastFilter
	{
		// Token: 0x040037F1 RID: 14321
		protected static Material s_ETC1DefaultUI;

		// Token: 0x040037F2 RID: 14322
		private List<Vector2> outputList = new List<Vector2>();

		// Token: 0x040037F3 RID: 14323
		[SerializeField]
		private Sprite m_Sprite;

		// Token: 0x040037F4 RID: 14324
		[NonSerialized]
		private Sprite m_OverrideSprite;

		// Token: 0x040037F5 RID: 14325
		internal float m_EventAlphaThreshold = 1f;

		// Token: 0x040037F6 RID: 14326
		[SerializeField]
		private ResolutionMode m_improveResolution;

		// Token: 0x040037F7 RID: 14327
		[SerializeField]
		protected float m_Resolution;

		// Token: 0x040037F8 RID: 14328
		[SerializeField]
		private bool m_useNativeSize;

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x06003E98 RID: 16024 RVA: 0x0016FF95 File Offset: 0x0016E195
		// (set) Token: 0x06003E99 RID: 16025 RVA: 0x0016FF9D File Offset: 0x0016E19D
		public Sprite sprite
		{
			get
			{
				return this.m_Sprite;
			}
			set
			{
				if (SetPropertyUtility.SetClass<Sprite>(ref this.m_Sprite, value))
				{
					this.GeneratedUVs();
				}
				this.SetAllDirty();
			}
		}

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x06003E9A RID: 16026 RVA: 0x0016FFB9 File Offset: 0x0016E1B9
		// (set) Token: 0x06003E9B RID: 16027 RVA: 0x0016FFC1 File Offset: 0x0016E1C1
		public Sprite overrideSprite
		{
			get
			{
				return this.activeSprite;
			}
			set
			{
				if (SetPropertyUtility.SetClass<Sprite>(ref this.m_OverrideSprite, value))
				{
					this.GeneratedUVs();
				}
				this.SetAllDirty();
			}
		}

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x06003E9C RID: 16028 RVA: 0x0016FFDD File Offset: 0x0016E1DD
		protected Sprite activeSprite
		{
			get
			{
				if (!(this.m_OverrideSprite != null))
				{
					return this.sprite;
				}
				return this.m_OverrideSprite;
			}
		}

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06003E9D RID: 16029 RVA: 0x0016FFFA File Offset: 0x0016E1FA
		// (set) Token: 0x06003E9E RID: 16030 RVA: 0x00170002 File Offset: 0x0016E202
		public float eventAlphaThreshold
		{
			get
			{
				return this.m_EventAlphaThreshold;
			}
			set
			{
				this.m_EventAlphaThreshold = value;
			}
		}

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06003E9F RID: 16031 RVA: 0x0017000B File Offset: 0x0016E20B
		// (set) Token: 0x06003EA0 RID: 16032 RVA: 0x00170013 File Offset: 0x0016E213
		public ResolutionMode ImproveResolution
		{
			get
			{
				return this.m_improveResolution;
			}
			set
			{
				this.m_improveResolution = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x06003EA1 RID: 16033 RVA: 0x00170022 File Offset: 0x0016E222
		// (set) Token: 0x06003EA2 RID: 16034 RVA: 0x0017002A File Offset: 0x0016E22A
		public float Resoloution
		{
			get
			{
				return this.m_Resolution;
			}
			set
			{
				this.m_Resolution = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x06003EA3 RID: 16035 RVA: 0x00170039 File Offset: 0x0016E239
		// (set) Token: 0x06003EA4 RID: 16036 RVA: 0x00170041 File Offset: 0x0016E241
		public bool UseNativeSize
		{
			get
			{
				return this.m_useNativeSize;
			}
			set
			{
				this.m_useNativeSize = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x06003EA5 RID: 16037 RVA: 0x00170050 File Offset: 0x0016E250
		protected UIPrimitiveBase()
		{
			base.useLegacyMeshGeneration = false;
		}

		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x06003EA6 RID: 16038 RVA: 0x00170075 File Offset: 0x0016E275
		public static Material defaultETC1GraphicMaterial
		{
			get
			{
				if (UIPrimitiveBase.s_ETC1DefaultUI == null)
				{
					UIPrimitiveBase.s_ETC1DefaultUI = Canvas.GetETC1SupportedCanvasMaterial();
				}
				return UIPrimitiveBase.s_ETC1DefaultUI;
			}
		}

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x06003EA7 RID: 16039 RVA: 0x00170094 File Offset: 0x0016E294
		public override Texture mainTexture
		{
			get
			{
				if (!(this.activeSprite == null))
				{
					return this.activeSprite.texture;
				}
				if (this.material != null && this.material.mainTexture != null)
				{
					return this.material.mainTexture;
				}
				return Graphic.s_WhiteTexture;
			}
		}

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x06003EA8 RID: 16040 RVA: 0x001700F0 File Offset: 0x0016E2F0
		public bool hasBorder
		{
			get
			{
				return this.activeSprite != null && this.activeSprite.border.sqrMagnitude > 0f;
			}
		}

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x06003EA9 RID: 16041 RVA: 0x00170128 File Offset: 0x0016E328
		public float pixelsPerUnit
		{
			get
			{
				float num = 100f;
				if (this.activeSprite)
				{
					num = this.activeSprite.pixelsPerUnit;
				}
				float num2 = 100f;
				if (base.canvas)
				{
					num2 = base.canvas.referencePixelsPerUnit;
				}
				return num / num2;
			}
		}

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x06003EAA RID: 16042 RVA: 0x00170178 File Offset: 0x0016E378
		// (set) Token: 0x06003EAB RID: 16043 RVA: 0x001701C6 File Offset: 0x0016E3C6
		public override Material material
		{
			get
			{
				if (this.m_Material != null)
				{
					return this.m_Material;
				}
				if (this.activeSprite && this.activeSprite.associatedAlphaSplitTexture != null)
				{
					return UIPrimitiveBase.defaultETC1GraphicMaterial;
				}
				return this.defaultMaterial;
			}
			set
			{
				base.material = value;
			}
		}

		// Token: 0x06003EAC RID: 16044 RVA: 0x001701D0 File Offset: 0x0016E3D0
		protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
		{
			UIVertex[] array = new UIVertex[4];
			for (int i = 0; i < vertices.Length; i++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = this.color;
				simpleVert.position = vertices[i];
				simpleVert.uv0 = uvs[i];
				array[i] = simpleVert;
			}
			return array;
		}

		// Token: 0x06003EAD RID: 16045 RVA: 0x00170234 File Offset: 0x0016E434
		protected Vector2[] IncreaseResolution(Vector2[] input)
		{
			return this.IncreaseResolution(new List<Vector2>(input)).ToArray();
		}

		// Token: 0x06003EAE RID: 16046 RVA: 0x00170248 File Offset: 0x0016E448
		protected List<Vector2> IncreaseResolution(List<Vector2> input)
		{
			this.outputList.Clear();
			ResolutionMode improveResolution = this.ImproveResolution;
			if (improveResolution != ResolutionMode.PerSegment)
			{
				if (improveResolution == ResolutionMode.PerLine)
				{
					float num = 0f;
					for (int i = 0; i < input.Count - 1; i++)
					{
						num += Vector2.Distance(input[i], input[i + 1]);
					}
					this.ResolutionToNativeSize(num);
					float num2 = num / this.m_Resolution;
					int num3 = 0;
					for (int j = 0; j < input.Count - 1; j++)
					{
						Vector2 vector = input[j];
						this.outputList.Add(vector);
						Vector2 vector2 = input[j + 1];
						float num4 = Vector2.Distance(vector, vector2) / num2;
						float num5 = 1f / num4;
						int num6 = 0;
						while ((float)num6 < num4)
						{
							this.outputList.Add(Vector2.Lerp(vector, vector2, (float)num6 * num5));
							num3++;
							num6++;
						}
						this.outputList.Add(vector2);
					}
				}
			}
			else
			{
				for (int k = 0; k < input.Count - 1; k++)
				{
					Vector2 vector3 = input[k];
					this.outputList.Add(vector3);
					Vector2 vector4 = input[k + 1];
					this.ResolutionToNativeSize(Vector2.Distance(vector3, vector4));
					float num2 = 1f / this.m_Resolution;
					for (float num7 = 1f; num7 < this.m_Resolution; num7 += 1f)
					{
						this.outputList.Add(Vector2.Lerp(vector3, vector4, num2 * num7));
					}
					this.outputList.Add(vector4);
				}
			}
			return this.outputList;
		}

		// Token: 0x06003EAF RID: 16047 RVA: 0x000063A5 File Offset: 0x000045A5
		protected virtual void GeneratedUVs()
		{
		}

		// Token: 0x06003EB0 RID: 16048 RVA: 0x000063A5 File Offset: 0x000045A5
		protected virtual void ResolutionToNativeSize(float distance)
		{
		}

		// Token: 0x06003EB1 RID: 16049 RVA: 0x000063A5 File Offset: 0x000045A5
		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		// Token: 0x06003EB2 RID: 16050 RVA: 0x000063A5 File Offset: 0x000045A5
		public virtual void CalculateLayoutInputVertical()
		{
		}

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x06003EB3 RID: 16051 RVA: 0x00029CA8 File Offset: 0x00027EA8
		public virtual float minWidth
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x06003EB4 RID: 16052 RVA: 0x00170400 File Offset: 0x0016E600
		public virtual float preferredWidth
		{
			get
			{
				if (this.overrideSprite == null)
				{
					return 0f;
				}
				return this.overrideSprite.rect.size.x / this.pixelsPerUnit;
			}
		}

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x06003EB5 RID: 16053 RVA: 0x00040DED File Offset: 0x0003EFED
		public virtual float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x06003EB6 RID: 16054 RVA: 0x00029CA8 File Offset: 0x00027EA8
		public virtual float minHeight
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x06003EB7 RID: 16055 RVA: 0x00170440 File Offset: 0x0016E640
		public virtual float preferredHeight
		{
			get
			{
				if (this.overrideSprite == null)
				{
					return 0f;
				}
				return this.overrideSprite.rect.size.y / this.pixelsPerUnit;
			}
		}

		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x06003EB8 RID: 16056 RVA: 0x00040DED File Offset: 0x0003EFED
		public virtual float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x06003EB9 RID: 16057 RVA: 0x00007A3C File Offset: 0x00005C3C
		public virtual int layoutPriority
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06003EBA RID: 16058 RVA: 0x00170480 File Offset: 0x0016E680
		public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			if (this.m_EventAlphaThreshold >= 1f)
			{
				return true;
			}
			Sprite overrideSprite = this.overrideSprite;
			if (overrideSprite == null)
			{
				return true;
			}
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, screenPoint, eventCamera, out vector);
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			vector.x += base.rectTransform.pivot.x * pixelAdjustedRect.width;
			vector.y += base.rectTransform.pivot.y * pixelAdjustedRect.height;
			vector = this.MapCoordinate(vector, pixelAdjustedRect);
			Rect textureRect = overrideSprite.textureRect;
			Vector2 vector2 = new Vector2(vector.x / textureRect.width, vector.y / textureRect.height);
			float u = Mathf.Lerp(textureRect.x, textureRect.xMax, vector2.x) / (float)overrideSprite.texture.width;
			float v = Mathf.Lerp(textureRect.y, textureRect.yMax, vector2.y) / (float)overrideSprite.texture.height;
			bool result;
			try
			{
				result = (overrideSprite.texture.GetPixelBilinear(u, v).a >= this.m_EventAlphaThreshold);
			}
			catch (UnityException ex)
			{
				Debug.LogError("Using clickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read. " + ex.Message + " Also make sure to disable sprite packing for this sprite.", this);
				result = true;
			}
			return result;
		}

		// Token: 0x06003EBB RID: 16059 RVA: 0x001705E8 File Offset: 0x0016E7E8
		private Vector2 MapCoordinate(Vector2 local, Rect rect)
		{
			Rect rect2 = this.sprite.rect;
			return new Vector2(local.x * rect.width, local.y * rect.height);
		}

		// Token: 0x06003EBC RID: 16060 RVA: 0x00170618 File Offset: 0x0016E818
		private Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
		{
			for (int i = 0; i <= 1; i++)
			{
				float num = border[i] + border[i + 2];
				if (rect.size[i] < num && num != 0f)
				{
					float num2 = rect.size[i] / num;
					ref Vector4 ptr = ref border;
					int index = i;
					ptr[index] *= num2;
					ptr = ref border;
					index = i + 2;
					ptr[index] *= num2;
				}
			}
			return border;
		}

		// Token: 0x06003EBD RID: 16061 RVA: 0x001706AF File Offset: 0x0016E8AF
		protected override void OnEnable()
		{
			base.OnEnable();
			this.SetAllDirty();
		}
	}
}
