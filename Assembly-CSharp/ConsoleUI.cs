using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200079C RID: 1948
public class ConsoleUI : SingletonComponent<ConsoleUI>
{
	// Token: 0x04002B42 RID: 11074
	public RustText text;

	// Token: 0x04002B43 RID: 11075
	public InputField outputField;

	// Token: 0x04002B44 RID: 11076
	public InputField inputField;

	// Token: 0x04002B45 RID: 11077
	public GameObject AutocompleteDropDown;

	// Token: 0x04002B46 RID: 11078
	public GameObject ItemTemplate;

	// Token: 0x04002B47 RID: 11079
	public Color errorColor;

	// Token: 0x04002B48 RID: 11080
	public Color warningColor;

	// Token: 0x04002B49 RID: 11081
	public Color inputColor;
}
