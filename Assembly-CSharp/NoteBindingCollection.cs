using System;
using Rust.Instruments;
using UnityEngine;

// Token: 0x020003FD RID: 1021
[CreateAssetMenu]
public class NoteBindingCollection : ScriptableObject
{
	// Token: 0x04001AC7 RID: 6855
	public NoteBindingCollection.NoteData[] BaseBindings;

	// Token: 0x04001AC8 RID: 6856
	public float MinimumNoteTime;

	// Token: 0x04001AC9 RID: 6857
	public float MaximumNoteLength;

	// Token: 0x04001ACA RID: 6858
	public bool AllowAutoplay = true;

	// Token: 0x04001ACB RID: 6859
	public float AutoplayLoopDelay = 0.25f;

	// Token: 0x04001ACC RID: 6860
	public string NotePlayedStatName;

	// Token: 0x04001ACD RID: 6861
	public string KeyMidiMapShortname = "";

	// Token: 0x04001ACE RID: 6862
	public bool AllowSustain;

	// Token: 0x04001ACF RID: 6863
	public bool AllowFullKeyboardInput = true;

	// Token: 0x04001AD0 RID: 6864
	public string InstrumentShortName = "";

	// Token: 0x04001AD1 RID: 6865
	public InstrumentKeyController.InstrumentType NotePlayType;

	// Token: 0x04001AD2 RID: 6866
	public int MaxConcurrentNotes = 3;

	// Token: 0x04001AD3 RID: 6867
	public bool LoopSounds;

	// Token: 0x04001AD4 RID: 6868
	public float SoundFadeInTime;

	// Token: 0x04001AD5 RID: 6869
	public float minimumSoundFadeOutTime = 0.1f;

	// Token: 0x04001AD6 RID: 6870
	public InstrumentKeyController.KeySet PrimaryClickNote;

	// Token: 0x04001AD7 RID: 6871
	public InstrumentKeyController.KeySet SecondaryClickNote = new InstrumentKeyController.KeySet
	{
		Note = Notes.B
	};

	// Token: 0x04001AD8 RID: 6872
	public bool RunInstrumentAnimationController;

	// Token: 0x04001AD9 RID: 6873
	public bool PlayRepeatAnimations = true;

	// Token: 0x04001ADA RID: 6874
	public float AnimationDeadTime = 1f;

	// Token: 0x04001ADB RID: 6875
	public float AnimationResetDelay;

	// Token: 0x04001ADC RID: 6876
	public float RecentlyPlayedThreshold = 1f;

	// Token: 0x04001ADD RID: 6877
	[Range(0f, 1f)]
	public float CrossfadeNormalizedAnimationTarget;

	// Token: 0x04001ADE RID: 6878
	public float AnimationCrossfadeDuration = 0.15f;

	// Token: 0x04001ADF RID: 6879
	public float CrossfadePlayerSpeedMulti = 1f;

	// Token: 0x04001AE0 RID: 6880
	public int DefaultOctave;

	// Token: 0x04001AE1 RID: 6881
	public int ShiftedOctave = 1;

	// Token: 0x04001AE2 RID: 6882
	public bool UseClosestMidiNote = true;

	// Token: 0x04001AE3 RID: 6883
	private const float MidiNoteUpOctaveShift = 2f;

	// Token: 0x04001AE4 RID: 6884
	private const float MidiNoteDownOctaveShift = 0.1f;

	// Token: 0x060022D3 RID: 8915 RVA: 0x000DF4B0 File Offset: 0x000DD6B0
	public bool FindNoteData(Notes note, int octave, InstrumentKeyController.NoteType type, out NoteBindingCollection.NoteData data, out int noteIndex)
	{
		for (int i = 0; i < this.BaseBindings.Length; i++)
		{
			NoteBindingCollection.NoteData noteData = this.BaseBindings[i];
			if (noteData.Note == note && noteData.Type == type && noteData.NoteOctave == octave)
			{
				data = noteData;
				noteIndex = i;
				return true;
			}
		}
		data = default(NoteBindingCollection.NoteData);
		noteIndex = -1;
		return false;
	}

	// Token: 0x060022D4 RID: 8916 RVA: 0x000DF514 File Offset: 0x000DD714
	public bool FindNoteDataIndex(Notes note, int octave, InstrumentKeyController.NoteType type, out int noteIndex)
	{
		for (int i = 0; i < this.BaseBindings.Length; i++)
		{
			NoteBindingCollection.NoteData noteData = this.BaseBindings[i];
			if (noteData.Note == note && noteData.Type == type && noteData.NoteOctave == octave)
			{
				noteIndex = i;
				return true;
			}
		}
		noteIndex = -1;
		return false;
	}

	// Token: 0x060022D5 RID: 8917 RVA: 0x000DF568 File Offset: 0x000DD768
	public NoteBindingCollection.NoteData CreateMidiBinding(NoteBindingCollection.NoteData basedOn, int octave, int midiCode)
	{
		NoteBindingCollection.NoteData result = basedOn;
		result.NoteOctave = octave;
		result.MidiNoteNumber = midiCode;
		int num = octave - basedOn.NoteOctave;
		if (octave > basedOn.NoteOctave)
		{
			result.PitchOffset = (float)num * 2f;
		}
		else
		{
			result.PitchOffset = 1f - Mathf.Abs((float)num * 0.1f);
		}
		return result;
	}

	// Token: 0x02000CD1 RID: 3281
	[Serializable]
	public struct NoteData
	{
		// Token: 0x040044EB RID: 17643
		public SoundDefinition NoteSound;

		// Token: 0x040044EC RID: 17644
		public SoundDefinition NoteStartSound;

		// Token: 0x040044ED RID: 17645
		public Notes Note;

		// Token: 0x040044EE RID: 17646
		public InstrumentKeyController.NoteType Type;

		// Token: 0x040044EF RID: 17647
		public int MidiNoteNumber;

		// Token: 0x040044F0 RID: 17648
		public int NoteOctave;

		// Token: 0x040044F1 RID: 17649
		[InstrumentIKTarget]
		public InstrumentKeyController.IKNoteTarget NoteIKTarget;

		// Token: 0x040044F2 RID: 17650
		public InstrumentKeyController.AnimationSlot AnimationSlot;

		// Token: 0x040044F3 RID: 17651
		public int NoteSoundPositionTarget;

		// Token: 0x040044F4 RID: 17652
		public int[] AdditionalMidiTargets;

		// Token: 0x040044F5 RID: 17653
		public float PitchOffset;

		// Token: 0x06004FBB RID: 20411 RVA: 0x001A6F58 File Offset: 0x001A5158
		public bool MatchMidiCode(int code)
		{
			if (this.MidiNoteNumber == code)
			{
				return true;
			}
			if (this.AdditionalMidiTargets != null)
			{
				int[] additionalMidiTargets = this.AdditionalMidiTargets;
				for (int i = 0; i < additionalMidiTargets.Length; i++)
				{
					if (additionalMidiTargets[i] == code)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06004FBC RID: 20412 RVA: 0x001A6F96 File Offset: 0x001A5196
		public string ToNoteString()
		{
			return string.Format("{0}{1}{2}", this.Note, (this.Type == InstrumentKeyController.NoteType.Sharp) ? "#" : string.Empty, this.NoteOctave);
		}
	}
}
