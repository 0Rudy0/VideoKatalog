﻿#pragma checksum "..\..\..\HomeVideo\CategoryForm.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "CBD24BF6DDA442DDCF72B16B77A038224859EA11"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Video_katalog {
    
    
    /// <summary>
    /// CategoryForm
    /// </summary>
    public partial class CategoryForm : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\..\HomeVideo\CategoryForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox categoriesListBox;
        
        #line default
        #line hidden
        
        
        #line 8 "..\..\..\HomeVideo\CategoryForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button addNewCategoryBTN;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\HomeVideo\CategoryForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button removeCategoryBTN;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\HomeVideo\CategoryForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button editCategoryBTN;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VideoKatalog.View;component/homevideo/categoryform.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\HomeVideo\CategoryForm.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.categoriesListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 2:
            this.addNewCategoryBTN = ((System.Windows.Controls.Button)(target));
            
            #line 8 "..\..\..\HomeVideo\CategoryForm.xaml"
            this.addNewCategoryBTN.Click += new System.Windows.RoutedEventHandler(this.addNewCategoryBTN_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.removeCategoryBTN = ((System.Windows.Controls.Button)(target));
            
            #line 11 "..\..\..\HomeVideo\CategoryForm.xaml"
            this.removeCategoryBTN.Click += new System.Windows.RoutedEventHandler(this.removeCategoryBTN_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.editCategoryBTN = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\..\HomeVideo\CategoryForm.xaml"
            this.editCategoryBTN.Click += new System.Windows.RoutedEventHandler(this.editCategoryBTN_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

