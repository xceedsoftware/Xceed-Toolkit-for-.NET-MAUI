# Xceed is excited to introduce its new Toolkit for MAUI! 

This toolkit is an open-source and free version that includes additional controls and features to supplement the existing "basic controls". Our aim is to bridge the gap and provide developers with the tools they need to create exceptional user experiences.

For those familiar with WPF, this toolkit will feel familiar, and we've made every effort to create a seamless transition for you. Our goal is to provide a comfortable and intuitive experience, making it easy to get started and get productive quickly.

Currently, the toolkit includes a range of controls, with more to come in the future. These controls include an improved Border, Button, RepeatButton, ToggleButton, ContentControl Card control which includes 48 accent colors for each control. We're committed to continually improving and updating the toolkit to meet the evolving needs of developers.

Thank you for considering Xceed's Toolkit for MAUI. We're excited to see what you create with it! Meanwhile, here’s more information on the controls we provide: 

### Border

One of the issues we have with the basic border control offered in MAUI is the lack of having independent corner radius as an example, we fixed that, here’s a list of additional features: 

-	A BorderBrush property of type Brush, instead of Stroke of type Brush.
-	A BorderThickness property of type Thickness, instead of StrokeThickness of type double (for 
independent borders).
-	A CornerRadius property of type CornerRadius (for rounded corners).
-	Fewer border properties (StrokeDashArray, StrokeLineCap, etc.).
-	The Content's Labels benefit from the FontSize, FontAttributes, FontFamily, and TextColor properties.

### Button

The default .NET MAUI Framework Button did not include some essential features such as MouseOver and pressed styles, and did not allow for the addition of any content, such as images. We have addressed these limitations with our custom Button implementation, which includes the following features:

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


