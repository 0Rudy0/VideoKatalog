   M S B A M L   `   `   `  ���� L  HVideoKatalog.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:Video_katalog.ConvertersVideo_katalog.Converters      Video_katalog.SerieForm   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     KVideo_katalog_Converters&clr-namespace:Video_katalog.Converters     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     X TXceed.Wpf.Toolkit, Version=2.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  -     �Name$  
mainWindow�� � WindowStartupLocation$ CenterOwner=� � Title$ Serija��$	��734��$
��1192�� � 	Resources /  )Video_katalog.Converters.BitrateConverter   bitrateConverter/  )Video_katalog.Converters.RuntimeConverter  runtimeConverter,  &Video_katalog.Converters.SizeConverter  sizeConverter-  'Video_katalog.Converters.MoneyConverter  moneyConverter��%<   &	        &	      &	 
     &	              .��� -    �Name$ mainGrid��.+�%  Xceed.Wpf.Toolkit.BusyIndicator  -     Name$ busyIndicator��$
��Left=�$��8,8,0,0q�$	��Top=�.��� -   $ imdbAndTechInfoGrid��$
��Left=�$	��Top=�.+��� $��	Pogledano��$��1030.343,0,0,-48q�$��Bottom=�$��18.667(�$
��Left=� d� 	IsChecked �� *
IsViewed+� -   $ imdbInfoGrid�� J� OpacityMask
 �   ����	#FFE0E0E0$
��Left=�$	��936��$	��Top=�$	��544�� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect  	   
BlurRadius$	 26Q�.+�#  Xceed.Wpf.Toolkit.SplitButton  -   
  Name$
 refreshSplitButton��$
��Left=�$��8,8,0,0q�$	��Top=�$��93��$��94.723�����	#56000000 � ToolTip$" Osvježi informacije o filmu��&   Xceed.Wpf.Toolkit.DropDownButton   IsOpen .     DropDownContent �� .+��� -    ��Name$ refreshAllButPoster��$��Sve osim postera��$
��Left=�$��90��$��25��$��11(���Z� ��Z� �� -   $ refreshEpisodes��$��Sezone i epizode��$
��Left=�$��90��$��25��$��11(���Z� ��Z� �� -	   $ refreshOnlyPoster��$��Samo poster��$
��Left=�$��90��$��25��$��11(���Z� ��Z� �� -
   $ refreshSummary��$��Radnja��$
��Left=�$��90��$��25��$��11(���Z� ��Z� �� -   $ refreshCastAndCrew��$��Glumci i redatelji��$
��Left=�$��90��$��25��$��11(���Z� ��Z� �� -   $ refreshRating��$��Ocjena��$
��Left=�$��90��$��25��$��11(���Z� ��Z� �� -   $ refreshGenres��$��Žanrovi��$
��Left=�$��90��$��25��$��11(���Z� ��Z� .���� $��80��$��74��$;��5/VideoKatalog.View;component/Resources/refresh70p.png���� -    ��Name$ customNameTextBox��$
��Left=�$��28.133��$��108.25,22.707,0,0q�$	��Top=�$��255.75��$��14.667(�$ �� HorizontalScrollBarVisibility$ Disabled=�$
��Bold"�$��Calibri)�$��1,2,1,1q�
���   �$��2q� �� MaxLines$ 1������ *Name+�� -   $ originalNameTextBox��$��28.133��$��108.25,74.59,0,0q�$	��Top=�$��14.667(�$ Disabled=�$
��Bold"�$��Calibri)�$��1,2,1,1q�$��255.75��$
��Left=�
���   �$��2q�$ 1������ *
OrigName+�� -   $ summaryTextBox��$��392,140.962,336,0q� �� TextWrapping$
 Wrap=�" �� VerticalScrollBarVisibility$
 Auto=�$	��Top=�$��358.667��$��Stretch=�$��Stretch=����� *	Summary+�� $
��Left=�$��108.25,8,0,0q�$
��Wrap=�$	��Top=� J� RenderTransformOrigin$ 0.673,0.597&����	#B2000000��  � Language$ hr-hr�$�Naziv���� $
��Left=�$��108.25,58.63,0,0q�$
��Wrap=�$	��Top=����	#B2000000$��81.268���� $ hr-hr�$�Orginalni naziv���� $
��Left=�$��8.545,125.176,0,0q�$
��Wrap=�$	��Top=�$��Redatelj(i)��$ 
1.12,0.334&����	#B2000000�� $
��Left=�$��194.122,125.176,0,0q�$
��Wrap=����	#B2000000$	��Top=��� $ hr-hr�$�Glumci���� -    ��Name$ summaryLabel��$
��Left=�$��392,125.176,0,0q�$
��Wrap=�$	��Top=����	#B2000000$ 0.526,0.594&��� $ hr-hr�$�Radnja���� -   $ ratingTextBox��$��376,74.59,0,0q�$ 0.702,0.597&�$��18.667(�$��28.133��$	��Top=�$ Disabled=�$��1,-2,1,1q�$ 1��$
��Left=�$��59.973������ *Rating+�� $��376,58.63,0,0q�$
��Wrap=�$	��Top=�$ 
0.66,1.138&����	#B2000000$
��Left=��� $ hr-hr�$�Ocjena��� -   $ 
posterGrid��$��618,68.629,0,0q����	#FFEBEBEB$	��300��$	��431��$
��Left=�$	��Top=�   $	 14Q�.+��� -    ��Name$ posterImage��$	��300��$	��431��$
��Fill=� ��  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.���  � Color$ Black�� � Offset$ 0Q�� $ 	#FF624545��$ 1Q��� -   $ internetLinkTextBox��$��8.545,385.805,0,0q�$ 
0.55,-0.25&�$	��Top=�$��22.195��$
��Left=�$��351.455��$ Disabled=�$ 1������ *InternetLink+ �� Mode$ TwoWay=��� -   $ trailerLinkTextBox��$��8.778,450.667,0,0q�$	��Top=�$��22.195��$ Disabled=�$��351.222��$
��Left=�$ 1������ *TrailerLink+$ TwoWay=��� -   $ internetLinkLabel��$��9.541,371.53,0,0q�$
��Wrap=�$
��Left=�$��14.275��$	��Top=����	#B2000000�� $ hr-hr�$�Internet link���� -   $ trailerLinkLabel��$��9.541,434.707,0,0q�$
��Wrap=�$	��Top=�$
��Left=����	#B2000000$ 0.446,0.672&��� $ hr-hr�$�Trailer link���� -   $ pasteImageButton��$��804,4.999,0,0q�$��55��$	��Top=�$
��Left=�$��55��$ Zalijepi poster����Z� ��Z� .���� $��50��$��50��$9��3/VideoKatalog.View;component/Resources/paste50p.png��$ Zalijepi poster���� -   $ pasteTrailerLink��$��24��$��335,472.861,0,0q�$	��Top=�$��9.333(�$��Verdana)�$
��Left=�$��25��$ Zalijepi link����Z� ��Z� .���� $��20��$��20��$9��3/VideoKatalog.View;component/Resources/paste20p.png��$ Zalijepi link���� -   $ pasteInternetLink��$��335,408,0,0q�$��9.333(�$��Verdana)�$
��Left=�$��25��$��24��$	��Top=�$ Zalijepi link����Z� ��Z� .���� $��20��$��20��$9��3/VideoKatalog.View;component/Resources/paste20p.png��$ zalijepi link���� -    ��Name$ castListBox��$��194.122,140.962,0,0q�$
��Left=�$��165.878��$��85��$	��Top=����� *Actors+�� -   $ directorListBox��$��85��$��8.778,140.962,0,0q�$	��Top=�$
��Left=�$��151.222������ *	Directors+�� -   $ addDirector��$��20��$��160,140.962,0,0q�$	��Top=�$
��Bold"�$��18.667(�$��-4,-13,-4,-4q�$" Dodaj proizvoljnog redatelja��$ 0.704,0.235&�$
��Left=�$��20����Z� ��Z� .���� $��16��$��16��$8��2/VideoKatalog.View;component/Resources/plus20p.png��$��	0,8.9,0,0q��� -   $ removeDirector��$��20��$��160,164.962,0,0q�$	��Top=�$��18.667(�$��-4,-12.3,-4,-4q�$
��Bold"�$ Makni odabranog redatelja��$
��Left=�$��20����Z� ��Z� .���� $��16��$��16��$9��3/VideoKatalog.View;component/Resources/minus20p.png��$��	0,8.5,0,0q��� -    $ addActor��$
��Left=�$��360,140.962,0,0q�$
��Bold"�$��18.667(�$��-4,-12,-4,-4q�$ Dodaj proizvoljnog glumca��$	��Top=�$��20��$��20����Z� ��Z� .���� $��16��$��16��$8��2/VideoKatalog.View;component/Resources/plus20p.png��$��	0,7.7,0,0q��� -!   $ removeActor��$��20��$
��Left=�$��360,164.962,0,0q�$	��Top=�$��20��$
��Bold"�$��18.667(�$��-4,-12,-4,-4q�$ 0.409,0.552&�$ Makni odabranog glumca����Z� ��Z� .���� $��16��$��16��$9��3/VideoKatalog.View;component/Resources/minus20p.png��$��	0,7.9,0,0q��� -"    ��Name$ label1��$
��MAX:��$��270.805,115.002,0,0q�$
��Left=�$	��Top=��� -#    ��Name$ genresComboBox��$��376,21.08,336,0q�$	��Top=�$��28.133��  �� MaxDropDownHeight$	  479��! �� 
IsEditable! . " �� 
IsReadOnly" . # �� Text$#  ��$��13.333(�$��3,4,3,3q�.@��� -$   $ ��Name$$ actionCheckBox��$��Akcija��
���iii��� -%   $$ adventureCheckBox��$��Avantura��
���iii��� -&   $$ animationCheckBox��$��	Animacija��
���iii��� -'   $$ biographyCheckBox��$��
Biografija��
���iii��� -(   $$ comedyCheckBox��$��Komedija��
���iii��� -)   $$ crimeCheckBox��$��Krimi��
���iii��� -*   $$ documentaryCheckBox��$��Dokumentarni��
���iii��� -+   $$ dramaCheckBox��$��Drama��
���iii��� -,   $$ familyCheckBox��$��
Obiteljski��
���iii��� --   $$ fantasyCheckBox��$��
Fantastika��
���iii��� -.   $$ filmNoirCheckBox��$��Crno-bijeli��
���iii��� -/   $$ historyCheckBox��$��	Povijesni��
���iii��� -0   $$ horrorCheckBox��$��Horor��
���iii��� -1   $$ musicCheckBox��$��Muzički��
���iii��� -2   $$ musicalCheckBox��$��Pjevan��
���iii��� -3   $$ misteryCheckBox��$��	Misterija��
���iii��� -4   $$ romanceCheckBox��$��	Romantika��
���iii��� -5   $$ sciFiCheckBox��$��Znanstvena fantastika��
���iii��� -6   $$ sportCheckBox��$��Sportski��
���iii��� -7   $$ thrillerCheckBox��$��Triler��
���iii��� -8   $$ warCheckBox��$��Ratni��
���iii��� -9   $$ westernCheckBox��$��Vestern��
���iii��� $��376,5.12,0,0q�$
��Wrap=�$	��Top=�$��Žanrovi��$
��Left=�$ 0.628,0.461&����	#B2000000	� -:   % 	�Name$% posterImpAwardsRadio��$��
IMP Awards��$��617.999,24.959,0,0q�$	��Top=�$
 TrueY�& 	� 	GroupName$& posters��$
��Left=�	� -;   $% posterGoogleRadio��$��Google��$��618,43.919,0,0q�$	��Top=�$& posters��$
��Left=�	� -<   $% posterMoviePosterDBRadio��$��Movie Poster DB��$��617.999,4.999,0,0q�$	��Top=�$& posters��$
��Left=��� -=   $ searchPosterBTN��$��55��$��745,4.999,0,0q�$	��Top=�$
��Left=�$��55��$
��Bold"�
���   �$��0q�$��-4q�$ Trazi poster����Z� ��Z� .���� $��45��$��45��$:��4/VideoKatalog.View;component/Resources/search64p.png��$��UniformToFill=�$ Traži poster��	� ->   $% trailerYoutubeRadio��$��Youtube��$��9.541,472.861,0,0q�$	��Top=�$
 TrueY�$& trailer��$��10.667(�$
��Left=�	� -?   $% trailerGoogleRadio��$��Google��$
��Left=�$��9.541,487.047,0,0q�$	��Top=�$& trailer��$��10.667(��� -@   $ searchTrailer��$��29.903��$��69.221,473.667,0,0q�$	��Top=�$��9.333(�$��Verdana)�$
��Left=�$��31.431��$ Traži trailer����Z� ��Z� .���� $��23��$��23��$:��4/VideoKatalog.View;component/Resources/search30p.png���� -A   $ openTrailerLink��$��24��$��305,472.861,0,0q�$	��Top=�$��9.333(�$��Verdana)�$ Otvori link��$
��Left=�$��26����Z� ��Z� .���� $��20��$��20��$8��2/VideoKatalog.View;component/Resources/link20p.png��$	��Top=�$
��Left=�$ Otvori link���� -B   $ openInternetLink��$��305,408,0,0q�$��9.333(�$��Verdana)�$
��Left=�$��26��$��23��$	��Top=�$ Otvori link����Z� ��Z� .���� $��18��$��18��$8��2/VideoKatalog.View;component/Resources/link20p.png��$ Otvori link���� -C   $$ useOriginalNameCB��$"��Koristi samo orginalni naziv��$
��Left=�$��214.05,8,0,0q�$	��Top=�$ 0.484,0.747&�$��9.333(��� -D   $ copyAndOpenEditorBTN��$��55��$��863,4.999,0,0q�$	��Top=�$��10.667(�$, &Kopiraj sliku i otvori slikovni editor��$
��Left=�$��55����Z� ��Z� .���� $��47��$��50��$=��7/VideoKatalog.View;component/Resources/ptohostop48p.png��$, &Kopiraj sliku i otvori slikovni editor���� -E   ' ��Name$' imageQualityLevelSlider��$��629,510,0,0q�  SimpleSlider#���  $	��Top=�( � Maximum$	( 100Q�) � Minimum$) 1Q�* � Value$* 70Q�+ � UseLayoutRounding+ . $ 	Kvaliteta��$��18�����	#FF661B1B$
��Left=�$	��281����Z� ��Z� %	  Xceed.Wpf.Toolkit.IntegerUpDown	  -F   , 	 Name$, 	castLimit��$��308.999,115.176,0,0q�$	��Top=�$
��Left=�$( "Maksimalan broj prikazanih glumaca���� -G   $ 
addEpisode��$��364,252.361,0,271.639q�$
��Bold"�$��18.667(�$��-4,-13,-4,-4q�$ Dodaj epizodu��$ 0.704,0.235&�$
��Left=�$��20����Z� ��Z�    -   Color$- 	#FFBDEDA8��.   ShadowDepth$. 1Q�.���� $��16��$��16��$8��2/VideoKatalog.View;component/Resources/plus20p.png��$��	0,8.9,0,0q�$ Dodaj epizodu���� -H   $ 	addSeason��$��20��$��90.778,252.361,0,0q�$	��Top=�$
��Bold"�$��18.667(�$��-4,-13,-4,-4q�$ Dodaj sezonu��$ 0.704,0.235&�$
��Left=�$��20����Z� ��Z�    $- 	#FFBDEDA8��$. 1Q�.���� $��16��$��16��$8��2/VideoKatalog.View;component/Resources/plus20p.png��$��	0,8.9,0,0q�$ Dodaj sezonu���� -I   $ removeEpisode��$��364,276.361,0,0q�$��18.667(�$��-4,-12.3,-4,-4q�$
��Bold"�$ Makni odabranu epizodu��$
��Left=�$��20��$��20��$	��Top=���Z� ��Z�    $- 	#FFFFBBBB��$. 1Q�.���� $��16��$��16��$9��3/VideoKatalog.View;component/Resources/minus20p.png��$��	0,8.5,0,0q�$ Makni odabranu epizodu���� -J   $ removeSeason��$��20��$��90.778,276.361,0,0q�$	��Top=�$��18.667(�$��-4,-12.3,-4,-4q�$
��Bold"�$ Makni odabranu sezonu��$
��Left=�$��20����Z� ��Z�    $- 	#FFFFBBBB��$. 1Q�.���� $��16��$��16��$9��3/VideoKatalog.View;component/Resources/minus20p.png��$��	0,8.5,0,0q�$ Makni odabranu sezonu���� $
��Left=�$��8.545,233.187,0,0q�$
��Wrap=�$	��Top=�$��SEZONE��$ 
1.12,0.334&����	#B2000000$��14.667(�$
��Bold"��� -K   $ seasonListBox��$��8.778,252.361,0,0q�$/ �� IsSynchronizedWithCurrentItem$
/ TrueY�$��78��$
��Left=�$��103.334��$	��Top=�����    �� -L   $ episodeListBox��$��120,252.361,0,0q�$	��240��$
��Left=�$��10.667(�$��Disabled=�$��103.334��$	��Top=�����    �� $
��Left=�$��120,234.96,0,0q�$
��Wrap=����	#B2000000$	��Top=�$��EPIZODE��$��13.333(�$
��Bold"��� -M   $ 
editSeason��$��20��$��90.778,335.695,0,0q�$	��Top=�$��18.667(�$��-4,-12.3,-4,-4q�$
��Bold"�$ Izmijeni odabranu sezonu��$
��Left=�$��20����Z� ��Z� .���� $��16��$��16��$8��2/VideoKatalog.View;component/Resources/edit20p.png��$��	0,8.5,0,0q�$ Izmjeni odabranu sezonu���� -N   $ editEpisode��$��20��$��364,335.695,0,0q�$	��Top=�$��18.667(�$��-4,-12.3,-4,-4q�$
��Bold"�$ Izmijeni odabranu epizodu��$
��Left=�$��20����Z� ��Z� .���� $��16��$��16��$8��2/VideoKatalog.View;component/Resources/edit20p.png��$��	0,8.5,0,0q�$ Izmijeni odabranu epizodu���� -O   $ setInfoToMain��$��472,58.63,0,0q�$	��Top=�$��9.333(�$��44.093��$
��Left=�$	��128��.���� $!��Prikaži glavne informacije��$��Center=�$��Center=�$
��Wrap=�$��13.333(�0 �� TextAlignment$0 Center=�$C =Postavi internet link, trailer link i radnju od glavne serije��� -P   $ serieTechInfoGrid��$��	976,0,0,0q�
 �   ����	#FFBAD6DA$	��Top=�$	��544��$
��Left=�$	��168��   $	 26Q�.+��� -Q   $ widthTextBox��$��8,323.665,0,0q�$
 Wrap=�$
��Left=�$��67.287��{�.  $4��./Video%20Katalog;component/Fonts/#Cambria Math)�$��16(�
���   �$	��Top=����� *Width+�� -R   $ heightTextBox��$��92.713,323.665,0,0q�$
 Wrap=�{�.  $
��Left=�$��67.287��$4��./Video%20Katalog;component/Fonts/#Cambria Math)�$��16(�
���   �$	��Top=����� *Height+�� -S   $ bitrateTextBox��$��8.525,273.167,0,0q�$
 Wrap=�{�.  $��16(�$��151.475��$
��Left=�$��24��$	��Top=�$4��./Video%20Katalog;component/Fonts/#Cambria Math)�
���   ����� *	Bitrate+1 �� 	Converter#1 [  �� -T   $ runtimeTextBox��$��92.713,377.167,0,0q�$
 Wrap=�$ 0.444,0.736&�{�.  $
��Left=�$��67.287��$��24��$	��Top=�$4��./Video%20Katalog;component/Fonts/#Cambria Math)�$��16(�
���   ����� *	Runtime+#1 [ �� -U   $ aspectRatioTextBox��$��24��$��8,377.167,0,0q�$
 Wrap=�$	��Top=�$
��Left=�$��67.287��{�.  $4��./Video%20Katalog;component/Fonts/#Cambria Math)�$��16(�
���   ����� *AspectRatio+�� -V   $ sizeTextBox��$��26.08��$��8.762,225.127,0,0q�$
 Wrap=�$	��Top=�{�.  $��16(�$
��Left=�$��151.238��
���   �$4��./Video%20Katalog;component/Fonts/#Cambria Math)����� *Size+#1 [ �� $
��Left=�$��9.263,307.812,0,0q�$
��Wrap=�$��
Rezolucija��$��15.854��$	��Top=����	#BF000000�� $
��Left=�$��8,362.98,0,0q�$
��Wrap=�$	��Top=�$��Aspect ratio�����	#BF000000$��10.667(��� $��9.263,257.206,0,0q�$
��Wrap=�$��Bitrate��$
��Left=�$	��Top=����	#B2000000�� $��20��$��	8,152,0,0q�$
��Wrap=�$	��Top=�$��14.667(�$��# HDD��$
��Left=����	#B2000000�� $
��Left=�$��9.263,209.166,0,0q�$
��Wrap=�$	��Top=�$��	Veličina��$ 0.617,0.433&����	#B2000000�� $��92.713,361.206,0,0q�$
��Wrap=�$	��Top=�$��Trajanje��$ 0.496,0.687&�$
��Left=����	#BF000000�� $
��Left=�$��9.234,410,0,0q�$
��Wrap=�$��13.333(�$��Audio��$	��Top=�$��17.74�����	#B2000000�� -W   $ audioLanguagesListBox��$��	8,428,0,0q�$��44��$	��Top=�$
��Left=�$	��152������ *	AudioList+�� -X   $ addAudioLng��$��140,408,0,0q�$
��Bold"�$��14.667(�$��-4,-11,-4,-4q�$
��Left=�$��20��$ Izmijeni audio jezik��$��20��$	��Top=���Z� ��Z� .���� $��16��$��16��$8��2/VideoKatalog.View;component/Resources/edit20p.png��$��-0.2,6,0,-1.2q�$ Izmijeni audio jezik���� $
��Left=�$��8.503,473,0,0q�$
��Wrap=�$	��Top=�$��13.333(�$��Jezici titlova��$��17.733��$��77.71�����	#B2000000�� -Y   $ subtitleLangsListBox��$��	8,491,0,0q�$
��Left=�$	��132��$��44��$	��Top=����� *SubtitleLanguageList+�� -Z   $ addSubtitleLng��$��140,491,0,0q�$
��Bold"�$��14.667(�$��-4,-10.8,-4,-4q�$	��Top=�$��20��$
��Left=�$��20��$" Dodaj prozvoljni jezik titla����Z� ��Z� .���� $��16��$��16��$8��2/VideoKatalog.View;component/Resources/plus20p.png��$��	0,6.8,0,0q��� -[   $ removeSubtitleLng��$��20��$
��Left=�$��140,515,0,0q�$	��Top=�$��20��$
��Bold"�$��14.667(�$��-4,-11,-4,-4q�$ Makni odabrani jezik����Z� ��Z� .���� $��16��$��16��$9��3/VideoKatalog.View;component/Resources/minus20p.png��$��	0,6.8,0,0q��� -\   $ hddComboBox��$��8.5,172,0,0q�$	��Top=�$��24��$��13.333(�$
��Left=�$��123.5���� -]   $ loadFileButton��$��124.43��$��8,19.57,0,0q�$	��Top=�$
��Bold"�$��Center=�$��Center=�$��24(�$
��Left=�$	��152�����	#57000000$ Učitaj��.���� $��116.43��$	��128��$@��:/VideoKatalog.View;component/Resources/browseVideo128p.png��$ Učitaj���� $��79.596,326.932,79.667,0q�$
��Wrap=�$��X��$��13.333(�$	��Top=�$
��Bold"��� -^   $ 	addNewHdd��$��136,172,0,0q�$
��Bold"�$��14.667(�$��-4,-10.8,-4,-4q�$	��Top=�$��24��$
��Left=�$��24��$ Dodaj novi HDD����Z� ��Z� .���� $��20��$��20��$8��2/VideoKatalog.View;component/Resources/plus20p.png��$��	0,6.8,0,0q�$ Dodaj novi HDD���� -_   $ 	loadLabel��$��8.25,1.61,5.25,0q�$
��Wrap=�$��Učitaj folder��$	��Top=�$
��Bold"��� -`   $ cancelNewMovieButton��$��Right=�$��85��$��
0,0,300,15q�$��Bottom=�$	��110��$
��Bold"�$��14.667(�$# Odustani od unosa nove serije������ $	 0,1&�$	 0,0&�.��� $ 	#FFF3F3F3��$ 0Q�� $ 	#FFF5B6AF��$ 1Q�� $ 	#FFF1E6E5��$ 0.435Q�.���� $��70��$��70��$<��6/VideoKatalog.View;component/Resources/dismiss128p.png��$# Odustani od unosa nove serije���� -a   $ acceptNewMovieButton��$��85��$
��Bold"�$��14.667(�$ 0.5,0.5&�$ Prihvati unos nove serije��$
��Left=�$��Bottom=�$	��110��$��
300,0,0,15q�z�V� .~��� �� �� 2 �� Angle$2 -0.642Q�T� ���� $	 0,1&�$	 0,0&�.��� $ White��$
 0.01Q�� $ 	#FFE4FDE4��$ 0.474Q�� $ 	#FFB5FFB2��$ 0.974Q�.���� $��70��$��70��$;��5/VideoKatalog.View;component/Resources/accept128p.png��$ Prihvati unos nove serije��� -b   $ releaseDateGrid��$
��Left=�$��62��$��	10,0,0,56q�$��Bottom=�$	��128��   $	 11Q����� 3 �� Color#3 � ��.+��� $��
8,8.04,0,0q�$
��Wrap=�$��Datum izlaska:��$	��Top=�$��13.333(�$
��Left=�$��88��(
  "System.Windows.Controls.DatePicker
  -c   4 
 Name$4 releaseDatePicker��$��8,30,8,0q�$	��Top=�