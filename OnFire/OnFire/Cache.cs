using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;


namespace OnFire.Data
{
    public class Cache
    {
        public object DataSnapshot { get; private set; }
        //public long SnapSize { get; private set; } = 0;

        public void CompareCache(object newBlock)
        {
            if(JsonSerializer.Serialize(DataSnapshot)!= JsonSerializer.Serialize(newBlock))
            {
                DataSnapshot = newBlock;
                OnCacheChanged?.Invoke(this, DataSnapshot);
            }
        }

        public delegate void CacheModifiedHandler(object sender,object snapshot);
        public event CacheModifiedHandler OnCacheChanged;
    }
}
