﻿using System;
using System.Collections.Generic;
using MKEditor.Game;
using ODL;

namespace MKEditor.Widgets
{
    public class MapSelectPanel : Widget
    {
        public Container allmapcontainer;
        public TreeView mapview;

        public MapSelectPanel(object Parent, string Name = "mapSelectTab")
            : base(Parent, Name)
        {
            Label Header = new Label(this);
            Header.SetText("All Maps");
            Header.SetFont(Font.Get("Fonts/Ubuntu-B", 16));
            Header.SetPosition(5, 5);

            Sprites["sep"] = new Sprite(this.Viewport, new SolidBitmap(288, 2, new Color(28, 50, 73)));
            Sprites["sep"].Y = 30;

            Sprites["bar"] = new Sprite(this.Viewport, new SolidBitmap(1, Size.Height - 30, new Color(28, 50, 73)));
            Sprites["bar"].Y = 30;

            allmapcontainer = new Container(this);
            allmapcontainer.SetPosition(0, 35);
            allmapcontainer.VAutoScroll = true;

            VScrollBar vs = new VScrollBar(this);
            allmapcontainer.SetVScrollBar(vs);

            mapview = new TreeView(allmapcontainer);
            mapview.SetWidth(212);
            mapview.OnSelectedNodeChanged += delegate (object sender, MouseEventArgs e)
            {
                SetMap(mapview.SelectedNode.Object as Map, true);
            };
            mapview.TrailingBlank = 64;
            allmapcontainer.SetContextMenuList(new List<IMenuItem>()
            {
                new MenuItem("New Map")
                {
                    OnLeftClick = NewMap
                },
                new MenuItem("Edit Map")
                {
                    OnLeftClick = EditMap,
                    IsClickable = delegate (object sender, ConditionEventArgs e)
                    {
                        e.ConditionValue = mapview.HoveringNode != null;
                    }
                },
                new MenuSeparator(),
                new MenuItem("Delete")
                {
                    OnLeftClick = DeleteMap,
                    Shortcut = "Del",
                    IsClickable = delegate (object sender, ConditionEventArgs e)
                    {
                        e.ConditionValue = mapview.HoveringNode != null && mapview.Nodes.Count > 1;
                    }
                }
            });
            OnWidgetSelected += WidgetSelected;
        }

        public List<TreeNode> PopulateList(List<object> Maps, bool first = false)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            for (int i = (first ? 0 : 2); i < Maps.Count; i++)
            {
                if (Maps[i] is int)
                {
                    nodes.Add(new TreeNode() { Object = Data.Maps[(int) Maps[i]] });
                    Data.Maps[(int) Maps[i]].Added = true;
                }
                else
                {
                    List<object> list = (List<object>) Maps[i];
                    TreeNode n = new TreeNode();
                    n.Object = Data.Maps[(int) list[0]];
                    n.Collapsed = (bool) list[1];
                    Data.Maps[(int) list[0]].Added = true;
                    n.Nodes = PopulateList(list);
                    nodes.Add(n);
                }
            }
            if (first)
            {
                foreach (KeyValuePair<int, Map> kvp in Data.Maps)
                {
                    if (!kvp.Value.Added)
                    {
                        nodes.Add(new TreeNode() { Object = kvp.Value });
                        kvp.Value.Added = true;
                        Editor.ProjectSettings.MapOrder.Add(kvp.Key);
                    }
                }
                mapview.SetNodes(nodes);
            }
            return nodes;
        }

        public void SetMap(Map Map, bool CalledFromTreeView = false)
        {
            Editor.MainWindow.MapWidget.SetMap(Map);
            Editor.ProjectSettings.LastMapID = Map.ID;
            Editor.ProjectSettings.LastLayer = 0;
            if (!CalledFromTreeView) // Has yet to update the selection
            {
                TreeNode node = null;
                for (int i = 0; i < mapview.Nodes.Count; i++)
                {
                    if ((mapview.Nodes[i].Object as Map).ID == Map.ID)
                    {
                        node = mapview.Nodes[i];
                        break;
                    }
                    else
                    {
                        TreeNode n = mapview.Nodes[i].FindNode(n => (n.Object as Map).ID == Map.ID);
                        if (n != null)
                        {
                            node = n;
                            break;
                        }
                    }
                }
                mapview.SetSelectedNode(node, false);
            }
        }

        public override void SizeChanged(object sender, SizeEventArgs e)
        {
            base.SizeChanged(sender, e);
            allmapcontainer.SetSize(this.Size.Width - 11, this.Size.Height - allmapcontainer.Position.Y);
            Sprites["bar"].X = Size.Width - 11;
            (Sprites["bar"].Bitmap as SolidBitmap).SetSize(1, Size.Height - 30);
            allmapcontainer.VScrollBar.SetPosition(Size.Width - 9, 33);
            allmapcontainer.VScrollBar.SetSize(8, Size.Height - 35);
        }

        private void NewMap(object sender, MouseEventArgs e)
        {
            Editor.UnsavedChanges = true;
            Map Map = new Map();
            Map.ID = Editor.GetFreeMapID();
            Map.DevName = "Untitled Map";
            Map.DisplayName = "Untitled Map";
            Map.SetSize(15, 15);
            MapPropertiesWindow mpw = new MapPropertiesWindow(Map, this.Window);
            mpw.OnClosed += delegate (object _, EventArgs ev)
            {
                if (mpw.UpdateMapViewer)
                {
                    Editor.AddMap(Map, mapview.HoveringNode == null ? 0 : (mapview.HoveringNode.Object as Map).ID);
                }
            };
        }

        public bool SetCollapsed(List<object> collection, int MapID, bool IsCollapsed)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                object o = collection[i];
                if (o is List<object>)
                {
                    List<object> sub = o as List<object>;
                    if ((int) sub[0] == MapID)
                    {
                        (collection[i] as List<object>)[1] = IsCollapsed;
                    }
                    else
                    {
                        SetCollapsed(collection[i] as List<object>, MapID, IsCollapsed);
                    }
                }
            }
            return false;
        }

        private void EditMap(object sender, MouseEventArgs e)
        {
            Map map = mapview.SelectedNode.Object as Map;
            MapPropertiesWindow mpw = new MapPropertiesWindow(map, this.Window);
            mpw.OnClosed += delegate (object _, EventArgs ev)
            {
                if (mpw.UnsavedChanges) Editor.UnsavedChanges = true;
                if (mpw.UpdateMapViewer)
                {
                    Editor.MainWindow.MapWidget.SetMap(map);
                }
            };
        }

        private void DeleteMap(object sender, MouseEventArgs e)
        {
            if (mapview.Nodes.Count <= 1) return;
            string message = "Are you sure you want to delete this map?";
            if (mapview.HoveringNode.Nodes.Count > 0) message += " All of its children will also be deleted.";
            MessageBox confirm = new MessageBox("Warning", message, ButtonTypes.YesNoCancel);
            confirm.OnClosed += delegate (object s, EventArgs ev)
            {
                if (confirm.Result == 0) // Yes
                {
                    Editor.UnsavedChanges = true;
                    DeleteMapRecursively(mapview.HoveringNode);
                    for (int i = 0; i < mapview.Nodes.Count; i++)
                    {
                        if (mapview.Nodes[i] == mapview.HoveringNode)
                        {
                            mapview.Nodes.RemoveAt(i);
                            mapview.SetSelectedNode(i >= mapview.Nodes.Count ? mapview.Nodes[i - 1] : mapview.Nodes[i]);
                        }
                        else if (mapview.Nodes[i].ContainsNode(mapview.HoveringNode))
                        {
                            mapview.SetSelectedNode(mapview.Nodes[i].RemoveNode(mapview.HoveringNode));
                        }
                    }
                    RemoveID(Editor.ProjectSettings.MapOrder, (mapview.HoveringNode.Object as Map).ID, true);
                    mapview.Redraw();
                }
            };
        }

        private void DeleteMapRecursively(TreeNode node)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                DeleteMapRecursively(node.Nodes[i]);
            }
            Data.Maps.Remove((node.Object as Map).ID);
        }

        private void RemoveID(List<object> collection, int ID, bool first = false)
        {
            for (int i = (first ? 0 : 2); i < collection.Count; i++)
            {
                if (collection[i] is bool) continue;
                else if (collection[i] is int)
                {
                    if ((int) collection[i] == ID)
                    {
                        collection.RemoveAt(i);
                        break;
                    }
                }
                else
                {
                    List<object> sub = (List<object>) collection[i];
                    if (sub[0] is int && (int) sub[0] == ID)
                    {
                        collection.RemoveAt(i);
                    }
                    else if (sub.Count == 3 && sub[2] is int && (int) sub[2] == ID)
                    {
                        collection[i] = (int) sub[0];
                    }
                    else
                    {
                        RemoveID(sub, ID);
                    }
                }
            }
        }
    }
}