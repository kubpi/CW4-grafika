   M S B A M L   `   `   `  ����F  BCW4-grafika, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:CW4_grafikaCW4_grafika  5   	       CW4_grafika.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:CW4_grafika     5   	     � WindowStartupLocation$  CenterScreen=�5       � Title$$ Aplikacja wczytywania obrazów��5   	   $	��900��6/   $
��1600��6<    � 	Resources 5	      .   (CW4_grafika.BooleanToVisibilityConverter    BooleanToVisibilityConverter��%   &	          5
   
   6-   5      .��5      �  J� Effect 5   
   3  -System.Windows.Media.Effects.DropShadowEffect  5         Color$ Black��6      
BlurRadius$ 15Q�6-      Opacity$
 0.15Q�6=      ShadowDepth$ 2Q�6L   6^    � ColumnDefinitions 5   
   �� 5      $��* �6   	 � RowDefinitions	 5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $��* �6   .+�5   
   �� 
 �� Orientation$
 
Horizontal=�6   $��0��6.   .+�5      	� -   5        $��	AddButton��5      $	��Add��6+    	� 	GroupName$ OperationGroup��69   $��5q�6T   $��
40 50 0 70q�6`    � ToolTip$	 PBM��6t   
������� d� 	IsChecked 6�   �� *IsPbmChecked+ �� Mode$ TwoWay=�6�   	� 5      -   5        $��SubtractButton��5      $��Subtract��60   $ OperationGroup��6C   $��5q�6^   $��
40 50 0 70q�6j   $	 PGM��6~   
������� 6�   �� *IsPgmChecked+$ TwoWay=�6�   	� 5      -   5        $��MultiplyButton��5      $��Multiply��60   $ OperationGroup��6C   $��5q�6^   $��
40 50 0 70q�6j   $	 PPM��6~   
������� 6�   �� *IsPpmChecked+$ TwoWay=�6�   	� 5!      -   5        $��DivideButton��5!      $��Divide��6.   $ OperationGroup��6?   $��5q�6Z   $��
40 50 0 70q�6f   $	 PPM��6z   
������� 6�   �� *IsPpmChecked+$ TwoWay=�6�   	� 5#      -   5        $��BrightnessButton��5#      $��
Brightness��62   $ OperationGroup��6G   $��5q�6b   $��
40 50 0 70q�6n   $ 
Brightness��6�   
������� 5$   !   �� *IsBrightnessSelected+$ TwoWay=�	� 5&      -   5        $��
GrayButton��5&      $
��Gray��6,   $ OperationGroup��6;   $��5q�6V   $��
40 50 0 70q�6b   $	 PPM��6v   
������� 6�   �� *IsPpmChecked+$ TwoWay=�6�   5(      �� 5)   
   $
 
Horizontal=�6   $
��Left=�6.   $��	0 10 0 10q�6I   $��1��6\   y�6j   �� *IsOperationSelected+ �� 	Converter# [  .+�5*      �� $��Center=�67   $��	10 0 20 0q�6R   $��15(�6e   ��6   �� *OperationMode+6e   �� 5+      $
 Vertical=�6   $��0 0 10 0q�60   .+�5,      �� $��50��6e   $��0 0 0 5q�6p   $��14(�6�   ��6   �� *ColorR+$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5-       � Minimum$ 0Q�6    � Maximum$	 255Q�6%   $	��100��6Y    �� TickFrequency$ 5Q�6e    � Value 63   �� *ColorR+$ TwoWay=�6e   5.      �� 5/      $
 Vertical=�6   $��0 0 10 0q�60   .+�50      �� $��50��6d   $��0 0 0 5q�6o   $��14(�6�   ��6   �� *ColorG+$ TwoWay=�$ PropertyChanged=�6�   �� 51      $ 0Q�6   $	 255Q�6%   $	��100��6Y   $ 5Q�6e    63   �� *ColorG+$ TwoWay=�6e   52      �� 53      $
 Vertical=�6   $��0 0 10 0q�60   .+�54      �� $��50��6d   $��0 0 0 5q�6o   $��14(�6�   ��6   �� *ColorB+$ TwoWay=�$ PropertyChanged=�6�   �� 55      $ 0Q�6   $	 255Q�6%   $	��100��6Y   $ 5Q�6e    63   �� *ColorB+$ TwoWay=�6e   56      57      �� 58   
   $
 
Horizontal=�6   $
��Left=�6.   $��	0 10 0 10q�6I   $��1��6\   y�6i   �� *IsBrightnessSelected+# [  .+�59      �� $��
Brightness��6   $��Center=�6*   $��	10 0 20 0q�6E   $��15(�6X   �� 5:      $
 Vertical=�6   $��0 0 10 0q�60   .+�5;      �� $��50��6m   $��0 0 0 5q�6x   $��14(�6�   ��6   �� *BrightnessLevel+$ TwoWay=�$ PropertyChanged=�6�   �� 5<      $
 -100Q�6   $	 100Q�6(   $	��100��6e   $ 1Q�6q    66   �� *BrightnessLevel+$ TwoWay=�6q   5>      5?      �� 5@   
   $
 
Horizontal=�6   $��2��6.   .+�5B      �� -   5        $��Wczytaj Obraz��5B      6-   �� 5C      -   5        $��Zastosuj��5C      6(   �� 5D      -	   5        $��Zastosuj Jasność��5D      60   5E      �� 5H   
   $��3��6)   ��6   �� *Image+6)   5I      5J      