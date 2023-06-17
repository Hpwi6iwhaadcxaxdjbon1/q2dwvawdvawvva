using System;
using Rust.Instruments;
using UnityEngine;

// Token: 0x020003F9 RID: 1017
public class InstrumentDebugInput : MonoBehaviour
{
	// Token: 0x04001AA6 RID: 6822
	public InstrumentKeyController KeyController;

	// Token: 0x04001AA7 RID: 6823
	public InstrumentKeyController.KeySet Note = new InstrumentKeyController.KeySet
	{
		Note = Notes.A,
		NoteType = InstrumentKeyController.NoteType.Regular,
		OctaveShift = 3
	};

	// Token: 0x04001AA8 RID: 6824
	public float Frequency = 0.75f;

	// Token: 0x04001AA9 RID: 6825
	public float StopAfter = 0.1f;

	// Token: 0x04001AAA RID: 6826
	public SoundDefinition OverrideDefinition;
}
