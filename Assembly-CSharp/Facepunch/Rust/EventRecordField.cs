using System;
using UnityEngine;

namespace Facepunch.Rust
{
	// Token: 0x02000B01 RID: 2817
	public struct EventRecordField
	{
		// Token: 0x04003CE0 RID: 15584
		public string Key1;

		// Token: 0x04003CE1 RID: 15585
		public string Key2;

		// Token: 0x04003CE2 RID: 15586
		public string String;

		// Token: 0x04003CE3 RID: 15587
		public long? Number;

		// Token: 0x04003CE4 RID: 15588
		public double? Float;

		// Token: 0x04003CE5 RID: 15589
		public Vector3? Vector;

		// Token: 0x04003CE6 RID: 15590
		public Guid? Guid;

		// Token: 0x04003CE7 RID: 15591
		public bool IsObject;

		// Token: 0x060044D7 RID: 17623 RVA: 0x00193CE8 File Offset: 0x00191EE8
		public EventRecordField(string key1)
		{
			this.Key1 = key1;
			this.Key2 = null;
			this.String = null;
			this.Number = null;
			this.Float = null;
			this.Vector = null;
			this.Guid = null;
			this.IsObject = false;
		}

		// Token: 0x060044D8 RID: 17624 RVA: 0x00193D44 File Offset: 0x00191F44
		public EventRecordField(string key1, string key2)
		{
			this.Key1 = key1;
			this.Key2 = key2;
			this.String = null;
			this.Number = null;
			this.Float = null;
			this.Vector = null;
			this.Guid = null;
			this.IsObject = false;
		}
	}
}
