using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.UnitTests;
using NUnit.Framework;

namespace Microsoft.Maui.Controls.Xaml.UnitTests
{
	public partial class Issue1306 : ListView
	{
		public Issue1306()
		{
			InitializeComponent();
		}

		public Issue1306(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		public class Tests
		{
			[SetUp] public void Setup() => DispatcherProvider.SetCurrent(new DispatcherProviderStub());
			[TearDown] public void TearDown() => DispatcherProvider.SetCurrent(null);

			[TestCase(false)]
			[TestCase(true)]
			public void AssignBindingMarkupToBindingBase(bool useCompiledXaml)
			{
				var listView = new Issue1306(useCompiledXaml);

				Assert.NotNull(listView.GroupDisplayBinding);
				Assert.NotNull(listView.GroupShortNameBinding);
				Assert.That(listView.GroupDisplayBinding, Is.TypeOf<Binding>());
				Assert.That(listView.GroupShortNameBinding, Is.TypeOf<Binding>());
				Assert.AreEqual("Key", (listView.GroupDisplayBinding as Binding).Path);
				Assert.AreEqual("Key", (listView.GroupShortNameBinding as Binding).Path);
			}
		}
	}
}

