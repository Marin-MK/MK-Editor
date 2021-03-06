﻿using System;
using System.Collections.Generic;
using odl;
using RPGStudioMK.Game;
using amethyst;

namespace RPGStudioMK.Widgets
{
    public class MapPropertiesWindow : PopupWindow
    {
        public Map Map;
        public Map OldMap;

        public bool UnsavedChanges = false;
        public bool UpdateMapViewer = false;

        TextBox MapName;
        TextBox DisplayName;
        NumericBox Width;
        NumericBox Height;
        ListBox Tilesets;
        ListBox Autotiles;

        public MapPropertiesWindow(Map Map)
        {
            this.OldMap = Map;
            this.Map = Map.Clone();
            this.SetTitle($"Map Properties - {Utilities.Digits(Map.ID, 3)}: {Map.DevName}");
            MinimumSize = MaximumSize = new Size(540, 460);
            SetSize(MaximumSize);
            this.Center();
            Label settings = new Label(this);
            settings.SetText("Info");
            settings.SetFont(Font.Get("Fonts/Ubuntu-B", 14));
            settings.SetPosition(12, 26);

            GroupBox box1 = new GroupBox(this);
            box1.SetPosition(19, 47);
            box1.SetSize(450, 203);

            Font f = Font.Get("Fonts/ProductSans-M", 12);

            Label namelabel = new Label(box1);
            namelabel.SetText("Working Name:");
            namelabel.SetFont(f);
            namelabel.SetPosition(7, 6);
            MapName = new TextBox(box1);
            MapName.SetPosition(6, 22);
            MapName.SetSize(136, 27);
            MapName.SetInitialText(Map.DevName);
            MapName.OnTextChanged += delegate (BaseEventArgs e)
            {
                this.Map.DevName = MapName.Text;
            };

            Label displaynamelabel = new Label(box1);
            displaynamelabel.SetText("In-game Name:");
            displaynamelabel.SetFont(f);
            displaynamelabel.SetPosition(7, 52);
            DisplayName = new TextBox(box1);
            DisplayName.SetPosition(6, 68);
            DisplayName.SetSize(136, 27);
            DisplayName.SetInitialText(Map.DisplayName);
            DisplayName.OnTextChanged += delegate (BaseEventArgs e)
            {
                this.Map.DisplayName = DisplayName.Text;
            };

            Label widthlabel = new Label(box1);
            widthlabel.SetText("Width:");
            widthlabel.SetFont(f);
            widthlabel.SetPosition(7, 99);
            Width = new NumericBox(box1);
            Width.SetPosition(6, 115);
            Width.MinValue = 1;
            Width.MaxValue = 255;
            Width.SetSize(66, 27);
            Width.SetValue(this.Map.Width);
            Width.OnValueChanged += delegate (BaseEventArgs e)
            {
                this.Map.Width = Width.Value;
            };

            Label heightlabel = new Label(box1);
            heightlabel.SetText("Height:");
            heightlabel.SetFont(f);
            heightlabel.SetPosition(78, 99);
            Height = new NumericBox(box1);
            Height.SetPosition(77, 115);
            Height.MinValue = 1;
            Height.MaxValue = 255;
            Height.SetSize(66, 27);
            Height.SetValue(this.Map.Height);
            Height.OnValueChanged += delegate (BaseEventArgs e)
            {
                this.Map.Height = Height.Value;
            };

            Tilesets = new ListBox(box1);
            Tilesets.SetPosition(162, 22);
            List<ListItem> tilesetitems = new List<ListItem>();
            for (int i = 0; i < this.Map.TilesetIDs.Count; i++)
            {
                int id = this.Map.TilesetIDs[i];
                Tileset tileset = Data.Tilesets[id];
                tilesetitems.Add(new ListItem(tileset));
            }
            Tilesets.SetItems(tilesetitems);
            Tilesets.SetButtonText("Add Tileset");
            Tilesets.ListDrawer.OnButtonClicked += AddTileset;

            Autotiles = new ListBox(box1);
            Autotiles.SetPosition(312, 22);
            List<ListItem> autotileitems = new List<ListItem>();
            for (int i = 0; i < this.Map.AutotileIDs.Count; i++)
            {
                int id = this.Map.AutotileIDs[i];
                Autotile autotile = Data.Autotiles[id];
                autotileitems.Add(new ListItem(autotile));
            }
            Autotiles.SetItems(autotileitems);
            Autotiles.SetButtonText("Add Autotile");
            Autotiles.ListDrawer.OnButtonClicked += AddAutotile;

            Label tilesetslabel = new Label(box1);
            tilesetslabel.SetText("Tilesets:");
            tilesetslabel.SetFont(f);
            tilesetslabel.SetPosition(163, 6);

            Label autotileslabel = new Label(box1);
            autotileslabel.SetText("Autotiles:");
            autotileslabel.SetFont(f);
            autotileslabel.SetPosition(313, 6);

            CreateButton("Cancel", Cancel);
            CreateButton("OK", OK);
        }

        public void AddTileset(BaseEventArgs e)
        {
            TilesetPickerMap picker = new TilesetPickerMap(Map);
            picker.OnClosed += delegate (BaseEventArgs e2)
            {
                bool update = false;
                if (Map.TilesetIDs.Count != picker.ResultIDs.Count) update = true;
                if (!update)
                {
                    for (int i = 0; i < picker.ResultIDs.Count; i++)
                    {
                        if (picker.ResultIDs[i] != Map.TilesetIDs[i])
                        {
                            update = true;
                            break;
                        }
                    }
                }
                if (update)
                {
                    Map.TilesetIDs = picker.ResultIDs;
                    List<ListItem> tilesetitems = new List<ListItem>();
                    for (int i = 0; i < this.Map.TilesetIDs.Count; i++)
                    {
                        int id = this.Map.TilesetIDs[i];
                        Tileset tileset = Data.Tilesets[id];
                        tilesetitems.Add(new ListItem(tileset));
                    }
                    Tilesets.SetItems(tilesetitems);
                }
            };
        }

        public void AddAutotile(BaseEventArgs e)
        {
            AutotilePicker picker = new AutotilePicker(Map);
            picker.OnClosed += delegate (BaseEventArgs e2)
            {
                bool update = false;
                if (Map.AutotileIDs.Count != picker.ResultIDs.Count) update = true;
                if (!update)
                {
                    for (int i = 0; i < picker.ResultIDs.Count; i++)
                    {
                        if (picker.ResultIDs[i] != Map.AutotileIDs[i])
                        {
                            update = true;
                            break;
                        }
                    }
                }
                if (update)
                {
                    Map.AutotileIDs = picker.ResultIDs;
                    List<ListItem> autotileitems = new List<ListItem>();
                    for (int i = 0; i < this.Map.AutotileIDs.Count; i++)
                    {
                        int id = this.Map.AutotileIDs[i];
                        Autotile autotile = Data.Autotiles[id];
                        autotileitems.Add(new ListItem(autotile));
                    }
                    Autotiles.SetItems(autotileitems);
                }
            };
        }

        public void OK(BaseEventArgs e)
        {
            this.UpdateMapViewer = true;
            Action Finalize = delegate
            {
                Close();
            };
            Action Continue = delegate
            {
                // Updates autotiles
                bool autotileschanged = false;
                if (Map.AutotileIDs.Count != OldMap.AutotileIDs.Count) autotileschanged = true;
                if (!autotileschanged)
                    for (int i = 0; i < Map.AutotileIDs.Count; i++)
                    {
                        if (Map.AutotileIDs[i] != OldMap.AutotileIDs[i])
                        {
                            autotileschanged = true;
                            break;
                        }
                    }
                if (autotileschanged)
                {
                    UnsavedChanges = true;
                    bool warn = false;
                    for (int layer = 0; layer < Map.Layers.Count; layer++)
                    {
                        for (int i = 0; i < Map.Width * Map.Height; i++)
                        {
                            if (Map.Layers[layer].Tiles[i] == null || Map.Layers[layer].Tiles[i].TileType == TileType.Tileset) continue;
                            int autotileID = OldMap.AutotileIDs[Map.Layers[layer].Tiles[i].Index];
                            if (!Map.AutotileIDs.Contains(autotileID))
                            {
                                warn = true;
                                break;
                            }
                        }
                    }
                    if (warn)
                    {
                        MessageBox msg = new MessageBox("Warning", "One of the deleted autotiles was still in use. By choosing to continue, tiles of that autotile will be deleted.", new List<string>() { "Continue", "Cancel" }, IconType.Warning);
                        msg.OnButtonPressed += delegate (BaseEventArgs e2)
                        {
                            if (msg.Result == 0) // Continue
                            {
                                for (int layer = 0; layer < Map.Layers.Count; layer++)
                                {
                                    for (int i = 0; i < Map.Width * Map.Height; i++)
                                    {
                                        if (Map.Layers[layer].Tiles[i] == null || Map.Layers[layer].Tiles[i].TileType == TileType.Tileset) continue;
                                        int autotileID = OldMap.AutotileIDs[Map.Layers[layer].Tiles[i].Index];
                                        if (!Map.AutotileIDs.Contains(autotileID))
                                        {
                                            Map.Layers[layer].Tiles[i] = null;
                                        }
                                        else Map.Layers[layer].Tiles[i].Index = Map.AutotileIDs.IndexOf(autotileID);
                                    }
                                }
                                Finalize();
                            }
                            else if (msg.Result == 1) // Cancel
                            {
                                UnsavedChanges = false;
                                UpdateMapViewer = false;
                            }
                        };
                    }
                    else
                    {
                        for (int layer = 0; layer < Map.Layers.Count; layer++)
                        {
                            for (int i = 0; i < Map.Width * Map.Height; i++)
                            {
                                if (Map.Layers[layer].Tiles[i] == null || Map.Layers[layer].Tiles[i].TileType == TileType.Tileset) continue;
                                int autotileID = OldMap.AutotileIDs[Map.Layers[layer].Tiles[i].Index];
                                if (!Map.AutotileIDs.Contains(autotileID))
                                {
                                    throw new Exception("Impossible-to-reach code has been reached. This indicates a flaw in autotile conversion. Please contact the maintainer.");
                                }
                                else Map.Layers[layer].Tiles[i].Index = Map.AutotileIDs.IndexOf(autotileID);
                            }
                        }
                        Finalize();
                    }
                }
                if (!autotileschanged) Finalize();
            };
            // Resizes Map
            if (Map.Width != OldMap.Width || Map.Height != OldMap.Height)
            {
                Map.Resize(OldMap.Width, Map.Width, OldMap.Height, Map.Height);
                UnsavedChanges = true;
            }
            // Marks name change
            if (Map.DevName != OldMap.DevName || Map.DisplayName != OldMap.DisplayName) UnsavedChanges = true;
            // Updates tilesets
            bool tilesetschanged = false;
            if (Map.TilesetIDs.Count != OldMap.TilesetIDs.Count) tilesetschanged = true;
            if (!tilesetschanged)
                for (int i = 0; i < Map.TilesetIDs.Count; i++)
                {
                    if (Map.TilesetIDs[i] != OldMap.TilesetIDs[i])
                    {
                        tilesetschanged = true;
                        break;
                    }
                }
            if (tilesetschanged)
            {
                UnsavedChanges = true;
                bool warn = false;
                for (int layer = 0; layer < Map.Layers.Count; layer++)
                {
                    for (int i = 0; i < Map.Width * Map.Height; i++)
                    {
                        if (Map.Layers[layer].Tiles[i] == null || Map.Layers[layer].Tiles[i].TileType == TileType.Autotile) continue;
                        int tilesetID = OldMap.TilesetIDs[Map.Layers[layer].Tiles[i].Index];
                        if (!Map.TilesetIDs.Contains(tilesetID))
                        {
                            warn = true;
                            break;
                        }
                    }
                }
                if (warn)
                {
                    MessageBox msg = new MessageBox("Warning", "One of the deleted tilesets was still in use. By choosing to continue, tiles of that tileset will be deleted.", new List<string>() { "Continue", "Cancel" }, IconType.Warning);
                    msg.OnButtonPressed += delegate (BaseEventArgs e2)
                    {
                        if (msg.Result == 0) // Continue
                        {
                            for (int layer = 0; layer < Map.Layers.Count; layer++)
                            {
                                for (int i = 0; i < Map.Width * Map.Height; i++)
                                {
                                    if (Map.Layers[layer].Tiles[i] == null || Map.Layers[layer].Tiles[i].TileType == TileType.Autotile) continue;
                                    int tilesetID = OldMap.TilesetIDs[Map.Layers[layer].Tiles[i].Index];
                                    if (!Map.TilesetIDs.Contains(tilesetID))
                                    {
                                        Map.Layers[layer].Tiles[i] = null;
                                    }
                                    else Map.Layers[layer].Tiles[i].Index = Map.TilesetIDs.IndexOf(tilesetID);
                                }
                            }
                            Continue();
                        }
                        else if (msg.Result == 1) // Cancel
                        {
                            UnsavedChanges = false;
                            UpdateMapViewer = false;
                        }
                    };
                }
                else
                {
                    for (int layer = 0; layer < Map.Layers.Count; layer++)
                    {
                        for (int i = 0; i < Map.Width * Map.Height; i++)
                        {
                            if (Map.Layers[layer].Tiles[i] == null || Map.Layers[layer].Tiles[i].TileType == TileType.Autotile) continue;
                            int tilesetID = OldMap.TilesetIDs[Map.Layers[layer].Tiles[i].Index];
                            if (!Map.TilesetIDs.Contains(tilesetID))
                            {
                                throw new Exception("Impossible-to-reach code has been reached.");
                            }
                            else Map.Layers[layer].Tiles[i].Index = Map.TilesetIDs.IndexOf(tilesetID);
                        }
                    }
                    Continue();
                }
            }
            if (!tilesetschanged) Continue();
        }

        public void Cancel(BaseEventArgs e)
        {
            Close();
        }
    }

    public class GroupBox : Widget
    {
        public GroupBox(IContainer Parent) : base(Parent)
        {
            Sprites["bg"] = new Sprite(this.Viewport);
        }

        public override void SizeChanged(BaseEventArgs e)
        {
            base.SizeChanged(e);
            if (Sprites["bg"].Bitmap != null) Sprites["bg"].Bitmap.Dispose();
            Sprites["bg"].Bitmap = new Bitmap(Size);
            Sprites["bg"].Bitmap.Unlock();
            Sprites["bg"].Bitmap.DrawRect(Size, 59, 91, 124);
            Sprites["bg"].Bitmap.DrawRect(1, 1, Size.Width - 2, Size.Height - 2, 17, 27, 38);
            Sprites["bg"].Bitmap.FillRect(2, 2, Size.Width - 4, Size.Height - 4, 24, 38, 53);
            Sprites["bg"].Bitmap.Lock();
        }
    }
}
