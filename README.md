### Table of Contents
- [Welcome](#introduction)
- [Getting Started](#getting-started)

#### Controls:

- [Border](#border)
- [Button](#button)
- [RepeatButton](#repeatbutton)
- [ContentControl](#contentcontrol)
- [ButtonSpinner](#buttonspinner)
- [Card](#card)

<a id="introduction"></a>
# Xceed is excited to introduce its new Toolkit for MAUI! 

We're thrilled to introduce our new Toolkit for MAUI, an open-source and free version that includes additional controls and features to supplement the existing "basic controls". Our goal is to provide developers with the tools they need to create exceptional user experiences.

Our toolkit is designed with developers in mind, and we've taken care to make it as user-friendly and intuitive as possible. Whether you're an experienced developer or just getting started, you'll find that our controls and features are easy to use and integrate seamlessly into your projects.

With MAUI's cross-platform capabilities, you can develop applications that work seamlessly on Windows, Android, Mac, and iOS, without the need for separate code bases. We're committed to ensuring that our toolkit is compatible with all MAUI-supported platforms and that the experience is identical across all of them.

For those familiar with WPF, this toolkit will feel familiar, and we've made every effort to create a seamless transition for you. Our goal is to provide a comfortable and intuitive experience, making it easy to get started and get productive quickly.

Currently, the toolkit includes a range of controls, with more to come in the future. These controls include an improved Border, Button, RepeatButton, ToggleButton, ContentControl, Card control, all these controls includes 48 accent colors for each control. We're committed to continually improving and updating the toolkit to meet the evolving needs of developers.

Thank you for considering Xceed's Toolkit for MAUI. We're excited to see what you create with it! Meanwhile, here’s more information on the controls we provide: 
<a id="border"></a>
### Border

One of the issues we have with the basic border control offered in MAUI is the lack of having independent corner radius as an example, we fixed that, here’s a list of additional features: 

-	A BorderBrush property of type Brush, instead of Stroke of type Brush.
-	A BorderThickness property of type Thickness, instead of StrokeThickness of type double (for 
independent borders).
-	A CornerRadius property of type CornerRadius (for rounded corners).
-	Fewer border properties (StrokeDashArray, StrokeLineCap, etc.).
-	The Content's Labels benefit from the FontSize, FontAttributes, FontFamily, and TextColor properties.

<a id="button"></a>
### Button

![Alt Text](multistatebutton.png)

The default .NET MAUI Framework Button did not include some essential features such as MouseOver and pressed styles (as shown in the screenshot above), and did not allow for the addition of any content, such as images. We have addressed these limitations with our custom Button implementation, which includes the following features:

- BorderBrush (type Brush) for more flexibility than BorderColor of type Color.
- BorderThickness (type Thickness) to allow for independent borders rather than BorderWidth of type double.
- ClickMode to trigger the Clicked event on a Press or Release.
- Content (type object) to display the user's View or a Label if they pass a string.
- CornerRadius (type CornerRadius) for independently rounded corners rather than int.
- HorizontalContentAlignment (type LayoutOptions) to align content horizontally.
- VerticalContentAlignment (type LayoutOptions) to align content vertically.
- ContentTemplate (from ContentControl) to configure the content.
- Labels in Content can use properties such as FontSize, FontAttributes, FontFamily, and TextColor.
- PointerOver and PointerPressed (which are already present with Pressed).

Note: The BorderColor property has been removed as Background already exists.

With our custom Button implementation, you can create highly flexible and customizable buttons for your .NET MAUI apps.

<a id="repeatbutton"></a>
### RepeatButton

We have created a new control called RepeatButton, which is derived from the Xceed Button. This control includes the Delay and Interval properties, which allow the Clicked event to be raised more than once.

The RepeatButton control is similar to a standard button, but it provides the additional functionality of allowing the Clicked event to be raised repeatedly while the button is pressed. The Delay and Interval properties control the amount of time before the first repeated Clicked event is raised, and the frequency of subsequent events, respectively.

With the RepeatButton control, you can create more responsive and interactive user interfaces in your .NET MAUI apps.

<a id="contentcontrol"></a>
### ContentControl

We have created a new control called ContentControl, which allows you to set the content and its associated DataTemplate.

The ContentControl is a versatile control that can display any type of content, including text, images, and other controls. By setting the Content property, you can specify the content to be displayed, and by setting the ContentTemplate property, you can specify how the content should be displayed.

The DataTemplate allows you to define a layout for the content, including styles and formatting. By using the ContentControl in conjunction with DataTemplates, you can create highly customized and dynamic user interfaces in your .NET MAUI apps.

<a id="buttonspinner"></a>
### ButtonSpinner

We have created a new control called ButtonSpinner, which is derived from the ContentControl. This control allows you to display content along with two RepeatButtons (Spinners) and includes several useful properties and events.

The ButtonSpinner control includes the following properties:

- AllowSpin: controls whether the Spinners are allowed to raise events.
- SpinnerDownContentTemplate and SpinnerUpContentTemplate: specify the DataTemplates for the Spinners.
- SpinnerLocation: determines the location of the Spinners (either left or right of the content).
- ValidSpinDirection: specifies the allowed spin direction (either up or down).
- In addition to these properties, the ButtonSpinner control raises a Spin event when the RepeatButtons are clicked or held down. This event can be used to respond to user input and update the content as needed.

With the ButtonSpinner control, you can create highly interactive and customizable UI components in your .NET MAUI apps.

<a id="card"></a>
### Card

We have created a new control called the Card Control, which provides a visually appealing way to display content in your .NET MAUI apps.

The Card Control is a frame with a subtle shadow effect, which creates the illusion of depth and makes the content stand out from the background. This makes it an ideal choice for displaying important information or creating a visually striking user interface.

With the Card Control, you can easily add depth and visual interest to your .NET MAUI apps without the need for complex styling or layout. Simply add the control to your XAML code and specify the content to be displayed within it.

<a id="getting-started"></a>
# Getting Started

The first thing you will want to do is get the package on NuGet or add simply clone the project and add it to your references.
Once this is done, you will need to add a reference to the Xceed namespace : 

```
xmlns:xctk="clr-namespace:Xceed.Maui.Toolkit;assembly=Xceed.Maui.Toolkit"
```

Then, you will need to add our ResourceDictionary to your app or page :

````xaml
<Application.Resources>
    <ResourceDictionary>
      <ResourceDictionary.MergedDictionaries>
        <xctk:FluentDesign AccentColor="Pink" />
      </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
  </Application.Resources>
````

You can also add the following line in your Application declaration : 

````xaml
UserAppTheme="Dark"
````

This can also be done through code:

````csharp
Application.Current.UserAppTheme = AppTheme.Dark;
`````

If you wish to offer your users to switch between Light and Dark, you can use this line of code:

`````csharp
Application.Current.UserAppTheme = ( Application.Current.UserAppTheme == AppTheme.Dark) ? AppTheme.Light : AppTheme.Dark;
``````

This will cycle between the light and dark theme.

