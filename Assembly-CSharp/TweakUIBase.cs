using System;
using UnityEngine;

// Token: 0x02000886 RID: 2182
public class TweakUIBase : MonoBehaviour
{
	// Token: 0x040030FF RID: 12543
	public string convarName = "effects.motionblur";

	// Token: 0x04003100 RID: 12544
	public bool ApplyImmediatelyOnChange = true;

	// Token: 0x04003101 RID: 12545
	internal ConsoleSystem.Command conVar;

	// Token: 0x060036A2 RID: 13986 RVA: 0x0014A536 File Offset: 0x00148736
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x060036A3 RID: 13987 RVA: 0x0014A540 File Offset: 0x00148740
	protected virtual void Init()
	{
		this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
		if (this.conVar == null)
		{
			Debug.LogWarning("TweakUI Convar Missing: " + this.convarName, base.gameObject);
			return;
		}
		this.conVar.OnValueChanged += this.OnConVarChanged;
	}

	// Token: 0x060036A4 RID: 13988 RVA: 0x0014A59A File Offset: 0x0014879A
	public virtual void OnApplyClicked()
	{
		if (this.ApplyImmediatelyOnChange)
		{
			return;
		}
		this.SetConvarValue();
	}

	// Token: 0x060036A5 RID: 13989 RVA: 0x0014A5AB File Offset: 0x001487AB
	public virtual void UnapplyChanges()
	{
		if (this.ApplyImmediatelyOnChange)
		{
			return;
		}
		this.ResetToConvar();
	}

	// Token: 0x060036A6 RID: 13990 RVA: 0x0014A5BC File Offset: 0x001487BC
	protected virtual void OnConVarChanged(ConsoleSystem.Command obj)
	{
		this.ResetToConvar();
	}

	// Token: 0x060036A7 RID: 13991 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ResetToConvar()
	{
	}

	// Token: 0x060036A8 RID: 13992 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void SetConvarValue()
	{
	}

	// Token: 0x060036A9 RID: 13993 RVA: 0x0014A5C4 File Offset: 0x001487C4
	private void OnDestroy()
	{
		if (this.conVar != null)
		{
			this.conVar.OnValueChanged -= this.OnConVarChanged;
		}
	}
}
