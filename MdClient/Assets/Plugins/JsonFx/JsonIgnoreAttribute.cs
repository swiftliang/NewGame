#region License
/*---------------------------------------------------------------------------------*\

	Distributed under the terms of an MIT-style license:

	The MIT License

	Copyright (c) 2006-2009 Stephen M. McKamey

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.

\*---------------------------------------------------------------------------------*/
#endregion License

using System;
using System.Reflection;
#if !UNITY3D
using System.Xml.Serialization;
#endif

#if WINDOWS_STORE
using TP = System.Reflection.TypeInfo;
#else
using TP = System.Type;
#endif

using TCU = JsonFx.Json.TypeCoercionUtility;

namespace JsonFx.Json
{
	/// <summary>
	/// Designates a property or field to not be serialized.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple=false)]
	public sealed class JsonIgnoreAttribute : Attribute
	{
		#region Methods

		/// <summary>
		/// Gets a value which indicates if should be ignored in Json serialization.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsJsonIgnore(object value)
		{
			if (value == null)
			{
				return false;
			}

			Type type = value.GetType();

			ICustomAttributeProvider provider = null;
			if (TCU.GetTypeInfo(type).IsEnum) {
				provider = TCU.GetTypeInfo(type).GetField(Enum.GetName(type, value));
			} else {
				provider = value as ICustomAttributeProvider;
			}

			if (provider == null) {
				throw new ArgumentException();
			}

			return provider.IsDefined(typeof(JsonIgnoreAttribute), true);
		}

		/// <summary>
		/// Gets a value which indicates if should be ignored in Json serialization.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsXmlIgnore(object value)
		{
			if (value == null)
			{
				return false;
			}

			Type type = value.GetType();

			ICustomAttributeProvider provider = null;
			if (TCU.GetTypeInfo(type).IsEnum)
			{
				provider = TCU.GetTypeInfo(type).GetField(Enum.GetName(type, value));
			}
			else
			{
				provider = value as ICustomAttributeProvider;
			}

			if (provider == null)
			{
				throw new ArgumentException();
			}

#if !UNITY3D
			return provider.IsDefined(typeof(XmlIgnoreAttribute), true);
#else
			return false;
#endif
		}

		#endregion Methods
	}
}
