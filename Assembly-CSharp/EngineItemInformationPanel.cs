using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200081D RID: 2077
public class EngineItemInformationPanel : ItemInformationPanel
{
	// Token: 0x04002E96 RID: 11926
	[SerializeField]
	private Text tier;

	// Token: 0x04002E97 RID: 11927
	[SerializeField]
	private Translate.Phrase low;

	// Token: 0x04002E98 RID: 11928
	[SerializeField]
	private Translate.Phrase medium;

	// Token: 0x04002E99 RID: 11929
	[SerializeField]
	private Translate.Phrase high;

	// Token: 0x04002E9A RID: 11930
	[SerializeField]
	private GameObject accelerationRoot;

	// Token: 0x04002E9B RID: 11931
	[SerializeField]
	private GameObject topSpeedRoot;

	// Token: 0x04002E9C RID: 11932
	[SerializeField]
	private GameObject fuelEconomyRoot;
}
