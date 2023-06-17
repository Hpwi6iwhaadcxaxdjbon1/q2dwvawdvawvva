using System;
using UnityEngine;

// Token: 0x02000138 RID: 312
public class InstrumentViewmodel : MonoBehaviour
{
	// Token: 0x04000F13 RID: 3859
	public Animator ViewAnimator;

	// Token: 0x04000F14 RID: 3860
	public bool UpdateA = true;

	// Token: 0x04000F15 RID: 3861
	public bool UpdateB = true;

	// Token: 0x04000F16 RID: 3862
	public bool UpdateC = true;

	// Token: 0x04000F17 RID: 3863
	public bool UpdateD = true;

	// Token: 0x04000F18 RID: 3864
	public bool UpdateE = true;

	// Token: 0x04000F19 RID: 3865
	public bool UpdateF = true;

	// Token: 0x04000F1A RID: 3866
	public bool UpdateG = true;

	// Token: 0x04000F1B RID: 3867
	public bool UpdateRecentlyPlayed = true;

	// Token: 0x04000F1C RID: 3868
	public bool UpdatePlayedNoteTrigger;

	// Token: 0x04000F1D RID: 3869
	public bool UseTriggers;

	// Token: 0x04000F1E RID: 3870
	private readonly int note_a = Animator.StringToHash("play_A");

	// Token: 0x04000F1F RID: 3871
	private readonly int note_b = Animator.StringToHash("play_B");

	// Token: 0x04000F20 RID: 3872
	private readonly int note_c = Animator.StringToHash("play_C");

	// Token: 0x04000F21 RID: 3873
	private readonly int note_d = Animator.StringToHash("play_D");

	// Token: 0x04000F22 RID: 3874
	private readonly int note_e = Animator.StringToHash("play_E");

	// Token: 0x04000F23 RID: 3875
	private readonly int note_f = Animator.StringToHash("play_F");

	// Token: 0x04000F24 RID: 3876
	private readonly int note_g = Animator.StringToHash("play_G");

	// Token: 0x04000F25 RID: 3877
	private readonly int recentlyPlayedHash = Animator.StringToHash("recentlyPlayed");

	// Token: 0x04000F26 RID: 3878
	private readonly int playedNoteHash = Animator.StringToHash("playedNote");

	// Token: 0x060016D2 RID: 5842 RVA: 0x000AF1A8 File Offset: 0x000AD3A8
	public void UpdateSlots(InstrumentKeyController.AnimationSlot currentSlot, bool recentlyPlayed, bool playedNoteThisFrame)
	{
		if (this.ViewAnimator == null)
		{
			return;
		}
		if (this.UpdateA)
		{
			this.UpdateState(this.note_a, currentSlot == InstrumentKeyController.AnimationSlot.One);
		}
		if (this.UpdateB)
		{
			this.UpdateState(this.note_b, currentSlot == InstrumentKeyController.AnimationSlot.Two);
		}
		if (this.UpdateC)
		{
			this.UpdateState(this.note_c, currentSlot == InstrumentKeyController.AnimationSlot.Three);
		}
		if (this.UpdateD)
		{
			this.UpdateState(this.note_d, currentSlot == InstrumentKeyController.AnimationSlot.Four);
		}
		if (this.UpdateE)
		{
			this.UpdateState(this.note_e, currentSlot == InstrumentKeyController.AnimationSlot.Five);
		}
		if (this.UpdateF)
		{
			this.UpdateState(this.note_f, currentSlot == InstrumentKeyController.AnimationSlot.Six);
		}
		if (this.UpdateG)
		{
			this.UpdateState(this.note_g, currentSlot == InstrumentKeyController.AnimationSlot.Seven);
		}
		if (this.UpdateRecentlyPlayed)
		{
			this.ViewAnimator.SetBool(this.recentlyPlayedHash, recentlyPlayed);
		}
		if (this.UpdatePlayedNoteTrigger && playedNoteThisFrame)
		{
			this.ViewAnimator.SetTrigger(this.playedNoteHash);
		}
	}

	// Token: 0x060016D3 RID: 5843 RVA: 0x000AF2A1 File Offset: 0x000AD4A1
	private void UpdateState(int param, bool state)
	{
		if (!this.UseTriggers)
		{
			this.ViewAnimator.SetBool(param, state);
			return;
		}
		if (state)
		{
			this.ViewAnimator.SetTrigger(param);
		}
	}
}
