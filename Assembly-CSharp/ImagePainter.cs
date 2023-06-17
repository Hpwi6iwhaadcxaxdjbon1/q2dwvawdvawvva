using System;
using Painting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x020007EE RID: 2030
public class ImagePainter : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IInitializePotentialDragHandler
{
	// Token: 0x04002D67 RID: 11623
	public ImagePainter.OnDrawingEvent onDrawing = new ImagePainter.OnDrawingEvent();

	// Token: 0x04002D68 RID: 11624
	public MonoBehaviour redirectRightClick;

	// Token: 0x04002D69 RID: 11625
	[Tooltip("Spacing scale will depend on your texel size, tweak to what's right.")]
	public float spacingScale = 1f;

	// Token: 0x04002D6A RID: 11626
	internal Brush brush;

	// Token: 0x04002D6B RID: 11627
	internal ImagePainter.PointerState[] pointerState = new ImagePainter.PointerState[]
	{
		new ImagePainter.PointerState(),
		new ImagePainter.PointerState(),
		new ImagePainter.PointerState()
	};

	// Token: 0x17000456 RID: 1110
	// (get) Token: 0x06003582 RID: 13698 RVA: 0x000B8990 File Offset: 0x000B6B90
	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	// Token: 0x06003583 RID: 13699 RVA: 0x00147180 File Offset: 0x00145380
	public virtual void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			return;
		}
		Vector2 position;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.position, eventData.pressEventCamera, out position);
		this.DrawAt(position, eventData.button);
		this.pointerState[(int)eventData.button].isDown = true;
	}

	// Token: 0x06003584 RID: 13700 RVA: 0x001471D1 File Offset: 0x001453D1
	public virtual void OnPointerUp(PointerEventData eventData)
	{
		this.pointerState[(int)eventData.button].isDown = false;
	}

	// Token: 0x06003585 RID: 13701 RVA: 0x001471E8 File Offset: 0x001453E8
	public virtual void OnDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnDrag", eventData);
			}
			return;
		}
		Vector2 position;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.position, eventData.pressEventCamera, out position);
		this.DrawAt(position, eventData.button);
	}

	// Token: 0x06003586 RID: 13702 RVA: 0x00147244 File Offset: 0x00145444
	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnBeginDrag", eventData);
			}
			return;
		}
	}

	// Token: 0x06003587 RID: 13703 RVA: 0x0014726E File Offset: 0x0014546E
	public virtual void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnEndDrag", eventData);
			}
			return;
		}
	}

	// Token: 0x06003588 RID: 13704 RVA: 0x00147298 File Offset: 0x00145498
	public virtual void OnInitializePotentialDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnInitializePotentialDrag", eventData);
			}
			return;
		}
	}

	// Token: 0x06003589 RID: 13705 RVA: 0x001472C4 File Offset: 0x001454C4
	private void DrawAt(Vector2 position, PointerEventData.InputButton button)
	{
		if (this.brush == null)
		{
			return;
		}
		ImagePainter.PointerState pointerState = this.pointerState[(int)button];
		Vector2 vector = this.rectTransform.Unpivot(position);
		if (pointerState.isDown)
		{
			Vector2 vector2 = pointerState.lastPos - vector;
			Vector2 normalized = vector2.normalized;
			for (float num = 0f; num < vector2.magnitude; num += Mathf.Max(this.brush.spacing, 1f) * Mathf.Max(this.spacingScale, 0.1f))
			{
				this.onDrawing.Invoke(vector + num * normalized, this.brush);
			}
			pointerState.lastPos = vector;
			return;
		}
		this.onDrawing.Invoke(vector, this.brush);
		pointerState.lastPos = vector;
	}

	// Token: 0x0600358A RID: 13706 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x0600358B RID: 13707 RVA: 0x0014738C File Offset: 0x0014558C
	public void UpdateBrush(Brush brush)
	{
		this.brush = brush;
	}

	// Token: 0x02000E7F RID: 3711
	[Serializable]
	public class OnDrawingEvent : UnityEvent<Vector2, Brush>
	{
	}

	// Token: 0x02000E80 RID: 3712
	internal class PointerState
	{
		// Token: 0x04004BE9 RID: 19433
		public Vector2 lastPos;

		// Token: 0x04004BEA RID: 19434
		public bool isDown;
	}
}
