using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000779 RID: 1913
public class EnumListUI : MonoBehaviour
{
	// Token: 0x04002AFE RID: 11006
	public Transform PrefabItem;

	// Token: 0x04002AFF RID: 11007
	public Transform Container;

	// Token: 0x04002B00 RID: 11008
	private Action<object> clickedAction;

	// Token: 0x04002B01 RID: 11009
	private CanvasScaler canvasScaler;

	// Token: 0x060034D2 RID: 13522 RVA: 0x001461C0 File Offset: 0x001443C0
	private void Awake()
	{
		this.Hide();
	}

	// Token: 0x060034D3 RID: 13523 RVA: 0x001461C8 File Offset: 0x001443C8
	public void Show(List<object> values, Action<object> clicked)
	{
		base.gameObject.SetActive(true);
		this.clickedAction = clicked;
		foreach (object obj in this.Container)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		foreach (object obj2 in values)
		{
			Transform transform = UnityEngine.Object.Instantiate<Transform>(this.PrefabItem);
			transform.SetParent(this.Container, false);
			transform.GetComponent<EnumListItemUI>().Init(obj2, obj2.ToString(), this);
		}
	}

	// Token: 0x060034D4 RID: 13524 RVA: 0x00146298 File Offset: 0x00144498
	public void ItemClicked(object value)
	{
		Action<object> action = this.clickedAction;
		if (action != null)
		{
			action(value);
		}
		this.Hide();
	}

	// Token: 0x060034D5 RID: 13525 RVA: 0x001462B2 File Offset: 0x001444B2
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}
}
