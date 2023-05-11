#if UNITY_EDITOR && UNITY_2022_1_OR_NEWER
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace UnityEngine.InputSystem.Editor
{
    internal static partial class ContextMenu
    {
        private static readonly string rename_String = "Rename";
        private static readonly string delete_String = "Delete";
        public static void GetContextMenuForActionMapItem(InputActionsTreeViewItem targetElement, ListView listView)
        {
            var _ = new ContextualMenuManipulator(menuEvent =>
            {
                menuEvent.menu.AppendAction(rename_String, action =>
                {
                    listView.SetSelection(listView.itemsSource.IndexOf(targetElement.label.text));
                    targetElement.FocusOnRenameTextField();
                });
                menuEvent.menu.AppendAction(delete_String, action => {targetElement.OnDeleteItem();});
            }) { target = targetElement };
        }

        public static void GetContextMenuForActionOrCompositeItem(InputActionsTreeViewItem targetElement, TreeView treeView, int index)
        {
            var _ = new ContextualMenuManipulator(menuEvent =>
            {
                menuEvent.menu.AppendAction(rename_String, action =>
                {
                    treeView.SetSelection(index);
                    targetElement.FocusOnRenameTextField();
                });
                menuEvent.menu.AppendAction(delete_String, action => {targetElement.OnDeleteItem();});
            }) { target = targetElement };
        }

        public static void GetContextMenuForBindingItem(InputActionsTreeViewItem targetElement)
        {
            var _ = new ContextualMenuManipulator(menuEvent =>
            {
                menuEvent.menu.AppendAction("Delete", action => {targetElement.OnDeleteItem();});
            }) { target = targetElement };
        }
    }
}
#endif
