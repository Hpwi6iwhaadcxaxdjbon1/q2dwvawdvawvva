using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008B9 RID: 2233
public class TabToggle : MonoBehaviour
{
	// Token: 0x04003223 RID: 12835
	public Transform TabHolder;

	// Token: 0x04003224 RID: 12836
	public Transform ContentHolder;

	// Token: 0x04003225 RID: 12837
	public bool FadeIn;

	// Token: 0x04003226 RID: 12838
	public bool FadeOut;

	// Token: 0x0600372E RID: 14126 RVA: 0x0014C9F4 File Offset: 0x0014ABF4
	public void Awake()
	{
		if (this.TabHolder)
		{
			for (int i = 0; i < this.TabHolder.childCount; i++)
			{
				Button c = this.TabHolder.GetChild(i).GetComponent<Button>();
				if (c)
				{
					c.onClick.AddListener(delegate()
					{
						this.SwitchTo(c);
					});
				}
			}
		}
	}

	// Token: 0x0600372F RID: 14127 RVA: 0x0014CA74 File Offset: 0x0014AC74
	public void SwitchTo(Button sourceTab)
	{
		string name = sourceTab.transform.name;
		if (this.TabHolder)
		{
			for (int i = 0; i < this.TabHolder.childCount; i++)
			{
				Button component = this.TabHolder.GetChild(i).GetComponent<Button>();
				if (component)
				{
					component.interactable = (component.name != name);
				}
			}
		}
		if (this.ContentHolder)
		{
			for (int j = 0; j < this.ContentHolder.childCount; j++)
			{
				Transform child = this.ContentHolder.GetChild(j);
				if (child.name == name)
				{
					this.Show(child.gameObject);
				}
				else
				{
					this.Hide(child.gameObject);
				}
			}
		}
	}

	// Token: 0x06003730 RID: 14128 RVA: 0x0014CB38 File Offset: 0x0014AD38
	private void Hide(GameObject go)
	{
		if (!go.activeSelf)
		{
			return;
		}
		CanvasGroup component = go.GetComponent<CanvasGroup>();
		if (this.FadeOut && component)
		{
			LeanTween.alphaCanvas(component, 0f, 0.1f).setOnComplete(delegate()
			{
				go.SetActive(false);
			});
			return;
		}
		go.SetActive(false);
	}

	// Token: 0x06003731 RID: 14129 RVA: 0x0014CBAC File Offset: 0x0014ADAC
	private void Show(GameObject go)
	{
		if (go.activeSelf)
		{
			return;
		}
		CanvasGroup component = go.GetComponent<CanvasGroup>();
		if (this.FadeIn && component)
		{
			component.alpha = 0f;
			LeanTween.alphaCanvas(component, 1f, 0.1f);
		}
		go.SetActive(true);
	}
}
