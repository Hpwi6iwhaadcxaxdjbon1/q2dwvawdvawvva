using System;
using Rust.UI;
using UnityEngine.UI;

// Token: 0x020007CE RID: 1998
public class PickAFriend : UIDialog
{
	// Token: 0x04002CB2 RID: 11442
	public InputField input;

	// Token: 0x04002CB3 RID: 11443
	public RustText headerText;

	// Token: 0x04002CB4 RID: 11444
	public bool AutoSelectInputField;

	// Token: 0x04002CB5 RID: 11445
	public bool AllowMultiple;

	// Token: 0x04002CB6 RID: 11446
	public Action<ulong, string> onSelected;

	// Token: 0x04002CB7 RID: 11447
	public Translate.Phrase sleepingBagHeaderPhrase = new Translate.Phrase("assign_to_friend", "Assign To a Friend");

	// Token: 0x04002CB8 RID: 11448
	public Translate.Phrase turretHeaderPhrase = new Translate.Phrase("authorize_a_friend", "Authorize a Friend");

	// Token: 0x04002CB9 RID: 11449
	public SteamFriendsList friendsList;

	// Token: 0x17000453 RID: 1107
	// (set) Token: 0x06003547 RID: 13639 RVA: 0x001466BB File Offset: 0x001448BB
	public Func<ulong, bool> shouldShowPlayer
	{
		set
		{
			if (this.friendsList != null)
			{
				this.friendsList.shouldShowPlayer = value;
			}
		}
	}
}
