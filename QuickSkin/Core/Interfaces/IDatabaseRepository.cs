using System;
using System.Collections.Generic;

namespace QuickSkin.Core.Interfaces;

public interface IDatabaseRepository<T> : IDisposable
{
    public T? Get(string id);
    
    public IEnumerable<T> GetAll();
    
    public int Count();
    
    public void Insert(T item);
    
    public void Update(T item);
    
    public void Update(string id, string[] fields, string[] values);
    
    public void Delete(string id);
    
    public bool Exists(string id);
}
