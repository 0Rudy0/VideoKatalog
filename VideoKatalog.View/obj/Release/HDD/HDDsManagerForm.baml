   M S B A M L   `   `   `  ���� L  HVideoKatalog.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:Video_katalog.ConvertersVideo_katalog.Converters  #    Video_katalog.HDDsManagerForm   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     BlocalConverters&clr-namespace:Video_katalog.Converters     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     � WindowStartupLocation$  CenterOwner=� � Title$ Tvrdi diskovi��$	��278��$	��426�� � 	Resources ,  &Video_katalog.Converters.SizeConverter   sizeConverter,  &Video_katalog.Converters.DateConverter  dateConverter;  5Video_katalog.Converters.ProgressBarValueToBackground  pogressBarValueToBackground  SimpleProgressBar �� 
TargetType �� Property �� Value �� 
TargetType ��Name  PressedBrush �� CornerRadius	 `� 	Converter
 �� ElementName �� Path��%X  &	        &	      &	 
     &	      7        �� " �.���  � �� �� " �.R�� .+��� $ 
PART_Track��#���  ���#888$��1q�$ 2���� $ PART_Indicator��$
��Left=�$��1q�$ 2�����	#FFBBC6C4��`� 8	   .1��� $
 filledHddProgressBar��$ Maximum��� $
 filledHddProgressBar��$ Value�.��� .+��� -    ��Name$ hddComboBox��$��8,56,0,0q�  SimpleComboBox#���  $ �� IsSynchronizedWithCurrentItem$
 TrueY�$	��Top=�$
��Left=�$��118.8��$��16(� �� MaxDropDownHeight$	 200��$��
Trajan Pro)����� �� -    ��Name$ 	buyDateTB��$��90.198,106,0,0q�$
��Wrap=�$	��Top=�$
��Left=�$��13.333(�$
��Bold"�$��85������ *PurchaseDate+ �� 	Converter# [ �� -   $ warrantyLengthTB��$
��Left=�$��90.198,0,0,62.3q�$
��Wrap=�$��13.333(�$
��Bold"�$��17.733��$��Bottom=�$��74������ *WarrantyLength+�� -   $ 
capacityTB��$
��Left=�$��131.5,59.5,0,0q�$
��Wrap=�$	��Top=�$��13.333(�$
��Bold"�$��57�� �� TextAlignment$ Center=����� �� $��16,104,0,118.266q�$
��Wrap=�$��Kupljen:��$
��Left=�$��13.333(��� $
��Left=�$��16,158.266,0,0q�$
��Wrap=�$��
Garancija:��$	��Top=�$��13.333(�� $��	192,9,0,0q����	#FFE7E7E7$
��Left=�$	��Top=�$	��200��$	��215�� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect  .+��� $��0,0,0,66.283q�$3��-/Video%20Katalog;component/Resources/HDD1.png��$��Center=� J� OpacityMask ��  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.���  � Color$ Black�� � Offset$ 0Q�� $ 	#FFA94949��$ 1Q��� -   $ freeSpacePercentage��$��0,0,23,52.3q�$
��Wrap=�$
��56 %��$��Right=�$��Bottom=�$��16(�$0��*/Zavrsni Rad;component/Fonts/#Cambria Math)��� -   $ freeSpaceTB��$��77.9,0,0,12.5q�$
��Wrap=�$��Bottom=�$��18.667(�$
��Left=����� *	FreeSpace+# [  �� $
��Left=�$��15.25,0,0,14.176q�$
��Wrap=�$��	Slobodno:��$��Bottom=�� -    �Name$ filledHddProgressBar��$��17.191��$��8,0,8,34.136q�#���  $��Bottom=��� -	   $ warrantyValidTB��$
��Left=�$��90.198,181.7,0,0q�$
��Wrap=�$	��Top=�$��13.333(�$
��Bold"����	#FFE20707$��96���� -
    ��Name$ addNewHddBTN��$
��Left=�$��8,8,0,0q�$	��Top=�$��28��$��28�� � ToolTip$ 
Dodaj novi��.���� $��20��$��20��$6��0/Video%20Katalog;component/Resources/plus20p.png��$ 
Dodaj novi���� -   $ removeHddBtn��$
��Left=�$��40,8,0,0q�$	��Top=�$��28��$��28��$ Izbriši��.���� $��20��$��20��$7��1/Video%20Katalog;component/Resources/minus20p.png��$ Izbriši���� -   $ 
editHddBTN��$
��Left=�$��72,8,0,0q�$	��Top=�$��28��$��28��$ Izmijeni��.���� $��20��$��20��$6��0/Video%20Katalog;component/Resources/edit20p.png��$ Izmijeni��