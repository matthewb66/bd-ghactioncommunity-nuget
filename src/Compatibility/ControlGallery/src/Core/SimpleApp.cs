using System.Diagnostics;

namespace Microsoft.Maui.Controls.Compatibility.ControlGallery
{
#pragma warning disable CS0618 // Type or member is obsolete
	public class SimpleApp : Application
	{
		public SimpleApp()
		{
			var label = new Label { VerticalOptions = LayoutOptions.CenterAndExpand };

			if (Current.Properties.ContainsKey("LabelText"))
			{
				label.Text = (string)Current.Properties["LabelText"] + " Restored!";
				Debug.WriteLine("Initialized");
			}
			else
			{
				Current.Properties["LabelText"] = "Wowza";
				label.Text = (string)Current.Properties["LabelText"] + " Set!";
				Debug.WriteLine("Saved");
			}

			MainPage = new ContentPage
			{
				Content = new StackLayout
				{
					Children =
					{
						label
					}
				}
			};

			SerializeProperties();
		}

		static async void SerializeProperties()
		{
			await Current.SavePropertiesAsync();
		}
	}
#pragma warning restore CS0618 // Type or member is obsolete
}