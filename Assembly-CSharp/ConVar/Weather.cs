using System;
using System.Globalization;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AE8 RID: 2792
	[ConsoleSystem.Factory("weather")]
	public class Weather : ConsoleSystem
	{
		// Token: 0x04003C49 RID: 15433
		[ServerVar]
		public static float wetness_rain = 0.4f;

		// Token: 0x04003C4A RID: 15434
		[ServerVar]
		public static float wetness_snow = 0.2f;

		// Token: 0x06004339 RID: 17209 RVA: 0x0018D768 File Offset: 0x0018B968
		[ClientVar]
		[ServerVar]
		public static void load(ConsoleSystem.Arg args)
		{
			if (!SingletonComponent<Climate>.Instance)
			{
				return;
			}
			string name = args.GetString(0, "");
			if (string.IsNullOrEmpty(name))
			{
				args.ReplyWith("Weather preset name invalid.");
				return;
			}
			WeatherPreset weatherPreset = Array.Find<WeatherPreset>(SingletonComponent<Climate>.Instance.WeatherPresets, (WeatherPreset x) => x.name.Contains(name, CompareOptions.IgnoreCase));
			if (weatherPreset == null)
			{
				args.ReplyWith("Weather preset not found: " + name);
				return;
			}
			SingletonComponent<Climate>.Instance.WeatherOverrides.Set(weatherPreset);
			if (args.IsServerside)
			{
				ServerMgr.SendReplicatedVars("weather.");
			}
		}

		// Token: 0x0600433A RID: 17210 RVA: 0x0018D811 File Offset: 0x0018BA11
		[ClientVar]
		[ServerVar]
		public static void reset(ConsoleSystem.Arg args)
		{
			if (!SingletonComponent<Climate>.Instance)
			{
				return;
			}
			SingletonComponent<Climate>.Instance.WeatherOverrides.Reset();
			if (args.IsServerside)
			{
				ServerMgr.SendReplicatedVars("weather.");
			}
		}

		// Token: 0x0600433B RID: 17211 RVA: 0x0018D844 File Offset: 0x0018BA44
		[ClientVar]
		[ServerVar]
		public static void report(ConsoleSystem.Arg args)
		{
			if (!SingletonComponent<Climate>.Instance)
			{
				return;
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn(SingletonComponent<Climate>.Instance.WeatherStatePrevious.name);
			textTable.AddColumn("|");
			textTable.AddColumn(SingletonComponent<Climate>.Instance.WeatherStateTarget.name);
			textTable.AddColumn("|");
			textTable.AddColumn(SingletonComponent<Climate>.Instance.WeatherStateNext.name);
			int num = Mathf.RoundToInt(SingletonComponent<Climate>.Instance.WeatherStateBlend * 100f);
			if (num < 100)
			{
				textTable.AddRow(new string[]
				{
					"fading out (" + (100 - num) + "%)",
					"|",
					"fading in (" + num + "%)",
					"|",
					"up next"
				});
			}
			else
			{
				textTable.AddRow(new string[]
				{
					"previous",
					"|",
					"current",
					"|",
					"up next"
				});
			}
			args.ReplyWith(textTable.ToString() + Environment.NewLine + SingletonComponent<Climate>.Instance.WeatherState.ToString());
		}

		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x0600433C RID: 17212 RVA: 0x0018D986 File Offset: 0x0018BB86
		// (set) Token: 0x0600433D RID: 17213 RVA: 0x0018D9A9 File Offset: 0x0018BBA9
		[ReplicatedVar(Default = "1")]
		public static float clear_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 1f;
				}
				return SingletonComponent<Climate>.Instance.Weather.ClearChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.ClearChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x0600433E RID: 17214 RVA: 0x0018D9CD File Offset: 0x0018BBCD
		// (set) Token: 0x0600433F RID: 17215 RVA: 0x0018D9F0 File Offset: 0x0018BBF0
		[ReplicatedVar(Default = "0")]
		public static float dust_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.DustChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.DustChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x06004340 RID: 17216 RVA: 0x0018DA14 File Offset: 0x0018BC14
		// (set) Token: 0x06004341 RID: 17217 RVA: 0x0018DA37 File Offset: 0x0018BC37
		[ReplicatedVar(Default = "0")]
		public static float fog_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.FogChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.FogChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x06004342 RID: 17218 RVA: 0x0018DA5B File Offset: 0x0018BC5B
		// (set) Token: 0x06004343 RID: 17219 RVA: 0x0018DA7E File Offset: 0x0018BC7E
		[ReplicatedVar(Default = "0")]
		public static float overcast_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.OvercastChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.OvercastChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x06004344 RID: 17220 RVA: 0x0018DAA2 File Offset: 0x0018BCA2
		// (set) Token: 0x06004345 RID: 17221 RVA: 0x0018DAC5 File Offset: 0x0018BCC5
		[ReplicatedVar(Default = "0")]
		public static float storm_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.StormChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.StormChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x06004346 RID: 17222 RVA: 0x0018DAE9 File Offset: 0x0018BCE9
		// (set) Token: 0x06004347 RID: 17223 RVA: 0x0018DB0C File Offset: 0x0018BD0C
		[ReplicatedVar(Default = "0")]
		public static float rain_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.RainChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.RainChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x06004348 RID: 17224 RVA: 0x0018DB30 File Offset: 0x0018BD30
		// (set) Token: 0x06004349 RID: 17225 RVA: 0x0018DB53 File Offset: 0x0018BD53
		[ReplicatedVar(Default = "-1")]
		public static float rain
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Rain;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Rain = value;
			}
		}

		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x0600434A RID: 17226 RVA: 0x0018DB72 File Offset: 0x0018BD72
		// (set) Token: 0x0600434B RID: 17227 RVA: 0x0018DB95 File Offset: 0x0018BD95
		[ReplicatedVar(Default = "-1")]
		public static float wind
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Wind;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Wind = value;
			}
		}

		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x0600434C RID: 17228 RVA: 0x0018DBB4 File Offset: 0x0018BDB4
		// (set) Token: 0x0600434D RID: 17229 RVA: 0x0018DBD7 File Offset: 0x0018BDD7
		[ReplicatedVar(Default = "-1")]
		public static float thunder
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Thunder;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Thunder = value;
			}
		}

		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x0600434E RID: 17230 RVA: 0x0018DBF6 File Offset: 0x0018BDF6
		// (set) Token: 0x0600434F RID: 17231 RVA: 0x0018DC19 File Offset: 0x0018BE19
		[ReplicatedVar(Default = "-1")]
		public static float rainbow
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Rainbow;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Rainbow = value;
			}
		}

		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x06004350 RID: 17232 RVA: 0x0018DC38 File Offset: 0x0018BE38
		// (set) Token: 0x06004351 RID: 17233 RVA: 0x0018DC60 File Offset: 0x0018BE60
		[ReplicatedVar(Default = "-1")]
		public static float fog
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Fogginess;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Fogginess = value;
			}
		}

		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x06004352 RID: 17234 RVA: 0x0018DC84 File Offset: 0x0018BE84
		// (set) Token: 0x06004353 RID: 17235 RVA: 0x0018DCAC File Offset: 0x0018BEAC
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_rayleigh
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.RayleighMultiplier;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.RayleighMultiplier = value;
			}
		}

		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x06004354 RID: 17236 RVA: 0x0018DCD0 File Offset: 0x0018BED0
		// (set) Token: 0x06004355 RID: 17237 RVA: 0x0018DCF8 File Offset: 0x0018BEF8
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_mie
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.MieMultiplier;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.MieMultiplier = value;
			}
		}

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x06004356 RID: 17238 RVA: 0x0018DD1C File Offset: 0x0018BF1C
		// (set) Token: 0x06004357 RID: 17239 RVA: 0x0018DD44 File Offset: 0x0018BF44
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_brightness
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Brightness;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Brightness = value;
			}
		}

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x06004358 RID: 17240 RVA: 0x0018DD68 File Offset: 0x0018BF68
		// (set) Token: 0x06004359 RID: 17241 RVA: 0x0018DD90 File Offset: 0x0018BF90
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_contrast
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Contrast;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Contrast = value;
			}
		}

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x0600435A RID: 17242 RVA: 0x0018DDB4 File Offset: 0x0018BFB4
		// (set) Token: 0x0600435B RID: 17243 RVA: 0x0018DDDC File Offset: 0x0018BFDC
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_directionality
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Directionality;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Directionality = value;
			}
		}

		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x0600435C RID: 17244 RVA: 0x0018DE00 File Offset: 0x0018C000
		// (set) Token: 0x0600435D RID: 17245 RVA: 0x0018DE28 File Offset: 0x0018C028
		[ReplicatedVar(Default = "-1")]
		public static float cloud_size
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Size;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Size = value;
			}
		}

		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x0600435E RID: 17246 RVA: 0x0018DE4C File Offset: 0x0018C04C
		// (set) Token: 0x0600435F RID: 17247 RVA: 0x0018DE74 File Offset: 0x0018C074
		[ReplicatedVar(Default = "-1")]
		public static float cloud_opacity
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Opacity;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Opacity = value;
			}
		}

		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x06004360 RID: 17248 RVA: 0x0018DE98 File Offset: 0x0018C098
		// (set) Token: 0x06004361 RID: 17249 RVA: 0x0018DEC0 File Offset: 0x0018C0C0
		[ReplicatedVar(Default = "-1")]
		public static float cloud_coverage
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coverage;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coverage = value;
			}
		}

		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x06004362 RID: 17250 RVA: 0x0018DEE4 File Offset: 0x0018C0E4
		// (set) Token: 0x06004363 RID: 17251 RVA: 0x0018DF0C File Offset: 0x0018C10C
		[ReplicatedVar(Default = "-1")]
		public static float cloud_sharpness
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Sharpness;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Sharpness = value;
			}
		}

		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x06004364 RID: 17252 RVA: 0x0018DF30 File Offset: 0x0018C130
		// (set) Token: 0x06004365 RID: 17253 RVA: 0x0018DF58 File Offset: 0x0018C158
		[ReplicatedVar(Default = "-1")]
		public static float cloud_coloring
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coloring;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coloring = value;
			}
		}

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x06004366 RID: 17254 RVA: 0x0018DF7C File Offset: 0x0018C17C
		// (set) Token: 0x06004367 RID: 17255 RVA: 0x0018DFA4 File Offset: 0x0018C1A4
		[ReplicatedVar(Default = "-1")]
		public static float cloud_attenuation
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Attenuation;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Attenuation = value;
			}
		}

		// Token: 0x170005F7 RID: 1527
		// (get) Token: 0x06004368 RID: 17256 RVA: 0x0018DFC8 File Offset: 0x0018C1C8
		// (set) Token: 0x06004369 RID: 17257 RVA: 0x0018DFF0 File Offset: 0x0018C1F0
		[ReplicatedVar(Default = "-1")]
		public static float cloud_saturation
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Saturation;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Saturation = value;
			}
		}

		// Token: 0x170005F8 RID: 1528
		// (get) Token: 0x0600436A RID: 17258 RVA: 0x0018E014 File Offset: 0x0018C214
		// (set) Token: 0x0600436B RID: 17259 RVA: 0x0018E03C File Offset: 0x0018C23C
		[ReplicatedVar(Default = "-1")]
		public static float cloud_scattering
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Scattering;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Scattering = value;
			}
		}

		// Token: 0x170005F9 RID: 1529
		// (get) Token: 0x0600436C RID: 17260 RVA: 0x0018E060 File Offset: 0x0018C260
		// (set) Token: 0x0600436D RID: 17261 RVA: 0x0018E088 File Offset: 0x0018C288
		[ReplicatedVar(Default = "-1")]
		public static float cloud_brightness
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Brightness;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Brightness = value;
			}
		}
	}
}
