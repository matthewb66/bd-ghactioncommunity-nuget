using Microsoft.Maui.Controls;
using NUnit.Framework;

namespace Microsoft.Maui.Controls.Xaml.UnitTests
{
	public partial class Issue3090 : ContentPage
	{
		public Issue3090()
		{
			InitializeComponent();
		}

		public Issue3090(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		public class Tests
		{
			[TestCase(false)]
			[TestCase(true)]
			public void NewDoesNotThrow(bool useCompiledXaml)
			{
				var p = new Issue3090(useCompiledXaml);
			}
		}
	}
}