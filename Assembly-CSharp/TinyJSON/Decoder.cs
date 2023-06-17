using System;
using System.IO;
using System.Text;

namespace TinyJSON
{
	// Token: 0x020009C5 RID: 2501
	public sealed class Decoder : IDisposable
	{
		// Token: 0x04003659 RID: 13913
		private const string whiteSpace = " \t\n\r";

		// Token: 0x0400365A RID: 13914
		private const string wordBreak = " \t\n\r{}[],:\"";

		// Token: 0x0400365B RID: 13915
		private StringReader json;

		// Token: 0x06003BB4 RID: 15284 RVA: 0x00160FC0 File Offset: 0x0015F1C0
		private Decoder(string jsonString)
		{
			this.json = new StringReader(jsonString);
		}

		// Token: 0x06003BB5 RID: 15285 RVA: 0x00160FD4 File Offset: 0x0015F1D4
		public static Variant Decode(string jsonString)
		{
			Variant result;
			using (Decoder decoder = new Decoder(jsonString))
			{
				result = decoder.DecodeValue();
			}
			return result;
		}

		// Token: 0x06003BB6 RID: 15286 RVA: 0x0016100C File Offset: 0x0015F20C
		public void Dispose()
		{
			this.json.Dispose();
			this.json = null;
		}

		// Token: 0x06003BB7 RID: 15287 RVA: 0x00161020 File Offset: 0x0015F220
		private ProxyObject DecodeObject()
		{
			ProxyObject proxyObject = new ProxyObject();
			this.json.Read();
			for (;;)
			{
				Decoder.Token nextToken = this.NextToken;
				if (nextToken == Decoder.Token.None)
				{
					break;
				}
				if (nextToken == Decoder.Token.CloseBrace)
				{
					return proxyObject;
				}
				if (nextToken != Decoder.Token.Comma)
				{
					string text = this.DecodeString();
					if (text == null)
					{
						goto Block_4;
					}
					if (this.NextToken != Decoder.Token.Colon)
					{
						goto Block_5;
					}
					this.json.Read();
					proxyObject.Add(text, this.DecodeValue());
				}
			}
			return null;
			Block_4:
			return null;
			Block_5:
			return null;
		}

		// Token: 0x06003BB8 RID: 15288 RVA: 0x00161090 File Offset: 0x0015F290
		private ProxyArray DecodeArray()
		{
			ProxyArray proxyArray = new ProxyArray();
			this.json.Read();
			bool flag = true;
			while (flag)
			{
				Decoder.Token nextToken = this.NextToken;
				if (nextToken == Decoder.Token.None)
				{
					return null;
				}
				if (nextToken != Decoder.Token.CloseBracket)
				{
					if (nextToken != Decoder.Token.Comma)
					{
						proxyArray.Add(this.DecodeByToken(nextToken));
					}
				}
				else
				{
					flag = false;
				}
			}
			return proxyArray;
		}

		// Token: 0x06003BB9 RID: 15289 RVA: 0x001610E0 File Offset: 0x0015F2E0
		private Variant DecodeValue()
		{
			Decoder.Token nextToken = this.NextToken;
			return this.DecodeByToken(nextToken);
		}

		// Token: 0x06003BBA RID: 15290 RVA: 0x001610FC File Offset: 0x0015F2FC
		private Variant DecodeByToken(Decoder.Token token)
		{
			switch (token)
			{
			case Decoder.Token.OpenBrace:
				return this.DecodeObject();
			case Decoder.Token.OpenBracket:
				return this.DecodeArray();
			case Decoder.Token.String:
				return this.DecodeString();
			case Decoder.Token.Number:
				return this.DecodeNumber();
			case Decoder.Token.True:
				return new ProxyBoolean(true);
			case Decoder.Token.False:
				return new ProxyBoolean(false);
			case Decoder.Token.Null:
				return null;
			}
			return null;
		}

		// Token: 0x06003BBB RID: 15291 RVA: 0x0016116C File Offset: 0x0015F36C
		private Variant DecodeString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.json.Read();
			bool flag = true;
			while (flag)
			{
				if (this.json.Peek() == -1)
				{
					break;
				}
				char nextChar = this.NextChar;
				if (nextChar != '"')
				{
					if (nextChar != '\\')
					{
						stringBuilder.Append(nextChar);
					}
					else if (this.json.Peek() == -1)
					{
						flag = false;
					}
					else
					{
						nextChar = this.NextChar;
						if (nextChar <= '\\')
						{
							if (nextChar == '"' || nextChar == '/' || nextChar == '\\')
							{
								stringBuilder.Append(nextChar);
							}
						}
						else if (nextChar <= 'f')
						{
							if (nextChar != 'b')
							{
								if (nextChar == 'f')
								{
									stringBuilder.Append('\f');
								}
							}
							else
							{
								stringBuilder.Append('\b');
							}
						}
						else if (nextChar != 'n')
						{
							switch (nextChar)
							{
							case 'r':
								stringBuilder.Append('\r');
								break;
							case 't':
								stringBuilder.Append('\t');
								break;
							case 'u':
							{
								StringBuilder stringBuilder2 = new StringBuilder();
								for (int i = 0; i < 4; i++)
								{
									stringBuilder2.Append(this.NextChar);
								}
								stringBuilder.Append((char)Convert.ToInt32(stringBuilder2.ToString(), 16));
								break;
							}
							}
						}
						else
						{
							stringBuilder.Append('\n');
						}
					}
				}
				else
				{
					flag = false;
				}
			}
			return new ProxyString(stringBuilder.ToString());
		}

		// Token: 0x06003BBC RID: 15292 RVA: 0x001612C3 File Offset: 0x0015F4C3
		private Variant DecodeNumber()
		{
			return new ProxyNumber(this.NextWord);
		}

		// Token: 0x06003BBD RID: 15293 RVA: 0x001612D0 File Offset: 0x0015F4D0
		private void ConsumeWhiteSpace()
		{
			while (" \t\n\r".IndexOf(this.PeekChar) != -1)
			{
				this.json.Read();
				if (this.json.Peek() == -1)
				{
					break;
				}
			}
		}

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06003BBE RID: 15294 RVA: 0x00161304 File Offset: 0x0015F504
		private char PeekChar
		{
			get
			{
				int num = this.json.Peek();
				if (num != -1)
				{
					return Convert.ToChar(num);
				}
				return '\0';
			}
		}

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06003BBF RID: 15295 RVA: 0x00161329 File Offset: 0x0015F529
		private char NextChar
		{
			get
			{
				return Convert.ToChar(this.json.Read());
			}
		}

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06003BC0 RID: 15296 RVA: 0x0016133C File Offset: 0x0015F53C
		private string NextWord
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				while (" \t\n\r{}[],:\"".IndexOf(this.PeekChar) == -1)
				{
					stringBuilder.Append(this.NextChar);
					if (this.json.Peek() == -1)
					{
						break;
					}
				}
				return stringBuilder.ToString();
			}
		}

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x06003BC1 RID: 15297 RVA: 0x00161388 File Offset: 0x0015F588
		private Decoder.Token NextToken
		{
			get
			{
				this.ConsumeWhiteSpace();
				if (this.json.Peek() == -1)
				{
					return Decoder.Token.None;
				}
				char peekChar = this.PeekChar;
				if (peekChar <= '[')
				{
					switch (peekChar)
					{
					case '"':
						return Decoder.Token.String;
					case '#':
					case '$':
					case '%':
					case '&':
					case '\'':
					case '(':
					case ')':
					case '*':
					case '+':
					case '.':
					case '/':
						break;
					case ',':
						this.json.Read();
						return Decoder.Token.Comma;
					case '-':
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						return Decoder.Token.Number;
					case ':':
						return Decoder.Token.Colon;
					default:
						if (peekChar == '[')
						{
							return Decoder.Token.OpenBracket;
						}
						break;
					}
				}
				else
				{
					if (peekChar == ']')
					{
						this.json.Read();
						return Decoder.Token.CloseBracket;
					}
					if (peekChar == '{')
					{
						return Decoder.Token.OpenBrace;
					}
					if (peekChar == '}')
					{
						this.json.Read();
						return Decoder.Token.CloseBrace;
					}
				}
				string nextWord = this.NextWord;
				if (nextWord == "false")
				{
					return Decoder.Token.False;
				}
				if (nextWord == "true")
				{
					return Decoder.Token.True;
				}
				if (!(nextWord == "null"))
				{
					return Decoder.Token.None;
				}
				return Decoder.Token.Null;
			}
		}

		// Token: 0x02000EE4 RID: 3812
		private enum Token
		{
			// Token: 0x04004D7D RID: 19837
			None,
			// Token: 0x04004D7E RID: 19838
			OpenBrace,
			// Token: 0x04004D7F RID: 19839
			CloseBrace,
			// Token: 0x04004D80 RID: 19840
			OpenBracket,
			// Token: 0x04004D81 RID: 19841
			CloseBracket,
			// Token: 0x04004D82 RID: 19842
			Colon,
			// Token: 0x04004D83 RID: 19843
			Comma,
			// Token: 0x04004D84 RID: 19844
			String,
			// Token: 0x04004D85 RID: 19845
			Number,
			// Token: 0x04004D86 RID: 19846
			True,
			// Token: 0x04004D87 RID: 19847
			False,
			// Token: 0x04004D88 RID: 19848
			Null
		}
	}
}
