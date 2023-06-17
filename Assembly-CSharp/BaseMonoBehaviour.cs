using System;
using ConVar;
using UnityEngine;

// Token: 0x020008EB RID: 2283
public abstract class BaseMonoBehaviour : FacepunchBehaviour
{
	// Token: 0x060037AA RID: 14250 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool IsDebugging()
	{
		return false;
	}

	// Token: 0x060037AB RID: 14251 RVA: 0x0014DD57 File Offset: 0x0014BF57
	public virtual string GetLogColor()
	{
		return "yellow";
	}

	// Token: 0x060037AC RID: 14252 RVA: 0x0014DD60 File Offset: 0x0014BF60
	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str, object arg1)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		string text = string.Format(str, arg1);
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[]
		{
			log.ToString().PadRight(10),
			this.ToString(),
			text,
			this.GetLogColor()
		}), base.gameObject);
	}

	// Token: 0x060037AD RID: 14253 RVA: 0x0014DDD0 File Offset: 0x0014BFD0
	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str, object arg1, object arg2)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		string text = string.Format(str, arg1, arg2);
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[]
		{
			log.ToString().PadRight(10),
			this.ToString(),
			text,
			this.GetLogColor()
		}), base.gameObject);
	}

	// Token: 0x060037AE RID: 14254 RVA: 0x0014DE44 File Offset: 0x0014C044
	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[]
		{
			log.ToString().PadRight(10),
			this.ToString(),
			str,
			this.GetLogColor()
		}), base.gameObject);
	}

	// Token: 0x02000EAD RID: 3757
	public enum LogEntryType
	{
		// Token: 0x04004C9E RID: 19614
		General,
		// Token: 0x04004C9F RID: 19615
		Network,
		// Token: 0x04004CA0 RID: 19616
		Hierarchy,
		// Token: 0x04004CA1 RID: 19617
		Serialization
	}
}
