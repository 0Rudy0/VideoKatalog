   M S B A M L   `   `   `  ���� L  HVideoKatalog.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:Video_katalog.ConvertersVideo_katalog.Converters  &     Video_katalog.CamerasManagerForm   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     KVideo_katalog_Converters&clr-namespace:Video_katalog.Converters     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006       � WindowStartupLocation$  CenterOwner=� � Title$ Kamere��$	��278��$	��360�� � 	Resources ,  &Video_katalog.Converters.DateConverter   dateConverter��%   &	          .��� .+��� $��Right=�$��	0,56,8,72q�$	��112��$5��//Video%20Katalog;component/Resources/camera.png���� -    ��Name$ cameraComboBox��$��8,56,168.802,0q�  SimpleComboBox#���  $ �� IsSynchronizedWithCurrentItem$
 TrueY�$	��Top=�$��16(� �� MaxDropDownHeight$	 200��$��
Trajan Pro)����� �� -    ��Name$ 	buyDateTB��$��90.198,106,0,0q�$
��Wrap=�$	��Top=�$
��Left=�$��13.333(�$
��Bold"�$��85������ *PurchaseDate+ �� 	Converter# [  �� -   $ warrantyLengthTB��$
��Left=�$��	90,0,0,63q�$
��Wrap=�$��13.333(�$
��Bold"�$��17.733��$��Bottom=�$��74������ *WarrantyLengt+�� $��
16,104,0,0q�$
��Wrap=�$��	Kupljena:��$
��Left=�$��13.333(�$	��Top=��� $
��Left=�$��16,158.266,0,0q�$
��Wrap=�$��
Garancija:��$	��Top=�$��13.333(��� -   $ warrantyValidTB��$
��Left=�$��90.198,181.7,0,0q�$
��Wrap=�$	��Top=�$��13.333(�$
��Bold"����	#FFE20707$��96���� -    ��Name$ addNewCameraBTN��$
��Left=�$��8,8,0,0q�$	��Top=�$��28��$��28��	 � ToolTip$	 
Dodaj novi��.���� $��20��$��20��$6��0/Video%20Katalog;component/Resources/plus20p.png��$	 
Dodaj novi���� -   $ removeCameraBtn��$
��Left=�$��40,8,0,0q�$	��Top=�$��28��$��28��$	 Izbriši��.���� $��20��$��20��$7��1/Video%20Katalog;component/Resources/minus20p.png��$	 Izbriši���� -   $ editCameraBTN��$
��Left=�$��72,8,0,0q�$	��Top=�$��28��$��28��$	 Izmijeni��.���� $��20��$��20��$6��0/Video%20Katalog;component/Resources/edit20p.png��$	 Izmijeni��