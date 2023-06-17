using System;
using Facepunch.Extend;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000880 RID: 2176
public class ToggleHUDLayer : MonoBehaviour, IClientComponent
{
	// Token: 0x040030F4 RID: 12532
	public Toggle toggleControl;

	// Token: 0x040030F5 RID: 12533
	public TextMeshProUGUI textControl;

	// Token: 0x040030F6 RID: 12534
	public string hudComponentName;

	// Token: 0x0600368D RID: 13965 RVA: 0x0014A290 File Offset: 0x00148490
	protected void OnEnable()
	{
		UIHUD instance = SingletonComponent<UIHUD>.Instance;
		if (instance != null)
		{
			Transform transform = instance.transform.FindChildRecursive(this.hudComponentName);
			if (transform != null)
			{
				Canvas component = transform.GetComponent<Canvas>();
				if (component != null)
				{
					this.toggleControl.isOn = component.enabled;
					return;
				}
				this.toggleControl.isOn = transform.gameObject.activeSelf;
				return;
			}
			else
			{
				Debug.LogWarning(base.GetType().Name + ": Couldn't find child: " + this.hudComponentName);
			}
		}
	}

	// Token: 0x0600368E RID: 13966 RVA: 0x0014A320 File Offset: 0x00148520
	public void OnToggleChanged()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "global.hudcomponent", new object[]
		{
			this.hudComponentName,
			this.toggleControl.isOn
		});
	}
}
