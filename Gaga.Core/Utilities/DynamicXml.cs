﻿/*
string xml = @"<Students>
                <Student ID=""100"">
                    <Name>Arul</Name>
                    <Mark>90</Mark>
                </Student>
                <Student>
                    <Name>Arul2</Name>
                    <Mark>80</Mark>
                </Student>
            </Students>";

dynamic students = DynamicXml.Parse(xml);

var id = students.Student[0].ID;
var name1 = students.Student[1].Name;

foreach(var std in students.Student)
{
    Console.WriteLine(std.Mark);
}
*/
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace Gaga.Core.Utilities
{
	public class DynamicXml : DynamicObject
	{
		XElement _root;
		private DynamicXml(XElement root)
		{
			_root = root;
		}

		public static DynamicXml Parse(string xmlString)
		{
			return new DynamicXml(XDocument.Parse(xmlString).Root);
		}

		public static DynamicXml Load(string filename)
		{
			return new DynamicXml(XDocument.Load(filename).Root);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			result = null;

			var att = _root.Attribute(binder.Name);
			if (att != null)
			{
				result = att.Value;
				return true;
			}

			var nodes = _root.Elements(binder.Name);
			if (nodes.Count() > 1)
			{
				result = nodes.Select(n => new DynamicXml(n)).ToList();
				return true;
			}

			var node = _root.Element(binder.Name);
			if (node != null)
			{
				if (node.HasElements)
				{
					result = new DynamicXml(node);
				}
				else
				{
					result = node.Value;
				}
				return true;
			}

			return true;
		}
	}
}
