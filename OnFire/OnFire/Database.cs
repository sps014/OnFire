using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OnFire.Net;

namespace OnFire.Data
{
    public class Database
    {
        public string DatabaseURL
        {
            get;
            set;
        }

        private Cache DataCache = new Cache();

        private List<string> childKeyList=new List<string>();
        private bool ListenDataChange;
        private RestClient client = new RestClient();

        public string Key
        {
            get
            {
                if (childKeyList.Count == 0)
                    return "";
                else
                    return childKeyList[childKeyList.Count - 1];
            }
        }


        public Database()
        {
            DatabaseURL = null;
        }
        public Database(string databaseUrl)
        {
            DatabaseURL = databaseUrl;
        }
        public Database(string databaseUrl,List<string> childKeyList)
        {
            DatabaseURL = databaseUrl;
            this.childKeyList = childKeyList;
        }


        public Database Child(string childKey)
        {
            List<string> childKeyList = new List<string>();
            childKeyList.AddRange(this.childKeyList);
            childKeyList.Add(childKey);
            return new Database(DatabaseURL, childKeyList);
        }
        public async Task<Dictionary<string, object>> GetChildren()
        {
            Dictionary<string, object> children;
            if (ListenDataChange)
            {
                children = DataCache.DataSnapshot as Dictionary<string, object>;
            }
            else
            {
                children = await client.Get<Dictionary<string, object>>(FullyQualifiedJsonURL);
            }

            return children;

        }



        public async Task<T> GetValue<T>()
        {
            if(ListenDataChange)
            {
                return (T)DataCache.DataSnapshot;
            }
            else
            {
                return await client.Get<T>(FullyQualifiedJsonURL);
            }
        }
        public async void SaveValue<T>(T value)
        {
            await client.Put<T>(FullyQualifiedJsonURL, value);
        }
        public async void DeleteValue()
        {
            await client.Delete(FullyQualifiedJsonURL);
        }



        public async void SubscribeDataChange<T>()
        {
            ListenDataChange = true;

            DataCache.OnCacheChanged += (sender, e) => OnDataChange?.Invoke(this, e);

            while (ListenDataChange)
            {
                T obj =  await client.Get<T>(FullyQualifiedJsonURL);
                DataCache.CompareCache(obj);
            }
        }
        public  void UnsubscribeDataChange()
        {
            ListenDataChange = false;
        }


        private string SubChildPath
        {
            get
            {
                StringBuilder childNodeBuilder = new StringBuilder("");

                for (int i = 0; i < childKeyList.Count; i++)
                {
                    childNodeBuilder.Append(childKeyList[i]);
                    if (i != childKeyList.Count - 1)
                        childNodeBuilder.Append("/");
                }
                return childNodeBuilder.ToString();
            }
        }
        private string FullyQualifiedJsonURL
        {
            get
            {
                return DatabaseURL + SubChildPath + "/.json";
            }
        }

        public delegate void OnDataChangeHandler(object sender, object snapshot);
        public event OnDataChangeHandler OnDataChange;
    }
}
