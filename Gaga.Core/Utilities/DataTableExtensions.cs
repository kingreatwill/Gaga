using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace Gaga.Core.Utilities
{
	public static  class DataTableExtensions
	{
		//public static List<dynamic> ToDynamic(this DataTable dt)
		//{
		//	var dynamicDt = new List<dynamic>();
		//	foreach (DataRow row in dt.Rows)
		//	{
		//		dynamic dyn = new ExpandoObject();
		//		foreach (DataColumn column in dt.Columns)
		//		{
		//			var dic = (IDictionary<string, object>)dyn;
		//			dic[column.ColumnName] = row[column];
		//			dynamicDt.Add(dyn);
		//		}
		//	}
		//	return dynamicDt;
		//}


		#region "Convert DataTable to List<dynamic>"

		public static List<dynamic> ToDynamicList(this DataTable dt)
		{
			List<string> cols = (dt.Columns.Cast<DataColumn>()).Select(column => column.ColumnName).ToList();
			return ToDynamicList(ToDictionary(dt), getNewObject(cols));
		}
		public static List<Dictionary<string, object>> ToDictionary(DataTable dt)
		{
			var columns = dt.Columns.Cast<DataColumn>();
			var Temp = dt.AsEnumerable().Select(dataRow => columns.Select(column =>
								 new { Column = column.ColumnName, Value = dataRow[column] })
							 .ToDictionary(data => data.Column, data => data.Value)).ToList();
			return Temp.ToList();
		}
		public static List<dynamic> ToDynamicList(List<Dictionary<string, object>> list, Type TypeObj)
		{
			dynamic temp = new List<dynamic>();
			foreach (Dictionary<string, object> step in list)
			{
				object Obj = Activator.CreateInstance(TypeObj);
				PropertyInfo[] properties = Obj.GetType().GetProperties();
				Dictionary<string, object> DictList = (Dictionary<string, object>)step;
				foreach (KeyValuePair<string, object> keyValuePair in DictList)
				{
					foreach (PropertyInfo property in properties)
					{
						if (property.Name == keyValuePair.Key)
						{
							property.SetValue(Obj, keyValuePair.Value.ToString(), null);
							break;
						}
					}
				}
				temp.Add(Obj);
			}
			return temp;
		}
		private static Type getNewObject(List<string> list)
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = "tmpAssembly";
			AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			ModuleBuilder module = assemblyBuilder.DefineDynamicModule("tmpModule");
			TypeBuilder typeBuilder = module.DefineType("WebgridRowCellCollection", TypeAttributes.Public);
			foreach (string step in list)
			{
				string propertyName = step;
				FieldBuilder field = typeBuilder.DefineField(propertyName, typeof(string), FieldAttributes.Public);
				PropertyBuilder property = typeBuilder.DefineProperty(propertyName, System.Reflection.PropertyAttributes.None, typeof(string), new Type[] { typeof(string) });
				MethodAttributes GetSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig;
				MethodBuilder currGetPropMthdBldr = typeBuilder.DefineMethod("get_value", GetSetAttr, typeof(string), Type.EmptyTypes);
				ILGenerator currGetIL = currGetPropMthdBldr.GetILGenerator();
				currGetIL.Emit(OpCodes.Ldarg_0);
				currGetIL.Emit(OpCodes.Ldfld, field);
				currGetIL.Emit(OpCodes.Ret);
				MethodBuilder currSetPropMthdBldr = typeBuilder.DefineMethod("set_value", GetSetAttr, null, new Type[] { typeof(string) });
				ILGenerator currSetIL = currSetPropMthdBldr.GetILGenerator();
				currSetIL.Emit(OpCodes.Ldarg_0);
				currSetIL.Emit(OpCodes.Ldarg_1);
				currSetIL.Emit(OpCodes.Stfld, field);
				currSetIL.Emit(OpCodes.Ret);
				property.SetGetMethod(currGetPropMthdBldr);
				property.SetSetMethod(currSetPropMthdBldr);
			}
			Type obj = typeBuilder.CreateType();
			return obj;
		}

		#endregion
	}

	public static class DataTableX
	{
		public static IEnumerable<dynamic> AsDynamicEnumerable(this DataTable table)
		{
			// Validate argument here..

			return table.AsEnumerable().Select(row => new DynamicRow(row));
		}

		private sealed class DynamicRow : DynamicObject
		{
			private readonly DataRow _row;

			internal DynamicRow(DataRow row) { _row = row; }

			// Interprets a member-access as an indexer-access on the 
			// contained DataRow.
			public override bool TryGetMember(GetMemberBinder binder, out object result)
			{
				var retVal = _row.Table.Columns.Contains(binder.Name);
				result = retVal ? _row[binder.Name] : null;
				return retVal;
			}
		}
	}
}
