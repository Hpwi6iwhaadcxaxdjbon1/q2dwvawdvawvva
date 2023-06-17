using System;

namespace ConVar
{
	// Token: 0x02000AB8 RID: 2744
	[ConsoleSystem.Factory("env")]
	public class Env : ConsoleSystem
	{
		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x060041C1 RID: 16833 RVA: 0x0018671D File Offset: 0x0018491D
		// (set) Token: 0x060041C0 RID: 16832 RVA: 0x001866F8 File Offset: 0x001848F8
		[ServerVar]
		public static bool progresstime
		{
			get
			{
				return !(TOD_Sky.Instance == null) && TOD_Sky.Instance.Components.Time.ProgressTime;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Components.Time.ProgressTime = value;
			}
		}

		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x060041C3 RID: 16835 RVA: 0x00186762 File Offset: 0x00184962
		// (set) Token: 0x060041C2 RID: 16834 RVA: 0x00186742 File Offset: 0x00184942
		[ServerVar(ShowInAdminUI = true)]
		public static float time
		{
			get
			{
				if (TOD_Sky.Instance == null)
				{
					return 0f;
				}
				return TOD_Sky.Instance.Cycle.Hour;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Cycle.Hour = value;
			}
		}

		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x060041C5 RID: 16837 RVA: 0x001867A6 File Offset: 0x001849A6
		// (set) Token: 0x060041C4 RID: 16836 RVA: 0x00186786 File Offset: 0x00184986
		[ServerVar]
		public static int day
		{
			get
			{
				if (TOD_Sky.Instance == null)
				{
					return 0;
				}
				return TOD_Sky.Instance.Cycle.Day;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Cycle.Day = value;
			}
		}

		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x060041C7 RID: 16839 RVA: 0x001867E6 File Offset: 0x001849E6
		// (set) Token: 0x060041C6 RID: 16838 RVA: 0x001867C6 File Offset: 0x001849C6
		[ServerVar]
		public static int month
		{
			get
			{
				if (TOD_Sky.Instance == null)
				{
					return 0;
				}
				return TOD_Sky.Instance.Cycle.Month;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Cycle.Month = value;
			}
		}

		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x060041C9 RID: 16841 RVA: 0x00186826 File Offset: 0x00184A26
		// (set) Token: 0x060041C8 RID: 16840 RVA: 0x00186806 File Offset: 0x00184A06
		[ServerVar]
		public static int year
		{
			get
			{
				if (TOD_Sky.Instance == null)
				{
					return 0;
				}
				return TOD_Sky.Instance.Cycle.Year;
			}
			set
			{
				if (TOD_Sky.Instance == null)
				{
					return;
				}
				TOD_Sky.Instance.Cycle.Year = value;
			}
		}

		// Token: 0x060041CA RID: 16842 RVA: 0x00186848 File Offset: 0x00184A48
		[ServerVar]
		public static void addtime(ConsoleSystem.Arg arg)
		{
			if (TOD_Sky.Instance == null)
			{
				return;
			}
			DateTime dateTime = TOD_Sky.Instance.Cycle.DateTime.AddTicks(arg.GetTicks(0, 0L));
			TOD_Sky.Instance.Cycle.DateTime = dateTime;
		}

		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x060041CB RID: 16843 RVA: 0x00186894 File Offset: 0x00184A94
		// (set) Token: 0x060041CC RID: 16844 RVA: 0x0018689B File Offset: 0x00184A9B
		[ReplicatedVar(Default = "0")]
		public static float oceanlevel
		{
			get
			{
				return WaterSystem.OceanLevel;
			}
			set
			{
				WaterSystem.OceanLevel = value;
			}
		}
	}
}
