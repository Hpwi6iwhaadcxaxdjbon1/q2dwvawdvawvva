using System;
using System.Linq;
using Rust;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020008BE RID: 2238
public class ToggleGroupCookie : MonoBehaviour
{
	// Token: 0x17000467 RID: 1127
	// (get) Token: 0x0600373E RID: 14142 RVA: 0x0014CD12 File Offset: 0x0014AF12
	public ToggleGroup group
	{
		get
		{
			return base.GetComponent<ToggleGroup>();
		}
	}

	// Token: 0x0600373F RID: 14143 RVA: 0x0014CD1C File Offset: 0x0014AF1C
	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString("ToggleGroupCookie_" + base.name);
		if (!string.IsNullOrEmpty(@string))
		{
			Transform transform = base.transform.Find(@string);
			if (transform)
			{
				Toggle component = transform.GetComponent<Toggle>();
				if (component)
				{
					Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].isOn = false;
					}
					component.isOn = false;
					component.isOn = true;
					this.SetupListeners();
					return;
				}
			}
		}
		Toggle toggle = this.group.ActiveToggles().FirstOrDefault((Toggle x) => x.isOn);
		if (toggle)
		{
			toggle.isOn = false;
			toggle.isOn = true;
		}
		this.SetupListeners();
	}

	// Token: 0x06003740 RID: 14144 RVA: 0x0014CDF4 File Offset: 0x0014AFF4
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleChanged));
		}
	}

	// Token: 0x06003741 RID: 14145 RVA: 0x0014CE38 File Offset: 0x0014B038
	private void SetupListeners()
	{
		Toggle[] componentsInChildren = base.GetComponentsInChildren<Toggle>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
		}
	}

	// Token: 0x06003742 RID: 14146 RVA: 0x0014CE74 File Offset: 0x0014B074
	private void OnToggleChanged(bool b)
	{
		Toggle toggle = base.GetComponentsInChildren<Toggle>().FirstOrDefault((Toggle x) => x.isOn);
		if (toggle)
		{
			PlayerPrefs.SetString("ToggleGroupCookie_" + base.name, toggle.gameObject.name);
		}
	}
}
