   M S B A M L   `   `   `  ����C  ?IPcamera, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:IPcameraIPcamera  5   	       IPcamera.AddCameraWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   X TXceed.Wpf.Toolkit, Version=4.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4;wpfExtended)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   "localclr-namespace:IPcamera  5   	     � Title$  
Add Camera��5	   	   $	��200��6   $	��600��6)    � 
ResizeMode$ NoResize=�65   .��5
      �  � ColumnDefinitions 5   
   �� 5      $
��auto �6   6.   �� 5      6!   �� 5      6!   �� 5      6!    � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5      $
��auto �6   6,   �� 5      $
��auto �6   �� 5      .+�5   
   �� $��Address:��6   $��0��6#   $��0��63   6B   �� 5   
   $��Onvif Address:��6   $��0��6)   $��1��69   6H   (  "Xceed.Wpf.Toolkit.WatermarkTextBox  5   
   -   5          Name$ TextBoxCameraAddress��5   '   $��0��6E   $��1��6R   $��3��6b   $��3q�6v      	Watermark$ Address��6�   6�     5   
   -   5        $ textBoxOnvifAddress��5   '   $��1��6D   $��1��6Q   $��3��6a   $��3q�6u   $ Onvif Address��6�   6�     5   
   -   5        $ textBoxUsername��5   '   $��2��6@   $��2��6M   $��1��6]   $��3q�6q   $ Username��6|   6�   ,  &Xceed.Wpf.Toolkit.WatermarkPasswordBox  5   
   -   5          Name$ passwordBox��5   +   $��2��6@   $��3��6M   $��3��6]   $��3q�6q   $ Password��6|   6�   �� 5   
   -   5         ��Name$ BtnOk��5      $��OK��6    $��3��6-   $��2��6:   $��10q�6J   $��25��6V   6x   �� 5    
   -   5        $ 	BtnCancel��5       $��Cancel��6$   $��3��65   $��3��6B   $��10q�6R   $��25��6^   6�   5!      5"      