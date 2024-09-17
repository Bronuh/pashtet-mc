#region

using System.Collections;
using System.Linq;

#endregion

namespace KludgeBox.Collections;

public class GuaranteedWeightedRandom<T> : IEnumerable
{
    public int Count => _items.Count;
    public double TotalWeight => _items.TotalWeight;
    
    private WeightedRandom<T> _items = [];
    private Dictionary<WeightedItem<T>, int> _misses = [];
    
    /// <inheritdoc />
    public IEnumerator GetEnumerator()
    {
        return _items.GetEnumerator();
    }
    
    public void Add(T item, double weight)
    {
        var weightedItem = _items.Add(item, weight);

        _misses[weightedItem] = 0;
    }
    
    public void Remove(T item)
    {
        var weightedItem = _items.Remove(item);

        if (weightedItem != null)
        {
            _misses.Remove(weightedItem);
        }
    }


    public void ChangeWeight(T item, double weight)
    {
        _items.ChangeWeight(item, weight);
    }

    public T PickRandom()
    {
        var weights = _misses.Keys.ToList();
        weights.Sort((item, other) => item.Weight.CompareTo(other.Weight));

        bool itemPicked = false;
        T item = default;
        
        foreach (WeightedItem<T> weightedItem in weights)
        {
            var chance = weightedItem.Weight / _items.TotalWeight;
            var guaranteedThreshold = (int)Mathf.Ceil(1 / chance);
            
            if(_misses[weightedItem] > guaranteedThreshold)
            {
                _misses[weightedItem] = 0;
                item = weightedItem.Item; 
                itemPicked = true;
                break;
            }
        }

        if(!itemPicked)
        {
            item = _items.PickRandom();
        }
        
        foreach (WeightedItem<T> weightedItem in _items)
        {
            if(!item.Equals(weightedItem.Item))
            {
                _misses[weightedItem]++;
            }
        }

        return item;
    }
}