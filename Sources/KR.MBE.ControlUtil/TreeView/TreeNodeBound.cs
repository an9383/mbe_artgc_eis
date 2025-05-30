using System;
using System.Collections;
using System.Windows.Forms;


namespace KR.MBE.UI.ControlUtil
{
    /// <summary>
    /// Summary description for TreeNodeBound.
    /// </summary>
    public class TreeNodeBound : TreeNode
    {

        private object _value;
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        private object _parentValue;
        public object ParentValue
        {
            get
            {
                return _parentValue;
            }
            set
            {
                _parentValue = value;
            }
        }

        private object _sortValue;
        public object SortValue
        {
            get
            {
                return _sortValue;
            }
            set
            {
                _sortValue = value;
            }
        }

        private object _levelValue;
        public object LevelValue
        {
            get
            {
                return _levelValue;
            }
            set
            {
                _levelValue = value;
            }
        }

        private object _validationMessage;
        public object ValidationMessage
        {
            get
            {
                return _validationMessage;
            }
            set
            {
                _validationMessage = value;
            }
        }

        // 2020.04.21. °­¼º¹¬ Ãß°¡
        public TreeNodeBound() : base()
        {

        }

        public TreeNodeBound( string name ) : base( name )
        {

        }

        #region Sorting
        public void SortChilds()
        {
            TreeNode[] nodes = ( TreeNode[] )System.Collections.ArrayList.Adapter( this.Nodes ).ToArray( typeof( TreeNode ) );
            Array.Sort( nodes, new TreeNodeComparer() );
            this.Nodes.Clear();
            this.Nodes.AddRange( nodes );
        }

        public void SortChildsBySortValue()
        {
            TreeNodeBound[] nodes = ( TreeNodeBound[] )System.Collections.ArrayList.Adapter( this.Nodes ).ToArray( typeof( TreeNodeBound ) );
            Array.Sort( nodes, new TreeNodeBoundComparer() );
            this.Nodes.Clear();
            this.Nodes.AddRange( nodes );
        }
        #endregion
    }

    #region TreeNodeComparer (for sorting only
    internal class TreeNodeComparer : object, IComparer
    {
        public int Compare( object x, object y )
        {
            TreeNode xNode = ( TreeNode )x;
            TreeNode yNode = ( TreeNode )y;
            return xNode.Text.CompareTo( yNode.Text );
        }
    }
    internal class TreeNodeBoundComparer : object, IComparer
    {
        public int Compare( object x, object y )
        {
            TreeNodeBound xNode = ( TreeNodeBound )x;
            TreeNodeBound yNode = ( TreeNodeBound )y;
            int iReturn = xNode.SortValue.ToString().CompareTo( yNode.SortValue.ToString() );
            if( iReturn == 0 )
            {
                iReturn = xNode.Value.ToString().CompareTo( yNode.Value.ToString() );
            }
            return iReturn;
        }
    }
    #endregion



}
