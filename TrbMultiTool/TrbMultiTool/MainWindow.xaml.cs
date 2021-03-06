﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TrbMultiTool;
using TrbMultiTool.FileFormats;

namespace TrbMultiTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        private TSFLFileDialog fileDialog = new();

		public MainWindow()
		{
			InitializeComponent();
			AddGamesToComboBox();
		}

		private void AddGamesToComboBox()
		{
			ChooseGameComboBox.Items.Add(Game.Barnyard);
			ChooseGameComboBox.Items.Add(Game.NicktoonsUnite);
			ChooseGameComboBox.Items.Add(Game.NicktoonsBattleForVolcanoIsland);
			ChooseGameComboBox.Items.Add(Game.NicktoonsAttackOfTheToybots);
			ChooseGameComboBox.Items.Add(Game.ElTigre);
			ChooseGameComboBox.Items.Add(Game.MarvelSuperHeroSquad);
			ChooseGameComboBox.Items.Add(Game.DeBlob);
			ChooseGameComboBox.SelectedIndex = 0;
		}

		private void ChooseGameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            switch ((Game)ChooseGameComboBox.SelectedIndex)
            {
                case Game.Barnyard:
                    ImageBox.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/barnyard.png"));
                    break;
                case Game.NicktoonsUnite:
                    ImageBox.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/nicktoonsunite.png"));
                    break;
                case Game.NicktoonsBattleForVolcanoIsland:
					ImageBox.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/nbfvi.jpg"));
					break;
                case Game.NicktoonsAttackOfTheToybots:
					ImageBox.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/naott.jfif"));
					break;
                case Game.ElTigre:
					ImageBox.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/eltigre.jpg"));
					break;
                case Game.MarvelSuperHeroSquad:
					ImageBox.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/mshs.jpg"));
					break;
                case Game.DeBlob:
					ImageBox.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/deblob.jpg"));
					break;
                default:
                    break;
            }
        }

        private async void ChooseFileButton_Click(object sender, RoutedEventArgs e)
		{
			if (fileDialog.Open())
			{
				loadingIcon.Visibility = Visibility.Visible;
				var game = (Game)ChooseGameComboBox.SelectedIndex;
				var trb = await Task.Run(() => new Trb(fileDialog.fileName, game));

                if (trb.finishedLoading)
                {
                    loadingIcon.Visibility = Visibility.Hidden;

					if (trb.ttls.Any() || trb.ttexes.Any())
                    {
						var TTLWindow = new TtlWindow(trb.ttls, trb.ttexes);
						TTLWindow.Show();
					}

					if (trb.tmdls.Any())
                    {
                        var TMDLWindow = new TmdlWindow(trb.tmdls);
                        TMDLWindow.Show();
                    }

					if(trb.ContainsXui)
                    {
						var window = new XuiWindow(trb.xuis);
						
						window.Show();
					}
                }
			}
		}

        private async void ExtractFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (fileDialog.Open())
            {
                loadingIcon.Visibility = Visibility.Visible;
                var game = (Game)ChooseGameComboBox.SelectedIndex;
                var trb = await Task.Run(() => new Trb(fileDialog.fileName, game, true));

                if (trb.finishedLoading)
                    loadingIcon.Visibility = Visibility.Hidden;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
			Environment.Exit(0);
        }

        private void createLocaleFile_Click(object sender, RoutedEventArgs e)
        {
			LocaleStringsFile file = new();
			OpenFileDialog fd = new();
			fd.FileName = "LocaleStrings.txt";
			fd.Filter = $"Text File|*.txt";

			if (fd.ShowDialog() == true && !string.IsNullOrWhiteSpace(fd.FileName))
            {
				Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
				dlg.FileName = "eng-uk.trb";
				dlg.Filter = $"TRB File|*.trb";

				if (dlg.ShowDialog() == true && !string.IsNullOrWhiteSpace(dlg.FileName))
                {
					string[] lines = File.ReadAllLines(fd.FileName);

					foreach (string str in lines)
					{
						file.AddString(str.Replace("\\n", "\n"));
					}

					file.GenerateFile(dlg.FileName);
				}
            }
		}
    }
}
