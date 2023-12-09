using System.Collections;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Basket.API.Attributes;
using StackExchange.Redis;

namespace Basket.API.Extensions;

public static class ObjectExtensions
{
    public static HashEntry[] ToHashEntries(this object obj)
    {
        var properties = obj
            .GetType()
            .GetProperties()
            .Where(property => property.CustomAttributes.All(attr => attr.AttributeType != typeof(IgnoreOnHashAttribute)));

        return properties
            .Where(property => property.GetValue(obj) is not null)
            .Select(property =>
            {
                var propertyValue = property.GetValue(obj)!;

                var hashValue = propertyValue is IEnumerable
                    ? JsonSerializer.Serialize(propertyValue)
                    : propertyValue.ToString();

                return new HashEntry(property.Name, hashValue);
            })
            .ToArray();
    }

    public static T ToObject<T>(this HashEntry[] entries)
    {
        var value= typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, Array.Empty<Type>())!.Invoke(Array.Empty<object>());
        
        var typeProperties = typeof(T).GetProperties();

        foreach (var entry in entries)
        {
            var property = typeProperties.FirstOrDefault(p => p.Name == entry.Name);
                
            if (property is null)
                continue;

            object propertyValue;

            if (entry.Value.StartsWith("{") || entry.Value.StartsWith("["))
                propertyValue = JsonSerializer.Deserialize(entry.Value.ToString(), property.PropertyType)!;
            else
                propertyValue = TypeDescriptor.GetConverter(property.PropertyType).ConvertFromString(entry.Value.ToString())!; 
            
            property.SetValue(
                value, 
                propertyValue,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                null,
                CultureInfo.InvariantCulture);
        }

        return (T)value;
    }
}