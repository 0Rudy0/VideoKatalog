   M S B A M L   `   `   `  ����L  HVideoKatalog.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:Video_katalog.ConvertersVideo_katalog.Converters  5   	   #    Video_katalog.SelectSerieForm   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   BlocalConverters&clr-namespace:Video_katalog.Converters     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6F     � WindowStartupLocation$  CenterOwner=�5   	    � Title$ 
Sve serije��6-   $	��550��6@   $	��368��6M    � 	Resources 5      7  1Video_katalog.Converters.NameToImagePathConverter   nameToImagePathConverter  serieTemplate ��Name �� ConverterParameter �� 	Converter ��Name �� TextAlignment��%  &	        &	      7     5   
   63   �� 5	   
   .R�5
      � $��70��6
   $	��310��6   ���	#5DEDEDED$	��0,5q�69   .+�5      �� $ seriePoster��6   $
��Left=�6-   $��8,8,0,0q�6H   $��37.57��6Y   $	��Top=�6g   $��54.032��6   ��6�   �� *Name+$ serie��8   �� 5      $ serieNameTB��6   $��42,0,0,0q�6%   $
��Wrap=�67   $��Center=�6|   $��Center=�6�   $��16(�6�   $
��Bold"�6�   $��
Trajan Pro)�6�   $ Center=�6�   $	��248��6  ��6K   �� *Name+6  5      5      5      .��5      � .+�5   
   �� -   5         ��Name$ allSerieListBox��5      $��
0,38,0,112q�6+   ��.  $��Center=�6e   $	��337��6�   #���  6�   6�   �� 5      $��Center=�6   $��0,8,0,0q�6.   $
��Wrap=�6?   $��Odaberi seriju��6S   $	��Top=�6i   $
��Bold"�6�   $��16(�6�   �� 5      -   5        	 ��Name$	 acceptButton��5      $��0,0,-160,15q�6$   $��Bottom=�69   $��Center=�6T   $��89��6q   .��5      �� $��Center=�6   $��Center=�61   $6��0/VideoKatalog.View;component/Resources/OK64p.png��6L   $��54��6�   5      �� 5      -   5        $ seasonListBox��5      $��72��6&   $��8,0,0,8q�62     SimpleListBox#���  6C   $��Bottom=�6k   $
��Left=�6�   $	��152��6�   �� 5      -   5        $ selectSeasonLabel��5      $
��Left=�6,   $��26.943,0,0,84q�6G   $
��Wrap=�6^   $��Odaberi sezonu��6r   $��Bottom=�6�   $
��Bold"�6�   $��16(�6�   5      5      