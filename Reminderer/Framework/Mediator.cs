using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminderer.Framework
{
    public static class Mediator
    {
        private static IDictionary<string, List<Action<object>>> _subscriptionDict = new Dictionary<string, List<Action<object>>>();

        //Could technically add a list of participants who will listen to the broadcast message but for the purpose of paging this should suffice.

        public static void Subscribe(string key, Action<object> callback)
        {
            if (_subscriptionDict.ContainsKey(key) == false)
            {
                var cbList = new List<Action<object>>();
                cbList.Add(callback);
                _subscriptionDict.Add(key, cbList);
            }
            else
            {
                bool keyExists = false;
                foreach (var action in _subscriptionDict[key])
                {
                    if (action.Method.ToString() == callback.Method.ToString())
                        keyExists = true;
                }

                if (keyExists == false)
                    _subscriptionDict[key].Add(callback);
            }
        }

        public static void Unsubscribe(string key, Action<object> callback)
        {
            if (_subscriptionDict.ContainsKey(key))
                if (_subscriptionDict[key].Contains(callback))
                    _subscriptionDict[key].Remove(callback);
        }

        public static void Broadcast(string key, object args = null)
        {
            if (_subscriptionDict.ContainsKey(key))
            {
                foreach (var callback in _subscriptionDict[key])
                {
                    callback(args);
                }
            }
        }
    }
}
