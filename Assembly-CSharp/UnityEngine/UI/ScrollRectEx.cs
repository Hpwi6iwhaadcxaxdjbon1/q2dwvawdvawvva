using System;
using Rust;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x02000A2B RID: 2603
	[AddComponentMenu("UI/Scroll Rect Ex", 37)]
	[SelectionBase]
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class ScrollRectEx : UIBehaviour, IInitializePotentialDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutGroup, ILayoutController
	{
		// Token: 0x04003763 RID: 14179
		public PointerEventData.InputButton scrollButton;

		// Token: 0x04003764 RID: 14180
		public PointerEventData.InputButton altScrollButton;

		// Token: 0x04003765 RID: 14181
		[SerializeField]
		private RectTransform m_Content;

		// Token: 0x04003766 RID: 14182
		[SerializeField]
		private bool m_Horizontal = true;

		// Token: 0x04003767 RID: 14183
		[SerializeField]
		private bool m_Vertical = true;

		// Token: 0x04003768 RID: 14184
		[SerializeField]
		private ScrollRectEx.MovementType m_MovementType = ScrollRectEx.MovementType.Elastic;

		// Token: 0x04003769 RID: 14185
		[SerializeField]
		private float m_Elasticity = 0.1f;

		// Token: 0x0400376A RID: 14186
		[SerializeField]
		private bool m_Inertia = true;

		// Token: 0x0400376B RID: 14187
		[SerializeField]
		private float m_DecelerationRate = 0.135f;

		// Token: 0x0400376C RID: 14188
		[SerializeField]
		private float m_ScrollSensitivity = 1f;

		// Token: 0x0400376D RID: 14189
		[SerializeField]
		private RectTransform m_Viewport;

		// Token: 0x0400376E RID: 14190
		[SerializeField]
		private Scrollbar m_HorizontalScrollbar;

		// Token: 0x0400376F RID: 14191
		[SerializeField]
		private Scrollbar m_VerticalScrollbar;

		// Token: 0x04003770 RID: 14192
		[SerializeField]
		private ScrollRectEx.ScrollbarVisibility m_HorizontalScrollbarVisibility;

		// Token: 0x04003771 RID: 14193
		[SerializeField]
		private ScrollRectEx.ScrollbarVisibility m_VerticalScrollbarVisibility;

		// Token: 0x04003772 RID: 14194
		[SerializeField]
		private float m_HorizontalScrollbarSpacing;

		// Token: 0x04003773 RID: 14195
		[SerializeField]
		private float m_VerticalScrollbarSpacing;

		// Token: 0x04003774 RID: 14196
		[SerializeField]
		private ScrollRectEx.ScrollRectEvent m_OnValueChanged = new ScrollRectEx.ScrollRectEvent();

		// Token: 0x04003775 RID: 14197
		private Vector2 m_PointerStartLocalCursor = Vector2.zero;

		// Token: 0x04003776 RID: 14198
		private Vector2 m_ContentStartPosition = Vector2.zero;

		// Token: 0x04003777 RID: 14199
		private RectTransform m_ViewRect;

		// Token: 0x04003778 RID: 14200
		private Bounds m_ContentBounds;

		// Token: 0x04003779 RID: 14201
		private Bounds m_ViewBounds;

		// Token: 0x0400377A RID: 14202
		private Vector2 m_Velocity;

		// Token: 0x0400377B RID: 14203
		private bool m_Dragging;

		// Token: 0x0400377C RID: 14204
		private Vector2 m_PrevPosition = Vector2.zero;

		// Token: 0x0400377D RID: 14205
		private Bounds m_PrevContentBounds;

		// Token: 0x0400377E RID: 14206
		private Bounds m_PrevViewBounds;

		// Token: 0x0400377F RID: 14207
		[NonSerialized]
		private bool m_HasRebuiltLayout;

		// Token: 0x04003780 RID: 14208
		private bool m_HSliderExpand;

		// Token: 0x04003781 RID: 14209
		private bool m_VSliderExpand;

		// Token: 0x04003782 RID: 14210
		private float m_HSliderHeight;

		// Token: 0x04003783 RID: 14211
		private float m_VSliderWidth;

		// Token: 0x04003784 RID: 14212
		[NonSerialized]
		private RectTransform m_Rect;

		// Token: 0x04003785 RID: 14213
		private RectTransform m_HorizontalScrollbarRect;

		// Token: 0x04003786 RID: 14214
		private RectTransform m_VerticalScrollbarRect;

		// Token: 0x04003787 RID: 14215
		private DrivenRectTransformTracker m_Tracker;

		// Token: 0x04003788 RID: 14216
		private readonly Vector3[] m_Corners = new Vector3[4];

		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x06003DC6 RID: 15814 RVA: 0x0016A5FE File Offset: 0x001687FE
		// (set) Token: 0x06003DC7 RID: 15815 RVA: 0x0016A606 File Offset: 0x00168806
		public RectTransform content
		{
			get
			{
				return this.m_Content;
			}
			set
			{
				this.m_Content = value;
			}
		}

		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x06003DC8 RID: 15816 RVA: 0x0016A60F File Offset: 0x0016880F
		// (set) Token: 0x06003DC9 RID: 15817 RVA: 0x0016A617 File Offset: 0x00168817
		public bool horizontal
		{
			get
			{
				return this.m_Horizontal;
			}
			set
			{
				this.m_Horizontal = value;
			}
		}

		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06003DCA RID: 15818 RVA: 0x0016A620 File Offset: 0x00168820
		// (set) Token: 0x06003DCB RID: 15819 RVA: 0x0016A628 File Offset: 0x00168828
		public bool vertical
		{
			get
			{
				return this.m_Vertical;
			}
			set
			{
				this.m_Vertical = value;
			}
		}

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x06003DCC RID: 15820 RVA: 0x0016A631 File Offset: 0x00168831
		// (set) Token: 0x06003DCD RID: 15821 RVA: 0x0016A639 File Offset: 0x00168839
		public ScrollRectEx.MovementType movementType
		{
			get
			{
				return this.m_MovementType;
			}
			set
			{
				this.m_MovementType = value;
			}
		}

		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x06003DCE RID: 15822 RVA: 0x0016A642 File Offset: 0x00168842
		// (set) Token: 0x06003DCF RID: 15823 RVA: 0x0016A64A File Offset: 0x0016884A
		public float elasticity
		{
			get
			{
				return this.m_Elasticity;
			}
			set
			{
				this.m_Elasticity = value;
			}
		}

		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x06003DD0 RID: 15824 RVA: 0x0016A653 File Offset: 0x00168853
		// (set) Token: 0x06003DD1 RID: 15825 RVA: 0x0016A65B File Offset: 0x0016885B
		public bool inertia
		{
			get
			{
				return this.m_Inertia;
			}
			set
			{
				this.m_Inertia = value;
			}
		}

		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x06003DD2 RID: 15826 RVA: 0x0016A664 File Offset: 0x00168864
		// (set) Token: 0x06003DD3 RID: 15827 RVA: 0x0016A66C File Offset: 0x0016886C
		public float decelerationRate
		{
			get
			{
				return this.m_DecelerationRate;
			}
			set
			{
				this.m_DecelerationRate = value;
			}
		}

		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x06003DD4 RID: 15828 RVA: 0x0016A675 File Offset: 0x00168875
		// (set) Token: 0x06003DD5 RID: 15829 RVA: 0x0016A67D File Offset: 0x0016887D
		public float scrollSensitivity
		{
			get
			{
				return this.m_ScrollSensitivity;
			}
			set
			{
				this.m_ScrollSensitivity = value;
			}
		}

		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x06003DD6 RID: 15830 RVA: 0x0016A686 File Offset: 0x00168886
		// (set) Token: 0x06003DD7 RID: 15831 RVA: 0x0016A68E File Offset: 0x0016888E
		public RectTransform viewport
		{
			get
			{
				return this.m_Viewport;
			}
			set
			{
				this.m_Viewport = value;
				this.SetDirtyCaching();
			}
		}

		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x06003DD8 RID: 15832 RVA: 0x0016A69D File Offset: 0x0016889D
		// (set) Token: 0x06003DD9 RID: 15833 RVA: 0x0016A6A8 File Offset: 0x001688A8
		public Scrollbar horizontalScrollbar
		{
			get
			{
				return this.m_HorizontalScrollbar;
			}
			set
			{
				if (this.m_HorizontalScrollbar)
				{
					this.m_HorizontalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
				}
				this.m_HorizontalScrollbar = value;
				if (this.m_HorizontalScrollbar)
				{
					this.m_HorizontalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
				}
				this.SetDirtyCaching();
			}
		}

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x06003DDA RID: 15834 RVA: 0x0016A714 File Offset: 0x00168914
		// (set) Token: 0x06003DDB RID: 15835 RVA: 0x0016A71C File Offset: 0x0016891C
		public Scrollbar verticalScrollbar
		{
			get
			{
				return this.m_VerticalScrollbar;
			}
			set
			{
				if (this.m_VerticalScrollbar)
				{
					this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
				}
				this.m_VerticalScrollbar = value;
				if (this.m_VerticalScrollbar)
				{
					this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
				}
				this.SetDirtyCaching();
			}
		}

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x06003DDC RID: 15836 RVA: 0x0016A788 File Offset: 0x00168988
		// (set) Token: 0x06003DDD RID: 15837 RVA: 0x0016A790 File Offset: 0x00168990
		public ScrollRectEx.ScrollbarVisibility horizontalScrollbarVisibility
		{
			get
			{
				return this.m_HorizontalScrollbarVisibility;
			}
			set
			{
				this.m_HorizontalScrollbarVisibility = value;
				this.SetDirtyCaching();
			}
		}

		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x06003DDE RID: 15838 RVA: 0x0016A79F File Offset: 0x0016899F
		// (set) Token: 0x06003DDF RID: 15839 RVA: 0x0016A7A7 File Offset: 0x001689A7
		public ScrollRectEx.ScrollbarVisibility verticalScrollbarVisibility
		{
			get
			{
				return this.m_VerticalScrollbarVisibility;
			}
			set
			{
				this.m_VerticalScrollbarVisibility = value;
				this.SetDirtyCaching();
			}
		}

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x06003DE0 RID: 15840 RVA: 0x0016A7B6 File Offset: 0x001689B6
		// (set) Token: 0x06003DE1 RID: 15841 RVA: 0x0016A7BE File Offset: 0x001689BE
		public float horizontalScrollbarSpacing
		{
			get
			{
				return this.m_HorizontalScrollbarSpacing;
			}
			set
			{
				this.m_HorizontalScrollbarSpacing = value;
				this.SetDirty();
			}
		}

		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x06003DE2 RID: 15842 RVA: 0x0016A7CD File Offset: 0x001689CD
		// (set) Token: 0x06003DE3 RID: 15843 RVA: 0x0016A7D5 File Offset: 0x001689D5
		public float verticalScrollbarSpacing
		{
			get
			{
				return this.m_VerticalScrollbarSpacing;
			}
			set
			{
				this.m_VerticalScrollbarSpacing = value;
				this.SetDirty();
			}
		}

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x06003DE4 RID: 15844 RVA: 0x0016A7E4 File Offset: 0x001689E4
		// (set) Token: 0x06003DE5 RID: 15845 RVA: 0x0016A7EC File Offset: 0x001689EC
		public ScrollRectEx.ScrollRectEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				this.m_OnValueChanged = value;
			}
		}

		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x06003DE6 RID: 15846 RVA: 0x0016A7F8 File Offset: 0x001689F8
		protected RectTransform viewRect
		{
			get
			{
				if (this.m_ViewRect == null)
				{
					this.m_ViewRect = this.m_Viewport;
				}
				if (this.m_ViewRect == null)
				{
					this.m_ViewRect = (RectTransform)base.transform;
				}
				return this.m_ViewRect;
			}
		}

		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x06003DE7 RID: 15847 RVA: 0x0016A844 File Offset: 0x00168A44
		// (set) Token: 0x06003DE8 RID: 15848 RVA: 0x0016A84C File Offset: 0x00168A4C
		public Vector2 velocity
		{
			get
			{
				return this.m_Velocity;
			}
			set
			{
				this.m_Velocity = value;
			}
		}

		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x06003DE9 RID: 15849 RVA: 0x0016A855 File Offset: 0x00168A55
		private RectTransform rectTransform
		{
			get
			{
				if (this.m_Rect == null)
				{
					this.m_Rect = base.GetComponent<RectTransform>();
				}
				return this.m_Rect;
			}
		}

		// Token: 0x06003DEA RID: 15850 RVA: 0x0016A878 File Offset: 0x00168A78
		protected ScrollRectEx()
		{
		}

		// Token: 0x06003DEB RID: 15851 RVA: 0x0016A900 File Offset: 0x00168B00
		public virtual void Rebuild(CanvasUpdate executing)
		{
			if (executing == CanvasUpdate.Prelayout)
			{
				this.UpdateCachedData();
			}
			if (executing == CanvasUpdate.PostLayout)
			{
				this.UpdateBounds();
				this.UpdateScrollbars(Vector2.zero);
				this.UpdatePrevData();
				this.m_HasRebuiltLayout = true;
			}
		}

		// Token: 0x06003DEC RID: 15852 RVA: 0x0016A930 File Offset: 0x00168B30
		private void UpdateCachedData()
		{
			Transform transform = base.transform;
			this.m_HorizontalScrollbarRect = ((this.m_HorizontalScrollbar == null) ? null : (this.m_HorizontalScrollbar.transform as RectTransform));
			this.m_VerticalScrollbarRect = ((this.m_VerticalScrollbar == null) ? null : (this.m_VerticalScrollbar.transform as RectTransform));
			bool flag = this.viewRect.parent == transform;
			bool flag2 = !this.m_HorizontalScrollbarRect || this.m_HorizontalScrollbarRect.parent == transform;
			bool flag3 = !this.m_VerticalScrollbarRect || this.m_VerticalScrollbarRect.parent == transform;
			bool flag4 = flag && flag2 && flag3;
			this.m_HSliderExpand = (flag4 && this.m_HorizontalScrollbarRect && this.horizontalScrollbarVisibility == ScrollRectEx.ScrollbarVisibility.AutoHideAndExpandViewport);
			this.m_VSliderExpand = (flag4 && this.m_VerticalScrollbarRect && this.verticalScrollbarVisibility == ScrollRectEx.ScrollbarVisibility.AutoHideAndExpandViewport);
			this.m_HSliderHeight = ((this.m_HorizontalScrollbarRect == null) ? 0f : this.m_HorizontalScrollbarRect.rect.height);
			this.m_VSliderWidth = ((this.m_VerticalScrollbarRect == null) ? 0f : this.m_VerticalScrollbarRect.rect.width);
		}

		// Token: 0x06003DED RID: 15853 RVA: 0x0016AA90 File Offset: 0x00168C90
		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_HorizontalScrollbar)
			{
				this.m_HorizontalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
			}
			if (this.m_VerticalScrollbar)
			{
				this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
		}

		// Token: 0x06003DEE RID: 15854 RVA: 0x0016AAFC File Offset: 0x00168CFC
		protected override void OnDisable()
		{
			if (Application.isQuitting)
			{
				return;
			}
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (this.m_HorizontalScrollbar)
			{
				this.m_HorizontalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
			}
			if (this.m_VerticalScrollbar)
			{
				this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
			}
			this.m_HasRebuiltLayout = false;
			this.m_Tracker.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
			base.OnDisable();
		}

		// Token: 0x06003DEF RID: 15855 RVA: 0x0016AB8C File Offset: 0x00168D8C
		public override bool IsActive()
		{
			return base.IsActive() && this.m_Content != null;
		}

		// Token: 0x06003DF0 RID: 15856 RVA: 0x0016ABA4 File Offset: 0x00168DA4
		private void EnsureLayoutHasRebuilt()
		{
			if (!this.m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
			{
				Canvas.ForceUpdateCanvases();
			}
		}

		// Token: 0x06003DF1 RID: 15857 RVA: 0x0016ABBA File Offset: 0x00168DBA
		public virtual void StopMovement()
		{
			this.m_Velocity = Vector2.zero;
		}

		// Token: 0x06003DF2 RID: 15858 RVA: 0x0016ABC8 File Offset: 0x00168DC8
		public virtual void OnScroll(PointerEventData data)
		{
			if (!this.IsActive())
			{
				return;
			}
			this.EnsureLayoutHasRebuilt();
			this.UpdateBounds();
			Vector2 scrollDelta = data.scrollDelta;
			scrollDelta.y *= -1f;
			if (this.vertical && !this.horizontal)
			{
				if (Mathf.Abs(scrollDelta.x) > Mathf.Abs(scrollDelta.y))
				{
					scrollDelta.y = scrollDelta.x;
				}
				scrollDelta.x = 0f;
			}
			if (this.horizontal && !this.vertical)
			{
				if (Mathf.Abs(scrollDelta.y) > Mathf.Abs(scrollDelta.x))
				{
					scrollDelta.x = scrollDelta.y;
				}
				scrollDelta.y = 0f;
			}
			Vector2 vector = this.m_Content.anchoredPosition;
			vector += scrollDelta * this.m_ScrollSensitivity;
			if (this.m_MovementType == ScrollRectEx.MovementType.Clamped)
			{
				vector += this.CalculateOffset(vector - this.m_Content.anchoredPosition);
			}
			this.SetContentAnchoredPosition(vector);
			this.UpdateBounds();
		}

		// Token: 0x06003DF3 RID: 15859 RVA: 0x0016ACD6 File Offset: 0x00168ED6
		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			this.m_Velocity = Vector2.zero;
		}

		// Token: 0x06003DF4 RID: 15860 RVA: 0x0016AD00 File Offset: 0x00168F00
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			if (!this.IsActive())
			{
				return;
			}
			this.UpdateBounds();
			this.m_PointerStartLocalCursor = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.position, eventData.pressEventCamera, out this.m_PointerStartLocalCursor);
			this.m_ContentStartPosition = this.m_Content.anchoredPosition;
			this.m_Dragging = true;
		}

		// Token: 0x06003DF5 RID: 15861 RVA: 0x0016AD7A File Offset: 0x00168F7A
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			this.m_Dragging = false;
		}

		// Token: 0x06003DF6 RID: 15862 RVA: 0x0016ADA0 File Offset: 0x00168FA0
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			if (!this.IsActive())
			{
				return;
			}
			Vector2 a;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.position, eventData.pressEventCamera, out a))
			{
				return;
			}
			this.UpdateBounds();
			Vector2 b = a - this.m_PointerStartLocalCursor;
			Vector2 vector = this.m_ContentStartPosition + b;
			Vector2 vector2 = this.CalculateOffset(vector - this.m_Content.anchoredPosition);
			vector += vector2;
			if (this.m_MovementType == ScrollRectEx.MovementType.Elastic)
			{
				if (vector2.x != 0f)
				{
					vector.x -= ScrollRectEx.RubberDelta(vector2.x, this.m_ViewBounds.size.x);
				}
				if (vector2.y != 0f)
				{
					vector.y -= ScrollRectEx.RubberDelta(vector2.y, this.m_ViewBounds.size.y);
				}
			}
			this.SetContentAnchoredPosition(vector);
		}

		// Token: 0x06003DF7 RID: 15863 RVA: 0x0016AEAC File Offset: 0x001690AC
		protected virtual void SetContentAnchoredPosition(Vector2 position)
		{
			if (!this.m_Horizontal)
			{
				position.x = this.m_Content.anchoredPosition.x;
			}
			if (!this.m_Vertical)
			{
				position.y = this.m_Content.anchoredPosition.y;
			}
			if (position != this.m_Content.anchoredPosition)
			{
				this.m_Content.anchoredPosition = position;
				this.UpdateBounds();
			}
		}

		// Token: 0x06003DF8 RID: 15864 RVA: 0x0016AF1C File Offset: 0x0016911C
		protected virtual void LateUpdate()
		{
			if (!this.m_Content)
			{
				return;
			}
			this.EnsureLayoutHasRebuilt();
			this.UpdateScrollbarVisibility();
			this.UpdateBounds();
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			Vector2 vector = this.CalculateOffset(Vector2.zero);
			if (!this.m_Dragging && (vector != Vector2.zero || this.m_Velocity != Vector2.zero))
			{
				Vector2 vector2 = this.m_Content.anchoredPosition;
				for (int i = 0; i < 2; i++)
				{
					if (this.m_MovementType == ScrollRectEx.MovementType.Elastic && vector[i] != 0f)
					{
						float value = this.m_Velocity[i];
						vector2[i] = Mathf.SmoothDamp(this.m_Content.anchoredPosition[i], this.m_Content.anchoredPosition[i] + vector[i], ref value, this.m_Elasticity, float.PositiveInfinity, unscaledDeltaTime);
						this.m_Velocity[i] = value;
					}
					else if (this.m_Inertia)
					{
						ref Vector2 ptr = ref this.m_Velocity;
						int index = i;
						ptr[index] *= Mathf.Pow(this.m_DecelerationRate, unscaledDeltaTime);
						if (Mathf.Abs(this.m_Velocity[i]) < 1f)
						{
							this.m_Velocity[i] = 0f;
						}
						ptr = ref vector2;
						index = i;
						ptr[index] += this.m_Velocity[i] * unscaledDeltaTime;
					}
					else
					{
						this.m_Velocity[i] = 0f;
					}
				}
				if (this.m_Velocity != Vector2.zero)
				{
					if (this.m_MovementType == ScrollRectEx.MovementType.Clamped)
					{
						vector = this.CalculateOffset(vector2 - this.m_Content.anchoredPosition);
						vector2 += vector;
					}
					this.SetContentAnchoredPosition(vector2);
				}
			}
			if (this.m_Dragging && this.m_Inertia)
			{
				Vector3 b = (this.m_Content.anchoredPosition - this.m_PrevPosition) / unscaledDeltaTime;
				this.m_Velocity = Vector3.Lerp(this.m_Velocity, b, unscaledDeltaTime * 10f);
			}
			if (this.m_ViewBounds != this.m_PrevViewBounds || this.m_ContentBounds != this.m_PrevContentBounds || this.m_Content.anchoredPosition != this.m_PrevPosition)
			{
				this.UpdateScrollbars(vector);
				this.m_OnValueChanged.Invoke(this.normalizedPosition);
				this.UpdatePrevData();
			}
		}

		// Token: 0x06003DF9 RID: 15865 RVA: 0x0016B1B8 File Offset: 0x001693B8
		private void UpdatePrevData()
		{
			if (this.m_Content == null)
			{
				this.m_PrevPosition = Vector2.zero;
			}
			else
			{
				this.m_PrevPosition = this.m_Content.anchoredPosition;
			}
			this.m_PrevViewBounds = this.m_ViewBounds;
			this.m_PrevContentBounds = this.m_ContentBounds;
		}

		// Token: 0x06003DFA RID: 15866 RVA: 0x0016B20C File Offset: 0x0016940C
		private void UpdateScrollbars(Vector2 offset)
		{
			if (this.m_HorizontalScrollbar)
			{
				if (this.m_ContentBounds.size.x > 0f)
				{
					this.m_HorizontalScrollbar.size = Mathf.Clamp01((this.m_ViewBounds.size.x - Mathf.Abs(offset.x)) / this.m_ContentBounds.size.x);
				}
				else
				{
					this.m_HorizontalScrollbar.size = 1f;
				}
				this.m_HorizontalScrollbar.value = this.horizontalNormalizedPosition;
			}
			if (this.m_VerticalScrollbar)
			{
				if (this.m_ContentBounds.size.y > 0f)
				{
					this.m_VerticalScrollbar.size = Mathf.Clamp01((this.m_ViewBounds.size.y - Mathf.Abs(offset.y)) / this.m_ContentBounds.size.y);
				}
				else
				{
					this.m_VerticalScrollbar.size = 1f;
				}
				this.m_VerticalScrollbar.value = this.verticalNormalizedPosition;
			}
		}

		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x06003DFB RID: 15867 RVA: 0x0016B321 File Offset: 0x00169521
		// (set) Token: 0x06003DFC RID: 15868 RVA: 0x0016B334 File Offset: 0x00169534
		public Vector2 normalizedPosition
		{
			get
			{
				return new Vector2(this.horizontalNormalizedPosition, this.verticalNormalizedPosition);
			}
			set
			{
				this.SetNormalizedPosition(value.x, 0);
				this.SetNormalizedPosition(value.y, 1);
			}
		}

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x06003DFD RID: 15869 RVA: 0x0016B350 File Offset: 0x00169550
		// (set) Token: 0x06003DFE RID: 15870 RVA: 0x0016B3F0 File Offset: 0x001695F0
		public float horizontalNormalizedPosition
		{
			get
			{
				this.UpdateBounds();
				if (this.m_ContentBounds.size.x <= this.m_ViewBounds.size.x)
				{
					return (float)((this.m_ViewBounds.min.x > this.m_ContentBounds.min.x) ? 1 : 0);
				}
				return (this.m_ViewBounds.min.x - this.m_ContentBounds.min.x) / (this.m_ContentBounds.size.x - this.m_ViewBounds.size.x);
			}
			set
			{
				this.SetNormalizedPosition(value, 0);
			}
		}

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x06003DFF RID: 15871 RVA: 0x0016B3FC File Offset: 0x001695FC
		// (set) Token: 0x06003E00 RID: 15872 RVA: 0x0016B49C File Offset: 0x0016969C
		public float verticalNormalizedPosition
		{
			get
			{
				this.UpdateBounds();
				if (this.m_ContentBounds.size.y <= this.m_ViewBounds.size.y)
				{
					return (float)((this.m_ViewBounds.min.y > this.m_ContentBounds.min.y) ? 1 : 0);
				}
				return (this.m_ViewBounds.min.y - this.m_ContentBounds.min.y) / (this.m_ContentBounds.size.y - this.m_ViewBounds.size.y);
			}
			set
			{
				this.SetNormalizedPosition(value, 1);
			}
		}

		// Token: 0x06003E01 RID: 15873 RVA: 0x0016B3F0 File Offset: 0x001695F0
		private void SetHorizontalNormalizedPosition(float value)
		{
			this.SetNormalizedPosition(value, 0);
		}

		// Token: 0x06003E02 RID: 15874 RVA: 0x0016B49C File Offset: 0x0016969C
		private void SetVerticalNormalizedPosition(float value)
		{
			this.SetNormalizedPosition(value, 1);
		}

		// Token: 0x06003E03 RID: 15875 RVA: 0x0016B4A8 File Offset: 0x001696A8
		private void SetNormalizedPosition(float value, int axis)
		{
			this.EnsureLayoutHasRebuilt();
			this.UpdateBounds();
			float num = this.m_ContentBounds.size[axis] - this.m_ViewBounds.size[axis];
			float num2 = this.m_ViewBounds.min[axis] - value * num;
			float num3 = this.m_Content.localPosition[axis] + num2 - this.m_ContentBounds.min[axis];
			Vector3 localPosition = this.m_Content.localPosition;
			if (Mathf.Abs(localPosition[axis] - num3) > 0.01f)
			{
				localPosition[axis] = num3;
				this.m_Content.localPosition = localPosition;
				this.m_Velocity[axis] = 0f;
				this.UpdateBounds();
			}
		}

		// Token: 0x06003E04 RID: 15876 RVA: 0x0016B583 File Offset: 0x00169783
		private static float RubberDelta(float overStretching, float viewSize)
		{
			return (1f - 1f / (Mathf.Abs(overStretching) * 0.55f / viewSize + 1f)) * viewSize * Mathf.Sign(overStretching);
		}

		// Token: 0x06003E05 RID: 15877 RVA: 0x0016B5AE File Offset: 0x001697AE
		protected override void OnRectTransformDimensionsChange()
		{
			this.SetDirty();
		}

		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x06003E06 RID: 15878 RVA: 0x0016B5B6 File Offset: 0x001697B6
		private bool hScrollingNeeded
		{
			get
			{
				return !Application.isPlaying || this.m_ContentBounds.size.x > this.m_ViewBounds.size.x + 0.01f;
			}
		}

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x06003E07 RID: 15879 RVA: 0x0016B5E9 File Offset: 0x001697E9
		private bool vScrollingNeeded
		{
			get
			{
				return !Application.isPlaying || this.m_ContentBounds.size.y > this.m_ViewBounds.size.y + 0.01f;
			}
		}

		// Token: 0x06003E08 RID: 15880 RVA: 0x0016B61C File Offset: 0x0016981C
		public virtual void SetLayoutHorizontal()
		{
			this.m_Tracker.Clear();
			if (this.m_HSliderExpand || this.m_VSliderExpand)
			{
				this.m_Tracker.Add(this, this.viewRect, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.SizeDeltaY);
				this.viewRect.anchorMin = Vector2.zero;
				this.viewRect.anchorMax = Vector2.one;
				this.viewRect.sizeDelta = Vector2.zero;
				this.viewRect.anchoredPosition = Vector2.zero;
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
				this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			if (this.m_VSliderExpand && this.vScrollingNeeded)
			{
				this.viewRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.viewRect.sizeDelta.y);
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
				this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			if (this.m_HSliderExpand && this.hScrollingNeeded)
			{
				this.viewRect.sizeDelta = new Vector2(this.viewRect.sizeDelta.x, -(this.m_HSliderHeight + this.m_HorizontalScrollbarSpacing));
				this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			if (this.m_VSliderExpand && this.vScrollingNeeded && this.viewRect.sizeDelta.x == 0f && this.viewRect.sizeDelta.y < 0f)
			{
				this.viewRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.viewRect.sizeDelta.y);
			}
		}

		// Token: 0x06003E09 RID: 15881 RVA: 0x0016B878 File Offset: 0x00169A78
		public virtual void SetLayoutVertical()
		{
			this.UpdateScrollbarLayout();
			this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
			this.m_ContentBounds = this.GetBounds();
		}

		// Token: 0x06003E0A RID: 15882 RVA: 0x0016B8D4 File Offset: 0x00169AD4
		private void UpdateScrollbarVisibility()
		{
			if (this.m_VerticalScrollbar && this.m_VerticalScrollbarVisibility != ScrollRectEx.ScrollbarVisibility.Permanent && this.m_VerticalScrollbar.gameObject.activeSelf != this.vScrollingNeeded)
			{
				this.m_VerticalScrollbar.gameObject.SetActive(this.vScrollingNeeded);
			}
			if (this.m_HorizontalScrollbar && this.m_HorizontalScrollbarVisibility != ScrollRectEx.ScrollbarVisibility.Permanent && this.m_HorizontalScrollbar.gameObject.activeSelf != this.hScrollingNeeded)
			{
				this.m_HorizontalScrollbar.gameObject.SetActive(this.hScrollingNeeded);
			}
		}

		// Token: 0x06003E0B RID: 15883 RVA: 0x0016B968 File Offset: 0x00169B68
		private void UpdateScrollbarLayout()
		{
			if (this.m_VSliderExpand && this.m_HorizontalScrollbar)
			{
				this.m_Tracker.Add(this, this.m_HorizontalScrollbarRect, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.SizeDeltaX);
				this.m_HorizontalScrollbarRect.anchorMin = new Vector2(0f, this.m_HorizontalScrollbarRect.anchorMin.y);
				this.m_HorizontalScrollbarRect.anchorMax = new Vector2(1f, this.m_HorizontalScrollbarRect.anchorMax.y);
				this.m_HorizontalScrollbarRect.anchoredPosition = new Vector2(0f, this.m_HorizontalScrollbarRect.anchoredPosition.y);
				if (this.vScrollingNeeded)
				{
					this.m_HorizontalScrollbarRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.m_HorizontalScrollbarRect.sizeDelta.y);
				}
				else
				{
					this.m_HorizontalScrollbarRect.sizeDelta = new Vector2(0f, this.m_HorizontalScrollbarRect.sizeDelta.y);
				}
			}
			if (this.m_HSliderExpand && this.m_VerticalScrollbar)
			{
				this.m_Tracker.Add(this, this.m_VerticalScrollbarRect, DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaY);
				this.m_VerticalScrollbarRect.anchorMin = new Vector2(this.m_VerticalScrollbarRect.anchorMin.x, 0f);
				this.m_VerticalScrollbarRect.anchorMax = new Vector2(this.m_VerticalScrollbarRect.anchorMax.x, 1f);
				this.m_VerticalScrollbarRect.anchoredPosition = new Vector2(this.m_VerticalScrollbarRect.anchoredPosition.x, 0f);
				if (this.hScrollingNeeded)
				{
					this.m_VerticalScrollbarRect.sizeDelta = new Vector2(this.m_VerticalScrollbarRect.sizeDelta.x, -(this.m_HSliderHeight + this.m_HorizontalScrollbarSpacing));
					return;
				}
				this.m_VerticalScrollbarRect.sizeDelta = new Vector2(this.m_VerticalScrollbarRect.sizeDelta.x, 0f);
			}
		}

		// Token: 0x06003E0C RID: 15884 RVA: 0x0016BB70 File Offset: 0x00169D70
		private void UpdateBounds()
		{
			this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
			this.m_ContentBounds = this.GetBounds();
			if (this.m_Content == null)
			{
				return;
			}
			Vector3 size = this.m_ContentBounds.size;
			Vector3 center = this.m_ContentBounds.center;
			Vector3 vector = this.m_ViewBounds.size - size;
			if (vector.x > 0f)
			{
				center.x -= vector.x * (this.m_Content.pivot.x - 0.5f);
				size.x = this.m_ViewBounds.size.x;
			}
			if (vector.y > 0f)
			{
				center.y -= vector.y * (this.m_Content.pivot.y - 0.5f);
				size.y = this.m_ViewBounds.size.y;
			}
			this.m_ContentBounds.size = size;
			this.m_ContentBounds.center = center;
		}

		// Token: 0x06003E0D RID: 15885 RVA: 0x0016BCB0 File Offset: 0x00169EB0
		private Bounds GetBounds()
		{
			if (this.m_Content == null)
			{
				return default(Bounds);
			}
			Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			Matrix4x4 worldToLocalMatrix = this.viewRect.worldToLocalMatrix;
			this.m_Content.GetWorldCorners(this.m_Corners);
			for (int i = 0; i < 4; i++)
			{
				Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(this.m_Corners[i]);
				vector = Vector3.Min(lhs, vector);
				vector2 = Vector3.Max(lhs, vector2);
			}
			Bounds result = new Bounds(vector, Vector3.zero);
			result.Encapsulate(vector2);
			return result;
		}

		// Token: 0x06003E0E RID: 15886 RVA: 0x0016BD68 File Offset: 0x00169F68
		private Vector2 CalculateOffset(Vector2 delta)
		{
			Vector2 zero = Vector2.zero;
			if (this.m_MovementType == ScrollRectEx.MovementType.Unrestricted)
			{
				return zero;
			}
			Vector2 vector = this.m_ContentBounds.min;
			Vector2 vector2 = this.m_ContentBounds.max;
			if (this.m_Horizontal)
			{
				vector.x += delta.x;
				vector2.x += delta.x;
				if (vector.x > this.m_ViewBounds.min.x)
				{
					zero.x = this.m_ViewBounds.min.x - vector.x;
				}
				else if (vector2.x < this.m_ViewBounds.max.x)
				{
					zero.x = this.m_ViewBounds.max.x - vector2.x;
				}
			}
			if (this.m_Vertical)
			{
				vector.y += delta.y;
				vector2.y += delta.y;
				if (vector2.y < this.m_ViewBounds.max.y)
				{
					zero.y = this.m_ViewBounds.max.y - vector2.y;
				}
				else if (vector.y > this.m_ViewBounds.min.y)
				{
					zero.y = this.m_ViewBounds.min.y - vector.y;
				}
			}
			return zero;
		}

		// Token: 0x06003E0F RID: 15887 RVA: 0x0016BEDE File Offset: 0x0016A0DE
		protected void SetDirty()
		{
			if (!this.IsActive())
			{
				return;
			}
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
		}

		// Token: 0x06003E10 RID: 15888 RVA: 0x0016BEF4 File Offset: 0x0016A0F4
		protected void SetDirtyCaching()
		{
			if (!this.IsActive())
			{
				return;
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
		}

		// Token: 0x06003E11 RID: 15889 RVA: 0x0016BF10 File Offset: 0x0016A110
		public void CenterOnPosition(Vector2 pos)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			Vector2 vector = new Vector2(this.content.localScale.x, this.content.localScale.y);
			pos.x *= vector.x;
			pos.y *= vector.y;
			Vector2 vector2 = new Vector2(this.content.rect.width * vector.x - rectTransform.rect.width, this.content.rect.height * vector.y - rectTransform.rect.height);
			pos.x = pos.x / vector2.x + this.content.pivot.x;
			pos.y = pos.y / vector2.y + this.content.pivot.y;
			if (this.movementType != ScrollRectEx.MovementType.Unrestricted)
			{
				pos.x = Mathf.Clamp(pos.x, 0f, 1f);
				pos.y = Mathf.Clamp(pos.y, 0f, 1f);
			}
			this.normalizedPosition = pos;
		}

		// Token: 0x06003E12 RID: 15890 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LayoutComplete()
		{
		}

		// Token: 0x06003E13 RID: 15891 RVA: 0x000063A5 File Offset: 0x000045A5
		public void GraphicUpdateComplete()
		{
		}

		// Token: 0x06003E14 RID: 15892 RVA: 0x0005DA2D File Offset: 0x0005BC2D
		Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}

		// Token: 0x02000F0B RID: 3851
		public enum MovementType
		{
			// Token: 0x04004E26 RID: 20006
			Unrestricted,
			// Token: 0x04004E27 RID: 20007
			Elastic,
			// Token: 0x04004E28 RID: 20008
			Clamped
		}

		// Token: 0x02000F0C RID: 3852
		public enum ScrollbarVisibility
		{
			// Token: 0x04004E2A RID: 20010
			Permanent,
			// Token: 0x04004E2B RID: 20011
			AutoHide,
			// Token: 0x04004E2C RID: 20012
			AutoHideAndExpandViewport
		}

		// Token: 0x02000F0D RID: 3853
		[Serializable]
		public class ScrollRectEvent : UnityEvent<Vector2>
		{
		}
	}
}
