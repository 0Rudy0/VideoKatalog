   M S B A M L   `   `   `  ����L  HVideoKatalog.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:Video_katalog.ConvertersVideo_katalog.Converters  5   	   #    Video_katalog.HDDsManagerForm   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   BlocalConverters&clr-namespace:Video_katalog.Converters     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6F   -   5          � WindowStartupLocation$  CenterOwner=�5   	    � Title$ Tvrdi diskovi��6-   $	��278��6C   $	��426��6P    � 	Resources 5      ,  &Video_katalog.Converters.SizeConverter   sizeConverter,  &Video_katalog.Converters.DateConverter  dateConverter;  5Video_katalog.Converters.ProgressBarValueToBackground  pogressBarValueToBackground  SimpleProgressBar �� 
TargetType �� Property �� Value �� 
TargetType ��Name  PressedBrush �� CornerRadius	 `� 	Converter
 �� ElementName �� Path��%a  &	        &	      &	 &     &	 9     7    5   
   6(     5	   
   6(     5
   
   67   �� 5   
   " �6*   .�5      ��  � �� 5      �� 5      " �6&   .R�5      � .+�5      �� $ 
PART_Track��6%   #���  69   ���#888$��1q�6x   $ 2��6�   �� 5      $ PART_Indicator��6@   $
��Left=�6%   $��1q�6X   $ 2��6l   ���	#FFBBC6C4��5   "   `� 5   &   8	   .1�5   *   �� $
 filledHddProgressBar��62   $ Maximum�6U   �� 5   *   $
 filledHddProgressBar��62   $ Value�6U   5   '   5      5!      5"      5$      5%      5&      .��5'      � .+�5(      �� -   5         ��Name$ hddComboBox��5(      $��8,56,0,0q�6%     SimpleComboBox#���  67   $ �� IsSynchronizedWithCurrentItem$
 TrueY�6`   $	��Top=�6�   $
��Left=�6�   $��118.8��6�   $��16(�6�    �� MaxDropDownHeight$	 200��6  $��
Trajan Pro)�64  ��6�   �� 64  �� 5)   
   -   5         ��Name$ 	buyDateTB��5)      $��90.198,106,0,0q�6'   $
��Wrap=�6?   $	��Top=�6�   $
��Left=�6�   $��13.333(�6�   $
��Bold"�6�   $��85��6�   ��6S   �� *PurchaseDate+ �� 	Converter# [ 6�   �� 5*   
   -   5        $ warrantyLengthTB��5*      $
��Left=�6.   $��90.198,0,0,62.3q�6I   $
��Wrap=�6b   $��13.333(�6�   $
��Bold"�6�   $��17.733��6�   $��Bottom=�6�   $��74��6�   ��6v   �� *WarrantyLength+6�   �� 5+      -   5        $ 
capacityTB��5+      $
��Left=�6%   $��131.5,59.5,0,0q�6@   $
��Wrap=�6X   $	��Top=�6}   $��13.333(�6�   $
��Bold"�6�   $��57��6�    �� TextAlignment$ Center=�6�   ��6l   �� 6�   �� 5,      $��16,104,0,118.266q�6   $
��Wrap=�6+   $��Kupljen:��6?   $
��Left=�6O   $��13.333(�6j   �� 5-      $
��Left=�6   $��16,158.266,0,0q�6,   $
��Wrap=�6D   $��
Garancija:��6X   $	��Top=�6j   $��13.333(�6�   � 5.      $��	192,9,0,0q�6   ���	#FFE7E7E7$
��Left=�66   $	��Top=�6Q   $	��200��6i   $	��215��6u    J� Effect 5/      3  -System.Windows.Media.Effects.DropShadowEffect  50   	   .+�52      �� $��0,0,0,66.283q�6   $3��-/Video%20Katalog;component/Resources/HDD1.png��6$   $��Center=�6[    J� OpacityMask 53   	   �� 54   
    �� EndPoint$ 0.5,1&�6    �� 
StartPoint$ 0.5,0&�6/   .��55      �  � Color$ Black��6    � Offset$ 0Q�6&   � 56      $ 	#FFA94949��6   $ 1Q�6*   57      59   	   �� 5:      -   5        $ freeSpacePercentage��5:      $��0,0,23,52.3q�6/   $
��Wrap=�6D   $
��56 %��6X   $��Right=�6d   $��Bottom=�6�   $��16(�6�   $0��*/Zavrsni Rad;component/Fonts/#Cambria Math)�6�   �� 5;      -   5        $ freeSpaceTB��5;      $��77.9,0,0,12.5q�6'   $
��Wrap=�6>   $��Bottom=�6�   $��18.667(�6�   $
��Left=�6�   ��6R   �� *	FreeSpace+# [  6�   �� 5<      $
��Left=�6   $��15.25,0,0,14.176q�6-   $
��Wrap=�6G   $��	Slobodno:��6[   $��Bottom=�6l   � 5=      -   5         �Name$ filledHddProgressBar��5=      $��17.191��62   $��8,0,8,34.136q�6B   #���  6X   $��Bottom=�6�   5>      �� 5?      -	   5        $ warrantyValidTB��5?      $
��Left=�6*   $��90.198,181.7,0,0q�6E   $
��Wrap=�6_   $	��Top=�6s   $��13.333(�6�   $
��Bold"�6�   ���	#FFE20707$��96��6�   �� 5@      -
   5         ��Name$ addNewHddBTN��5@      $
��Left=�6$   $��8,8,0,0q�6?   $	��Top=�6P   $��28��6h   $��28��6s    � ToolTip$ 
Dodaj novi��6   .��5A      �� $��20��6   $��20��6   $6��0/Video%20Katalog;component/Resources/plus20p.png��6%   $ 
Dodaj novi��6_   5B      �� 5C      -   5        $ removeHddBtn��5C      $
��Left=�6$   $��40,8,0,0q�6?   $	��Top=�6Q   $��28��6i   $��28��6t   $ Izbriši��6�   .��5D      �� $��20��6   $��20��6   $7��1/Video%20Katalog;component/Resources/minus20p.png��6%   $ Izbriši��6`   5E      �� 5F      -   5        $ 
editHddBTN��5F      $
��Left=�6"   $��72,8,0,0q�6=   $	��Top=�6O   $��28��6g   $��28��6r   $ Izmijeni��6~   .��5G      �� $��20��6   $��20��6   $6��0/Video%20Katalog;component/Resources/edit20p.png��6%   $ Izmijeni��6_   5H      5J      5K      