using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace QuickSkin.Common.Services.DataBases;

/// <summary>
///     提供基于 Sqlite 的数据库操作服务，包括建表、删表、增删改查等常用功能。
/// </summary>
public class DatabaseService : IDisposable
{
    private readonly SqliteConnection _connection;

    /// <summary>
    ///     初始化数据库服务。
    /// </summary>
    /// <param name="dbPath">数据库文件路径</param>
    public DatabaseService(string dbPath)
    {
        string? dbDir = Path.GetDirectoryName(dbPath);

        if (!string.IsNullOrEmpty(dbDir) && !Directory.Exists(dbDir))
            Directory.CreateDirectory(dbDir);

        string connectionString = $"Data Source={dbPath}";
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
    }

    /// <summary>
    ///     释放数据库连接。
    /// </summary>
    public void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     创建表（如果不存在）。
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="columnsDefinition">字段定义，如："id INTEGER PRIMARY KEY, name TEXT"</param>
    public void CreateTable(string tableName, string columnsDefinition)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {tableName} ({columnsDefinition});";
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    ///     删除表（如果存在）。
    /// </summary>
    /// <param name="tableName">表名</param>
    public void DropTable(string tableName)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = $"DROP TABLE IF EXISTS {tableName};";
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    ///     插入一条数据。
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="data">字段名与值的字典</param>
    public void Insert(string tableName, Dictionary<string, object?> data)
    {
        if (data == null || data.Count == 0)
            throw new ArgumentException("插入数据不能为空", nameof(data));

        string columns = string.Join(", ", data.Keys);
        string paramNames = string.Join(", ", data.Keys.Select(k => "@" + k));
        using var cmd = _connection.CreateCommand();

        foreach (var kv in data)
        {
            cmd.Parameters.AddWithValue($"@{kv.Key}", kv.Value ?? DBNull.Value);
        }

        cmd.CommandText = $"INSERT INTO {tableName} ({columns}) VALUES ({paramNames});";
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    ///     更新数据。
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="data">要更新的字段名与值</param>
    /// <param name="whereClause">WHERE 子句（不含 WHERE 关键字）</param>
    /// <param name="whereParams">WHERE 子句参数</param>
    public void Update(
        string tableName,
        Dictionary<string, object?> data,
        string whereClause,
        Dictionary<string, object?>? whereParams = null
        )
    {
        if (data == null || data.Count == 0)
            throw new ArgumentException("更新数据不能为空", nameof(data));

        string setClause = string.Join(", ", data.Keys.Select(k => $"{k} = @{k}"));
        using var cmd = _connection.CreateCommand();

        foreach (var kv in data)
        {
            cmd.Parameters.AddWithValue($"@{kv.Key}", kv.Value ?? DBNull.Value);
        }

        if (whereParams != null)
        {
            foreach (var kv in whereParams)
            {
                cmd.Parameters.AddWithValue($"@{kv.Key}", kv.Value ?? DBNull.Value);
            }
        }

        cmd.CommandText = $"UPDATE {tableName} SET {setClause} WHERE {whereClause};";
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    ///     删除数据。
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="whereClause">WHERE 子句（不含 WHERE 关键字）</param>
    /// <param name="whereParams">WHERE 子句参数</param>
    public void Delete(string tableName, string whereClause, Dictionary<string, object>? whereParams = null)
    {
        using var cmd = _connection.CreateCommand();

        if (whereParams != null)
        {
            foreach (var kv in whereParams)
            {
                cmd.Parameters.AddWithValue($"@{kv.Key}", kv.Value);
            }
        }

        cmd.CommandText = $"DELETE FROM {tableName} WHERE {whereClause};";
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    ///     执行查询，返回结果集。
    /// </summary>
    /// <param name="sql">SQL 查询语句</param>
    /// <param name="parameters">参数字典</param>
    /// <returns>结果集，每行是一个字典</returns>
    public List<Dictionary<string, object?>> Query(string sql, Dictionary<string, object>? parameters = null)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = sql;

        if (parameters != null)
        {
            foreach (var kv in parameters)
            {
                cmd.Parameters.AddWithValue($"@{kv.Key}", kv.Value);
            }
        }

        var result = new List<Dictionary<string, object?>>();
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            var row = new Dictionary<string, object?>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }

            result.Add(row);
        }

        return result;
    }
}
