using System;
using System.Collections.Generic;
using System.Linq;
using QuickSkin.Core.Interfaces;
using QuickSkin.Models;

namespace QuickSkin.Common.Services.DataBases;

public class ReleaseItemRepository : IDatabaseRepository<ReleaseItemModel>
{
    private readonly DatabaseService _db;

    public ReleaseItemRepository(string tableName, string path)
    {
        _db = new DatabaseService(path);

        _db.CreateTable(tableName,
            $"{nameof(ReleaseItemModel.Name)} TEXT PRIMARY KEY, " +
            $"{nameof(ReleaseItemModel.Description)} TEXT, " +
            $"{nameof(ReleaseItemModel.IconId)} TEXT");

        TableName = tableName;
    }

    public string TableName { get; }

    public int Count()
    {
        var rows = _db.Query($"SELECT COUNT(*) as cnt FROM {TableName}");

        if (rows.Count > 0 && rows[0]["cnt"] != null)
        {
            return Convert.ToInt32(rows[0]["cnt"]);
        }

        return 0;
    }

    public void Insert(ReleaseItemModel model)
    {
        var data = new Dictionary<string, object?>
        {
            [nameof(ReleaseItemModel.Name)] = model.Name,
            [nameof(ReleaseItemModel.Description)] = model.Description,
            [nameof(ReleaseItemModel.IconId)] = model.IconId,
        };

        _db.Insert(TableName, data);
    }

    public void Update(ReleaseItemModel model)
    {
        var data = new Dictionary<string, object?>
        {
            [nameof(ReleaseItemModel.Description)] = model.Description,
            [nameof(ReleaseItemModel.IconId)] = model.IconId,
        };

        _db.Update(TableName, data, nameof(ReleaseItemModel.Name), new Dictionary<string, object?>
        {
            [nameof(ReleaseItemModel.Name)] = model.Name,
        });
    }

    public void Update(string id, string[] fields, string?[] values)
    {
        if (fields == null || values == null || fields.Length != values.Length || string.IsNullOrEmpty(id))
            throw new ArgumentException("字段与数值数量不一致，或id为空");

        var data = new Dictionary<string, object?>();

        for (int i = 0; i < fields.Length; i++)
        {
            data[fields[i]] = values[i];
        }

        _db.Update(
            TableName,
            data,
            $"{nameof(ReleaseItemModel.Name)} = @{nameof(ReleaseItemModel.Name)}",
            new Dictionary<string, object?>
            {
                [nameof(ReleaseItemModel.Name)] = id,
            }
        );
    }

    public ReleaseItemModel? Get(string id)
    {
        var rows = _db.Query($"SELECT * FROM {TableName} WHERE {nameof(ReleaseItemModel.Name)} = @{nameof(ReleaseItemModel.Name)}",
            new Dictionary<string, object>
            {
                [nameof(ReleaseItemModel.Name)] = id,
            });

        if (rows.Count == 0)
            return null;

        var row = rows[0];

        return new ReleaseItemModel
        {
            Name = row[nameof(ReleaseItemModel.Name)]?.ToString() ?? throw new InvalidOperationException(),
            Description = row[nameof(ReleaseItemModel.Description)]?.ToString(),
            IconId = row[nameof(ReleaseItemModel.IconId)]?.ToString(),
        };
    }

    public IEnumerable<ReleaseItemModel> GetAll()
    {
        var rows = _db.Query($"SELECT * FROM {TableName}");

        return rows.Select(row => new ReleaseItemModel
            {
                Name = row[nameof(ReleaseItemModel.Name)]?.ToString() ?? throw new InvalidOperationException(),
                Description = row[nameof(ReleaseItemModel.Description)]?.ToString(),
                IconId = row[nameof(ReleaseItemModel.IconId)]?.ToString(),
            })
            .ToList();
    }

    public void Delete(string value)
    {
        _db.Delete(TableName, $"{nameof(ReleaseItemModel.Name)} = @{nameof(ReleaseItemModel.Name)}", new Dictionary<string, object>
        {
            [nameof(ReleaseItemModel.Name)] = value,
        });
    }

    public bool Exists(string value)
    {
        var rows = _db.Query($"SELECT 1 FROM {TableName} WHERE {nameof(ReleaseItemModel.Name)} = @{nameof(ReleaseItemModel.Name)}", new Dictionary<string, object>
        {
            [nameof(ReleaseItemModel.Name)] = value,
        });

        return rows.Count > 0;
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }
}
