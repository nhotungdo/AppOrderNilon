﻿#pragma checksum "..\..\..\..\Views\OrderManagementWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "187A5E85ACFC41613A38D07E49A99E11AD8C8180"
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
using System.Windows.Controls.Ribbon;
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


namespace AppOrderNilon.Views {
    
    
    /// <summary>
    /// OrderManagementWindow
    /// </summary>
    public partial class OrderManagementWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 70 "..\..\..\..\Views\OrderManagementWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtSearch;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\..\Views\OrderManagementWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbStatus;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\..\..\Views\OrderManagementWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker dpFromDate;
        
        #line default
        #line hidden
        
        
        #line 107 "..\..\..\..\Views\OrderManagementWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker dpToDate;
        
        #line default
        #line hidden
        
        
        #line 134 "..\..\..\..\Views\OrderManagementWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dgOrders;
        
        #line default
        #line hidden
        
        
        #line 224 "..\..\..\..\Views\OrderManagementWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtTotalOrders;
        
        #line default
        #line hidden
        
        
        #line 231 "..\..\..\..\Views\OrderManagementWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtPendingOrders;
        
        #line default
        #line hidden
        
        
        #line 238 "..\..\..\..\Views\OrderManagementWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtTotalRevenue;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.18.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/AppOrderNilon;component/views/ordermanagementwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\OrderManagementWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.18.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 43 "..\..\..\..\Views\OrderManagementWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BackToDashboard_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 50 "..\..\..\..\Views\OrderManagementWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Logout_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.txtSearch = ((System.Windows.Controls.TextBox)(target));
            
            #line 75 "..\..\..\..\Views\OrderManagementWindow.xaml"
            this.txtSearch.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.Search_TextChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.cmbStatus = ((System.Windows.Controls.ComboBox)(target));
            
            #line 87 "..\..\..\..\Views\OrderManagementWindow.xaml"
            this.cmbStatus.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.StatusFilter_Changed);
            
            #line default
            #line hidden
            return;
            case 5:
            this.dpFromDate = ((System.Windows.Controls.DatePicker)(target));
            
            #line 103 "..\..\..\..\Views\OrderManagementWindow.xaml"
            this.dpFromDate.SelectedDateChanged += new System.EventHandler<System.Windows.Controls.SelectionChangedEventArgs>(this.DateFilter_Changed);
            
            #line default
            #line hidden
            return;
            case 6:
            this.dpToDate = ((System.Windows.Controls.DatePicker)(target));
            
            #line 110 "..\..\..\..\Views\OrderManagementWindow.xaml"
            this.dpToDate.SelectedDateChanged += new System.EventHandler<System.Windows.Controls.SelectionChangedEventArgs>(this.DateFilter_Changed);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 122 "..\..\..\..\Views\OrderManagementWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AddOrder_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.dgOrders = ((System.Windows.Controls.DataGrid)(target));
            
            #line 140 "..\..\..\..\Views\OrderManagementWindow.xaml"
            this.dgOrders.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.Order_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 12:
            this.txtTotalOrders = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 13:
            this.txtPendingOrders = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 14:
            this.txtTotalRevenue = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.18.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 9:
            
            #line 196 "..\..\..\..\Views\OrderManagementWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ViewOrder_Click);
            
            #line default
            #line hidden
            break;
            case 10:
            
            #line 202 "..\..\..\..\Views\OrderManagementWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.EditOrder_Click);
            
            #line default
            #line hidden
            break;
            case 11:
            
            #line 207 "..\..\..\..\Views\OrderManagementWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.DeleteOrder_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

