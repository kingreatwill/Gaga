
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Gaga.Core.Extensions
{
	public static class ObjectExtensions
	{
		public static string ToJson(this object obj)
		{
			string serializedObject = string.Empty;

			if (obj != null)
			{
				serializedObject = JsonConvert.SerializeObject(obj);

				//DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(obj.GetType());

				//using (MemoryStream memStream = new MemoryStream())
				//{
				//	jsonSerializer.WriteObject(memStream, obj);
				//	memStream.Position = 0;
				//	serializedObject = new StreamReader(memStream).ReadToEnd();
				//}
			}

			return serializedObject;
		}

		public static T JsonToObject<T>(this string jsonStr)
		{
			if (string.IsNullOrWhiteSpace(jsonStr))
				return default(T);
			return JsonConvert.DeserializeObject<T>(jsonStr);
		}

		//public static object FromJson(this Type type, string serializedObject)
		//{
		//	object filledObject = null;

		//	if (!string.IsNullOrEmpty(serializedObject))
		//	{
		//		DataContractJsonSerializer dcjs = new DataContractJsonSerializer(type);

		//		using (MemoryStream memStream = new MemoryStream())
		//		{
		//			StreamWriter writer = new StreamWriter(memStream, Encoding.Default);
		//			writer.Write(serializedObject);
		//			writer.Flush();

		//			memStream.Position = 0;

		//			try
		//			{
		//				filledObject = dcjs.ReadObject(memStream);
		//			}
		//			catch (SerializationException)
		//			{
		//				filledObject = null;
		//			}
		//		}
		//	}

		//	return filledObject;
		//}

		public static string ToNullableString(this object obj)
		{
			if (obj == null)
				return null;
			else
				return obj.ToString();
		}

		public static object DeepClone(this object obj)
		{
			if (obj == null)
				return null;

			using (MemoryStream memStream = new MemoryStream())
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter(null,
					 new StreamingContext(StreamingContextStates.Clone));
				binaryFormatter.Serialize(memStream, obj);
				memStream.Position = 0;
				return binaryFormatter.Deserialize(memStream);
			}
		}
	}
}
