using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020008A0 RID: 2208
[ExecuteInEditMode]
public class PieMenu : UIBehaviour
{
	// Token: 0x04003177 RID: 12663
	public static PieMenu Instance;

	// Token: 0x04003178 RID: 12664
	public Image middleBox;

	// Token: 0x04003179 RID: 12665
	public PieShape pieBackgroundBlur;

	// Token: 0x0400317A RID: 12666
	public PieShape pieBackground;

	// Token: 0x0400317B RID: 12667
	public PieShape pieSelection;

	// Token: 0x0400317C RID: 12668
	public GameObject pieOptionPrefab;

	// Token: 0x0400317D RID: 12669
	public GameObject optionsCanvas;

	// Token: 0x0400317E RID: 12670
	public PieMenu.MenuOption[] options;

	// Token: 0x0400317F RID: 12671
	public GameObject scaleTarget;

	// Token: 0x04003180 RID: 12672
	public GameObject arrowLeft;

	// Token: 0x04003181 RID: 12673
	public GameObject arrowRight;

	// Token: 0x04003182 RID: 12674
	public float sliceGaps = 10f;

	// Token: 0x04003183 RID: 12675
	[Range(0f, 1f)]
	public float outerSize = 1f;

	// Token: 0x04003184 RID: 12676
	[Range(0f, 1f)]
	public float innerSize = 0.5f;

	// Token: 0x04003185 RID: 12677
	[Range(0f, 1f)]
	public float iconSize = 0.8f;

	// Token: 0x04003186 RID: 12678
	[Range(0f, 360f)]
	public float startRadius;

	// Token: 0x04003187 RID: 12679
	[Range(0f, 360f)]
	public float radiusSize = 360f;

	// Token: 0x04003188 RID: 12680
	public Image middleImage;

	// Token: 0x04003189 RID: 12681
	public TextMeshProUGUI middleTitle;

	// Token: 0x0400318A RID: 12682
	public TextMeshProUGUI middleDesc;

	// Token: 0x0400318B RID: 12683
	public TextMeshProUGUI middleRequired;

	// Token: 0x0400318C RID: 12684
	public Color colorIconActive;

	// Token: 0x0400318D RID: 12685
	public Color colorIconHovered;

	// Token: 0x0400318E RID: 12686
	public Color colorIconDisabled;

	// Token: 0x0400318F RID: 12687
	public Color colorBackgroundDisabled;

	// Token: 0x04003190 RID: 12688
	public SoundDefinition clipOpen;

	// Token: 0x04003191 RID: 12689
	public SoundDefinition clipCancel;

	// Token: 0x04003192 RID: 12690
	public SoundDefinition clipChanged;

	// Token: 0x04003193 RID: 12691
	public SoundDefinition clipSelected;

	// Token: 0x04003194 RID: 12692
	public PieMenu.MenuOption defaultOption;

	// Token: 0x04003195 RID: 12693
	private bool isClosing;

	// Token: 0x04003196 RID: 12694
	private CanvasGroup canvasGroup;

	// Token: 0x04003198 RID: 12696
	public Material IconMaterial;

	// Token: 0x04003199 RID: 12697
	internal PieMenu.MenuOption selectedOption;

	// Token: 0x0400319A RID: 12698
	private static Color pieSelectionColor = new Color(0.804f, 0.255f, 0.169f, 1f);

	// Token: 0x0400319B RID: 12699
	private static Color middleImageColor = new Color(0.804f, 0.255f, 0.169f, 0.784f);

	// Token: 0x0400319C RID: 12700
	private static AnimationCurve easePunch = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.112586f, 0.9976035f),
		new Keyframe(0.3120486f, 0.01720615f),
		new Keyframe(0.4316337f, 0.17030682f),
		new Keyframe(0.5524869f, 0.03141804f),
		new Keyframe(0.6549395f, 0.002909959f),
		new Keyframe(0.770987f, 0.009817753f),
		new Keyframe(0.8838775f, 0.001939224f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x1700045E RID: 1118
	// (get) Token: 0x060036F2 RID: 14066 RVA: 0x0014B450 File Offset: 0x00149650
	// (set) Token: 0x060036F3 RID: 14067 RVA: 0x0014B458 File Offset: 0x00149658
	public bool IsOpen { get; private set; }

	// Token: 0x060036F4 RID: 14068 RVA: 0x0014B464 File Offset: 0x00149664
	protected override void Start()
	{
		base.Start();
		PieMenu.Instance = this;
		this.canvasGroup = base.GetComponentInChildren<CanvasGroup>();
		this.canvasGroup.alpha = 0f;
		this.canvasGroup.interactable = false;
		this.canvasGroup.blocksRaycasts = false;
		this.IsOpen = false;
		this.isClosing = true;
		base.gameObject.SetChildComponentsEnabled(false);
	}

	// Token: 0x060036F5 RID: 14069 RVA: 0x0014B4CB File Offset: 0x001496CB
	public void Clear()
	{
		this.options = new PieMenu.MenuOption[0];
	}

	// Token: 0x060036F6 RID: 14070 RVA: 0x0014B4DC File Offset: 0x001496DC
	public void AddOption(PieMenu.MenuOption option)
	{
		List<PieMenu.MenuOption> list = this.options.ToList<PieMenu.MenuOption>();
		list.Add(option);
		this.options = list.ToArray();
	}

	// Token: 0x060036F7 RID: 14071 RVA: 0x0014B508 File Offset: 0x00149708
	public void FinishAndOpen()
	{
		this.IsOpen = true;
		this.isClosing = false;
		this.SetDefaultOption();
		this.Rebuild();
		this.UpdateInteraction(false);
		this.PlayOpenSound();
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(this.scaleTarget);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f).setEase(LeanTweenType.easeOutCirc);
		this.scaleTarget.transform.localScale = Vector3.one * 1.5f;
		LeanTween.scale(this.scaleTarget, Vector3.one, 0.1f).setEase(LeanTweenType.easeOutBounce);
		PieMenu.Instance.gameObject.SetChildComponentsEnabled(true);
	}

	// Token: 0x060036F8 RID: 14072 RVA: 0x0014B5CB File Offset: 0x001497CB
	protected override void OnEnable()
	{
		base.OnEnable();
		this.Rebuild();
	}

	// Token: 0x060036F9 RID: 14073 RVA: 0x0014B5DC File Offset: 0x001497DC
	public void SetDefaultOption()
	{
		this.defaultOption = null;
		for (int i = 0; i < this.options.Length; i++)
		{
			if (!this.options[i].disabled)
			{
				if (this.defaultOption == null)
				{
					this.defaultOption = this.options[i];
				}
				if (this.options[i].selected)
				{
					this.defaultOption = this.options[i];
					return;
				}
			}
		}
	}

	// Token: 0x060036FA RID: 14074 RVA: 0x000063A5 File Offset: 0x000045A5
	public void PlayOpenSound()
	{
	}

	// Token: 0x060036FB RID: 14075 RVA: 0x000063A5 File Offset: 0x000045A5
	public void PlayCancelSound()
	{
	}

	// Token: 0x060036FC RID: 14076 RVA: 0x0014B648 File Offset: 0x00149848
	public void Close(bool success = false)
	{
		if (this.isClosing)
		{
			return;
		}
		this.isClosing = true;
		NeedsCursor component = base.GetComponent<NeedsCursor>();
		if (component != null)
		{
			component.enabled = false;
		}
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(this.scaleTarget);
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setEase(LeanTweenType.easeOutCirc);
		LeanTween.scale(this.scaleTarget, Vector3.one * (success ? 1.5f : 0.5f), 0.2f).setEase(LeanTweenType.easeOutCirc);
		PieMenu.Instance.gameObject.SetChildComponentsEnabled(false);
		this.IsOpen = false;
		this.selectedOption = null;
	}

	// Token: 0x060036FD RID: 14077 RVA: 0x0014B700 File Offset: 0x00149900
	private void Update()
	{
		if (this.pieBackground.innerSize != this.innerSize || this.pieBackground.outerSize != this.outerSize || this.pieBackground.startRadius != this.startRadius || this.pieBackground.endRadius != this.startRadius + this.radiusSize)
		{
			this.pieBackground.startRadius = this.startRadius;
			this.pieBackground.endRadius = this.startRadius + this.radiusSize;
			this.pieBackground.innerSize = this.innerSize;
			this.pieBackground.outerSize = this.outerSize;
			this.pieBackground.SetVerticesDirty();
		}
		this.UpdateInteraction(true);
		if (this.IsOpen)
		{
			CursorManager.HoldOpen(false);
			IngameMenuBackground.Enabled = true;
		}
	}

	// Token: 0x060036FE RID: 14078 RVA: 0x0014B7D4 File Offset: 0x001499D4
	public void Rebuild()
	{
		this.options = (from x in this.options
		orderby x.order
		select x).ToArray<PieMenu.MenuOption>();
		while (this.optionsCanvas.transform.childCount > 0)
		{
			if (UnityEngine.Application.isPlaying)
			{
				GameManager.DestroyImmediate(this.optionsCanvas.transform.GetChild(0).gameObject, true);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(this.optionsCanvas.transform.GetChild(0).gameObject);
			}
		}
		if (this.options.Length != 0)
		{
			for (int i = 0; i < this.options.Length; i++)
			{
				bool flag = false;
				if (this.options[i].allowMerge)
				{
					if (i > 0)
					{
						flag |= (this.options[i].order == this.options[i - 1].order);
					}
					if (i < this.options.Length - 1)
					{
						flag |= (this.options[i].order == this.options[i + 1].order);
					}
				}
				this.options[i].wantsMerge = flag;
			}
			int num = this.options.Length;
			int num2 = (from x in this.options
			where x.wantsMerge
			select x).Count<PieMenu.MenuOption>();
			int num3 = num - num2;
			int num4 = num3 + num2 / 2;
			float num5 = this.radiusSize / (float)num * 0.75f;
			float num6 = (this.radiusSize - num5 * (float)num2) / (float)num3;
			float num7 = this.startRadius - this.radiusSize / (float)num4 * 0.25f;
			for (int j = 0; j < this.options.Length; j++)
			{
				float num8 = this.options[j].wantsMerge ? 0.8f : 1f;
				float num9 = this.options[j].wantsMerge ? num5 : num6;
				GameObject gameObject = Facepunch.Instantiate.GameObject(this.pieOptionPrefab, null);
				gameObject.transform.SetParent(this.optionsCanvas.transform, false);
				this.options[j].option = gameObject.GetComponent<PieOption>();
				this.options[j].option.UpdateOption(num7, num9, this.sliceGaps, this.options[j].name, this.outerSize, this.innerSize, num8 * this.iconSize, this.options[j].sprite, this.options[j].showOverlay);
				this.options[j].option.imageIcon.material = ((this.options[j].overrideColorMode != null && this.options[j].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.SpriteColor) ? null : this.IconMaterial);
				num7 += num9;
			}
		}
		this.selectedOption = null;
	}

	// Token: 0x060036FF RID: 14079 RVA: 0x0014BADC File Offset: 0x00149CDC
	public void UpdateInteraction(bool allowLerp = true)
	{
		if (this.isClosing)
		{
			return;
		}
		Vector3 vector = UnityEngine.Input.mousePosition - new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
		float num = Mathf.Atan2(vector.x, vector.y) * 57.29578f;
		if (num < 0f)
		{
			num += 360f;
		}
		for (int i = 0; i < this.options.Length; i++)
		{
			float midRadius = this.options[i].option.midRadius;
			float sliceSize = this.options[i].option.sliceSize;
			if ((vector.magnitude < 32f && this.options[i] == this.defaultOption) || (vector.magnitude >= 32f && Mathf.Abs(Mathf.DeltaAngle(num, midRadius)) < sliceSize * 0.5f))
			{
				if (allowLerp)
				{
					this.pieSelection.startRadius = Mathf.MoveTowardsAngle(this.pieSelection.startRadius, this.options[i].option.background.startRadius, Time.deltaTime * Mathf.Abs(Mathf.DeltaAngle(this.pieSelection.startRadius, this.options[i].option.background.startRadius) * 30f + 10f));
					this.pieSelection.endRadius = Mathf.MoveTowardsAngle(this.pieSelection.endRadius, this.options[i].option.background.endRadius, Time.deltaTime * Mathf.Abs(Mathf.DeltaAngle(this.pieSelection.endRadius, this.options[i].option.background.endRadius) * 30f + 10f));
				}
				else
				{
					this.pieSelection.startRadius = this.options[i].option.background.startRadius;
					this.pieSelection.endRadius = this.options[i].option.background.endRadius;
				}
				this.middleImage.material = this.IconMaterial;
				if (this.options[i].overrideColorMode != null)
				{
					if (this.options[i].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.CustomColor)
					{
						Color customColor = this.options[i].overrideColorMode.Value.CustomColor;
						this.pieSelection.color = customColor;
						customColor.a = PieMenu.middleImageColor.a;
						this.middleImage.color = customColor;
					}
					else if (this.options[i].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.SpriteColor)
					{
						this.pieSelection.color = PieMenu.pieSelectionColor;
						this.middleImage.color = Color.white;
						this.middleImage.material = null;
					}
				}
				else
				{
					this.pieSelection.color = PieMenu.pieSelectionColor;
					this.middleImage.color = PieMenu.middleImageColor;
				}
				this.pieSelection.SetVerticesDirty();
				this.middleImage.sprite = this.options[i].sprite;
				this.middleTitle.text = this.options[i].name;
				this.middleDesc.text = this.options[i].desc;
				this.middleRequired.text = "";
				Facepunch.Input.Button buttonObjectWithBind = Facepunch.Input.GetButtonObjectWithBind("+prevskin");
				if (this.options[i].actionPrev != null && buttonObjectWithBind != null && buttonObjectWithBind.Code != KeyCode.None)
				{
					this.arrowLeft.SetActive(true);
					this.arrowLeft.GetComponentInChildren<TextMeshProUGUI>().text = buttonObjectWithBind.Code.ToShortname(true);
				}
				else
				{
					this.arrowLeft.SetActive(false);
				}
				Facepunch.Input.Button buttonObjectWithBind2 = Facepunch.Input.GetButtonObjectWithBind("+nextskin");
				if (this.options[i].actionNext != null && buttonObjectWithBind2 != null && buttonObjectWithBind2.Code != KeyCode.None)
				{
					this.arrowRight.SetActive(true);
					this.arrowRight.GetComponentInChildren<TextMeshProUGUI>().text = buttonObjectWithBind2.Code.ToShortname(true);
				}
				else
				{
					this.arrowRight.SetActive(false);
				}
				string text = this.options[i].requirements;
				if (text != null)
				{
					text = text.Replace("[e]", "<color=#CD412B>");
					text = text.Replace("[/e]", "</color>");
					this.middleRequired.text = text;
				}
				if (!this.options[i].showOverlay)
				{
					this.options[i].option.imageIcon.color = this.colorIconHovered;
				}
				if (this.selectedOption != this.options[i])
				{
					if (this.selectedOption != null && !this.options[i].disabled)
					{
						this.scaleTarget.transform.localScale = Vector3.one;
						LeanTween.scale(this.scaleTarget, Vector3.one * 1.03f, 0.2f).setEase(PieMenu.easePunch);
					}
					if (this.selectedOption != null)
					{
						this.selectedOption.option.imageIcon.RebuildHackUnity2019();
					}
					this.selectedOption = this.options[i];
					if (this.selectedOption != null)
					{
						this.selectedOption.option.imageIcon.RebuildHackUnity2019();
					}
				}
			}
			else
			{
				this.options[i].option.imageIcon.material = this.IconMaterial;
				if (this.options[i].overrideColorMode != null)
				{
					if (this.options[i].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.CustomColor)
					{
						this.options[i].option.imageIcon.color = this.options[i].overrideColorMode.Value.CustomColor;
					}
					else if (this.options[i].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.SpriteColor)
					{
						this.options[i].option.imageIcon.color = Color.white;
						this.options[i].option.imageIcon.material = null;
					}
				}
				else
				{
					this.options[i].option.imageIcon.color = this.colorIconActive;
				}
			}
			if (this.options[i].disabled)
			{
				this.options[i].option.imageIcon.color = this.colorIconDisabled;
				this.options[i].option.background.color = this.colorBackgroundDisabled;
			}
		}
	}

	// Token: 0x06003700 RID: 14080 RVA: 0x0000441C File Offset: 0x0000261C
	public bool DoSelect()
	{
		return true;
	}

	// Token: 0x06003701 RID: 14081 RVA: 0x000063A5 File Offset: 0x000045A5
	public void DoPrev()
	{
	}

	// Token: 0x06003702 RID: 14082 RVA: 0x000063A5 File Offset: 0x000045A5
	public void DoNext()
	{
	}

	// Token: 0x02000EA0 RID: 3744
	[Serializable]
	public class MenuOption
	{
		// Token: 0x04004C56 RID: 19542
		public string name;

		// Token: 0x04004C57 RID: 19543
		public string desc;

		// Token: 0x04004C58 RID: 19544
		public string requirements;

		// Token: 0x04004C59 RID: 19545
		public Sprite sprite;

		// Token: 0x04004C5A RID: 19546
		public bool disabled;

		// Token: 0x04004C5B RID: 19547
		public int order;

		// Token: 0x04004C5C RID: 19548
		public PieMenu.MenuOption.ColorMode? overrideColorMode;

		// Token: 0x04004C5D RID: 19549
		public bool showOverlay;

		// Token: 0x04004C5E RID: 19550
		[NonSerialized]
		public Action<BasePlayer> action;

		// Token: 0x04004C5F RID: 19551
		[NonSerialized]
		public Action<BasePlayer> actionPrev;

		// Token: 0x04004C60 RID: 19552
		[NonSerialized]
		public Action<BasePlayer> actionNext;

		// Token: 0x04004C61 RID: 19553
		[NonSerialized]
		public PieOption option;

		// Token: 0x04004C62 RID: 19554
		[NonSerialized]
		public bool selected;

		// Token: 0x04004C63 RID: 19555
		[NonSerialized]
		public bool allowMerge;

		// Token: 0x04004C64 RID: 19556
		[NonSerialized]
		public bool wantsMerge;

		// Token: 0x02000FD5 RID: 4053
		public struct ColorMode
		{
			// Token: 0x04005101 RID: 20737
			public PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption Mode;

			// Token: 0x04005102 RID: 20738
			public Color CustomColor;

			// Token: 0x02000FF4 RID: 4084
			public enum PieMenuSpriteColorOption
			{
				// Token: 0x040051C2 RID: 20930
				CustomColor,
				// Token: 0x040051C3 RID: 20931
				SpriteColor
			}
		}
	}
}
