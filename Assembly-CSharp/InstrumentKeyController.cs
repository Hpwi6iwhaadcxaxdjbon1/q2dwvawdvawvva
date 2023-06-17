using System;
using Rust.Instruments;
using UnityEngine;

// Token: 0x020003FC RID: 1020
public class InstrumentKeyController : MonoBehaviour
{
	// Token: 0x04001ABB RID: 6843
	public const float DEFAULT_NOTE_VELOCITY = 1f;

	// Token: 0x04001ABC RID: 6844
	public NoteBindingCollection Bindings;

	// Token: 0x04001ABD RID: 6845
	public InstrumentKeyController.NoteBinding[] NoteBindings = new InstrumentKeyController.NoteBinding[0];

	// Token: 0x04001ABE RID: 6846
	public Transform[] NoteSoundPositions;

	// Token: 0x04001ABF RID: 6847
	public InstrumentIKController IKController;

	// Token: 0x04001AC0 RID: 6848
	public Transform LeftHandProp;

	// Token: 0x04001AC1 RID: 6849
	public Transform RightHandProp;

	// Token: 0x04001AC2 RID: 6850
	public Animator InstrumentAnimator;

	// Token: 0x04001AC3 RID: 6851
	public BaseEntity RPCHandler;

	// Token: 0x04001AC4 RID: 6852
	public uint overrideAchievementId;

	// Token: 0x04001AC6 RID: 6854
	private const string ALL_NOTES_STATNAME = "played_notes";

	// Token: 0x170002ED RID: 749
	// (get) Token: 0x060022CF RID: 8911 RVA: 0x000DF454 File Offset: 0x000DD654
	// (set) Token: 0x060022D0 RID: 8912 RVA: 0x000DF45C File Offset: 0x000DD65C
	public bool PlayedNoteThisFrame { get; private set; }

	// Token: 0x060022D1 RID: 8913 RVA: 0x000DF465 File Offset: 0x000DD665
	public void ProcessServerPlayedNote(BasePlayer forPlayer)
	{
		if (forPlayer == null)
		{
			return;
		}
		forPlayer.stats.Add(this.Bindings.NotePlayedStatName, 1, (Stats)5);
		forPlayer.stats.Add("played_notes", 1, (Stats)5);
	}

	// Token: 0x02000CC9 RID: 3273
	public struct NoteBinding
	{
	}

	// Token: 0x02000CCA RID: 3274
	public enum IKType
	{
		// Token: 0x040044D2 RID: 17618
		LeftHand,
		// Token: 0x040044D3 RID: 17619
		RightHand,
		// Token: 0x040044D4 RID: 17620
		RightFoot
	}

	// Token: 0x02000CCB RID: 3275
	public enum NoteType
	{
		// Token: 0x040044D6 RID: 17622
		Regular,
		// Token: 0x040044D7 RID: 17623
		Sharp
	}

	// Token: 0x02000CCC RID: 3276
	public enum InstrumentType
	{
		// Token: 0x040044D9 RID: 17625
		Note,
		// Token: 0x040044DA RID: 17626
		Hold
	}

	// Token: 0x02000CCD RID: 3277
	public enum AnimationSlot
	{
		// Token: 0x040044DC RID: 17628
		None,
		// Token: 0x040044DD RID: 17629
		One,
		// Token: 0x040044DE RID: 17630
		Two,
		// Token: 0x040044DF RID: 17631
		Three,
		// Token: 0x040044E0 RID: 17632
		Four,
		// Token: 0x040044E1 RID: 17633
		Five,
		// Token: 0x040044E2 RID: 17634
		Six,
		// Token: 0x040044E3 RID: 17635
		Seven
	}

	// Token: 0x02000CCE RID: 3278
	[Serializable]
	public struct KeySet
	{
		// Token: 0x040044E4 RID: 17636
		public Notes Note;

		// Token: 0x040044E5 RID: 17637
		public InstrumentKeyController.NoteType NoteType;

		// Token: 0x040044E6 RID: 17638
		public int OctaveShift;

		// Token: 0x06004FBA RID: 20410 RVA: 0x001A6F1F File Offset: 0x001A511F
		public override string ToString()
		{
			return string.Format("{0}{1}{2}", this.Note, (this.NoteType == InstrumentKeyController.NoteType.Sharp) ? "#" : string.Empty, this.OctaveShift);
		}
	}

	// Token: 0x02000CCF RID: 3279
	public struct NoteOverride
	{
		// Token: 0x040044E7 RID: 17639
		public bool Override;

		// Token: 0x040044E8 RID: 17640
		public InstrumentKeyController.KeySet Note;
	}

	// Token: 0x02000CD0 RID: 3280
	[Serializable]
	public struct IKNoteTarget
	{
		// Token: 0x040044E9 RID: 17641
		public InstrumentKeyController.IKType TargetType;

		// Token: 0x040044EA RID: 17642
		public int IkIndex;
	}
}
