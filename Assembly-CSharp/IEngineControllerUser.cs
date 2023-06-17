using System;
using Rust;

// Token: 0x020004BB RID: 1211
public interface IEngineControllerUser : IEntity
{
	// Token: 0x06002789 RID: 10121
	bool HasFlag(BaseEntity.Flags f);

	// Token: 0x0600278A RID: 10122
	bool IsDead();

	// Token: 0x0600278B RID: 10123
	void SetFlag(BaseEntity.Flags f, bool b, bool recursive = false, bool networkupdate = true);

	// Token: 0x0600278C RID: 10124
	void Invoke(Action action, float time);

	// Token: 0x0600278D RID: 10125
	void CancelInvoke(Action action);

	// Token: 0x0600278E RID: 10126
	void OnEngineStartFailed();

	// Token: 0x0600278F RID: 10127
	bool MeetsEngineRequirements();
}
