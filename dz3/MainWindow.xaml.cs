using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;

namespace dz3 {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private List<string> foundPhotos;
		private int lastPhoto;
		private string apiKey = "37798550-bca0697702d7a21055b40961c";
		public MainWindow() {
			InitializeComponent();
			foundPhotos = new List<string>();
			lastPhoto = 0;
			leftButton.IsEnabled = false;
			rightButton.IsEnabled = false;
		}

		private async void search_button_Click(object sender, RoutedEventArgs e) {
			grid2.Children.Clear();
			lastPhoto = 0;
			foundPhotos.Clear();
			leftButton.IsEnabled = false;
			string url = $"https://pixabay.com/api/?key={apiKey}&q={Uri.EscapeDataString(textBox.Text)}";

			using (HttpClient client = new HttpClient()) {
				HttpResponseMessage response = await client.GetAsync(url);

				if (response.IsSuccessStatusCode) {
					string jsonString = await response.Content.ReadAsStringAsync();

					JObject jsonObject = JObject.Parse(jsonString);

					IEnumerable<JToken> fieldValues = jsonObject.SelectTokens($"$..largeImageURL");

					foreach (JToken fieldValue in fieldValues) {
						foundPhotos.Add(fieldValue.ToString());
					}
					if (foundPhotos.Count == 0) {
						MessageBox.Show("Картинок по такому запросу не знайдено");
						return;
					}
					int index = 0;
					for (int row = 0; row < 3; row++) {
						for (int column = 0; column < 5; column++) {
							if (index == foundPhotos.Count - 1)
								break;
							System.Windows.Controls.Image image = new System.Windows.Controls.Image { Height = 100, Width = 100 };
							BitmapImage bitmap = new BitmapImage();

							bitmap.BeginInit();
							bitmap.UriSource = new Uri(foundPhotos[index], UriKind.Absolute);
							bitmap.EndInit();

							image.Source = bitmap;
							Grid.SetRow(image, row);
							Grid.SetColumn(image, column);
							grid2.Children.Add(image);
							index++;
						}
					}
					if (foundPhotos.Count > 15) {
						rightButton.IsEnabled = true;
					}
				}
				else {
					MessageBox.Show("Ошибка HTTP-запроса: " + response.StatusCode);
				}
			}
		}

		private void left_button_Click(object sender, RoutedEventArgs e) {
			lastPhoto -= 15;
			grid2.Children.Clear();

			int index = lastPhoto;
			for (int row = 0; row < 3; row++) {
				for (int column = 0; column < 5; column++) {
					if (index == foundPhotos.Count - 1)
						break;
					System.Windows.Controls.Image image = new System.Windows.Controls.Image { Height = 100, Width = 100 };
					BitmapImage bitmap = new BitmapImage();

					bitmap.BeginInit();
					bitmap.UriSource = new Uri(foundPhotos[index], UriKind.Absolute);
					bitmap.EndInit();

					image.Source = bitmap;
					Grid.SetRow(image, row);
					Grid.SetColumn(image, column);
					grid2.Children.Add(image);
					index++;
				}
			}

			rightButton.IsEnabled = true;
			if (lastPhoto == 0)
				leftButton.IsEnabled = false;
		}

		private void right_button_Click(object sender, RoutedEventArgs e) {
			lastPhoto += 15;
			grid2.Children.Clear();

			int index = lastPhoto;
			for (int row = 0; row < 3; row++) {
				for (int column = 0; column < 5; column++) {
					if (index == foundPhotos.Count - 1)
						break;
					System.Windows.Controls.Image image = new System.Windows.Controls.Image { Height = 100, Width = 100 };
					BitmapImage bitmap = new BitmapImage();

					bitmap.BeginInit();
					bitmap.UriSource = new Uri(foundPhotos[index], UriKind.Absolute);
					bitmap.EndInit();

					image.Source = bitmap;
					Grid.SetRow(image, row);
					Grid.SetColumn(image, column);
					grid2.Children.Add(image);
					index++;
				}
			}

			leftButton.IsEnabled = true;
			if (foundPhotos.Count - lastPhoto < 15)
				rightButton.IsEnabled = false;
		}
	}
}
