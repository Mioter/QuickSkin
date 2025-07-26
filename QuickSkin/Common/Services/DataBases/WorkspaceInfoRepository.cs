using System;
using System.Collections.Generic;
using System.Linq;
using QuickSkin.Core.Enums;
using QuickSkin.Core.Interfaces;
using QuickSkin.Models;

namespace QuickSkin.Common.Services.DataBases;

public class WorkspaceInfoRepository : IDatabaseRepository<WorkspaceInfo>
{
    private const string TABLE_NAME = "WorkspaceInfos";

    private readonly DatabaseService _db;

    public WorkspaceInfoRepository(string dbPath)
    {
        _db = new DatabaseService(dbPath);

        _db.CreateTable(TABLE_NAME,
            $"{nameof(WorkspaceInfo.Id)} TEXT PRIMARY KEY, " +
            $"{nameof(WorkspaceInfo.Name)} TEXT, " +
            $"{nameof(WorkspaceInfo.InputPath)} TEXT, " +
            $"{nameof(WorkspaceInfo.OutputPath)} TEXT, " +
            $"{nameof(WorkspaceInfo.WorkingMode)} INTEGER, " +
            $"{nameof(WorkspaceInfo.IconId)} TEXT");
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
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

    public void Insert(WorkspaceInfo info)
    {
        var data = new Dictionary<string, object?>
        {
            [nameof(WorkspaceInfo.Id)] = info.Id,
            [nameof(WorkspaceInfo.Name)] = info.Name,
            [nameof(WorkspaceInfo.InputPath)] = info.InputPath,
            [nameof(WorkspaceInfo.OutputPath)] = info.OutputPath,
            [nameof(WorkspaceInfo.WorkingMode)] = (int)info.WorkingMode,
            [nameof(WorkspaceInfo.IconId)] = info.IconId,
        };

        _db.Insert(TABLE_NAME, data);
    }

    public void Update(WorkspaceInfo info)
    {
        var data = new Dictionary<string, object?>
        {
            [nameof(WorkspaceInfo.Name)] = info.Name,
            [nameof(WorkspaceInfo.InputPath)] = info.InputPath,
            [nameof(WorkspaceInfo.OutputPath)] = info.OutputPath,
            [nameof(WorkspaceInfo.WorkingMode)] = (int)info.WorkingMode,
            [nameof(WorkspaceInfo.IconId)] = info.IconId,
        };

        _db.Update(
            TABLE_NAME,
            data,
            $"{nameof(WorkspaceInfo.Id)} = @{nameof(WorkspaceInfo.Id)}",
            new Dictionary<string, object?>
            {
                [nameof(WorkspaceInfo.Id)] = info.Id,
            }
        );
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
            $"{nameof(WorkspaceInfo.Id)} = @{nameof(WorkspaceInfo.Id)}",
            new Dictionary<string, object?>
            {
                [nameof(WorkspaceInfo.Id)] = id,
            }
        );
    }

    public WorkspaceInfo? Get(string id)
    {
        var rows = _db.Query($"SELECT * FROM {TABLE_NAME} WHERE {nameof(WorkspaceInfo.Id)} = @{nameof(WorkspaceInfo.Id)}",
            new Dictionary<string, object>
            {
                [nameof(WorkspaceInfo.Id)] = id,
            });

        if (rows.Count == 0)
            return null;

        var row = rows[0];

        return new WorkspaceInfo
        {
            Id = row[nameof(WorkspaceInfo.Id)]?.ToString() ?? throw new InvalidOperationException(),
            Name = row[nameof(WorkspaceInfo.Name)]?.ToString() ?? throw new InvalidOperationException(),
            InputPath = row[nameof(WorkspaceInfo.InputPath)]?.ToString(),
            OutputPath = row[nameof(WorkspaceInfo.OutputPath)]?.ToString() ?? throw new InvalidOperationException(),
            WorkingMode = row[nameof(WorkspaceInfo.WorkingMode)] is int mode ? (WorkingMode)mode : (WorkingMode)Convert.ToInt32(row[nameof(WorkspaceInfo.WorkingMode)] ?? 0),
            IconId = row[nameof(WorkspaceInfo.IconId)]?.ToString(),
        };
    }

    public IEnumerable<WorkspaceInfo> GetAll()
    {
        var rows = _db.Query($"SELECT * FROM {TABLE_NAME}");

        return rows.Select(row => new WorkspaceInfo
            {
                Id = row[nameof(WorkspaceInfo.Id)]?.ToString() ?? throw new InvalidOperationException(),
                Name = row[nameof(WorkspaceInfo.Name)]?.ToString() ?? throw new InvalidOperationException(),
                InputPath = row[nameof(WorkspaceInfo.InputPath)]?.ToString(),
                OutputPath = row[nameof(WorkspaceInfo.OutputPath)]?.ToString() ?? throw new InvalidOperationException(),
                WorkingMode = row[nameof(WorkspaceInfo.WorkingMode)] is int mode ? (WorkingMode)mode : (WorkingMode)Convert.ToInt32(row[nameof(WorkspaceInfo.WorkingMode)] ?? 0),
                IconId = row[nameof(WorkspaceInfo.IconId)]?.ToString(),
            })
            .ToList();
    }

    public void Delete(string value)
    {
        _db.Delete(TABLE_NAME, $"{nameof(WorkspaceInfo.OutputPath)} = @{nameof(WorkspaceInfo.OutputPath)}", new Dictionary<string, object>
        {
            [nameof(WorkspaceInfo.OutputPath)] = value,
        });
    }

    public bool Exists(string value)
    {
        var rows = _db.Query($"SELECT 1 FROM {TABLE_NAME} WHERE {nameof(WorkspaceInfo.OutputPath)} = @{nameof(WorkspaceInfo.OutputPath)}", new Dictionary<string, object>
        {
            [nameof(WorkspaceInfo.OutputPath)] = value,
        });

        return rows.Count > 0;
    }
}
