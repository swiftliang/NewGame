using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fishing
{
    public class ArgList
    {
        public static ArgList Create(params object[] args)
        {
            return new ArgList(args);
        }
        ArgList(params object[] args)
        {
            this.args = new List<object>(args.Length);
            this.args.AddRange(args);
        }
        public T GetAt<T>(int idx)
        {
            if (idx < 0 || idx >= args.Count)
                return default(T);
            return (T)args[idx];
        }
        public T Next<T>()
        {
            return GetAt<T>(curPos++);
        }
        public void Rewind()
        {
            curPos = 0;
        }
        public bool Finish()
        {
            return curPos >= args.Count;
        }
        List<object> args;
        int curPos = 0;
    }
    public class ArgTable
    {
        public static ArgTable Create()
        {
            return new ArgTable();
        }
        ArgTable()
        {
            args = new Dictionary<string, object>();
        }
        public ArgTable Add(string key, object value)
        {
            if (args.ContainsKey(key))
                args[key] = value;
            else
                args.Add(key, value);
            return this;
        }
        public Dictionary<string, object>.ValueCollection Values
        {
            get
            {
                return args.Values;
            }
        }
        public T Get<T>(string key)
        {
            if (args.ContainsKey(key))
                return (T)args[key];
            else
                return default(T);
        }
        Dictionary<string, object> args;
    }

    public class Event
    {
        public EventType Type { get; set; }
        public ArgList args;
        public static Event Create(EventType t, params object[] args)
        {
            return new Event(t, args);
        }
        public static Event Create(EventType t)
        {
            return new Event(t);
        }
        Event(EventType t)
        {
            Type = t;
        }
        Event(EventType t, params object[] args)
        {
            Type = t;
            this.args = ArgList.Create(args);
        }
        public T Next<T>()
        {
            return args.Next<T>();
        }
        public void Rewind()
        {
            args.Rewind();
        }
        public bool Finish()
        {
            return args.Finish();
        }
    }
}
