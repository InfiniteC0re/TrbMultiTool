﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TrbMultiTool.FileFormats;
using static TrbMultiTool.FileFormats.PProperty;

namespace TrbMultiTool
{
    /// <summary>
    /// Interaction logic for QuestWindow.xaml
    /// </summary>
    public partial class PPropertyWindow : Window
    {
        public List<PProperty.TypeContent> TypeContentss { get; set; } = new();

        public List<PProperty.TypeContent> TypeContentsWithString { get; set; } = new();

        public PPropertyWindow()
        {
            InitializeComponent();
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tvi = (TreeViewItem)treeView.SelectedItem;
            if (tvi.Tag != null)
            {
                var tag = tvi.Tag as TypeContent;
                label.Content = tag.Type;
                textBox.Text = tag.Value;
            }
        }

        private bool CheckBTEC()
        {
            if (Trb.Tsfl.Sect.Label == "SECC")
            {
                MessageBox.Show("This file is BTEC encoded, so you should decode it before editing", "BTEC File Warning");
                return false;
            }

            return true;
        }

        private void swapButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckBTEC() || comboBox.SelectedIndex == -1) return;
            var typeContentCb = TypeContentsWithString[comboBox.SelectedIndex];
            var tvi = (TreeViewItem)treeView.SelectedItem;
            var tag = tvi.Tag as TypeContent;
            using BinaryWriter writer = new BinaryWriter(File.Open(Trb._fileName, FileMode.Open, FileAccess.ReadWrite));
            writer.BaseStream.Seek(Trb.Tsfl.Sect.Offset + tag.PointerPos, SeekOrigin.Begin);
            writer.Write((uint)typeContentCb.Offset);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckBTEC()) return;
            var tvi = (TreeViewItem)treeView.SelectedItem;
            var tag = tvi.Tag as TypeContent;
            using BinaryWriter writer = new BinaryWriter(File.Open(Trb._fileName, FileMode.Open, FileAccess.ReadWrite));
            writer.BaseStream.Seek(Trb.Tsfl.Sect.Offset + tag.Offset, SeekOrigin.Begin);
            switch (tag.Type)
            {
                case PProperty.Type.Int:
                    writer.Write(Convert.ToUInt32(textBox.Text));
                    break;
                case PProperty.Type.Unknown:
                    break;
                case PProperty.Type.Float:
                    writer.Write(Convert.ToSingle(textBox.Text));
                    break;
                case PProperty.Type.Bool:
                    writer.Write(Convert.ToBoolean(textBox.Text) ? 1 : 0);
                    break;
                case PProperty.Type.SubItem:
                    break;
                case PProperty.Type.Unknown2:
                    break;
                case PProperty.Type.Player:
                    break;
                case PProperty.Type.String:
                    if (textBox.Text.Length > tag.Value.Length)
                    {
                        MessageBox.Show("Your inputted string is too large!");
                    }
                    else
                    {
                        //var allText = FindVisualChildren<TreeViewItem>(treeView);
                        var remainingLength = tag.Value.Length - textBox.Text.Length;
                        var charArr = textBox.Text.ToCharArray();
                        for (int i = 0; i < remainingLength; i++)
                        {
                            charArr = charArr.Append('\0').ToArray();
                        }
                        writer.Write(charArr);
                    }
                    break;
                case PProperty.Type.UInt:
                    writer.Write(Convert.ToUInt32(textBox.Text));
                    break;
                default:
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TypeContentsWithString = TypeContentss.Where(x => x.Type == PProperty.Type.String).GroupBy(p => p.Value).Select(grp => grp.FirstOrDefault()).ToList();
            var tCWS2 = TypeContentsWithString.Select(x => x.Value);
            comboBox.Items.Clear();
            foreach (var item in tCWS2)
            {
                comboBox.Items.Add(item);
            }
        }

        private void comboBox_DropDownOpened(object sender, EventArgs e)
        {
            //TypeContentsWithString = TypeContentss.Where(x => x.Type == Quest.Type.String).Distinct().ToList();
            //var tCWS2 = TypeContentsWithString.Select(x => x.Value);
            //comboBox.Items.Clear();
            //foreach (var item in tCWS2)
            //{
            //    comboBox.Items.Add(item);
            //}
        }

        //IEnumerable<TreeViewItem> Collect(TreeView nodes)
        //{
        //    foreach (TreeViewItem node in nodes)
        //    {
        //        yield return node;

        //        foreach (var child in Collect(node.Items))
        //            yield return child;
        //    }
        //}
    }
}
