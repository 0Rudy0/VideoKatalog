   M S B A M L   `   `   `  ����L  HVideoKatalog.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:Video_katalog.ConvertersVideo_katalog.Converters  5   
   &     Video_katalog.CamerasManagerForm   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   KVideo_katalog_Converters&clr-namespace:Video_katalog.Converters     5   
   @d2http://schemas.microsoft.com/expression/blend/2008     6R   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6�     � WindowStartupLocation$  CenterOwner=�5   
    � Title$ Kamere��6.   $	��278��6=   $	��360��6J    � 	Resources 5      ,  &Video_katalog.Converters.DateConverter   dateConverter��%   &	          5      6+   5	      .��5
      � .+�5      �� $��Right=�6   $��	0,56,8,72q�6)   $	��112��6<   $5��//Video%20Katalog;component/Resources/camera.png��6H   �� 5      -   5         ��Name$ cameraComboBox��5      $��8,56,168.802,0q�6(     SimpleComboBox#���  6@   $ �� IsSynchronizedWithCurrentItem$
 TrueY�6i   $	��Top=�6�   $��16(�6�    �� MaxDropDownHeight$	 200��6�   $��
Trajan Pro)�6�   ��6�   �� 6�   �� 5      -   5         ��Name$ 	buyDateTB��5      $��90.198,106,0,0q�6$   $
��Wrap=�6<   $	��Top=�6�   $
��Left=�6�   $��13.333(�6�   $
��Bold"�6�   $��85��6�   ��6P   �� *PurchaseDate+ �� 	Converter# [  6�   �� 5      -   5        $ warrantyLengthTB��5      $
��Left=�6+   $��	90,0,0,63q�6F   $
��Wrap=�6Y   $��13.333(�6�   $
��Bold"�6�   $��17.733��6�   $��Bottom=�6�   $��74��6�   ��6m   �� *WarrantyLengt+6�   �� 5      $��
16,104,0,0q�6   $
��Wrap=�6%   $��	Kupljena:��69   $
��Left=�6J   $��13.333(�6e   $	��Top=�6w   �� 5      $
��Left=�6   $��16,158.266,0,0q�6,   $
��Wrap=�6D   $��
Garancija:��6X   $	��Top=�6j   $��13.333(�6�   �� 5      -   5        $ warrantyValidTB��5      $
��Left=�6*   $��90.198,181.7,0,0q�6E   $
��Wrap=�6_   $	��Top=�6s   $��13.333(�6�   $
��Bold"�6�   ���	#FFE20707$��96��6�   �� 5      -   5         ��Name$ addNewCameraBTN��5      $
��Left=�6'   $��8,8,0,0q�6B   $	��Top=�6S   $��28��6k   $��28��6v   	 � ToolTip$	 
Dodaj novi��6�   .��5      �� $��20��6   $��20��6   $6��0/Video%20Katalog;component/Resources/plus20p.png��6%   $	 
Dodaj novi��6_   5      �� 5      -   5        $ removeCameraBtn��5      $
��Left=�6'   $��40,8,0,0q�6B   $	��Top=�6T   $��28��6l   $��28��6w   $	 Izbriši��6�   .��5      �� $��20��6   $��20��6   $7��1/Video%20Katalog;component/Resources/minus20p.png��6%   $	 Izbriši��6`   5      �� 5      -   5        $ editCameraBTN��5      $
��Left=�6%   $��72,8,0,0q�6@   $	��Top=�6R   $��28��6j   $��28��6u   $	 Izmijeni��6�   .��5      �� $��20��6   $��20��6   $6��0/Video%20Katalog;component/Resources/edit20p.png��6%   $	 Izmijeni��6_   5      5      5      