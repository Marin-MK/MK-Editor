﻿using System;
using System.Collections.Generic;
using ODL;

namespace MKEditor.Widgets
{
    public class DatabaseDataList : Widget
    {
        public int SelectedIndex = -1;

        public DatabaseWidget DBWidget;

        public Container ListContainer;
        public ListDrawer DataList;
        public Button ChangeAmountButton;

        public TilesetEditor TilesetEditor;

        public DatabaseDataList(object Parent, string Name = "databaseDataList")
            : base(Parent, Name)
        {
            Sprites["listbox"] = new Sprite(this.Viewport);
            Sprites["listbox"].Y = 39;

            Sprites["header"] = new Sprite(this.Viewport);
            Sprites["header"].X = 10;
            Sprites["header"].Y = 10;

            ListContainer = new Container(this);
            ListContainer.SetPosition(3, 44);
            ListContainer.VAutoScroll = true;

            VScrollBar vslist = new VScrollBar(this);
            vslist.SetPosition(188, 41);
            ListContainer.SetVScrollBar(vslist);

            DataList = new ListDrawer(ListContainer);
            List<ListItem> Tilesets = new List<ListItem>();
            for (int i = 1; i < Game.Data.Tilesets.Count; i++)
            {
                Game.Tileset t = Game.Data.Tilesets[i];
                Tilesets.Add(new ListItem($"{Utilities.Digits(i, 3)}: {t?.Name}", t));
            }
            DataList.SetItems(Tilesets);
            DataList.OnSelectionChanged += delegate (object sender, EventArgs e)
            {
                TilesetEditor.SetTileset(DataList.SelectedItem.Object as Game.Tileset, DataList.SelectedIndex + 1);
            };

            ChangeAmountButton = new Button(this);
            ChangeAmountButton.SetSize(155, 37);
            ChangeAmountButton.SetText("Change Amount...");
            ChangeAmountButton.OnClicked += delegate (object sender, EventArgs e)
            {
                PopupWindow win = new PopupWindow(Window);
                win.SetSize(270, 125);
                win.SetTitle("Set tileset capacity");
                Label label = new Label(win);
                label.SetText("Set the maximum available number of tilesets.");
                label.SetPosition(5, 35);
                Label label2 = new Label(win);
                label2.SetText("Capacity:");
                label2.SetPosition(75, 60);
                NumericBox num = new NumericBox(win);
                num.SetSize(66, 27);
                num.SetPosition(130, 55);
                num.SetValue(Editor.ProjectSettings.TilesetCapacity);
                num.MinValue = 1;
                Button CancelButton = new Button(win);
                CancelButton.SetText("Cancel");
                CancelButton.SetPosition(win.Size.Width - CancelButton.Size.Width - 5, win.Size.Height - CancelButton.Size.Height - 5);
                CancelButton.OnClicked += delegate (object sender, EventArgs e) { win.Close(); };
                Button OKButton = new Button(win);
                OKButton.SetText("OK");
                OKButton.SetPosition(CancelButton.Position.X - OKButton.Size.Width, CancelButton.Position.Y);
                OKButton.OnClicked += delegate (object sender, EventArgs e)
                {
                    int NewValue = num.Value;
                    if (NewValue == Editor.ProjectSettings.TilesetCapacity)
                    {
                        win.Close();
                        return;
                    }
                    else if (NewValue > Editor.ProjectSettings.TilesetCapacity)
                    {
                        int Extra = NewValue - Editor.ProjectSettings.TilesetCapacity;
                        for (int i = 0; i < Extra; i++) Game.Data.Tilesets.Add(null);
                        Editor.ProjectSettings.TilesetCapacity = NewValue;
                        RefreshList();
                        win.Close();
                    }
                    else
                    {
                        int Lost = Editor.ProjectSettings.TilesetCapacity - NewValue;
                        int DefinedCount = 0;
                        for (int i = Game.Data.Tilesets.Count - 1; i >= 0; i--)
                        {
                            if (i == NewValue) break;
                            if (Game.Data.Tilesets[i] != null) DefinedCount++;
                        }
                        if (DefinedCount > 0)
                        {
                            MessageBox box = new MessageBox("Warning",
                                $"By resizing the tileset capacity from {Editor.ProjectSettings.TilesetCapacity} to {NewValue}, {Lost} entries will be removed, " +
                                $"of which {DefinedCount} {(DefinedCount == 1 ? "is a" : "are")} defined tileset{(DefinedCount == 1 ? "" : "s")}.\n" +
                                "Would you like to proceed and delete these tilesets?", ButtonType.YesNoCancel, IconType.Warning);
                            box.OnButtonPressed += delegate (object sender, EventArgs e)
                            {
                                if (box.Result == 0) // Yes -> resize tileset capacity and delete tilesets
                                {
                                    for (int i = Game.Data.Tilesets.Count - 1; i >= 0; i--)
                                    {
                                        if (i == NewValue) break;
                                        foreach (KeyValuePair<int, Game.Map> kvp in Game.Data.Maps)
                                        {
                                            if (kvp.Value.TilesetIDs.Contains(i)) kvp.Value.RemoveTileset(i);
                                        }
                                        if (Game.Data.Tilesets[i] != null)
                                        {
                                            Game.Data.Tilesets[i].TilesetBitmap.Dispose();
                                            Game.Data.Tilesets[i].TilesetListBitmap.Dispose();
                                        }
                                        Game.Data.Tilesets[i] = null;
                                    }
                                    Game.Data.Tilesets.RemoveRange(NewValue + 1, Lost);
                                    Editor.ProjectSettings.TilesetCapacity = NewValue;
                                    RefreshList();
                                    win.Close();
                                }
                                else // No, cancel -> do nothing
                                {
                                    win.Close();
                                }
                            };
                        }
                        else
                        {
                            Game.Data.Tilesets.RemoveRange(NewValue + 1, Lost);
                            Editor.ProjectSettings.TilesetCapacity = NewValue;
                            RefreshList();
                            win.Close();
                        }
                    }
                };
                win.Center();
            };
        }

        public void SetSelectedIndex(int Index)
        {
            if (this.SelectedIndex != Index)
            {
                this.SelectedIndex = Index;
                DataList.SetSelectedIndex(Index);
                Redraw();
            }
        }

        public void RedrawHeader()
        {
            DatabaseModeList DBML = DBWidget.DBModeList;
            if (Sprites["header"].Bitmap != null) Sprites["header"].Bitmap.Dispose();
            Font f = Font.Get("Fonts/Ubuntu-B", 20);
            Sprites["header"].Bitmap = new Bitmap(f.TextSize(DBML.Tabs[DBML.SelectedIndex][0]));
            Sprites["header"].Bitmap.Font = f;
            Sprites["header"].Bitmap.Unlock();
            Sprites["header"].Bitmap.DrawText(DBML.Tabs[DBML.SelectedIndex][0], Color.WHITE);
            Sprites["header"].Bitmap.Lock();
        }

        public void RefreshList()
        {
            List<ListItem> Tilesets = new List<ListItem>();
            for (int i = 1; i < Game.Data.Tilesets.Count; i++)
            {
                Game.Tileset t = Game.Data.Tilesets[i];
                Tilesets.Add(new ListItem($"{Utilities.Digits(i, 3)}: {t?.Name}", t));
            }
            DataList.SetItems(Tilesets);
            DataList.Redraw();
            if (DataList.SelectedIndex >= Tilesets.Count) DataList.SetSelectedIndex(Tilesets.Count - 1);
        }

        public override void SizeChanged(object sender, SizeEventArgs e)
        {
            base.SizeChanged(sender, e);
            ListContainer.SetSize(180, Size.Height - 81);
            ListContainer.VScrollBar.SetSize(ListContainer.VScrollBar.Size.Width, Size.Height - 42);
            ChangeAmountButton.SetPosition(17, Size.Height - 37);
            DataList.SetSize(ListContainer.Size);
            if (Sprites["listbox"].Bitmap != null) Sprites["listbox"].Bitmap.Dispose();
            Sprites["listbox"].Bitmap = new Bitmap(198, Size.Height - 39);
            Sprites["listbox"].Bitmap.Unlock();
            Sprites["listbox"].Bitmap.DrawLine(0, 0, 197, 0, new Color(28, 50, 73));
            Sprites["listbox"].Bitmap.DrawLine(186, 1, 186, Size.Height - 40, new Color(28, 50, 73));
            Sprites["listbox"].Bitmap.DrawLine(197, 1, 197, Size.Height - 40, new Color(28, 50, 73));
            Sprites["listbox"].Bitmap.Lock();
        }
    }
}
