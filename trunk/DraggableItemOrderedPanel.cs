using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    class DraggableItemOrderedPanel : OrderedPanel
    {
        /// <summary>
        /// The control being dragged currently.
        /// </summary>
        private Control m_Dragging;

        /// <summary>
        /// The last control to be clicked.
        /// </summary>
        private Control m_Clicked;

        /// <summary>
        /// The last position of the control in the ordered list.
        /// </summary>
        private int m_LastPosition;

        /// <summary>
        /// The drag offset for the item being dragged.
        /// </summary>
        private Point m_DragOffset;

        /// <summary>
        /// The number of pixels that must be moved from the click position to engage the dragging event.
        /// </summary>
        public int DragTolerance { get; set; }


        /// <summary>
        /// Creates a new DraggableItemOrderedPanel.
        /// </summary>
        public DraggableItemOrderedPanel()
            : base()
        {
            DragTolerance = 10;
            MouseMove += new System.Windows.Forms.MouseEventHandler(ChildControlDragging);
        }

        /// <summary>
        /// Raises the System.Windows.Forms.Control.ParentChanged event and hooks the Parent's MouseMove event.
        /// </summary>
        /// <param name="e">An System.EventArgs that contains the event data.</param>
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if(Parent != null)
                Parent.MouseMove += new System.Windows.Forms.MouseEventHandler(ChildControlDragging);
        }

        /// <summary>
        /// Raises the System.Windows.Forms.Control.ControlAdded event and hooks the MouseDown, MouseMove, and MouseUp events to the <see cref="ChildControlStartDrag"/>, <see cref="ChildControllDragging"/>, and <see cref="ChildControlStopDrag"/> event handlers respectively.
        /// </summary>
        /// <param name="e">An System.EventArgs that contains the event data.</param>
        protected override void OnControlAdded(System.Windows.Forms.ControlEventArgs e)
        {
            base.OnControlAdded(e);
            e.Control.MouseDown += new System.Windows.Forms.MouseEventHandler(ChildControlStartDrag);
            e.Control.MouseMove += new System.Windows.Forms.MouseEventHandler(ChildControlDragging);
            e.Control.MouseUp += new MouseEventHandler(ChildControlStopDrag);
        }


        /// <summary>
        /// Event handler for the MouseUp event of dragged objects.
        /// </summary>
        /// <param name="sender">The control sending the MouseUp event.</param>
        /// <param name="e">An System.Windows.Forms.MouseEventArgs.</param>
        void ChildControlStopDrag(object sender, MouseEventArgs e)
        {
            m_Clicked = null;
            if (m_Dragging != null)
            {
                Controls.Add(m_Dragging);
                Control beneath = null;
                Rectangle bounds = m_Dragging.Bounds;
                bounds.Inflate((int)(-m_Dragging.Width*.25),(int)(-m_Dragging.Height*.25));
                foreach (Control c in Controls)
                {
                    if (c != m_Dragging)
                    {
                        //passive... 
                        if(c.Bounds.IntersectsWith(bounds))
                            beneath = c;
                        //ABSOLUTE!
                        if (c.Bounds.Contains(PointToClient(MousePosition)))
                        {
                            beneath = c;
                            break;
                        }
                    }
                }
                if (beneath != null)
                    MoveControlTo(m_Dragging, GetControlOrder(beneath));
                else
                    MoveControlTo(m_Dragging, m_LastPosition);
                m_Dragging = null;
                OrderPanel();
            }
        }

        /// <summary>
        /// Event handler for the MouseMove event of dragged objects.
        /// </summary>
        /// <param name="sender">The control sending the MouseMove event.</param>
        /// <param name="e">An System.Windows.Forms.MouseEventArgs.</param>
        void ChildControlDragging(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_Clicked != null)
            {
                if(Math.Abs(m_DragOffset.X - -e.X) >= DragTolerance || Math.Abs(m_DragOffset.Y - -e.Y) >= DragTolerance)
                {
                    m_LastPosition = GetControlOrder(m_Clicked);
                    m_Dragging = m_Clicked;
                    if (Parent != null)
                        Parent.Controls.Add(m_Dragging);
                    m_Dragging.BringToFront();
                    m_Clicked = null;
                }
            }
            if (m_Dragging != null)
            {
                Point loc = m_Dragging.Parent.PointToClient(MousePosition);
                loc.Offset(m_DragOffset);
                m_Dragging.Location = loc;
            }
        }

        /// <summary>
        /// Event handler for the MouseDown event of dragged objects.
        /// </summary>
        /// <param name="sender">The control sending the MouseDown event.</param>
        /// <param name="e">An MouseEventArgs.</param>
        void ChildControlStartDrag(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left){
                m_DragOffset = new Point(-e.X,-e.Y);
                m_Clicked = sender as Control;
            }
        }
    }
}
