namespace TeamMetrics
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public class ConsoleUtil
	{
		public static DateTime ReadDateTime(string displayText, DateTime? defaultValue = null)
		{
			Console.Write(displayText);
			string input = Console.ReadLine();

			DateTime result;
			while (!DateTime.TryParse(input, out result))
			{
				if (defaultValue != null)
				{
					return defaultValue.Value;
				}

				Console.WriteLine("Invalid input. " + displayText);
				input = Console.ReadLine();
			}

			return result;
		}

		public static void PrintPropertyValues(TeamMetrics stats)
		{
			var properties = stats.GetType().GetProperties();

			foreach (var property in properties)
			{
				if (property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
				{
					Console.WriteLine("\n" + property.Name + ":");
					RenderEnumerableProperty(stats, property);
				}
				else
				{
					Console.WriteLine(property.Name + ": " + property.GetValue(stats));
				}
			}
		}

		private static void RenderEnumerableProperty(object obj, PropertyInfo property)
		{
			List<PropertyInfo> nestedProperties = null;
			int iteration = 0;
			Dictionary<string, Column> columns = null;

			foreach (var item in (IEnumerable)property.GetValue(obj, null))
			{
				iteration += 1;

				if (iteration == 1)
				{
					nestedProperties = item.GetType().GetProperties().ToList();
					columns = new Dictionary<string, Column>(nestedProperties.Count);

					foreach (var nestedProperty in nestedProperties)
					{
						var col = new Column(nestedProperty);
						columns.Add(col.Name, col);
					}

					var tableWidth = columns.Values.Sum(t => t.Width);
					var border = "\n" + new string('-', tableWidth) + "\n";

					Console.Write(border);

					foreach (var col in columns.Values)
					{
						Console.Write(col.Format, col.Name);
					}

					Console.Write(border);
				}

				// ReSharper disable once PossibleNullReferenceException
				foreach (var nestedProperty in nestedProperties)
				{
					var value = nestedProperty.GetValue(item);
					// ReSharper disable once PossibleNullReferenceException
					Console.Write(columns[nestedProperty.Name].Format, value);
				}

				Console.Write("\n");
			}
		}

		public static void WriteHeader(string displayText)
		{
			Console.WriteLine(displayText);
			Console.WriteLine(new string('-', displayText.Length));
		}

		private class Column
		{
			public Column(PropertyInfo property)
			{
				int extraSpace = 3;

				if (property.PropertyType == typeof(string))
				{
					extraSpace = 10;
				}

				this.Format += "{0," + (property.Name.Length + extraSpace) + "}";
				this.Name = property.Name;
				this.Width = extraSpace + property.Name.Length;
			}

			public string Format { get; }
			public string Name { get; }
			public int Width { get; }
		}
	}
}