using System.Collections.Generic;
using UnityEngine;

namespace com.immortalhydra.gdtb.todos
{
    [System.Serializable]
    public class TODO : ScriptableObject
    {
        public List<QQQ> QQQs;
        public List<QQQ> CompletedQQQs;
        public List<QQQ> CurrentQQQs;

        public static TODO Create()
        {
            var todo = CreateInstance<TODO>();

            todo.QQQs = new List<QQQ>();
            todo.CompletedQQQs = new List<QQQ>();
            todo.CurrentQQQs = new List<QQQ>();

            return todo;
        }

        public static TODO Create(List<QQQ> aQQQList)
        {
            var todo = CreateInstance<TODO>();

            todo.QQQs = aQQQList;
            todo.CompletedQQQs = new List<QQQ>();
            todo.CurrentQQQs = aQQQList;

            return todo;
        }
    }
}