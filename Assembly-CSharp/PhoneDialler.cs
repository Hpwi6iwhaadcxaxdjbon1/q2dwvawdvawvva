using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007CA RID: 1994
public class PhoneDialler : UIDialog
{
	// Token: 0x04002C81 RID: 11393
	public GameObject DialingRoot;

	// Token: 0x04002C82 RID: 11394
	public GameObject CallInProcessRoot;

	// Token: 0x04002C83 RID: 11395
	public GameObject IncomingCallRoot;

	// Token: 0x04002C84 RID: 11396
	public RustText ThisPhoneNumber;

	// Token: 0x04002C85 RID: 11397
	public RustInput PhoneNameInput;

	// Token: 0x04002C86 RID: 11398
	public RustText textDisplay;

	// Token: 0x04002C87 RID: 11399
	public RustText CallTimeText;

	// Token: 0x04002C88 RID: 11400
	public RustButton DefaultDialViewButton;

	// Token: 0x04002C89 RID: 11401
	public RustText[] IncomingCallNumber;

	// Token: 0x04002C8A RID: 11402
	public GameObject NumberDialRoot;

	// Token: 0x04002C8B RID: 11403
	public GameObject PromptVoicemailRoot;

	// Token: 0x04002C8C RID: 11404
	public RustButton ContactsButton;

	// Token: 0x04002C8D RID: 11405
	public RustText FailText;

	// Token: 0x04002C8E RID: 11406
	public NeedsCursor CursorController;

	// Token: 0x04002C8F RID: 11407
	public NeedsKeyboard KeyboardController;

	// Token: 0x04002C90 RID: 11408
	public Translate.Phrase WrongNumberPhrase;

	// Token: 0x04002C91 RID: 11409
	public Translate.Phrase NetworkBusy;

	// Token: 0x04002C92 RID: 11410
	public Translate.Phrase Engaged;

	// Token: 0x04002C93 RID: 11411
	public GameObjectRef DirectoryEntryPrefab;

	// Token: 0x04002C94 RID: 11412
	public Transform DirectoryRoot;

	// Token: 0x04002C95 RID: 11413
	public GameObject NoDirectoryRoot;

	// Token: 0x04002C96 RID: 11414
	public RustButton DirectoryPageUp;

	// Token: 0x04002C97 RID: 11415
	public RustButton DirectoryPageDown;

	// Token: 0x04002C98 RID: 11416
	public Transform ContactsRoot;

	// Token: 0x04002C99 RID: 11417
	public RustInput ContactsNameInput;

	// Token: 0x04002C9A RID: 11418
	public RustInput ContactsNumberInput;

	// Token: 0x04002C9B RID: 11419
	public GameObject NoContactsRoot;

	// Token: 0x04002C9C RID: 11420
	public RustButton AddContactButton;

	// Token: 0x04002C9D RID: 11421
	public SoundDefinition DialToneSfx;

	// Token: 0x04002C9E RID: 11422
	public Button[] NumberButtons;

	// Token: 0x04002C9F RID: 11423
	public Translate.Phrase AnsweringMachine;

	// Token: 0x04002CA0 RID: 11424
	public VoicemailDialog Voicemail;

	// Token: 0x04002CA1 RID: 11425
	public GameObject VoicemailRoot;
}
