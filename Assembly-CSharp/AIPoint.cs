using System;

// Token: 0x020001E0 RID: 480
public class AIPoint : BaseMonoBehaviour
{
	// Token: 0x0400124F RID: 4687
	private BaseEntity currentUser;

	// Token: 0x06001982 RID: 6530 RVA: 0x000BB4B8 File Offset: 0x000B96B8
	public bool InUse()
	{
		return this.currentUser != null;
	}

	// Token: 0x06001983 RID: 6531 RVA: 0x000BB4C6 File Offset: 0x000B96C6
	public bool IsUsedBy(BaseEntity user)
	{
		return this.InUse() && !(user == null) && user == this.currentUser;
	}

	// Token: 0x06001984 RID: 6532 RVA: 0x000BB4E9 File Offset: 0x000B96E9
	public bool CanBeUsedBy(BaseEntity user)
	{
		return (user != null && this.currentUser == user) || !this.InUse();
	}

	// Token: 0x06001985 RID: 6533 RVA: 0x000BB50D File Offset: 0x000B970D
	public void SetUsedBy(BaseEntity user, float duration = 5f)
	{
		this.currentUser = user;
		base.CancelInvoke(new Action(this.ClearUsed));
		base.Invoke(new Action(this.ClearUsed), duration);
	}

	// Token: 0x06001986 RID: 6534 RVA: 0x000BB53B File Offset: 0x000B973B
	public void SetUsedBy(BaseEntity user)
	{
		this.currentUser = user;
	}

	// Token: 0x06001987 RID: 6535 RVA: 0x000BB544 File Offset: 0x000B9744
	public void ClearUsed()
	{
		this.currentUser = null;
	}

	// Token: 0x06001988 RID: 6536 RVA: 0x000BB54D File Offset: 0x000B974D
	public void ClearIfUsedBy(BaseEntity user)
	{
		if (this.currentUser == user)
		{
			this.ClearUsed();
		}
	}
}
