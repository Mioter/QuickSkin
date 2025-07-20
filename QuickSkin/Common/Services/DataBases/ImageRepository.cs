using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;

namespace QuickSkin.Common.Services.DataBases;

/// <summary>
///     通用图片表操作仓库，支持通过 Id 读写图片（BLOB）。
/// </summary>
public class ImageRepository : IDisposable
{
    private const string TABLE_NAME = "ImageTable";
    private readonly DatabaseService _db;

    public ImageRepository(string dbPath)
    {
        _db = new DatabaseService(dbPath);
        _db.CreateTable(TABLE_NAME, "Id TEXT PRIMARY KEY, Image BLOB");
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    public void SaveImage(string id, Bitmap bitmap)
    {
        var data = new Dictionary<string, object?>
        {
            ["Id"] = id,
            ["Image"] = BitmapToBytes(bitmap) ?? (object)DBNull.Value,
        };

        // 先尝试更新，若无则插入
        if (Exists(id))
        {
            _db.Update(TABLE_NAME, new Dictionary<string, object?>
            {
                ["Image"] = data["Image"],
            }, "Id = @Id", new Dictionary<string, object?>
            {
                ["Id"] = id,
            });
        }
        else
        {
            _db.Insert(TABLE_NAME, data);
        }
    }

    public Bitmap? LoadImage(string id)
    {
        var rows = _db.Query($"SELECT Image FROM {TABLE_NAME} WHERE Id = @Id", new Dictionary<string, object>
        {
            ["Id"] = id,
        });

        if (rows.Count == 0) return null;

        byte[]? bytes = rows[0]["Image"] as byte[];

        return BytesToBitmap(bytes);
    }

    public void DeleteImage(string id)
    {
        _db.Delete(TABLE_NAME, "Id = @Id", new Dictionary<string, object>
        {
            ["Id"] = id,
        });
    }

    public bool Exists(string id)
    {
        var rows = _db.Query($"SELECT 1 FROM {TABLE_NAME} WHERE Id = @Id", new Dictionary<string, object>
        {
            ["Id"] = id,
        });

        return rows.Count > 0;
    }

    private static byte[]? BitmapToBytes(Bitmap? bitmap)
    {
        if (bitmap == null) return null;

        using var ms = new MemoryStream();
        bitmap.Save(ms);

        return ms.ToArray();
    }

    private static Bitmap? BytesToBitmap(byte[]? bytes)
    {
        if (bytes == null || bytes.Length == 0) return null;

        using var ms = new MemoryStream(bytes);

        return new Bitmap(ms);
    }
}
