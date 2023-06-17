using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000824 RID: 2084
public class ItemIcon : BaseMonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IDraggable, IInventoryChanged, IItemAmountChanged, IItemIconChanged
{
	// Token: 0x04002EB9 RID: 11961
	private Color backgroundColor;

	// Token: 0x04002EBA RID: 11962
	public Color selectedBackgroundColor = new Color(0.12156863f, 0.41960785f, 0.627451f, 0.78431374f);

	// Token: 0x04002EBB RID: 11963
	public float unoccupiedAlpha = 1f;

	// Token: 0x04002EBC RID: 11964
	public Color unoccupiedColor;

	// Token: 0x04002EBD RID: 11965
	public ItemContainerSource containerSource;

	// Token: 0x04002EBE RID: 11966
	public int slotOffset;

	// Token: 0x04002EBF RID: 11967
	[Range(0f, 64f)]
	public int slot;

	// Token: 0x04002EC0 RID: 11968
	public bool setSlotFromSiblingIndex = true;

	// Token: 0x04002EC1 RID: 11969
	public GameObject slots;

	// Token: 0x04002EC2 RID: 11970
	public CanvasGroup iconContents;

	// Token: 0x04002EC3 RID: 11971
	public CanvasGroup canvasGroup;

	// Token: 0x04002EC4 RID: 11972
	public Image iconImage;

	// Token: 0x04002EC5 RID: 11973
	public Image underlayImage;

	// Token: 0x04002EC6 RID: 11974
	public Text amountText;

	// Token: 0x04002EC7 RID: 11975
	public Text hoverText;

	// Token: 0x04002EC8 RID: 11976
	public Image hoverOutline;

	// Token: 0x04002EC9 RID: 11977
	public Image cornerIcon;

	// Token: 0x04002ECA RID: 11978
	public Image lockedImage;

	// Token: 0x04002ECB RID: 11979
	public Image progressImage;

	// Token: 0x04002ECC RID: 11980
	public Image backgroundImage;

	// Token: 0x04002ECD RID: 11981
	public Image backgroundUnderlayImage;

	// Token: 0x04002ECE RID: 11982
	public Image progressPanel;

	// Token: 0x04002ECF RID: 11983
	public Sprite emptySlotBackgroundSprite;

	// Token: 0x04002ED0 RID: 11984
	public CanvasGroup conditionObject;

	// Token: 0x04002ED1 RID: 11985
	public Image conditionFill;

	// Token: 0x04002ED2 RID: 11986
	public Image maxConditionFill;

	// Token: 0x04002ED3 RID: 11987
	public GameObject lightEnabled;

	// Token: 0x04002ED4 RID: 11988
	public bool allowSelection = true;

	// Token: 0x04002ED5 RID: 11989
	public bool allowDropping = true;

	// Token: 0x04002ED6 RID: 11990
	public bool allowMove = true;

	// Token: 0x04002ED7 RID: 11991
	public bool showCountDropShadow;

	// Token: 0x04002ED8 RID: 11992
	[NonSerialized]
	public Item item;

	// Token: 0x04002ED9 RID: 11993
	[NonSerialized]
	public bool invalidSlot;

	// Token: 0x04002EDA RID: 11994
	public SoundDefinition hoverSound;

	// Token: 0x060035E3 RID: 13795 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnPointerClick(PointerEventData eventData)
	{
	}

	// Token: 0x060035E4 RID: 13796 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnPointerEnter(PointerEventData eventData)
	{
	}

	// Token: 0x060035E5 RID: 13797 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnPointerExit(PointerEventData eventData)
	{
	}
}
