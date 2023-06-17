using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008BF RID: 2239
public class ToolsHUDUI : MonoBehaviour
{
	// Token: 0x04003240 RID: 12864
	[SerializeField]
	private GameObject prefab;

	// Token: 0x04003241 RID: 12865
	[SerializeField]
	private Transform parent;

	// Token: 0x04003242 RID: 12866
	private bool initialised;

	// Token: 0x06003744 RID: 14148 RVA: 0x0014CED4 File Offset: 0x0014B0D4
	protected void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06003745 RID: 14149 RVA: 0x0014CEDC File Offset: 0x0014B0DC
	private void Init()
	{
		if (this.initialised)
		{
			return;
		}
		UIHUD instance = SingletonComponent<UIHUD>.Instance;
		if (instance == null)
		{
			return;
		}
		this.initialised = true;
		foreach (Transform transform in instance.GetComponentsInChildren<Transform>())
		{
			string name = transform.name;
			if (name.ToLower().StartsWith("gameui.hud."))
			{
				if (name.ToLower() == "gameui.hud.crosshair")
				{
					foreach (object obj in transform)
					{
						Transform transform2 = (Transform)obj;
						this.AddToggleObj(transform2.name, "<color=yellow>Crosshair sub:</color> " + transform2.name);
					}
				}
				this.AddToggleObj(name, name.Substring(11));
			}
		}
	}

	// Token: 0x06003746 RID: 14150 RVA: 0x0014CFD0 File Offset: 0x0014B1D0
	private void AddToggleObj(string trName, string labelText)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prefab, Vector3.zero, Quaternion.identity, this.parent);
		gameObject.name = trName;
		ToggleHUDLayer component = gameObject.GetComponent<ToggleHUDLayer>();
		component.hudComponentName = trName;
		component.textControl.text = labelText;
	}

	// Token: 0x06003747 RID: 14151 RVA: 0x0014D00C File Offset: 0x0014B20C
	public void SelectAll()
	{
		Toggle[] componentsInChildren = this.parent.GetComponentsInChildren<Toggle>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].isOn = true;
		}
	}

	// Token: 0x06003748 RID: 14152 RVA: 0x0014D03C File Offset: 0x0014B23C
	public void SelectNone()
	{
		Toggle[] componentsInChildren = this.parent.GetComponentsInChildren<Toggle>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].isOn = false;
		}
	}
}
