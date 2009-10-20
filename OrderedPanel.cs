using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// This is a class that acts VERY similar to the FlowLayoutPanel,
    /// except for the fact that the FlowLayoutPanel will not let me
    /// drag items around from inside it (necessitating the creation
    /// of this component), and this class has severely limited
    /// functionality in comparison. This panel will only flow from
    /// left to right.
    /// </summary>
    class OrderedPanel : Panel
    {
        /// <summary>
        /// This List stores the order that the controls actually go in.
        /// </summary>
        private List<Control> m_ControlOrder;

        /// <summary>
        /// This property sets how much padding goes inbetween each item.
        /// </summary>
        public int ItemPadding { get; set; }

        /// <summary>
        /// Creates a new OrderedPanel.
        /// </summary>
        public OrderedPanel()
            : base()
        {
            m_ControlOrder = new List<Control>();
        }

        /// <summary>
        /// Fires the event indicating that the panel has been resized and reorders the panel. Inheriting controls should use this in favor of actually listening to the event, but should still call base.onResize to ensure that the event is fired for external listeners.
        /// </summary>
        /// <param name="e">An System.EventArgs that contains the event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            OrderPanel();
        }
        /// <summary>
        /// Raises the System.Windows.Forms.Control.LocationChanged event and reorders the panel.
        /// </summary>
        /// <param name="e">An System.EventArgs that contains the event data.</param>
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            OrderPanel();
        }

        /// <summary>
        /// Raises the System.Windows.Forms.Control.ControlAdded event and adds the control to the ordered list, and applies some event handlers for when the control is resized. Then it reorders the panel.
        /// </summary>
        /// <param name="e">An System.EventArgs that contains the event data.</param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            m_ControlOrder.Add(e.Control);
            e.Control.SizeChanged += new EventHandler(ChildControlSizeChanged);
            OrderPanel();
        }


        /// <summary>
        /// Moves the control specified in the parameters up in the ordered list.
        /// </summary>
        /// <param name="c">The control to move.</param>
        public void MoveControlUp(Control c)
        {
            if (m_ControlOrder.Contains(c))
            {
                int idx = m_ControlOrder.IndexOf(c);
                if (idx > 0)
                {
                    Control prev = m_ControlOrder[idx - 1];
                    m_ControlOrder[idx-1] = c;
                    m_ControlOrder[idx] = prev;
                }
            }
        }

        /// <summary>
        /// Moves the control specified in the parameters down in the ordered list.
        /// </summary>
        /// <param name="c">The control to move.</param>
        public void MoveControlDown(Control c)
        {
            if (m_ControlOrder.Contains(c))
            {
                int idx = m_ControlOrder.IndexOf(c);
                if (idx < m_ControlOrder.Count - 1)
                {
                    Control prev = m_ControlOrder[idx + 1];
                    m_ControlOrder[idx+1] = c;
                    m_ControlOrder[idx] = prev;
                }
            }
        }

        /// <summary>
        /// Moves the control specified in the parameters to the specified index in the parameters.
        /// </summary>
        /// <param name="c">The control to move.</param>
        /// <param name="index">The position to move it to</param>
        public void MoveControlTo(Control c, int index)
        {
            int myPosition = GetControlOrder(c);
            if (myPosition > -1)
            {
                for (int i = 0; i < Math.Abs(myPosition - index); i++)
                {
                    if (myPosition > index) MoveControlUp(c); else MoveControlDown(c);
                }
            }
        }

        /// <summary>
        /// Gets the index of the control in the order list.
        /// </summary>
        /// <param name="c">The control to get the index of.</param>
        /// <returns>The index of the control. Returns -1 if the control is not found.</returns>
        public int GetControlOrder(Control c)
        {
            return m_ControlOrder.Contains(c) ? m_ControlOrder.IndexOf(c) : -1;
        }

        /// <summary>
        /// Event hook for subcontrols being resized.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">An System.EventArgs that contains the event data.</param>
        void ChildControlSizeChanged(object sender, EventArgs e)
        {
            OrderPanel();
        }

        /// <summary>
        /// Raises the System.Windows.Forms.Control.ControlRemoved event and reorders the panel. Unhooks child controls.
        /// </summary>
        /// <param name="e">An System.EventArgs that contains the event data.</param>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            m_ControlOrder.Remove(e.Control);
            e.Control.SizeChanged -= ChildControlSizeChanged;
            OrderPanel();
        }

        /// <summary>
        /// Reorders the panel according to <see cref="m_ControlOrder"/>.
        /// </summary>
        public void OrderPanel()
        {
            Point current = new Point(ItemPadding,ItemPadding);
            int rowOffset = 0;
            foreach (Control c in m_ControlOrder)
            {
                rowOffset = Math.Max(current.Y + c.Height + ItemPadding, rowOffset);
                if (current.X + c.Width + ItemPadding > Width)
                    current = new Point(ItemPadding, rowOffset);
                if (c.Location.X != current.X || c.Location.Y - VerticalScroll.Value != current.Y)
                    c.Location = new Point(current.X,current.Y-VerticalScroll.Value);
                current = new Point(current.X + c.Width + ItemPadding, current.Y);
            }
        }
    }
}
