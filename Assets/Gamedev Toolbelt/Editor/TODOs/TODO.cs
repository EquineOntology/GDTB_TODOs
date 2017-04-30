using System.Collections.Generic;
using UnityEngine;

namespace com.immortalhydra.gdtb.todos
{
    [System.Serializable]
    public class TODO : ScriptableObject
    {
    #region FIELDS

        public List<QQQ> QQQs;
        public List<QQQ> CompletedQQQs;
        public List<QQQ> CurrentQQQs;

    #endregion

    #region MONOBEHAVIOUR METHODS

        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        private void OnDestroy()
        {
            foreach (var qqq in QQQs)
            {
                Destroy(qqq);
            }
            foreach (var qqq in CompletedQQQs)
            {
                Destroy(qqq);
            }
            foreach (var qqq in CurrentQQQs)
            {
                Destroy(qqq);
            }
        }

    #endregion

    #region CONSTRUCTORS
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
    #endregion
    }
}