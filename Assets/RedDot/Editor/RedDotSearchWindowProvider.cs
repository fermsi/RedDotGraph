using System.Collections.Generic;
using RedDot.Editor.Views;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace RedDot.Editor
{
    public class RedDotSearchWindowProvider:ScriptableObject, ISearchWindowProvider
    {
        public delegate bool SerchMenuWindowOnSelectEntryDelegate(SearchTreeEntry searchTreeEntry,            //声明一个delegate类
            SearchWindowContext context);

        public SerchMenuWindowOnSelectEntryDelegate OnSelectEntryHandler;                              //delegate回调方法

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (OnSelectEntryHandler == null)
            {
                return false;
            }
            return OnSelectEntryHandler(searchTreeEntry, context);
        }
    
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create RedDot Node")));                        //添加了一个一级菜单
    
//            entries.Add(new SearchTreeGroupEntry(new GUIContent("Example")) { level = 1 });      //添加了一个二级菜单
            entries.Add(new SearchTreeEntry(new GUIContent("RedDot Node")) { level = 1, userData = typeof(RedDotNodeView) });
            return entries;
        }
    }
}