using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace TinyJSON
{
	// Token: 0x020009D2 RID: 2514
	public static class JSON
	{
		// Token: 0x0400366A RID: 13930
		private static readonly Type includeAttrType = typeof(Include);

		// Token: 0x0400366B RID: 13931
		private static readonly Type excludeAttrType = typeof(Exclude);

		// Token: 0x0400366C RID: 13932
		private static readonly Type decodeAliasAttrType = typeof(DecodeAlias);

		// Token: 0x0400366D RID: 13933
		private static readonly Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

		// Token: 0x0400366E RID: 13934
		private const BindingFlags instanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		// Token: 0x0400366F RID: 13935
		private const BindingFlags staticBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		// Token: 0x04003670 RID: 13936
		private static readonly MethodInfo decodeTypeMethod = typeof(JSON).GetMethod("DecodeType", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x04003671 RID: 13937
		private static readonly MethodInfo decodeListMethod = typeof(JSON).GetMethod("DecodeList", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x04003672 RID: 13938
		private static readonly MethodInfo decodeDictionaryMethod = typeof(JSON).GetMethod("DecodeDictionary", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x04003673 RID: 13939
		private static readonly MethodInfo decodeArrayMethod = typeof(JSON).GetMethod("DecodeArray", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x04003674 RID: 13940
		private static readonly MethodInfo decodeMultiRankArrayMethod = typeof(JSON).GetMethod("DecodeMultiRankArray", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x06003BEA RID: 15338 RVA: 0x00162064 File Offset: 0x00160264
		public static Variant Load(string json)
		{
			if (json == null)
			{
				throw new ArgumentNullException("json");
			}
			return Decoder.Decode(json);
		}

		// Token: 0x06003BEB RID: 15339 RVA: 0x0016207A File Offset: 0x0016027A
		public static string Dump(object data)
		{
			return JSON.Dump(data, EncodeOptions.None);
		}

		// Token: 0x06003BEC RID: 15340 RVA: 0x00162084 File Offset: 0x00160284
		public static string Dump(object data, EncodeOptions options)
		{
			if (data != null)
			{
				Type type = data.GetType();
				if (!type.IsEnum && !type.IsPrimitive && !type.IsArray)
				{
					foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					{
						if (methodInfo.GetCustomAttributes(false).AnyOfType(typeof(BeforeEncode)) && methodInfo.GetParameters().Length == 0)
						{
							methodInfo.Invoke(data, null);
						}
					}
				}
			}
			return Encoder.Encode(data, options);
		}

		// Token: 0x06003BED RID: 15341 RVA: 0x001620FF File Offset: 0x001602FF
		public static void MakeInto<T>(Variant data, out T item)
		{
			item = JSON.DecodeType<T>(data);
		}

		// Token: 0x06003BEE RID: 15342 RVA: 0x00162110 File Offset: 0x00160310
		private static Type FindType(string fullName)
		{
			if (fullName == null)
			{
				return null;
			}
			Type type;
			if (JSON.typeCache.TryGetValue(fullName, out type))
			{
				return type;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				type = assemblies[i].GetType(fullName);
				if (type != null)
				{
					JSON.typeCache.Add(fullName, type);
					return type;
				}
			}
			return null;
		}

		// Token: 0x06003BEF RID: 15343 RVA: 0x00162170 File Offset: 0x00160370
		private static T DecodeType<T>(Variant data)
		{
			if (data == null)
			{
				return default(T);
			}
			Type type = typeof(T);
			if (type.IsEnum)
			{
				return (T)((object)Enum.Parse(type, data.ToString(CultureInfo.InvariantCulture)));
			}
			if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
			{
				return (T)((object)Convert.ChangeType(data, type));
			}
			if (type == typeof(Guid))
			{
				return (T)((object)new Guid(data.ToString(CultureInfo.InvariantCulture)));
			}
			if (type.IsArray)
			{
				if (type.GetArrayRank() == 1)
				{
					return (T)((object)JSON.decodeArrayMethod.MakeGenericMethod(new Type[]
					{
						type.GetElementType()
					}).Invoke(null, new object[]
					{
						data
					}));
				}
				ProxyArray proxyArray = data as ProxyArray;
				if (proxyArray == null)
				{
					throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
				}
				int[] array = new int[type.GetArrayRank()];
				if (!proxyArray.CanBeMultiRankArray(array))
				{
					throw new DecodeException("Error decoding multidimensional array; JSON data doesn't seem fit this structure.");
				}
				Type elementType = type.GetElementType();
				if (elementType == null)
				{
					throw new DecodeException("Array element type is expected to be not null, but it is.");
				}
				Array array2 = Array.CreateInstance(elementType, array);
				MethodInfo methodInfo = JSON.decodeMultiRankArrayMethod.MakeGenericMethod(new Type[]
				{
					elementType
				});
				try
				{
					methodInfo.Invoke(null, new object[]
					{
						proxyArray,
						array2,
						1,
						array
					});
				}
				catch (Exception innerException)
				{
					throw new DecodeException("Error decoding multidimensional array. Did you try to decode into an array of incompatible rank or element type?", innerException);
				}
				return (T)((object)Convert.ChangeType(array2, typeof(T)));
			}
			else
			{
				if (typeof(IList).IsAssignableFrom(type))
				{
					return (T)((object)JSON.decodeListMethod.MakeGenericMethod(type.GetGenericArguments()).Invoke(null, new object[]
					{
						data
					}));
				}
				if (typeof(IDictionary).IsAssignableFrom(type))
				{
					return (T)((object)JSON.decodeDictionaryMethod.MakeGenericMethod(type.GetGenericArguments()).Invoke(null, new object[]
					{
						data
					}));
				}
				ProxyObject proxyObject = data as ProxyObject;
				if (proxyObject == null)
				{
					throw new InvalidCastException("ProxyObject expected when decoding into '" + type.FullName + "'.");
				}
				string typeHint = proxyObject.TypeHint;
				T t;
				if (typeHint != null && typeHint != type.FullName)
				{
					Type type2 = JSON.FindType(typeHint);
					if (type2 == null)
					{
						throw new TypeLoadException("Could not load type '" + typeHint + "'.");
					}
					if (!type.IsAssignableFrom(type2))
					{
						throw new InvalidCastException(string.Concat(new string[]
						{
							"Cannot assign type '",
							typeHint,
							"' to type '",
							type.FullName,
							"'."
						}));
					}
					t = (T)((object)Activator.CreateInstance(type2));
					type = type2;
				}
				else
				{
					t = Activator.CreateInstance<T>();
				}
				foreach (KeyValuePair<string, Variant> keyValuePair in ((IEnumerable<KeyValuePair<string, Variant>>)((ProxyObject)data)))
				{
					FieldInfo fieldInfo = type.GetField(keyValuePair.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (fieldInfo == null)
					{
						foreach (FieldInfo fieldInfo2 in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
						{
							foreach (object obj in fieldInfo2.GetCustomAttributes(true))
							{
								if (JSON.decodeAliasAttrType.IsInstanceOfType(obj) && ((DecodeAlias)obj).Contains(keyValuePair.Key))
								{
									fieldInfo = fieldInfo2;
									break;
								}
							}
						}
					}
					if (fieldInfo != null)
					{
						bool flag = fieldInfo.IsPublic;
						foreach (object o in fieldInfo.GetCustomAttributes(true))
						{
							if (JSON.excludeAttrType.IsInstanceOfType(o))
							{
								flag = false;
							}
							if (JSON.includeAttrType.IsInstanceOfType(o))
							{
								flag = true;
							}
						}
						if (flag)
						{
							MethodInfo methodInfo2 = JSON.decodeTypeMethod.MakeGenericMethod(new Type[]
							{
								fieldInfo.FieldType
							});
							if (type.IsValueType)
							{
								object obj2 = t;
								fieldInfo.SetValue(obj2, methodInfo2.Invoke(null, new object[]
								{
									keyValuePair.Value
								}));
								t = (T)((object)obj2);
							}
							else
							{
								fieldInfo.SetValue(t, methodInfo2.Invoke(null, new object[]
								{
									keyValuePair.Value
								}));
							}
						}
					}
					PropertyInfo propertyInfo = type.GetProperty(keyValuePair.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (propertyInfo == null)
					{
						foreach (PropertyInfo propertyInfo2 in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
						{
							foreach (object obj3 in propertyInfo2.GetCustomAttributes(false))
							{
								if (JSON.decodeAliasAttrType.IsInstanceOfType(obj3) && ((DecodeAlias)obj3).Contains(keyValuePair.Key))
								{
									propertyInfo = propertyInfo2;
									break;
								}
							}
						}
					}
					if (propertyInfo != null && propertyInfo.CanWrite && propertyInfo.GetCustomAttributes(false).AnyOfType(JSON.includeAttrType))
					{
						MethodInfo methodInfo3 = JSON.decodeTypeMethod.MakeGenericMethod(new Type[]
						{
							propertyInfo.PropertyType
						});
						if (type.IsValueType)
						{
							object obj4 = t;
							propertyInfo.SetValue(obj4, methodInfo3.Invoke(null, new object[]
							{
								keyValuePair.Value
							}), null);
							t = (T)((object)obj4);
						}
						else
						{
							propertyInfo.SetValue(t, methodInfo3.Invoke(null, new object[]
							{
								keyValuePair.Value
							}), null);
						}
					}
				}
				foreach (MethodInfo methodInfo4 in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (methodInfo4.GetCustomAttributes(false).AnyOfType(typeof(AfterDecode)))
					{
						MethodBase methodBase = methodInfo4;
						object obj5 = t;
						object[] parameters;
						if (methodInfo4.GetParameters().Length != 0)
						{
							(parameters = new object[1])[0] = data;
						}
						else
						{
							parameters = null;
						}
						methodBase.Invoke(obj5, parameters);
					}
				}
				return t;
			}
		}

		// Token: 0x06003BF0 RID: 15344 RVA: 0x001627C4 File Offset: 0x001609C4
		private static List<T> DecodeList<T>(Variant data)
		{
			List<T> list = new List<T>();
			ProxyArray proxyArray = data as ProxyArray;
			if (proxyArray == null)
			{
				throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
			}
			foreach (Variant data2 in ((IEnumerable<Variant>)proxyArray))
			{
				list.Add(JSON.DecodeType<T>(data2));
			}
			return list;
		}

		// Token: 0x06003BF1 RID: 15345 RVA: 0x0016282C File Offset: 0x00160A2C
		private static Dictionary<TKey, TValue> DecodeDictionary<TKey, TValue>(Variant data)
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			Type typeFromHandle = typeof(TKey);
			ProxyObject proxyObject = data as ProxyObject;
			if (proxyObject == null)
			{
				throw new DecodeException("Variant is expected to be a ProxyObject here, but it is not.");
			}
			foreach (KeyValuePair<string, Variant> keyValuePair in ((IEnumerable<KeyValuePair<string, Variant>>)proxyObject))
			{
				TKey key = (TKey)((object)(typeFromHandle.IsEnum ? Enum.Parse(typeFromHandle, keyValuePair.Key) : Convert.ChangeType(keyValuePair.Key, typeFromHandle)));
				TValue value = JSON.DecodeType<TValue>(keyValuePair.Value);
				dictionary.Add(key, value);
			}
			return dictionary;
		}

		// Token: 0x06003BF2 RID: 15346 RVA: 0x001628D8 File Offset: 0x00160AD8
		private static T[] DecodeArray<T>(Variant data)
		{
			ProxyArray proxyArray = data as ProxyArray;
			if (proxyArray == null)
			{
				throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
			}
			T[] array = new T[proxyArray.Count];
			int num = 0;
			foreach (Variant data2 in ((IEnumerable<Variant>)proxyArray))
			{
				array[num++] = JSON.DecodeType<T>(data2);
			}
			return array;
		}

		// Token: 0x06003BF3 RID: 15347 RVA: 0x0016294C File Offset: 0x00160B4C
		private static void DecodeMultiRankArray<T>(ProxyArray arrayData, Array array, int arrayRank, int[] indices)
		{
			int count = arrayData.Count;
			for (int i = 0; i < count; i++)
			{
				indices[arrayRank - 1] = i;
				if (arrayRank < array.Rank)
				{
					JSON.DecodeMultiRankArray<T>(arrayData[i] as ProxyArray, array, arrayRank + 1, indices);
				}
				else
				{
					array.SetValue(JSON.DecodeType<T>(arrayData[i]), indices);
				}
			}
		}

		// Token: 0x06003BF4 RID: 15348 RVA: 0x001629AC File Offset: 0x00160BAC
		public static void SupportTypeForAOT<T>()
		{
			JSON.DecodeType<T>(null);
			JSON.DecodeList<T>(null);
			JSON.DecodeArray<T>(null);
			JSON.DecodeDictionary<short, T>(null);
			JSON.DecodeDictionary<ushort, T>(null);
			JSON.DecodeDictionary<int, T>(null);
			JSON.DecodeDictionary<uint, T>(null);
			JSON.DecodeDictionary<long, T>(null);
			JSON.DecodeDictionary<ulong, T>(null);
			JSON.DecodeDictionary<float, T>(null);
			JSON.DecodeDictionary<double, T>(null);
			JSON.DecodeDictionary<decimal, T>(null);
			JSON.DecodeDictionary<bool, T>(null);
			JSON.DecodeDictionary<string, T>(null);
		}

		// Token: 0x06003BF5 RID: 15349 RVA: 0x00162A1B File Offset: 0x00160C1B
		private static void SupportValueTypesForAOT()
		{
			JSON.SupportTypeForAOT<short>();
			JSON.SupportTypeForAOT<ushort>();
			JSON.SupportTypeForAOT<int>();
			JSON.SupportTypeForAOT<uint>();
			JSON.SupportTypeForAOT<long>();
			JSON.SupportTypeForAOT<ulong>();
			JSON.SupportTypeForAOT<float>();
			JSON.SupportTypeForAOT<double>();
			JSON.SupportTypeForAOT<decimal>();
			JSON.SupportTypeForAOT<bool>();
			JSON.SupportTypeForAOT<string>();
		}
	}
}
