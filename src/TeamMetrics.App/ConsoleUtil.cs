namespace TeamMetrics.App
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

		public static void PrintObject(object stats)
		{
			var properties = stats.GetType().GetProperties();

			foreach (var property in properties)
			{
				if (property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)) && property.PropertyType != typeof(string))
				{
					Console.WriteLine("\n" + property.Name + ":");
					PrintEnumerableProperty(stats, property);
				}
				else
				{
					Console.WriteLine(property.Name + ": " + property.GetValue(stats));
				}
			}
		}

		public static void WriteHeader(string displayText)
		{
			Console.WriteLine(displayText);
			Console.WriteLine(new string('-', displayText.Length));
		}

		public static string ReadString(string displayText, string defaultValue = null)
		{
			Console.Write(displayText);
			string input = Console.ReadLine();

			while (string.IsNullOrWhiteSpace(input))
			{
				if (defaultValue != null)
				{
					return defaultValue;
				}

				Console.WriteLine("Invalid input. " + displayText);
				input = Console.ReadLine();
			}

			return input;
		}

		public static void PrintEnumerable(IEnumerable enumerable)
		{
			List<PropertyInfo> nestedProperties = null;
			int iteration = 0;
			Dictionary<string, Column> columns = null;

			foreach (var item in enumerable)
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

		private static void PrintEnumerableProperty(object obj, PropertyInfo property)
		{
			var enumerable = (IEnumerable)property.GetValue(obj, null);

			PrintEnumerable(enumerable);
		}

		private class Column
		{
			public Column(PropertyInfo property)
			{
				int extraSpace = 3;

				if (property.PropertyType == typeof(string))
				{
					extraSpace = 15;
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