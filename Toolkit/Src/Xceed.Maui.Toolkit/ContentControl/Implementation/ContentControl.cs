/***************************************************************************************
 
  Xceed Toolkit for MAUI is a multiplatform toolkit offered by Xceed Software Inc., 
  makers of the popular WPF Toolkit.

  COPYRIGHT (C) 2023 Xceed Software Inc. ALL RIGHTS RESERVED.

  This program is provided to you under the terms of a Xceed Community License 
  For more details about the Xceed Community License please visit the products GitHub or NuGet page .

  DISCLAIMER: This code is provided as-is and without warranty of any kind, express or implied. The 
  author(s) and owner(s) of this code shall not be liable for any damages or losses resulting from 
  the use or inability to use the code. 

 
  *************************************************************************************/


using System.ComponentModel;

namespace Xceed.Maui.Toolkit
{
  [DefaultProperty( "Content" )]
  [ContentProperty( "Content" )]
  public partial class ContentControl : Control
  {
    #region Public Properties

    #region Content

    public static readonly BindableProperty ContentProperty = BindableProperty.Create( nameof( Content ), typeof( object ), typeof( ContentControl ), propertyChanged: OnContentChanged );

    public object Content
    {
      get => ( object )GetValue( ContentProperty );
      set => SetValue( ContentProperty, value );
    }

    private static void OnContentChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var contentControl = bindable as ContentControl;
      if( contentControl != null )
      {
        contentControl.OnContentChanged( ( object )oldValue, ( object )newValue );
      }
    }

    protected virtual void OnContentChanged( object oldValue, object newValue )
    {
      this.UpdateView();

      if( (this.View != null) && ( newValue is View ) )
      {
        // Update this ContentControl's Children label's and Entry's Font properties, when not already set.
        Control.UpdateFontProperties( this.View, this );
      }
    }

    #endregion

    #region ContentTemplate

    public static readonly BindableProperty ContentTemplateProperty = BindableProperty.Create( nameof( ContentTemplate ), typeof( DataTemplate ), typeof( ContentControl ), null, propertyChanged: OnContentTemplateChanged );

    public DataTemplate ContentTemplate
    {
      get => ( DataTemplate )GetValue( ContentTemplateProperty );
      set => SetValue( ContentTemplateProperty, value );
    }

    private static void OnContentTemplateChanged( BindableObject bindable, object oldValue, object newValue )
    {
      var contentControl = bindable as ContentControl;
      if( contentControl != null )
      {
        contentControl.OnContentTemplateChanged( ( DataTemplate )oldValue, ( DataTemplate )newValue );
      }
    }

    protected virtual void OnContentTemplateChanged( DataTemplate oldValue, DataTemplate newValue )
    {
      this.UpdateView( true );

      if( ( this.View != null ) && ( this.Content is View ) )
      {
        // Update this ContentControl's Children label's and Entry's Font properties, when not already set.
        Control.UpdateFontProperties( this.View, this );
      }
    }

    #endregion

    #region View

    public static readonly BindableProperty ViewProperty = BindableProperty.Create( nameof( View ), typeof( View ), typeof( ContentControl ), null );

    public View View
    {
      get => ( View )GetValue( ViewProperty );
      private set => SetValue( ViewProperty, value );
    }

    #endregion

    #endregion

    #region Partial Methods

    partial void SetShapeSize( View view );

    #endregion

    #region Protected Methods


    protected override void OnTextColorChanged( Color oldValue, Color newValue )
    {
      base.OnTextColorChanged( oldValue, newValue );

      Control.UpdateFontTextColor( this.View ?? this, oldValue, newValue );
    }

    protected override void OnFontAttributesChanged( FontAttributes oldValue, FontAttributes newValue )
    {
      base.OnFontAttributesChanged( oldValue, newValue );

      Control.UpdateFontAttributes( this.View ?? this, oldValue, newValue );
    }

    protected override void OnFontFamilyChanged( string oldValue, string newValue )
    {
      base.OnFontFamilyChanged( oldValue, newValue );

      Control.UpdateFontFamily( this.View ?? this, oldValue, newValue );
    }

    protected override void OnFontSizeChanged( double oldValue, double newValue )
    {
      base.OnFontSizeChanged( oldValue, newValue );

      Control.UpdateFontSize( this.View ?? this, oldValue, newValue );
    }

    protected override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      // Bug if <xctk:FluentDesign /> is defined in PageContent.Resource :
      // The BindingContext is not applied to all the visual children when defined in a DataTemplate,
      // so we make sure it will when OnApplyTemplate is called. 
      // When <xctk:FluentDesign /> is defined in Application.Resources, there is no bug.
      if( this.Content != null )
      {
        var children = this.GetVisualTreeDescendants();
        foreach( var element in children )
        {
          var elementView = element as View;
          if( ( elementView != null ) && ( elementView.BindingContext == null ) )
          {
            elementView.BindingContext = this.Content;
          }
        }
      }
    }

    protected override void OnBindingContextChanged()
    {
      if( (this.View != null) && (this.View.BindingContext == null) )
      {
        this.View.BindingContext = this.BindingContext;
      }
    }

    #endregion

    #region Private Methods

    private void UpdateView( bool reloadContentTemplate = false )
    {
      View view = null;

      if( this.ContentTemplate == null )
      {
        view = this.Content as View;
        if( view == null )
        {
          if( this.Content != null )
          {
            if( this.View is Label labelView )
            {
              labelView.Text = this.Content.ToString();
              return;
            }
            else
            {
              view = this.CreateLabel();
            }
          }
        }
        else
        {
          var children = view.GetVisualTreeDescendants();
          foreach( var element in children )
          {
            if( element is View elementView )
            {
              this.SetShapeSize( elementView );
            }
          }
        }
      }
      else
      {
        var updatedView = reloadContentTemplate ? this.ContentTemplate.CreateContent() as View : this.View;
        if( updatedView != null )
        {
          var children = updatedView.GetVisualTreeDescendants();
          foreach( var element in children )
          {
            if( element is View elementView )
            {
              this.SetShapeSize( elementView );

              // Set the BindingContext on each children.
              elementView.BindingContext = this.Content;
            }
          }

          if( reloadContentTemplate )
          {
            view = updatedView;
          }
          else
            return;
        }
        else if( this.Content != null )
        {
          view = this.CreateLabel();
        }
      }

      this.View = view;
    }

    private Label CreateLabel()
    {
      return new Label()
      {
        Text = this.Content.ToString(),
        TextColor = this.TextColor,
        FontAttributes = this.FontAttributes,
        FontFamily = this.FontFamily,
        FontSize = this.FontSize,
      };
    }

    #endregion
  }
}
