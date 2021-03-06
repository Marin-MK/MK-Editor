﻿using System;
using System.Collections.Generic;
using odl;
using amethyst;

namespace RPGStudioMK.Widgets
{
    public class ListBox : Widget
    {
        public int SelectedIndex { get { return ListDrawer.SelectedIndex; } }
        public ListItem SelectedItem { get { return ListDrawer.SelectedItem; } }
        public List<ListItem> Items { get { return ListDrawer.Items; } }
        public bool Enabled { get; protected set; } = true;

        public BaseEvent OnSelectionChanged
        {
            get 
            {
                return ListDrawer.OnSelectionChanged; 
            }
            set
            {
                ListDrawer.OnSelectionChanged = value;
            }
        }
        public BaseEvent OnDoubleClicked
        {
            get
            {
                return ListDrawer.OnDoubleClicked;
            }
            set
            {
                ListDrawer.OnDoubleClicked = value;
            }
        }

        public Container MainContainer;
        public ListDrawer ListDrawer;

        public ListBox(IContainer Parent) : base(Parent)
        {
            Sprites["bg"] = new Sprite(this.Viewport);
            MainContainer = new Container(this);
            MainContainer.SetPosition(1, 2);
            MainContainer.VAutoScroll = true;
            ListDrawer = new ListDrawer(MainContainer);
            VScrollBar vs = new VScrollBar(this);
            MainContainer.SetVScrollBar(vs);
            SetSize(132, 174);
        }

        public void SetEnabled(bool Enabled)
        {
            if (this.Enabled != Enabled)
            {
                this.Enabled = Enabled;
                RedrawBox();
                ListDrawer.SetEnabled(Enabled);
            }
        }

        public void SetButtonText(string Text)
        {
            ListDrawer.SetButton(true, Text);
        }

        public void SetItems(List<ListItem> Items)
        {
            ListDrawer.SetItems(Items);
        }

        public void RedrawBox()
        {
            Sprites["bg"].Bitmap?.Dispose();
            Sprites["bg"].Bitmap = new Bitmap(Size);
            Sprites["bg"].Bitmap.Unlock();
            Sprites["bg"].Bitmap.DrawRect(Size, this.Enabled ? new Color(86, 108, 134) : new Color(36, 34, 36));
            Sprites["bg"].Bitmap.FillRect(1, 1, Size.Width - 2, Size.Height - 2, this.Enabled ? new Color(10, 23, 37) : new Color(72, 72, 72));
            Sprites["bg"].Bitmap.SetPixel(0, 0, Color.ALPHA);
            Sprites["bg"].Bitmap.SetPixel(Size.Width - 1, 0, Color.ALPHA);
            Sprites["bg"].Bitmap.SetPixel(0, Size.Height - 1, Color.ALPHA);
            Sprites["bg"].Bitmap.SetPixel(Size.Width - 1, Size.Height - 1, Color.ALPHA);
            Color DarkOutline = this.Enabled ? new Color(40, 62, 84) : new Color(36, 34, 36);
            Sprites["bg"].Bitmap.SetPixel(1, 1, DarkOutline);
            Sprites["bg"].Bitmap.SetPixel(Size.Width - 2, 1, DarkOutline);
            Sprites["bg"].Bitmap.SetPixel(1, Size.Height - 2, DarkOutline);
            Sprites["bg"].Bitmap.SetPixel(Size.Width - 2, Size.Height - 2, DarkOutline);
            Sprites["bg"].Bitmap.DrawLine(Size.Width - 12, 1, Size.Width - 12, Size.Height - 2, DarkOutline);
            Sprites["bg"].Bitmap.Lock();
        }

        public override void SizeChanged(BaseEventArgs e)
        {
            base.SizeChanged(e);
            RedrawBox();
            MainContainer.SetSize(Size.Width - 13, Size.Height - 4);
            MainContainer.VScrollBar.SetPosition(Size.Width - 10, 2);
            ListDrawer.SetWidth(MainContainer.Size.Width);
            MainContainer.VScrollBar.SetSize(8, Size.Height - 4);
        }

        public void SetSelectedIndex(int idx, bool ForceRefresh = false)
        {
            ListDrawer.SetSelectedIndex(idx, ForceRefresh);
        }

        public override void Redraw()
        {
            base.Redraw();
            ListDrawer.Redraw();
        }
    }
}
