using System.Collections;
using KludgeBox.Core;

namespace KludgeBox.Collections;


public class RandomBucket<TItem> : IEnumerable<TItem>
{
    public IReadOnlyList<TItem> SafeItems => _items;
    
    private List<TItem> _items = new List<TItem>();
    public int Count => _items.Count;
    public IEnumerator<TItem> GetEnumerator()
    {
        if (Count > 0)
        {
            yield return Pick();
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(TItem item)
    {
        _items.Add(item);
    }
    
    public void Remove(TItem item)
    {
        _items.Remove(item);
    }
    
    public void Clear()
    {
        _items.Clear();
    }

    public TItem Pick(bool safe = false)
    {
        var id = Rand.RangeInclusive(0, Count-1);
        var item = _items[id];
        
        if(!safe)
            _items.RemoveAt(id);

        return item;
    }

    public List<TItem> PickMany(int count, bool safe = false)
    {
        var amount = Mathf.Clamp(count, 0, Count);
        var list = new List<TItem>(amount);
        
        for (var i = 0; i < amount; i++)
        {
            list.Add(Pick(safe));
        }
        
        return list;
    }
}