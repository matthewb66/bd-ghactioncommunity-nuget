// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Core.UnitTests;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.UnitTests;
using NUnit.Framework;

namespace Microsoft.Maui.Controls.Xaml.UnitTests
{
	public partial class Gh7494 : ContentPage
	{
		public Gh7494() => InitializeComponent();
		public Gh7494(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			[SetUp] public void Setup() => DispatcherProvider.SetCurrent(new DispatcherProviderStub());
			[TearDown] public void TearDown() => DispatcherProvider.SetCurrent(null);

			[Test]
			public void TemplateBindingInSpans([Values(false, true)] bool useCompiledXaml)
			{
				var layout = new Gh7494(useCompiledXaml);
				var view = layout.Content as Gh7494Content;
				var templatedLabel = ((StackLayout)view.Children[0]).Children[0] as Label;

				Assert.That(templatedLabel.FormattedText.Spans[0].Text, Is.EqualTo(view.Title));
			}
		}
	}

	public class Gh7494Content : ContentView
	{
		public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(Gh7494Content), default(string));

		public string Title
		{
			get => (string)GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
		}
	}
}
