using System;
using System.Collections.Generic;
using Facepunch;
using Rust.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000887 RID: 2183
public class TweakUIDropdown : TweakUIBase
{
	// Token: 0x04003102 RID: 12546
	public RustText Current;

	// Token: 0x04003103 RID: 12547
	public Image BackgroundImage;

	// Token: 0x04003104 RID: 12548
	public RustButton Opener;

	// Token: 0x04003105 RID: 12549
	public RectTransform Dropdown;

	// Token: 0x04003106 RID: 12550
	public RectTransform DropdownContainer;

	// Token: 0x04003107 RID: 12551
	public GameObject DropdownItemPrefab;

	// Token: 0x04003108 RID: 12552
	public TweakUIDropdown.NameValue[] nameValues;

	// Token: 0x04003109 RID: 12553
	public bool assignImageColor;

	// Token: 0x0400310A RID: 12554
	public UnityEvent onValueChanged = new UnityEvent();

	// Token: 0x0400310B RID: 12555
	public int currentValue;

	// Token: 0x060036AB RID: 13995 RVA: 0x0014A600 File Offset: 0x00148800
	protected override void Init()
	{
		base.Init();
		this.DropdownItemPrefab.SetActive(false);
		this.UpdateDropdownOptions();
		this.Opener.SetToggleFalse();
		this.ResetToConvar();
	}

	// Token: 0x060036AC RID: 13996 RVA: 0x0014A5BC File Offset: 0x001487BC
	protected void OnEnable()
	{
		this.ResetToConvar();
	}

	// Token: 0x060036AD RID: 13997 RVA: 0x0014A62C File Offset: 0x0014882C
	public void UpdateDropdownOptions()
	{
		List<RustButton> list = Pool.GetList<RustButton>();
		this.DropdownContainer.GetComponentsInChildren<RustButton>(false, list);
		foreach (RustButton rustButton in list)
		{
			UnityEngine.Object.Destroy(rustButton.gameObject);
		}
		Pool.FreeList<RustButton>(ref list);
		for (int i = 0; i < this.nameValues.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.DropdownItemPrefab, this.DropdownContainer);
			int itemIndex = i;
			RustButton component = gameObject.GetComponent<RustButton>();
			component.Text.SetPhrase(this.nameValues[i].label);
			component.OnPressed.AddListener(delegate()
			{
				this.ChangeValue(itemIndex);
			});
			gameObject.SetActive(true);
		}
	}

	// Token: 0x060036AE RID: 13998 RVA: 0x0014A708 File Offset: 0x00148908
	public void OnValueChanged()
	{
		if (this.ApplyImmediatelyOnChange)
		{
			this.SetConvarValue();
		}
	}

	// Token: 0x060036AF RID: 13999 RVA: 0x0014A718 File Offset: 0x00148918
	public void OnDropdownOpen()
	{
		RectTransform rectTransform = (RectTransform)base.transform;
		if (rectTransform.position.y <= (float)Screen.height / 2f)
		{
			this.Dropdown.pivot = new Vector2(0.5f, 0f);
			this.Dropdown.anchoredPosition = this.Dropdown.anchoredPosition.WithY(0f);
			return;
		}
		this.Dropdown.pivot = new Vector2(0.5f, 1f);
		this.Dropdown.anchoredPosition = this.Dropdown.anchoredPosition.WithY(-rectTransform.rect.height);
	}

	// Token: 0x060036B0 RID: 14000 RVA: 0x0014A7D0 File Offset: 0x001489D0
	public void ChangeValue(int index)
	{
		this.Opener.SetToggleFalse();
		int num = Mathf.Clamp(index, 0, this.nameValues.Length - 1);
		bool flag = num != this.currentValue;
		this.currentValue = num;
		if (this.ApplyImmediatelyOnChange)
		{
			this.SetConvarValue();
		}
		else
		{
			this.ShowValue(this.nameValues[this.currentValue].value);
		}
		if (flag)
		{
			UnityEvent unityEvent = this.onValueChanged;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x060036B1 RID: 14001 RVA: 0x0014A848 File Offset: 0x00148A48
	protected override void SetConvarValue()
	{
		base.SetConvarValue();
		TweakUIDropdown.NameValue nameValue = this.nameValues[this.currentValue];
		if (this.conVar == null)
		{
			return;
		}
		if (this.conVar.String == nameValue.value)
		{
			return;
		}
		this.conVar.Set(nameValue.value);
	}

	// Token: 0x060036B2 RID: 14002 RVA: 0x0014A89C File Offset: 0x00148A9C
	public override void ResetToConvar()
	{
		base.ResetToConvar();
		if (this.conVar == null)
		{
			return;
		}
		string @string = this.conVar.String;
		this.ShowValue(@string);
	}

	// Token: 0x060036B3 RID: 14003 RVA: 0x0014A8CC File Offset: 0x00148ACC
	private void ShowValue(string value)
	{
		int i = 0;
		while (i < this.nameValues.Length)
		{
			if (!(this.nameValues[i].value != value))
			{
				this.Current.SetPhrase(this.nameValues[i].label);
				this.currentValue = i;
				if (this.assignImageColor)
				{
					this.BackgroundImage.color = this.nameValues[i].imageColor;
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x02000E98 RID: 3736
	[Serializable]
	public class NameValue
	{
		// Token: 0x04004C3E RID: 19518
		public string value;

		// Token: 0x04004C3F RID: 19519
		public Color imageColor;

		// Token: 0x04004C40 RID: 19520
		public Translate.Phrase label;
	}
}
