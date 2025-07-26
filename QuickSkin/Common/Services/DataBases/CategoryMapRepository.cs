using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using QuickSkin.Core.Interfaces;
using QuickSkin.Models;

namespace QuickSkin.Common.Services.DataBases;

public class CategoryMapRepository : IDatabaseRepository<CategoryItem>
{
    private const string TABLE_NAME = "CategoryMap";
    private readonly DatabaseService _db;

    public CategoryMapRepository(string dbPath)
    {
        _db = new DatabaseService(dbPath);

        _db.CreateTable(TABLE_NAME,
            $"{nameof(CategoryItem.Id)} TEXT PRIMARY KEY, " +
            $"{nameof(CategoryItem.Name)} TEXT, " +
            $"{nameof(CategoryItem.IconKey)} TEXT, " +
            $"{nameof(CategoryItem.IconBrush)} TEXT, " +
            $"{nameof(CategoryItem.Description)} TEXT");
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    public void Insert(CategoryItem item)
    {
        var data = new Dictionary<string, object?>
        {
            [nameof(CategoryItem.Id)] = item.Id,
            [nameof(CategoryItem.Name)] = item.Name,
            [nameof(CategoryItem.IconKey)] = item.IconKey,
            [nameof(CategoryItem.IconBrush)] = item.IconBrush?.ToString(),
            [nameof(CategoryItem.Description)] = item.Description,
        };

        _db.Insert(TABLE_NAME, data);
    }

    public int Count()
    {
        var rows = _db.Query($"SELECT COUNT(*) as cnt FROM {TABLE_NAME}");

        if (rows.Count > 0 && rows[0]["cnt"] != null)
        {
            return Convert.ToInt32(rows[0]["cnt"]);
        }

        return 0;
    }

    public CategoryItem? Get(string id)
    {
        var rows = _db.Query($"SELECT * FROM {TABLE_NAME} WHERE {nameof(CategoryItem.Id)} = @{nameof(CategoryItem.Id)}",
            new Dictionary<string, object>
            {
                [nameof(CategoryItem.Id)] = id,
            });

        if (rows.Count == 0)
            return null;

        var row = rows[0];

        var item = new CategoryItem
        {
            Id = row[nameof(CategoryItem.Id)]?.ToString() ?? throw new InvalidOperationException(),
            Name = row[nameof(CategoryItem.Name)]?.ToString() ?? string.Empty,
            IconKey = row[nameof(CategoryItem.IconKey)]?.ToString(),
            IconBrush = row[nameof(CategoryItem.IconBrush)] is string brushStr ? SolidColorBrush.Parse(brushStr) : null,
            Description = row[nameof(CategoryItem.Description)]?.ToString(),
        };

        if (item.IconKey != null)
        {
            item.Icon = ResourceAccessor.Get<Geometry>(item.IconKey);
        }

        return item;
    }

    public IEnumerable<CategoryItem> GetAll()
    {
        var rows = _db.Query($"SELECT * FROM {TABLE_NAME}");

        return rows.Select(row =>
        {
            var item = new CategoryItem
            {
                Id = row[nameof(CategoryItem.Id)]?.ToString() ?? throw new InvalidOperationException(),
                Name = row[nameof(CategoryItem.Name)]?.ToString() ?? string.Empty,
                IconKey = row[nameof(CategoryItem.IconKey)]?.ToString(),
                IconBrush = row[nameof(CategoryItem.IconBrush)] is string brushStr ? SolidColorBrush.Parse(brushStr) : null,
                Description = row[nameof(CategoryItem.Description)]?.ToString(),
            };

            if (item.IconKey != null)
            {
                item.Icon = ResourceAccessor.Get<Geometry>(item.IconKey);
            }

            return item;
        }).ToList();
    }

    public bool Exists(string id)
    {
        var rows = _db.Query($"SELECT 1 FROM {TABLE_NAME} WHERE {nameof(CategoryItem.Id)} = @{nameof(CategoryItem.Id)}", new Dictionary<string, object>
        {
            [nameof(CategoryItem.Id)] = id,
        });

        return rows.Count > 0;
    }

    public void Update(CategoryItem item)
    {
        var data = new Dictionary<string, object?>
        {
            [nameof(CategoryItem.Name)] = item.Name,
            [nameof(CategoryItem.IconKey)] = item.IconKey,
            [nameof(CategoryItem.IconBrush)] = item.IconBrush?.ToString(),
            [nameof(CategoryItem.Description)] = item.Description,
        };

        _db.Update(TABLE_NAME, data, $"{nameof(CategoryItem.Id)} = @{nameof(CategoryItem.Id)}", new Dictionary<string, object?>
        {
            [nameof(CategoryItem.Id)] = item.Id,
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
            TABLE_NAME,
            data,
            $"{nameof(CategoryItem.Id)} = @{nameof(CategoryItem.Id)}",
            new Dictionary<string, object?>
            {
                [nameof(CategoryItem.Id)] = id,
            }
        );
    }

    public void Delete(string id)
    {
        _db.Delete(TABLE_NAME, $"{nameof(CategoryItem.Id)} = @{nameof(CategoryItem.Id)}", new Dictionary<string, object>
        {
            [nameof(CategoryItem.Id)] = id,
        });
    }

    public string? GetIdByCategoryName(string categoryName)
    {
        var rows = _db.Query($"SELECT {nameof(CategoryItem.Id)} FROM {TABLE_NAME} WHERE {nameof(CategoryItem.Name)} = @{nameof(CategoryItem.Name)}", new Dictionary<string, object>
        {
            [nameof(CategoryItem.Name)] = categoryName,
        });

        return rows.Count > 0 ? rows[0][nameof(CategoryItem.Id)]?.ToString() : null;
    }
}
