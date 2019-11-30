﻿using System;
using ODL;

namespace MKEditor.Widgets
{
    public class SaveButton : Widget
    {
        public SaveButton(object Parent, string Name = "saveButton")
            : base(Parent, Name)
        {
            Sprites["bg"] = new Sprite(this.Viewport, new Bitmap(70, 28));
            Sprites["bg"].Bitmap.Unlock();
            Sprites["bg"].Bitmap.FillRect(0, 0, 70, 28, new Color(87, 123, 168));
            Sprites["bg"].Bitmap.SetPixel(0, 0, Color.ALPHA);
            Sprites["bg"].Bitmap.SetPixel(69, 0, Color.ALPHA);
            Sprites["bg"].Bitmap.SetPixel(0, 27, Color.ALPHA);
            Sprites["bg"].Bitmap.SetPixel(69, 27, Color.ALPHA);
            Sprites["bg"].Bitmap.Lock();

            Sprites["text"] = new Sprite(this.Viewport);
            Sprites["text"].Y = 5;

            //Sprites["selector"] = new Sprite(this.Viewport, new SolidBitmap(106, 2, new Color(59, 227, 255)));
            //Sprites["selector"].Visible = false;
            //Sprites["selector"].Y = 50;

            Sprites["icon"] = new Sprite(this.Viewport);
            Sprites["icon"].Bitmap = Utilities.IconSheet;
            Sprites["icon"].SrcRect = new Rect(22 * 24, 0, 24, 24);
            Sprites["icon"].X = 3;
            Sprites["icon"].Y = 2;

            WidgetIM.OnHoverChanged += UpdateSelector;
            WidgetIM.OnMouseMoving += UpdateSelector;
            OnWidgetSelected += WidgetSelected;

            SetSize(70, 28);
        }
        
        protected override void Draw()
        {
            if (Sprites["text"].Bitmap != null) Sprites["text"].Bitmap.Dispose();
            Font f = Font.Get("Fonts/Ubuntu-B", 16);
            Size s = f.TextSize("Save");
            Sprites["text"].Bitmap = new Bitmap(s);
            Sprites["text"].Bitmap.Font = f;
            Sprites["text"].Bitmap.Unlock();
            Sprites["text"].Bitmap.DrawText("Save", Color.WHITE);
            Sprites["text"].Bitmap.Lock();
            Sprites["text"].X = Size.Width / 2 - 5;
            base.Draw();
        }

        public void UpdateSelector(object sender, MouseEventArgs e)
        {
            int ry = e.Y - Viewport.Y;
            //Sprites["selector"].Visible = WidgetIM.Hovering && ry < 42;
        }
    }
}