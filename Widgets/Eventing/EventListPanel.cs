﻿using System;
using ODL;
using MKEditor.Game;
using System.Collections.Generic;

namespace MKEditor.Widgets
{
    public class EventListPanel : Widget
    {
        public Container EventContainer;
        public EventListEntryWidget EventListEntryWidget;
        public VStackPanel StackPanel;
        public Map MapData;

        public EventListPanel(IContainer Parent) : base(Parent)
        {
            SetBackgroundColor(28, 50, 73);
            Label Header = new Label(this);
            Header.SetText("Events");
            Header.SetFont(Font.Get("Fonts/Ubuntu-B", 16));
            Header.SetPosition(5, 5);

            Sprites["sep"] = new Sprite(this.Viewport, new SolidBitmap(288, 2, new Color(10, 23, 37)));
            Sprites["sep"].Y = 30;

            Sprites["slider"] = new Sprite(this.Viewport, new SolidBitmap(10, Size.Height - 34, new Color(10, 23, 37)));
            Sprites["slider"].Y = 33;

            EventContainer = new Container(this);
            EventContainer.SetPosition(0, 33);
            EventContainer.VAutoScroll = true;
            VScrollBar vs = new VScrollBar(this);
            EventContainer.SetVScrollBar(vs);

            StackPanel = new VStackPanel(EventContainer);
            StackPanel.SetPosition(2, 1);
        }

        public override void SizeChanged(BaseEventArgs e)
        {
            base.SizeChanged(e);
            EventContainer.SetSize(Size.Width - 13, Size.Height - EventContainer.Position.Y);
            EventContainer.VScrollBar.SetPosition(Size.Width - 10, 34);
            EventContainer.VScrollBar.SetSize(8, Size.Height - 36);
            StackPanel.SetWidth(EventContainer.Size.Width - 2);
            Sprites["slider"].X = Size.Width - 11;
            (Sprites["slider"].Bitmap as SolidBitmap).SetSize(10, Size.Height - 34);
        }

        public void SetMap(Map Map)
        {
            this.MapData = Map;
            while (StackPanel.Widgets.Count > 0) StackPanel.Widgets[0].Dispose();
            List<int> Keys = new List<int>(Map.Events.Keys);
            Keys.Sort();
            foreach (int key in Keys)
            {
                EventListEntryWidget e = new EventListEntryWidget(StackPanel);
                e.SetEvent(Map.Events[key]);
            }
            if (Map.Events.Count == 0)
            {
                MultilineLabel label = new MultilineLabel(StackPanel);
                label.SetFont(Font.Get("Fonts/ProductSans-M", 14));
                label.SetText("This map does not have any events.");
                label.SetMargin(8, 8);
            }
            if (Map.Events.Count > 0) ((EventListEntryWidget) StackPanel.Widgets[0].Widgets[0]).SetSelected(true);
        }
    }
}
