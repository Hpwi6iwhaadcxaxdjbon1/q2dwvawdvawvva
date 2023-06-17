using System;

namespace Rust.Ai
{
	// Token: 0x02000B41 RID: 2881
	public static class Sense
	{
		// Token: 0x04003E47 RID: 15943
		private static BaseEntity[] query = new BaseEntity[512];

		// Token: 0x060045D9 RID: 17881 RVA: 0x00197D80 File Offset: 0x00195F80
		public static void Stimulate(Sensation sensation)
		{
			int inSphere = BaseEntity.Query.Server.GetInSphere(sensation.Position, sensation.Radius, Sense.query, new Func<BaseEntity, bool>(Sense.IsAbleToBeStimulated));
			float num = sensation.Radius * sensation.Radius;
			for (int i = 0; i < inSphere; i++)
			{
				if ((Sense.query[i].transform.position - sensation.Position).sqrMagnitude <= num)
				{
					Sense.query[i].OnSensation(sensation);
				}
			}
		}

		// Token: 0x060045DA RID: 17882 RVA: 0x00197E03 File Offset: 0x00196003
		private static bool IsAbleToBeStimulated(BaseEntity ent)
		{
			return ent is BasePlayer || ent is BaseNpc;
		}
	}
}
