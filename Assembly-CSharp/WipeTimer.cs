using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConVar;
using Cronos;
using Facepunch;
using Facepunch.Math;
using Newtonsoft.Json;
using ProtoBuf;
using TimeZoneConverter;
using UnityEngine;

// Token: 0x0200042D RID: 1069
public class WipeTimer : global::BaseEntity
{
	// Token: 0x04001C1E RID: 7198
	[ServerVar(Help = "0=sun,1=mon,2=tues,3=wed,4=thur,5=fri,6=sat")]
	public static int wipeDayOfWeek = 4;

	// Token: 0x04001C1F RID: 7199
	[ServerVar(Help = "Which hour to wipe? 14.5 = 2:30pm")]
	public static float wipeHourOfDay = 19f;

	// Token: 0x04001C20 RID: 7200
	[ServerVar(Help = "The timezone to use for wipes. Defaults to the server's time zone if not set or invalid. Value should be a TZ identifier as seen here: https://en.wikipedia.org/wiki/List_of_tz_database_time_zones")]
	public static string wipeTimezone = "Europe/London";

	// Token: 0x04001C21 RID: 7201
	[ServerVar(Help = "Unix timestamp (seconds) for the upcoming wipe. Overrides all other convars if set to a time in the future.")]
	public static long wipeUnixTimestampOverride = 0L;

	// Token: 0x04001C22 RID: 7202
	[ServerVar(Help = "Custom cron expression for the wipe schedule. Overrides all other convars (except wipeUnixTimestampOverride) if set. Uses Cronos as a parser: https://github.com/HangfireIO/Cronos/")]
	public static string wipeCronOverride = "";

	// Token: 0x04001C23 RID: 7203
	public bool useWipeDayOverride;

	// Token: 0x04001C24 RID: 7204
	public DayOfWeek wipeDayOfWeekOverride = DayOfWeek.Thursday;

	// Token: 0x04001C25 RID: 7205
	public WipeTimer.WipeFrequency wipeFrequency;

	// Token: 0x04001C26 RID: 7206
	[ServerVar(Name = "days_to_add_test")]
	public static int daysToAddTest = 0;

	// Token: 0x04001C27 RID: 7207
	[ServerVar(Name = "hours_to_add_test")]
	public static float hoursToAddTest = 0f;

	// Token: 0x04001C28 RID: 7208
	public static WipeTimer serverinstance;

	// Token: 0x04001C29 RID: 7209
	public static WipeTimer clientinstance;

	// Token: 0x04001C2A RID: 7210
	private string oldTags = "";

	// Token: 0x04001C2B RID: 7211
	private static string cronExprCacheKey = null;

	// Token: 0x04001C2C RID: 7212
	private static CronExpression cronExprCache = null;

	// Token: 0x04001C2D RID: 7213
	private static ValueTuple<WipeTimer.WipeFrequency, int, float>? cronCacheKey = null;

	// Token: 0x04001C2E RID: 7214
	private static string cronCache = null;

	// Token: 0x04001C2F RID: 7215
	private static string timezoneCacheKey = null;

	// Token: 0x04001C30 RID: 7216
	private static TimeZoneInfo timezoneCache = null;

	// Token: 0x0600240B RID: 9227 RVA: 0x000E6177 File Offset: 0x000E4377
	public override void InitShared()
	{
		base.InitShared();
		if (base.isServer)
		{
			WipeTimer.serverinstance = this;
		}
		if (base.isClient)
		{
			WipeTimer.clientinstance = this;
		}
	}

	// Token: 0x0600240C RID: 9228 RVA: 0x000E619B File Offset: 0x000E439B
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			WipeTimer.serverinstance = null;
		}
		if (base.isClient)
		{
			WipeTimer.clientinstance = null;
		}
		base.DestroyShared();
	}

	// Token: 0x0600240D RID: 9229 RVA: 0x000E61BF File Offset: 0x000E43BF
	public override void ServerInit()
	{
		base.ServerInit();
		this.RecalculateWipeFrequency();
		base.InvokeRepeating(new Action(this.TryAndUpdate), 1f, 4f);
	}

	// Token: 0x0600240E RID: 9230 RVA: 0x000E61EC File Offset: 0x000E43EC
	public void RecalculateWipeFrequency()
	{
		string tags = Server.tags;
		if (tags.Contains("monthly"))
		{
			this.wipeFrequency = WipeTimer.WipeFrequency.Monthly;
			return;
		}
		if (tags.Contains("biweekly"))
		{
			this.wipeFrequency = WipeTimer.WipeFrequency.BiWeekly;
			return;
		}
		if (tags.Contains("weekly"))
		{
			this.wipeFrequency = WipeTimer.WipeFrequency.Weekly;
			return;
		}
		this.wipeFrequency = WipeTimer.WipeFrequency.Monthly;
	}

	// Token: 0x0600240F RID: 9231 RVA: 0x000E6245 File Offset: 0x000E4445
	public void TryAndUpdate()
	{
		if (Server.tags != this.oldTags)
		{
			this.RecalculateWipeFrequency();
			this.oldTags = Server.tags;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06002410 RID: 9232 RVA: 0x000E6274 File Offset: 0x000E4474
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk && info.msg.landmine == null)
		{
			info.msg.landmine = Facepunch.Pool.Get<ProtoBuf.Landmine>();
			info.msg.landmine.triggeredID = (ulong)this.GetTicksUntilWipe();
		}
	}

	// Token: 0x06002411 RID: 9233 RVA: 0x000E62C4 File Offset: 0x000E44C4
	public TimeSpan GetTimeSpanUntilWipe()
	{
		DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow.AddDays((double)WipeTimer.daysToAddTest).AddHours((double)WipeTimer.hoursToAddTest);
		return this.GetWipeTime(dateTimeOffset) - dateTimeOffset;
	}

	// Token: 0x06002412 RID: 9234 RVA: 0x000E6300 File Offset: 0x000E4500
	public long GetTicksUntilWipe()
	{
		return this.GetTimeSpanUntilWipe().Ticks;
	}

	// Token: 0x06002413 RID: 9235 RVA: 0x000E631C File Offset: 0x000E451C
	[ServerVar]
	public static void PrintWipe(ConsoleSystem.Arg arg)
	{
		if (WipeTimer.serverinstance == null)
		{
			arg.ReplyWith("WipeTimer not found!");
			return;
		}
		WipeTimer.serverinstance.RecalculateWipeFrequency();
		WipeTimer.serverinstance.TryAndUpdate();
		TimeZoneInfo timeZone = WipeTimer.GetTimeZone();
		string text2;
		string text = TZConvert.TryWindowsToIana(timeZone.Id, out text2) ? text2 : timeZone.Id;
		DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow.AddDays((double)WipeTimer.daysToAddTest).AddHours((double)WipeTimer.hoursToAddTest);
		DateTimeOffset wipeTime = WipeTimer.serverinstance.GetWipeTime(dateTimeOffset);
		TimeSpan timeSpan = wipeTime - dateTimeOffset;
		string cronString = WipeTimer.GetCronString(WipeTimer.serverinstance.wipeFrequency, (int)(WipeTimer.serverinstance.useWipeDayOverride ? WipeTimer.serverinstance.wipeDayOfWeekOverride : ((DayOfWeek)WipeTimer.wipeDayOfWeek)), WipeTimer.wipeHourOfDay);
		CronExpression cronExpression = WipeTimer.GetCronExpression(WipeTimer.serverinstance.wipeFrequency, (int)(WipeTimer.serverinstance.useWipeDayOverride ? WipeTimer.serverinstance.wipeDayOfWeekOverride : ((DayOfWeek)WipeTimer.wipeDayOfWeek)), WipeTimer.wipeHourOfDay);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Format("Frequency: {0}", WipeTimer.serverinstance.wipeFrequency));
		stringBuilder.AppendLine(string.Concat(new string[]
		{
			"Timezone: ",
			timeZone.StandardName,
			" (ID=",
			timeZone.Id,
			", IANA=",
			text,
			")"
		}));
		stringBuilder.AppendLine(string.Format("Wipe day of week: {0}", (DayOfWeek)WipeTimer.wipeDayOfWeek));
		stringBuilder.AppendLine(string.Format("Wipe hour: {0}", WipeTimer.wipeHourOfDay));
		stringBuilder.AppendLine(string.Format("Test time: {0:O}", dateTimeOffset));
		stringBuilder.AppendLine(string.Format("Wipe time: {0:O}", wipeTime));
		stringBuilder.AppendLine(string.Format("Time until wipe: {0:g}", timeSpan));
		stringBuilder.AppendLine(string.Format("Ticks until wipe: {0}", timeSpan.Ticks));
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("Cron: " + cronString);
		stringBuilder.AppendLine("Next 10 occurrences:");
		int num = 0;
		foreach (DateTimeOffset dateTimeOffset2 in cronExpression.GetOccurrences(dateTimeOffset, dateTimeOffset.AddYears(2), timeZone, true, false).Take(10))
		{
			stringBuilder.AppendLine(string.Format("  {0}. {1:O}", num, dateTimeOffset2));
			num++;
		}
		arg.ReplyWith(stringBuilder.ToString());
	}

	// Token: 0x06002414 RID: 9236 RVA: 0x000E65D8 File Offset: 0x000E47D8
	[ServerVar]
	public static void PrintTimeZones(ConsoleSystem.Arg arg)
	{
		List<string> systemTzs = (from z in TimeZoneInfo.GetSystemTimeZones()
		select z.Id).ToList<string>();
		IReadOnlyCollection<string> knownWindowsTimeZoneIds = TZConvert.KnownWindowsTimeZoneIds;
		IReadOnlyCollection<string> knownIanaTimeZoneNames = TZConvert.KnownIanaTimeZoneNames;
		arg.ReplyWith(JsonConvert.SerializeObject(new
		{
			systemTzs = systemTzs,
			windowsTzs = knownWindowsTimeZoneIds,
			ianaTzs = knownIanaTimeZoneNames
		}));
	}

	// Token: 0x06002415 RID: 9237 RVA: 0x000E6634 File Offset: 0x000E4834
	public DateTimeOffset GetWipeTime(DateTimeOffset nowTime)
	{
		if (WipeTimer.wipeUnixTimestampOverride > 0L && WipeTimer.wipeUnixTimestampOverride > (long)Epoch.Current)
		{
			return Epoch.ToDateTime(WipeTimer.wipeUnixTimestampOverride);
		}
		DateTimeOffset result;
		try
		{
			result = (WipeTimer.GetCronExpression(this.wipeFrequency, (int)(this.useWipeDayOverride ? this.wipeDayOfWeekOverride : ((DayOfWeek)WipeTimer.wipeDayOfWeek)), WipeTimer.wipeHourOfDay).GetNextOccurrence(nowTime, WipeTimer.GetTimeZone(), false) ?? DateTimeOffset.MaxValue);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			result = DateTimeOffset.MaxValue;
		}
		return result;
	}

	// Token: 0x06002416 RID: 9238 RVA: 0x000E66D4 File Offset: 0x000E48D4
	private static CronExpression GetCronExpression(WipeTimer.WipeFrequency frequency, int dayOfWeek, float hourOfDay)
	{
		string cronString = WipeTimer.GetCronString(frequency, dayOfWeek, hourOfDay);
		if (cronString == WipeTimer.cronExprCacheKey && WipeTimer.cronExprCache != null)
		{
			return WipeTimer.cronExprCache;
		}
		WipeTimer.cronExprCache = CronExpression.Parse(cronString);
		WipeTimer.cronExprCacheKey = cronString;
		return WipeTimer.cronExprCache;
	}

	// Token: 0x06002417 RID: 9239 RVA: 0x000E6720 File Offset: 0x000E4920
	private static string GetCronString(WipeTimer.WipeFrequency frequency, int dayOfWeek, float hourOfDay)
	{
		if (!string.IsNullOrWhiteSpace(WipeTimer.wipeCronOverride))
		{
			return WipeTimer.wipeCronOverride;
		}
		ValueTuple<WipeTimer.WipeFrequency, int, float> valueTuple = new ValueTuple<WipeTimer.WipeFrequency, int, float>(frequency, dayOfWeek, hourOfDay);
		ValueTuple<WipeTimer.WipeFrequency, int, float> valueTuple2 = valueTuple;
		ValueTuple<WipeTimer.WipeFrequency, int, float>? valueTuple3 = WipeTimer.cronCacheKey;
		bool flag;
		if (valueTuple3 == null)
		{
			flag = false;
		}
		else
		{
			ValueTuple<WipeTimer.WipeFrequency, int, float> valueOrDefault = valueTuple3.GetValueOrDefault();
			flag = (valueTuple2.Item1 == valueOrDefault.Item1 && valueTuple2.Item2 == valueOrDefault.Item2 && valueTuple2.Item3 == valueOrDefault.Item3);
		}
		if (flag && WipeTimer.cronCache != null)
		{
			return WipeTimer.cronCache;
		}
		WipeTimer.cronCache = WipeTimer.BuildCronString(frequency, dayOfWeek, hourOfDay);
		WipeTimer.cronCacheKey = new ValueTuple<WipeTimer.WipeFrequency, int, float>?(valueTuple);
		return WipeTimer.cronCache;
	}

	// Token: 0x06002418 RID: 9240 RVA: 0x000E67C0 File Offset: 0x000E49C0
	private static string BuildCronString(WipeTimer.WipeFrequency frequency, int dayOfWeek, float hourOfDay)
	{
		int num = Mathf.FloorToInt(hourOfDay);
		int num2 = Mathf.FloorToInt((hourOfDay - (float)num) * 60f);
		switch (frequency)
		{
		case WipeTimer.WipeFrequency.Monthly:
			return string.Format("{0} {1} * * {2}#1", num2, num, dayOfWeek);
		case WipeTimer.WipeFrequency.Weekly:
			return string.Format("{0} {1} * * {2}", num2, num, dayOfWeek);
		case WipeTimer.WipeFrequency.BiWeekly:
			return string.Format("{0} {1} 1-7,15-21,29-31 * {2}", num2, num, dayOfWeek);
		default:
			throw new NotSupportedException(string.Format("WipeFrequency {0}", frequency));
		}
	}

	// Token: 0x06002419 RID: 9241 RVA: 0x000E6864 File Offset: 0x000E4A64
	private static TimeZoneInfo GetTimeZone()
	{
		if (string.IsNullOrWhiteSpace(WipeTimer.wipeTimezone))
		{
			return TimeZoneInfo.Local;
		}
		if (WipeTimer.wipeTimezone == WipeTimer.timezoneCacheKey && WipeTimer.timezoneCache != null)
		{
			return WipeTimer.timezoneCache;
		}
		if (TZConvert.TryGetTimeZoneInfo(WipeTimer.wipeTimezone, out WipeTimer.timezoneCache))
		{
			WipeTimer.timezoneCacheKey = WipeTimer.wipeTimezone;
			return WipeTimer.timezoneCache;
		}
		return TimeZoneInfo.Local;
	}

	// Token: 0x02000CDE RID: 3294
	public enum WipeFrequency
	{
		// Token: 0x0400453E RID: 17726
		Monthly,
		// Token: 0x0400453F RID: 17727
		Weekly,
		// Token: 0x04004540 RID: 17728
		BiWeekly
	}
}
