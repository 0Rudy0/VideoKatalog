   M S B A M L   `   `   `  ���� L  HVideoKatalog.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:Video_katalog.ConvertersVideo_katalog.Converters  !    Video_katalog.HomeVideoForm   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     KVideo_katalog_Converters&clr-namespace:Video_katalog.Converters     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     a
extToolkitJhttp://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit/extended     -     �Name$  
mainWindow�� � WindowStartupLocation$ CenterOwner=� � Title$ Kućni video��$	��726��$
��1048�� � 	Resources /  )Video_katalog.Converters.BitrateConverter   bitrateConverter/  )Video_katalog.Converters.RuntimeConverter  runtimeConverter,  &Video_katalog.Converters.SizeConverter  sizeConverter-  'Video_katalog.Converters.MoneyConverter  moneyConverter��%<   &	        &	      &	 
     &	              .��� .+�� -    �Name$ movieTechInfoGrid��$��	840,8,0,0q� J� OpacityMask
 �   ����	#FFBAD6DA$	��Top=�$	��544��$
��Left=�$	��168�� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     
BlurRadius$ 26Q�.+��� -    ��Name$ widthTextBox��$��10.783,324.894,0,0q�	 �� TextWrapping$
	 Wrap=�$
��Left=�$��64.983��{�.  $0��*/Zavrsni Rad;component/Fonts/#Cambria Math)�$��16(�
���   �$	��Top=����� *Width+�� -   $ heightTextBox��$��92.737,324.894,0,0q�$
	 Wrap=�{�.  $
��Left=�$��67.287��$0��*/Zavrsni Rad;component/Fonts/#Cambria Math)�$��16(�
���   �$	��Top=����� *Height+�� -   $ bitrateTextBox��$��10.759,270.901,0,0q�$
	 Wrap=�{�.  $��16(�$��149.241��$
��Left=�$��24��$	��Top=�$0��*/Zavrsni Rad;component/Fonts/#Cambria Math)�
���   ����� *	Bitrate+
 �� 	Converter#
 [  �� -   $ runtimeTextBox��$��92.713,383.001,0,0q�$
	 Wrap=� J� RenderTransformOrigin$ 0.444,0.736&�{�.  $
��Left=�$��67.287��$��24��$	��Top=�$0��*/Zavrsni Rad;component/Fonts/#Cambria Math)�$��16(�
���   ����� *	Runtime+#
 [ �� -   $ aspectRatioTextBox��$��24��$��10.996,383.001,0,0q�$
	 Wrap=�$	��Top=�$
��Left=�$��64.746��{�.  $0��*/Zavrsni Rad;component/Fonts/#Cambria Math)�$��16(�
���   ����� *AspectRatio+�� -   $ sizeTextBox��$��26.08��$��10.996,222.861,0,0q�$
	 Wrap=�$	��Top=�{�.  $��16(�$
��Left=�$��149.004��
���   �$0��*/Zavrsni Rad;component/Fonts/#Cambria Math)����� *Size+#
 [ �� $
��Left=�$��10,309.041,0,0q�$
��Wrap=�$��
Rezolucija��$��15.854��$	��Top=����	#BF000000�� $
��Left=�$��11.709,368,0,0q�$
��Wrap=�$	��Top=�$��Aspect ratio�����	#BF000000$��10.667(��� $��11.497,254.94,0,0q�$
��Wrap=�$��Bitrate��$
��Left=�$	��Top=����	#B2000000�� $��20��$��	8,152,0,0q�$
��Wrap=�$	��Top=�$��14.667(�$��# HDD��$
��Left=����	#B2000000�� $
��Left=�$��11.497,206.9,0,0q�$
��Wrap=�$	��Top=�$��	Veličina��$ 0.617,0.433&����	#B2000000�� $��92.713,368,0,0q�$
��Wrap=�$	��Top=�$��Trajanje��$ 0.496,0.687&�$
��Left=����	#BF000000�� $
��Left=�$��11.901,422.26,0,0q�$
��Wrap=�$��13.333(�$��Audio��$	��Top=�$��73��$��17.74�����	#B2000000�� -	    ��Name$ audioLanguagesListBox��$��11.17,440.26,0,0q�$��58.333��$	��Top=�$
��Left=�$��148.83��$��13.333(����� *	AudioList+�� -
    ��Name$ addAudioLng��$��140,420,0,0q�$
��Bold"�$��14.667(�$��-4,-11,-4,-4q�$��20��$	��Top=�$
��Left=�$��20�� � ToolTip$ Izmijeni jezik audia����Z� ��Z� .���� $��16��$��16��$6��0/Video%20Katalog;component/Resources/edit20p.png��$��-0.2,6,0,-1.2q�$ Izmijeni jezik audia���� -    ��Name$ hddComboBox��$��8.5,172,0,0q�$	��Top=�$��24��$��13.333(�$
��Left=�$��127.5������  �� SelectedItem �� *Hdd+�� -   $ loadFileButton��$��132.43��$��8,8,0,0q�$	��Top=�$
��Bold"�$��Center=�$��Center=�$��24(�$
��Left=�$	��141�����	#57000000.���� $	��128��$	��128��$>��8/Video%20Katalog;component/Resources/browseVideo128p.png��$" Učitaj video datoteku filma���� $��80,328,79.096,0q�$
��Wrap=�$��X��$��13.333(�$	��Top=�$
��Bold"��� -   $ 	addNewHdd��$��140,172,0,0q�$
��Bold"�$��14.667(�$��-4,-10.8,-4,-4q�$	��Top=�$��24��$
��Left=�$��24��$ Dodaj novi HDD����Z� ��Z� .���� $��20��$��20��$6��0/Video%20Katalog;component/Resources/plus20p.png��$��	0,6.8,0,0q�$ Dodaj novi HDD���� -   $ cancelButton��$��Right=�$��85��$��
0,0,250,25q�$��Bottom=�$	��110��$
��Bold"�$��14.667(�$ Odbaci i zatvori������  �� EndPoint$	 0,1&� �� 
StartPoint$	 0,0&�.���  � Color$ 	#FFF3F3F3�� � Offset$ 0Q�� $ 	#FFF5B6AF��$ 1Q�� $ 	#FFF1E6E5��$ 0.435Q�.���� $��70��$��70��$:��4/Video%20Katalog;component/Resources/dismiss128p.png���� -   $ acceptButton��$��85��$
��Bold"�$��14.667(�$ 0.5,0.5&�$ Prihvati��$
��Left=�$��Bottom=�$	��110��$��
250,0,0,25q�z�V� .~��� �� ��  �� Angle$ -0.642Q�T� ���� $	 0,1&�$	 0,0&�.��� $ White��$
 0.01Q�� $ 	#FFE4FDE4��$ 0.474Q�� $ 	#FFB5FFB2��$ 0.974Q�.���� $��70��$��70��$9��3/Video%20Katalog;component/Resources/accept128p.png��� -   $ imdbInfoGrid��
 �   ����	#FFE0E0E0$
��Left=�$	��792��$	��Top=�$	��544��$��8,8,0,0q�   $ 26Q�.+��� -   $ customNameTextBox��$
��Left=�$��28.133��$��7.901,21.959,0,0q�$	��Top=�$��203.949��$��14.667(�$ �� HorizontalScrollBarVisibility$ Disabled=�$
��Bold"�$��Calibri)�$��1,2,1,1q�$��2q�
���   � �� MaxLines$ 1������ *Name+�� -   $ summaryTextBox��$��242.995,152.04,0,0q�$
	 Wrap=�" �� VerticalScrollBarVisibility$
 Auto=�$
��Left=�$��197.005��$	��Top=�$��355.589������ *	Summary+�� $
��Left=�$��8.139,5.999,0,0q�$
��Wrap=�$	��Top=�$ 0.673,0.597&����	#B2000000��  � Language$ hr-hr�$�Naziv���� $
��Left=�$��7.901,204,0,0q�$
��Wrap=�$	��Top=�$��Kamerman��$ 
1.12,0.334&����	#B2000000�� $
��Left=�$��7.901,0,0,211.693q�$
��Wrap=����	#B2000000$��Bottom=�$��Osobe na snimci���� $
��Left=�$��242.995,136.08,0,0q�$
��Wrap=�$	��Top=����	#B2000000$ 0.526,0.594&�$��Radnja / komentar��� -   $ 
posterGrid��$��469,76.629,0,0q����	#FFEBEBEB$	��300��$	��431��$
��Left=�$	��Top=�   $ 14Q�.+��� -    ��Name$ posterImage��$	��300��$	��431��$
��Fill=� �� $ 0.5,1&�$ 0.5,0&�.��� $ Black��$ 0Q�� $ 	#FF624545��$ 1Q��� -   $ pasteImageButton��$��655,12.999,0,0q�$��55��$	��Top=�$
��Left=�$��55��$ Zalijepi poster����Z� ��Z� .���� $��50��$��50��$7��1/Video%20Katalog;component/Resources/paste50p.png��$ Zalijepi sliku���� -   $ castListBox��$��8.139,0,0,36.371q�$
��Left=�$��171.711��$��175.322��$��Bottom=����� *PersonsInVideo+�� -   $ camermanListBox��$��60��$��7.901,220,0,0q�$	��Top=�$
��Left=�$��171.949��$��13.333(����� *	Camermans+�� -   $ addCamerman��$��28��$
��Left=�$��183.85,220,0,0q�$	��Top=�$��28��$
��Bold"�$��18.667(�$��-4,-13,-4,-4q�$  ��$ 0.704,0.235&���Z� .���� $��20��$��20��$6��0/Video%20Katalog;component/Resources/plus20p.png��$��	0,8.9,0,0q�$ Dodaj kamermana���� -   $ removeCamerman��$��28��$
��Left=�$��183.85,252,0,0q�$	��Top=�$��28��$��18.667(�$��-4,-12.3,-4,-4q�$
��Bold"�$  ����Z� .���� $��20��$��20��$7��1/Video%20Katalog;component/Resources/minus20p.png��$��	0,8.5,0,0q�$ Makni odabranog kamermana���� -   $ 	addPerson��$
��Left=�$��183.85,0,0,183.693q�$��28��$
��Bold"�$��18.667(�$��-4,-12,-4,-4q�$  ��$��28��$��Bottom=���Z� .���� $��20��$��20��$6��0/Video%20Katalog;component/Resources/plus20p.png��$��	0,7.7,0,0q�$ Dodaj novu osobu���� -   $ removePerson��$
��Left=�$��183.85,0,0,151.693q�$��28��$
��Bold"�$��18.667(�$��-4,-12,-4,-4q�$ 0.409,0.552&�$  ��$��28��$��Bottom=���Z� .���� $��20��$��20��$7��1/Video%20Katalog;component/Resources/minus20p.png��$��	0,7.9,0,0q�$ Makni odabranu osobu���� -   $ categoryComboBox��$��7.901,82.867,0,0q�$	��Top=�$��29.133�� �� MaxDropDownHeight$	 479�� �� 
IsEditable .  �� 
IsReadOnly .  �� Text$  ��$��13.333(�$��	3,4.5,3,3q�
�������$
��Left=�$��171.949�� �� SelectedValue �� *VideoCategory+�� $��7.901,66.907,0,0q�$
��Wrap=�$	��Top=�$��
Kategorija��$
��Left=�$ 0.628,0.461&����	#B2000000�� -   $ copyAndOpenEditorBTN��$��55��$��714,12.999,0,0q�$	��Top=�$��10.667(�$, &Kopiraj sliku i otvori slikovni editor��$
��Left=�$��55����Z� ��Z� .���� $��47��$��50��$;��5/Video%20Katalog;component/Resources/ptohostop48p.png���� -     ��Name$  imageQualityLevelSlider��$��480,518,0,0q�  SimpleSlider#���  $	��Top=�! � Maximum$	! 100Q�" � Minimum$" 1Q�# � Value$# 70Q�$ � UseLayoutRounding$ . $ 	Kvaliteta��$��18�����	#FF661B1B$
��Left=�$	��281����Z� ��Z� (  "System.Windows.Controls.DatePicker  -   $��filmDatePicker��$��242.995,21.959,352,0q�$	��Top=�$��14.667(�$��28.133���� $��242.995,5.999,0,0q�$
��Wrap=�$	��Top=�$��Datum��$
��Left=�$ 0.628,0.461&����	#B2000000�� $��242.995,66.907,0,0q�$
��Wrap=�$	��Top=�$��Lokacija��$
��Left=�$ 0.628,0.461&����	#B2000000�� -    $ cameraComboBox��$��7.901,152.04,0,0q�$	��Top=�$��29.133��$	 479�� .  . $  ��$��13.333(�$��	3,4.5,3,3q�
�������$
��Left=�$��171.949�� �� *FilmingCamera+�� $��7.901,136.08,0,0q�$
��Wrap=�$	��Top=�$��Kamera��$
��Left=�$ 0.628,0.461&����	#B2000000�� -!   $ openCameraManager��$��28��$
��Left=�$��183.85,152.04,0,0q�$	��Top=�$��28��$
��Bold"�$��18.667(�$��-4,-13,-4,-4q�$  ��$ 0.704,0.235&���Z� .���� $��20��$��20��$6��0/Video%20Katalog;component/Resources/plus20p.png��$��	0,8.9,0,0q�$ Upravljanje kamerama���� -"   $ openCategoryManager��$��28��$
��Left=�$��183.85,82.867,0,0q�$	��Top=�$��28��$
��Bold"�$��18.667(�$��-4,-13,-4,-4q�$  ��$ 0.704,0.235&���Z� .���� $��20��$��20��$6��0/Video%20Katalog;component/Resources/plus20p.png��$��	0,8.9,0,0q�$ Upravljanje kategorijama���� -#   $ locationTextBox��$��29.133��$��242.995,82.867,352,0q�$	��Top=�$��14.667(�$ Disabled=�$
��Bold"�$��Calibri)�$��1,2,1,1q�$ 1������ *
Location+