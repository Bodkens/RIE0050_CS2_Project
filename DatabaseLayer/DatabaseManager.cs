﻿using DatabaseLayer;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Reflection;

namespace DatabaseLayer
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class KeyAttribute : Attribute
    {
        public KeyAttribute() { }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OrderAttribute : Attribute
    {
        public int Order { get; set; }
        public OrderAttribute(int order) { this.Order = order; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AttributeNameAttribute : Attribute
    {
        public string Name { get; set; }
        public AttributeNameAttribute(string name) { this.Name = name; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableNameAttribute : Attribute
    {
        public string Name { get; set; }
        public TableNameAttribute(string name) { this.Name = name; }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IgnoreAttribute : Attribute
    {

        public IgnoreAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ForeignKeyAttribute : Attribute
    {
        public string AttributeName { get; set; }
        public string PropertyName { get; set; }
        public ForeignKeyAttribute(string attribute_name, string property_name) { this.AttributeName = attribute_name; this.PropertyName = property_name; }
    }


    public class KeyNotDefinedException : Exception
    {
        public KeyNotDefinedException() { }
    }

    public abstract class DatabaseManager
    {
        private static string connectionString = @"Data Source=../../../../Database/database.db;";

        public static string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }


        public static void Insert<T>(T obj)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {

                using (SqliteCommand command = new SqliteCommand("", connection))
                {

                    var properties = typeof(T).GetProperties().Where(p => !Attribute.IsDefined(p, typeof(IgnoreAttribute))).OrderBy(p => (p.GetCustomAttribute(typeof(OrderAttribute), true) as OrderAttribute)?.Order).ToArray();
                    string parametersLine = "";

                    string attributesLine = "";

                    for (int i = 0; i < properties.Length; i++)
                    {


                        if (Attribute.IsDefined(properties[i], typeof(KeyAttribute)))
                        {
                            continue;
                        }

                        var foreignKeyAttribute = properties[i].GetCustomAttribute(typeof(ForeignKeyAttribute)) as ForeignKeyAttribute;

                        if (foreignKeyAttribute != null)
                        {


                            var keyProp = properties[i].PropertyType.GetProperty(foreignKeyAttribute.PropertyName);

                            if (properties[i].GetValue(obj) == null)
                            {
                                keyProp = null;
                            }


                            command.Parameters.AddWithValue($"@{i}",
                            keyProp?.GetValue(properties[i].GetValue(obj)) == null ? DBNull.Value : keyProp?.GetValue(properties[i].GetValue(obj)));

                        }

                        else
                        {
                            command.Parameters.AddWithValue($"@{i}",
                                properties[i]?.GetValue(obj) == null ? DBNull.Value : properties[i].GetValue(obj));
                        }



                        string attributeName = properties[i].Name;

                        string? customAttributeName = (properties[i].GetCustomAttribute(typeof(AttributeNameAttribute), true) as AttributeNameAttribute)?.Name;

                        if (customAttributeName != null)
                        {
                            attributeName = customAttributeName;
                        }

                        if (foreignKeyAttribute != null)
                        {
                            attributeName = foreignKeyAttribute.AttributeName;
                        }

                        if (i == properties.Length - 1)
                        {
                            parametersLine += $" @{i}";
                            attributesLine += $" {attributeName}";
                        }

                        else
                        {
                            parametersLine += $" @{i},";
                            attributesLine += $" {attributeName}, ";
                        }
                    }

                    string tableName = typeof(T).Name;

                    string? customTableName = (typeof(T).GetCustomAttribute(typeof(TableNameAttribute), true) as TableNameAttribute)?.Name;

                    if (customTableName != null)
                    {
                        tableName = customTableName;
                    }

                    connection.Open();

                    command.CommandText = $"INSERT INTO {tableName}({attributesLine}) VALUES({parametersLine})";

                    command.ExecuteNonQuery();
                }

            }
        }

        public static async Task InsertAsync<T>(T obj)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {

                using (SqliteCommand command = new SqliteCommand("", connection))
                {

                    var properties = typeof(T).GetProperties().Where(p => !Attribute.IsDefined(p, typeof(IgnoreAttribute))).OrderBy(p => (p.GetCustomAttribute(typeof(OrderAttribute), true) as OrderAttribute)?.Order).ToArray();
                    string parametersLine = "";

                    string attributesLine = "";

                    for (int i = 0; i < properties.Length; i++)
                    {


                        if (Attribute.IsDefined(properties[i], typeof(KeyAttribute)))
                        {
                            continue;
                        }

                        var foreignKeyAttribute = properties[i].GetCustomAttribute(typeof(ForeignKeyAttribute)) as ForeignKeyAttribute;

                        if (foreignKeyAttribute != null)
                        {


                            var keyProp = properties[i].PropertyType.GetProperty(foreignKeyAttribute.PropertyName);

                            if (properties[i].GetValue(obj) == null)
                            {
                                keyProp = null;
                            }


                            command.Parameters.AddWithValue($"@{i}",
                            keyProp?.GetValue(properties[i].GetValue(obj)) == null ? DBNull.Value : keyProp?.GetValue(properties[i].GetValue(obj)));

                        }

                        else
                        {
                            command.Parameters.AddWithValue($"@{i}",
                                properties[i]?.GetValue(obj) == null ? DBNull.Value : properties[i].GetValue(obj));
                        }



                        string attributeName = properties[i].Name;

                        string? customAttributeName = (properties[i].GetCustomAttribute(typeof(AttributeNameAttribute), true) as AttributeNameAttribute)?.Name;

                        if (customAttributeName != null)
                        {
                            attributeName = customAttributeName;
                        }

                        if (foreignKeyAttribute != null)
                        {
                            attributeName = foreignKeyAttribute.AttributeName;
                        }

                        if (i == properties.Length - 1)
                        {
                            parametersLine += $" @{i}";
                            attributesLine += $" {attributeName}";
                        }

                        else
                        {
                            parametersLine += $" @{i},";
                            attributesLine += $" {attributeName}, ";
                        }
                    }

                    string tableName = typeof(T).Name;

                    string? customTableName = (typeof(T).GetCustomAttribute(typeof(TableNameAttribute), true) as TableNameAttribute)?.Name;

                    if (customTableName != null)
                    {
                        tableName = customTableName;
                    }
                    connection.Open();

                    command.CommandText = $"INSERT INTO {tableName}({attributesLine}) VALUES({parametersLine})";

                    await command.ExecuteNonQueryAsync();
                }

            }
        }

        public static List<T> Select<T>(string sql, Dictionary<string, object?>? parameters = null)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                List<T> selected = new List<T>();
                using (SqliteCommand command = new SqliteCommand(sql, connection))
                {
                    connection.Open();

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters.Keys)
                        {
                            command.Parameters.AddWithValue(parameter, parameters[parameter] != null ? parameters[parameter] : DBNull.Value);
                        }
                    }


                    using SqliteDataReader reader = command.ExecuteReader();

                    var properties = typeof(T).GetProperties().Where(p => !Attribute.IsDefined(p, typeof(IgnoreAttribute))).OrderBy(p => (p.GetCustomAttribute(typeof(OrderAttribute), true) as OrderAttribute)?.Order).ToArray();

                    while (reader.Read())
                    {
                        List<object?> constructorArguments = new List<object?>();


                        foreach (var property in properties)
                        {

                            string attributeName = property.Name;

                            string? customName = (property.GetCustomAttribute(typeof(AttributeNameAttribute), true) as AttributeNameAttribute)?.Name;

                            if (customName != null)
                            {
                                attributeName = customName;
                            }
                            var foreignKeyAttribute = property.GetCustomAttribute(typeof(ForeignKeyAttribute)) as ForeignKeyAttribute;

                            if (foreignKeyAttribute != null)
                            {
                                attributeName = foreignKeyAttribute.AttributeName;
                            }

                            if (reader.GetValue(reader.GetOrdinal(attributeName)).GetType().Name == "Int64")
                            {

                                constructorArguments.Add(reader.GetInt32(reader.GetOrdinal(attributeName)));

                            }

                            else if (reader.GetValue(reader.GetOrdinal(attributeName)).GetType().Name == "DBNull")
                            {
                                constructorArguments.Add(null);
                            }

                            else
                            {
                                constructorArguments.Add(reader.GetValue(reader.GetOrdinal(attributeName)));
                            }
                        }


                        object?[] arr = constructorArguments.ToArray<object?>();


                        var instance = Activator.CreateInstance(typeof(T), arr);

                        if (instance != null)
                        {
                            selected.Add((T)instance);
                        }

                    }
                }

                return selected;

            }
        }

        public static async Task<List<T>> SelectAsync<T>(string sql, Dictionary<string, object?>? parameters = null)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                List<T> selected = new List<T>();
                using (SqliteCommand command = new SqliteCommand(sql, connection))
                {
                    connection.Open();

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters.Keys)
                        {
                            command.Parameters.AddWithValue(parameter, parameters[parameter] != null ? parameters[parameter] : DBNull.Value);
                        }
                    }


                    using SqliteDataReader reader = await command.ExecuteReaderAsync();

                    var properties = typeof(T).GetProperties().Where(p => !Attribute.IsDefined(p, typeof(IgnoreAttribute))).OrderBy(p => (p.GetCustomAttribute(typeof(OrderAttribute), true) as OrderAttribute)?.Order).ToArray();

                    while (reader.Read())
                    {
                        List<object?> constructorArguments = new List<object?>();


                        foreach (var property in properties)
                        {

                            string attributeName = property.Name;

                            string? customName = (property.GetCustomAttribute(typeof(AttributeNameAttribute), true) as AttributeNameAttribute)?.Name;

                            if (customName != null)
                            {
                                attributeName = customName;
                            }
                            var foreignKeyAttribute = property.GetCustomAttribute(typeof(ForeignKeyAttribute)) as ForeignKeyAttribute;

                            if (foreignKeyAttribute != null)
                            {
                                attributeName = foreignKeyAttribute.AttributeName;
                            }

                            if (reader.GetValue(reader.GetOrdinal(attributeName)).GetType().Name == "Int64")
                            {

                                constructorArguments.Add(reader.GetInt32(reader.GetOrdinal(attributeName)));

                            }

                            else if (reader.GetValue(reader.GetOrdinal(attributeName)).GetType().Name == "DBNull")
                            {
                                constructorArguments.Add(null);
                            }

                            else
                            {
                                constructorArguments.Add(reader.GetValue(reader.GetOrdinal(attributeName)));
                            }
                        }


                        object?[] arr = constructorArguments.ToArray<object?>();


                        var instance = Activator.CreateInstance(typeof(T), arr);

                        if (instance != null)
                        {
                            selected.Add((T)instance);
                        }

                    }
                }

                return selected;

            }
        }


        public static List<T> SelectAll<T>()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {

                List<T> selected = new List<T>();

                string tableName = typeof(T).Name;

                string? customTableName = (typeof(T).GetCustomAttribute(typeof(TableNameAttribute), true) as TableNameAttribute)?.Name;

                if (customTableName != null)
                {
                    tableName = customTableName;
                }

                using (SqliteCommand command = new SqliteCommand($"SELECT * FROM {tableName}", connection))
                {
                    connection.Open();

                    using SqliteDataReader reader = command.ExecuteReader();

                    var properties = typeof(T).GetProperties().Where(p => !Attribute.IsDefined(p, typeof(IgnoreAttribute))).OrderBy(p => (p.GetCustomAttribute(typeof(OrderAttribute), true) as OrderAttribute)?.Order).ToArray();

                    while (reader.Read())
                    {
                        List<object?> constructorArguments = new List<object?>();


                        foreach (var property in properties)
                        {

                            string attributeName = property.Name;

                            string? customName = (property.GetCustomAttribute(typeof(AttributeNameAttribute), true) as AttributeNameAttribute)?.Name;

                            if (customName != null)
                            {
                                attributeName = customName;
                            }
                            var foreignKeyAttribute = property.GetCustomAttribute(typeof(ForeignKeyAttribute)) as ForeignKeyAttribute;

                            if (foreignKeyAttribute != null)
                            {
                                attributeName = foreignKeyAttribute.AttributeName;
                            }

                            if (reader.GetValue(reader.GetOrdinal(attributeName)).GetType().Name == "Int64")
                            {

                                constructorArguments.Add(reader.GetInt32(reader.GetOrdinal(attributeName)));

                            }

                            else if (reader.GetValue(reader.GetOrdinal(attributeName)).GetType().Name == "DBNull")
                            {
                                constructorArguments.Add(null);
                            }

                            else
                            {
                                constructorArguments.Add(reader.GetValue(reader.GetOrdinal(attributeName)));
                            }
                        }


                        object?[] arr = constructorArguments.ToArray<object?>();


                        var instance = Activator.CreateInstance(typeof(T), arr);

                        if (instance != null)
                        {
                            selected.Add((T)instance);
                        }

                    }
                }

                return selected;
            }

        }

        public static async Task<List<T>> SelectAllAsync<T>()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {

                List<T> selected = new List<T>();

                string tableName = typeof(T).Name;

                string? customTableName = (typeof(T).GetCustomAttribute(typeof(TableNameAttribute), true) as TableNameAttribute)?.Name;

                if (customTableName != null)
                {
                    tableName = customTableName;
                }

                using (SqliteCommand command = new SqliteCommand($"SELECT * FROM {tableName}", connection))
                {
                    connection.Open();

                    using SqliteDataReader reader = await command.ExecuteReaderAsync();

                    var properties = typeof(T).GetProperties().Where(p => !Attribute.IsDefined(p, typeof(IgnoreAttribute))).OrderBy(p => (p.GetCustomAttribute(typeof(OrderAttribute), true) as OrderAttribute)?.Order).ToArray();

                    while (reader.Read())
                    {
                        List<object?> constructorArguments = new List<object?>();


                        foreach (var property in properties)
                        {

                            string attributeName = property.Name;

                            string? customName = (property.GetCustomAttribute(typeof(AttributeNameAttribute), true) as AttributeNameAttribute)?.Name;

                            if (customName != null)
                            {
                                attributeName = customName;
                            }
                            var foreignKeyAttribute = property.GetCustomAttribute(typeof(ForeignKeyAttribute)) as ForeignKeyAttribute;

                            if (foreignKeyAttribute != null)
                            {
                                attributeName = foreignKeyAttribute.AttributeName;
                            }

                            if (reader.GetValue(reader.GetOrdinal(attributeName)).GetType().Name == "Int64")
                            {

                                constructorArguments.Add(reader.GetInt32(reader.GetOrdinal(attributeName)));

                            }

                            else if (reader.GetValue(reader.GetOrdinal(attributeName)).GetType().Name == "DBNull")
                            {
                                constructorArguments.Add(null);
                            }

                            else
                            {
                                constructorArguments.Add(reader.GetValue(reader.GetOrdinal(attributeName)));
                            }
                        }


                        object?[] arr = constructorArguments.ToArray<object?>();


                        var instance = Activator.CreateInstance(typeof(T), arr);

                        if (instance != null)
                        {
                            selected.Add((T)instance);
                        }

                    }
                }

                return selected;
            }

        }


        public static void Delete<T>(T obj)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {

                var properties = typeof(T).GetProperties().Where(p => !Attribute.IsDefined(p, typeof(IgnoreAttribute))).OrderBy(p => (p.GetCustomAttribute(typeof(OrderAttribute), true) as OrderAttribute)?.Order).ToArray();



                PropertyInfo? key = null;

                foreach (var property in properties)
                {

                    if (Attribute.IsDefined(property, typeof(KeyAttribute)))
                    {
                        key = property;
                    }
                }

                if (key == null)
                {
                    throw new KeyNotDefinedException();
                }
                string keyName = key.Name;

                string? customKeyName = (key.GetCustomAttribute(typeof(AttributeNameAttribute), true) as AttributeNameAttribute)?.Name;

                if (customKeyName != null)
                {
                    keyName = customKeyName;
                }

                string tableName = typeof(T).Name;

                string? customTableName = (typeof(T).GetCustomAttribute(typeof(TableNameAttribute), true) as TableNameAttribute)?.Name;

                if (customTableName != null)
                {
                    tableName = customTableName;
                }
                connection.Open();

                using (SqliteCommand command = new SqliteCommand($"DELETE FROM {tableName} WHERE {keyName} = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", key.GetValue(obj) == null ? DBNull.Value : key.GetValue(obj));
                    command.ExecuteNonQuery();
                }
            }
        }

        public static async Task DeleteAsync<T>(T obj)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {

                var properties = typeof(T).GetProperties().Where(p => !Attribute.IsDefined(p, typeof(IgnoreAttribute))).OrderBy(p => (p.GetCustomAttribute(typeof(OrderAttribute), true) as OrderAttribute)?.Order).ToArray();



                PropertyInfo? key = null;

                foreach (var property in properties)
                {

                    if (Attribute.IsDefined(property, typeof(KeyAttribute)))
                    {
                        key = property;
                    }
                }

                if (key == null)
                {
                    throw new KeyNotDefinedException();
                }
                string keyName = key.Name;

                string? customKeyName = (key.GetCustomAttribute(typeof(AttributeNameAttribute), true) as AttributeNameAttribute)?.Name;

                if (customKeyName != null)
                {
                    keyName = customKeyName;
                }


                connection.Open();

                string tableName = typeof(T).Name;

                string? customTableName = (typeof(T).GetCustomAttribute(typeof(TableNameAttribute), true) as TableNameAttribute)?.Name;

                if (customTableName != null)
                {
                    tableName = customTableName;
                }

                using (SqliteCommand command = new SqliteCommand($"DELETE FROM {tableName} WHERE {keyName} = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", key.GetValue(obj) == null ? DBNull.Value : key.GetValue(obj));
                    await command.ExecuteNonQueryAsync();
                }
            }
        }



        public static void Update<T>(T obj)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                string tableName = typeof(T).Name;

                string? customTableName = (typeof(T).GetCustomAttribute(typeof(TableNameAttribute), true) as TableNameAttribute)?.Name;

                if (customTableName != null)
                {
                    tableName = customTableName;
                }
                using (SqliteCommand command = new SqliteCommand($"UPDATE {tableName} SET ", connection))
                {
                    PropertyInfo? key = null;

                    var properties = typeof(T).GetProperties().Where(p => !Attribute.IsDefined(p, typeof(IgnoreAttribute))).OrderBy(p => (p.GetCustomAttribute(typeof(OrderAttribute), true) as OrderAttribute)?.Order).ToArray();



                    for (int i = 0; i < properties.Length; i++)
                    {

                        string attributeName = properties[i].Name;

                        string? customAttributeName = (properties[i].GetCustomAttribute(typeof(AttributeNameAttribute), true) as AttributeNameAttribute)?.Name;

                        if (customAttributeName != null)
                        {
                            attributeName = customAttributeName;
                        }

                        if (Attribute.IsDefined(properties[i], typeof(KeyAttribute)))
                        {
                            key = properties[i];
                            continue;
                        }

                        var foreignKeyAttribute = properties[i].GetCustomAttribute(typeof(ForeignKeyAttribute)) as ForeignKeyAttribute;

                        if (foreignKeyAttribute != null)
                        {
                            attributeName = foreignKeyAttribute.AttributeName;

                        }


                        if (i == properties.Length - 1)
                        {
                            command.CommandText += $" {attributeName} = @{i}";
                        }
                        else
                        {
                            command.CommandText += $" {attributeName} = @{i}, ";
                        }

                        if (Attribute.IsDefined(properties[i], typeof(ForeignKeyAttribute)))
                        {

                            if (foreignKeyAttribute != null)
                            {
                                var keyProp = properties[i].PropertyType.GetProperty(foreignKeyAttribute.PropertyName);

                                if (properties[i].GetValue(obj) == null)
                                {
                                    keyProp = null;
                                }


                                command.Parameters.AddWithValue($"@{i}",
                                keyProp?.GetValue(properties[i].GetValue(obj)) == null ? DBNull.Value : keyProp?.GetValue(properties[i].GetValue(obj)));
                            }

                        }
                        else
                        {
                            command.Parameters.AddWithValue($"@{i}",
                                properties[i]?.GetValue(obj) == null ? DBNull.Value : properties[i].GetValue(obj));
                        }

                    }



                    if (key == null)
                    {
                        throw new KeyNotDefinedException();
                    }

                    string keyName = key.Name;

                    string? customKeyName = (key.GetCustomAttribute(typeof(AttributeNameAttribute), true) as AttributeNameAttribute)?.Name;

                    if (customKeyName != null)
                    {
                        keyName = customKeyName;
                    }

                    command.CommandText += $" WHERE {keyName} = @k";

                    command.Parameters.AddWithValue("@k", key.GetValue(obj) == null ? DBNull.Value : key.GetValue(obj));

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        public static async Task UpdateAsync<T>(T obj)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                string tableName = typeof(T).Name;

                string? customTableName = (typeof(T).GetCustomAttribute(typeof(TableNameAttribute), true) as TableNameAttribute)?.Name;

                if (customTableName != null)
                {
                    tableName = customTableName;
                }

                using (SqliteCommand command = new SqliteCommand($"UPDATE {tableName} SET ", connection))
                {
                    PropertyInfo? key = null;

                    var properties = typeof(T).GetProperties().Where(p => !Attribute.IsDefined(p, typeof(IgnoreAttribute))).OrderBy(p => (p.GetCustomAttribute(typeof(OrderAttribute), true) as OrderAttribute)?.Order).ToArray();



                    for (int i = 0; i < properties.Length; i++)
                    {

                        string attributeName = properties[i].Name;

                        string? customAttributeName = (properties[i].GetCustomAttribute(typeof(AttributeNameAttribute), true) as AttributeNameAttribute)?.Name;

                        if (customAttributeName != null)
                        {
                            attributeName = customAttributeName;
                        }

                        if (Attribute.IsDefined(properties[i], typeof(KeyAttribute)))
                        {
                            key = properties[i];
                            continue;
                        }

                        var foreignKeyAttribute = properties[i].GetCustomAttribute(typeof(ForeignKeyAttribute)) as ForeignKeyAttribute;

                        if (foreignKeyAttribute != null)
                        {
                            attributeName = foreignKeyAttribute.AttributeName;

                        }


                        if (i == properties.Length - 1)
                        {
                            command.CommandText += $" {attributeName} = @{i}";
                        }
                        else
                        {
                            command.CommandText += $" {attributeName} = @{i}, ";
                        }

                        if (Attribute.IsDefined(properties[i], typeof(ForeignKeyAttribute)))
                        {

                            if (foreignKeyAttribute != null)
                            {
                                var keyProp = properties[i].PropertyType.GetProperty(foreignKeyAttribute.PropertyName);

                                if (properties[i].GetValue(obj) == null)
                                {
                                    keyProp = null;
                                }


                                command.Parameters.AddWithValue($"@{i}",
                                keyProp?.GetValue(properties[i].GetValue(obj)) == null ? DBNull.Value : keyProp?.GetValue(properties[i].GetValue(obj)));
                            }

                        }
                        else
                        {
                            command.Parameters.AddWithValue($"@{i}",
                                properties[i]?.GetValue(obj) == null ? DBNull.Value : properties[i].GetValue(obj));
                        }

                    }



                    if (key == null)
                    {
                        throw new KeyNotDefinedException();
                    }

                    string keyName = key.Name;

                    string? customKeyName = (key.GetCustomAttribute(typeof(AttributeNameAttribute), true) as AttributeNameAttribute)?.Name;

                    if (customKeyName != null)
                    {
                        keyName = customKeyName;
                    }

                    command.CommandText += $" WHERE {keyName} = @k";

                    command.Parameters.AddWithValue("@k", key.GetValue(obj) == null ? DBNull.Value : key.GetValue(obj));

                    connection.Open();

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public static void ExecuteNonQuerry(string sql, Dictionary<string, object?>? parameters = null)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                using (SqliteCommand command = new SqliteCommand(sql, connection))
                {
                    connection.Open();
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters.Keys)
                        {
                            command.Parameters.AddWithValue(parameter, parameters[parameter] != null ? parameters[parameter] : DBNull.Value);
                        }
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public static async Task ExecuteNonQuerryAsync(string sql, Dictionary<string, object?>? parameters = null)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                using (SqliteCommand command = new SqliteCommand(sql, connection))
                {
                    connection.Open();
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters.Keys)
                        {
                            command.Parameters.AddWithValue(parameter, parameters[parameter] != null ? parameters[parameter] : DBNull.Value);
                        }
                    }

                    await command.ExecuteNonQueryAsync();
                }
            }
        }








    }

}