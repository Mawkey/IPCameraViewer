   M S B A M L   `   `   `  ����C  ?IPcamera, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:IPcameraIPcamera  5   	       IPcamera.GridSizeWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   X TXceed.Wpf.Toolkit, Version=4.1.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4;wpfExtended)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   "localclr-namespace:IPcamera  5   	     � Title$  	Grid Size��5	   	   $	��120��6   $	��200��6(    � 
ResizeMode$ NoResize=�64   .��5
      �  � RowDefinitions 5   
   �� 5      6   �� 5      6    � ColumnDefinitions 5   
   �� 5      $
��auto �6   6.   �� 5      6!   �� 5      $
��auto �6   6.   �� 5      6!   .+�5   
   �� $��x:��6   $��0��6   $��0��6-   $��Center=�6:   6^   %  Xceed.Wpf.Toolkit.IntegerUpDown  5   
   -   5          Name$ intInputWidth��5   $   $��1��6;   $��0��6K   $��25��6X   $��3q�6d   6q   �� 5   
   $��y:��6   $��2��6   $��0��6-   $��Center=�6:   6^     5   
   -   5        $ intInputHeight��5   $   $��3��6<   $��0��6L   $��25��6Y   $��3q�6e   6r   �� 5   
   -   5         ��Name$ buttonOK��5      $��1��6#   $��1��63   $��25��6@   $��OK��6L   $��3q�6Y   6}   �� 5   
   -   5        $ buttonCancel��5      $��3��6'   $��1��67   $��25��6D   $��Cancel��6P   $��3q�6a   6�   5      5       