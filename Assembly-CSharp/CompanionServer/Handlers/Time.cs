﻿using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer.Handlers
{
	// Token: 0x02000A05 RID: 2565
	public class Time : BaseHandler<AppEmpty>
	{
		// Token: 0x06003D33 RID: 15667 RVA: 0x00167370 File Offset: 0x00165570
		public override void Execute()
		{
			TOD_Sky instance = TOD_Sky.Instance;
			TOD_Time time = instance.Components.Time;
			AppTime appTime = Pool.Get<AppTime>();
			appTime.dayLengthMinutes = time.DayLengthInMinutes;
			appTime.timeScale = (time.ProgressTime ? Time.timeScale : 0f);
			appTime.sunrise = instance.SunriseTime;
			appTime.sunset = instance.SunsetTime;
			appTime.time = instance.Cycle.Hour;
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.time = appTime;
			base.Send(appResponse);
		}
	}
}
