   M S B A M L   `   `   `  ���� L  HVideoKatalog.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:Video_katalog.ConvertersVideo_katalog.Converters      Video_katalog.MovieForm   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     BlocalConverters&clr-namespace:Video_katalog.Converters     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     X TXceed.Wpf.Toolkit, Version=2.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  -     �Name$  
mainWindow�� � WindowStartupLocation$ CenterOwner=� � Title$
 Film��$	��726��$
��1128�� � 	Resources -  'Video_katalog.Converters.MoneyConverter   moneyConverter7  1Video_katalog.Converters.PersonsToStringConverter  personsToStringConverter8  2Video_katalog.Converters.LanguageToStringConverter  languageToStringConverter,  &Video_katalog.Converters.SizeConverter  sizeConverter/  )Video_katalog.Converters.BitrateConverter  bitrateConverter/  )Video_katalog.Converters.RuntimeConverter  runtimeConverter7  1Video_katalog.Converters.NameToImagePathConverter  nameToImagePathConverter��%i   &	        &	      &	 
     &	      &	      &	      &	                    .��� -    �Name$ mainGrid��$��0,4,0,6q�$	��678��.+�%  Xceed.Wpf.Toolkit.BusyIndicator  -     Name$ busyIndicator��$
��Left=�$��10,8,0,0q�$	��Top=�.��� -   $ imdbInfoGrid�� J� OpacityMask
 �   ����	#FFE0E0E0$
��Left=�$	��864��$	��Top=�$	��544�� J� Effect 3	  -System.Windows.Media.Effects.DropShadowEffect	   	  
BlurRadius$ 26Q�.+�#
  Xceed.Wpf.Toolkit.SplitButton
  -   	 
 Name$	 refreshSplitButton��$
��Left=�$��8,8,0,0q�$	��Top=�$��93��$��94.723�����	#56000000
 � ToolTip$"
 Osvježi informacije o filmu��&   Xceed.Wpf.Toolkit.DropDownButton   IsOpen .     DropDownContent �� ��Z� .+��� -    ��Name$ refreshAllButPoster��$��Sve osim postera��$
��Left=�$��90��$��25��$��11(���Z� ��Z� �� -   $ refreshOnlyPoster��$��Samo poster��$
��Left=�$��90��$��25��$��11(���Z� ��Z� �� -   $ refreshBudget��$��Budget��$
��Left=�$��90��$��25��$��11(���Z� ��Z� �� -	   $ refreshEarnings��$��Zarada��$
��Left=�$��90��$��25��$��11(���Z� ��Z� �� -
   $ refreshSummary��$��Radnja��$
��Left=�$��90��$��25��$��11(���Z� ��Z� �� -   $ refreshCastAndCrew��$��Glumci i redatelji��$
��Left=�$��90��$��25��$��11(���Z� ��Z� �� -   $ refreshRating��$��Ocjena��$
��Left=�$��90��$��25��$��11(���Z� ��Z� �� -   $ refreshGenres��$��Žanrovi��$
��Left=�$��90��$��25��$��11(���Z� ��Z� .���� $��80��$��72��$;��5/VideoKatalog.View;component/Resources/refresh70p.png���� -    ��Name$ customNameTextBox��$
��Left=�$��28.133��$��105,22.959,0,0q�$	��Top=�$��203.949��$��14.667(�$ �� HorizontalScrollBarVisibility$ Disabled=�$
��Bold"�$��Calibri)�$��1,2,1,1q�$��2q�
���   � �� MaxLines$ 1������ *Name+�� -   $ originalNameTextBox��$��28.133��$��105,73.296,0,0q�$	��Top=�$��14.667(�$ Disabled=�$
��Bold"�$��Calibri)�$��1,2,1,1q�$
��Left=�$��203.949��
���   �$��2q�$ 1������ *
OrigName+�� -   $ summaryTextBox��$��251.846,140.96,0,0q� �� TextWrapping$
 Wrap=�" �� VerticalScrollBarVisibility$
 Auto=�$
��Left=�$��276.426��$	��Top=�$��296.603������ *	Summary+�� $
��Left=�$��	105,8,0,0q�$
��Wrap=�$	��Top=� J� RenderTransformOrigin$ 0.673,0.597&����	#B2000000��  � Language$ hr-hr�$�Naziv���� $
��Left=�$��105,57.336,0,0q�$
��Wrap=�$	��Top=����	#B2000000�� $ hr-hr�$�Orginalni naziv���� $
��Left=�$��8.139,125,0,0q�$
��Wrap=�$	��Top=�$��Redatelj(i)��$ 
1.12,0.334&����	#B2000000�� $
��Left=�$��7.901,213.707,0,0q�$
��Wrap=����	#B2000000$	��Top=��� $ hr-hr�$�Glumci���� $
��Left=�$��251.846,125,0,0q�$
��Wrap=�$	��Top=����	#B2000000$ 0.526,0.594&��� $ hr-hr�$�Radnja���� -   $ ratingTextBox��$��318.782,73.296,0,0q�$
��Left=�$��62.002��$ 0.702,0.597&�$��18.667(�$��28.133��$	��Top=�$ Disabled=�$��1,0,1,1q�$ 1������ *Rating+�� $��318.782,57.336,0,0q�$
��Wrap=�$	��Top=�$
��Left=�$ 
0.66,1.138&����	#B2000000�� $ hr-hr�$�Ocjena���� -   $ budgetTextBox��$��66.017,0,0,138.702q�$
 Wrap=�$��123.33��$
��Left=�$��24��$��Bottom=�$��14.667(�$��Center=�$��Center=�$��1,0,1,1q�$ Disabled=�$
��Bold"�$ 1������ *Budget+ �� 	Converter# [  �� -   $ earningsTextBox��$��66.017,0,0,106.437q�$
 Wrap=�$��24��$��Bottom=�$
��Left=�$��123.092��$��14.667(�$��Center=�$��Center=�$��1,0,1,1q�$ Disabled=�$
��Bold"����	#FF133513$ 1������ *
Earnings+# [  �� $��7.901,384.4,0,0q�$
��Wrap=�$	��Top=�$ 0,2.694&�$
��Left=�$��13.333(����	#B2000000�� $ hr-hr�$�Budget���� $��7.9,417.1,0,0q�$
��Wrap=�$	��Top=�$
��Left=�$��13.333(����	#B2000000�� $ hr-hr�$�Zarada��� -   $ 
posterGrid��$��549,70.629,0,0q����	#FFEBEBEB$	��300��$	��431��$
��Left=�$	��Top=� 	  $ 14Q�.+��� -    ��Name$ posterImage��$	��300��$	��431��$
��Fill=� ��  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.���  � Color$ Black�� � Offset$ 0Q�� $ 	#FF624545��$ 1Q��� -   $ internetLinkTextBox��$��	8,482,0,0q�$ 
0.55,-0.25&�$��10.667(�$	��Top=�$
��20.1��$
��Left=�$��205.109��$ Disabled=�$ 1������ *InternetLink+�� -   $ trailerLinkTextBox��$��280,482,0,0q�$	��Top=�$��10.667(�$
��Left=�$��248.136��$
��20.1��$ Disabled=�$ 1������ *TrailerLink+�� $��8.513,467.725,0,0q�$
��Wrap=�$
��Left=�$��14.275��$	��Top=����	#B2000000�� $ hr-hr�$�Internet link���� $��280,467.725,0,0q�$
��Wrap=�$	��Top=�$
��Left=����	#B2000000$ 0.446,0.672&��� $ hr-hr�$�Trailer link���� -   $ pasteImageButton��$��735,6.999,0,0q�$��55��$	��Top=�$
��Left=�$��55��$
 Zalijepi poster����Z� ��Z� .���� $��50��$��50��$9��3/VideoKatalog.View;component/Resources/paste50p.png��$
 Zalijepi sliku���� -   $ pasteTrailerLink��$��24��$��503.136,502.1,0,0q�$	��Top=�$��9.333(�$��Verdana)�$
��Left=�$��25����Z� ��Z� .���� $��20��$��20��$9��3/VideoKatalog.View;component/Resources/paste20p.png��$
 Zalijepi link���� -   $ pasteInternetLink��$��188.109,502.1,0,0q�$��9.333(�$��Verdana)�$
��Left=�$��25��$��24��$	��Top=���Z� ��Z� .���� $��20��$��20��$9��3/VideoKatalog.View;component/Resources/paste20p.png��$
 zalijepi link���� -   $ yearTextBox��$��Right=�$��0,73.296,335.864,0q�$��64.135��$��18.667(�$��28.133��$	��Top=�$ Disabled=�$��1,0,1,1q�$ 1������ *Year+�� $��Right=�$��0,57.336,361.865,0q�$
��Wrap=�$	��Top=�$ 0.5,0.5&����	#B2000000�� $ hr-hr�$�Godina���� -    ��Name$ castListBox��$��7.901,229.667,0,182.64q�$
��Left=�$��181.208������ *Actors+�� -   $ directorListBox��$��44��$��8.139,140.96,0,0q�$	��Top=�$
��Left=�$��181.208������ *	Directors+�� -   $ addDirector��$��20��$
��Left=�$��193.347,140.96,0,0q�$	��Top=�$��20��$
��Bold"�$��18.667(�$��-4,-13,-4,-4q�$"
 Dodaj proizvoljnog redatelja��$ 0.704,0.235&���Z� ��Z� .���� $��16��$��16��$8��2/VideoKatalog.View;component/Resources/plus20p.png��$��	0,8.9,0,0q��� -   $ removeDirector��$��20��$
��Left=�$��193.347,164.96,0,0q�$	��Top=�$��20��$��18.667(�$��-4,-12.3,-4,-4q�$
��Bold"�$
 Makni odabranog redatelja����Z� ��Z� .���� $��16��$��16��$9��3/VideoKatalog.View;component/Resources/minus20p.png��$��	0,8.5,0,0q��� -    $ addActor��$
��Left=�$��193.109,229.667,0,0q�$��20��$
��Bold"�$��18.667(�$��-4,-12,-4,-4q�$
 Dodaj proizvoljnog glumca��$��20��$	��Top=���Z� ��Z� .���� $��16��$��16��$8��2/VideoKatalog.View;component/Resources/plus20p.png��$��	0,7.7,0,0q��� -!   $ removeActor��$
��Left=�$��193.109,253.667,0,270.333q�$��20��$
��Bold"�$��18.667(�$��-4,-12,-4,-4q�$ 0.409,0.552&�$
 Makni odabranog glumca����Z� ��Z� .���� $��16��$��16��$9��3/VideoKatalog.View;component/Resources/minus20p.png��$��	0,7.9,0,0q��� -"    ��Name$ label1��$
��MAX:��$��97.915,202,0,0q�$
��Left=�$	��Top=��� -#    ��Name$ genresComboBox��$��318.782,21.959,335.728,0q�$	��Top=�$��29.133�� �� MaxDropDownHeight$	 479�� �� 
IsEditable .   �� 
IsReadOnly  . ! �� Text$!  ��$��13.333(�$��	3,4.5,3,3q�.@��� -$   " ��Name$" actionCheckBox��$��Akcija��
���iii��� -%   $" adventureCheckBox��$��Avantura��
���iii��� -&   $" animationCheckBox��$��	Animacija��
���iii��� -'   $" biographyCheckBox��$��
Biografija��
���iii��� -(   $" comedyCheckBox��$��Komedija��
���iii��� -)   $" crimeCheckBox��$��Krimi��
���iii��� -*   $" documentaryCheckBox��$��Dokumentarni��
���iii��� -+   $" dramaCheckBox��$��Drama��
���iii��� -,   $" familyCheckBox��$��
Obiteljski��
���iii��� --   $" fantasyCheckBox��$��
Fantastika��
���iii��� -.   $" filmNoirCheckBox��$��Crno-bijeli��
���iii��� -/   $" historyCheckBox��$��	Povijesni��
���iii��� -0   $" horrorCheckBox��$��Horor��
���iii��� -1   $" musicCheckBox��$��Muzički��
���iii��� -2   $" musicalCheckBox��$��Pjevan��
���iii��� -3   $" misteryCheckBox��$��	Misterija��
���iii��� -4   $" romanceCheckBox��$��	Romantika��
���iii��� -5   $" sciFiCheckBox��$��Znanstvena fantastika��
���iii��� -6   $" sportCheckBox��$��Sportski��
���iii��� -7   $" thrillerCheckBox��$��Triler��
���iii��� -8   $" warCheckBox��$��Ratni��
���iii��� -9   $" westernCheckBox��$��Vestern��
���iii��� $��318.782,5.999,0,0q�$
��Wrap=�$	��Top=�$��Žanrovi��$
��Left=�$ 0.628,0.461&����	#B2000000	� -:   # 	�Name$# posterImpAwardsRadio��$��
IMP Awards��$��548.999,26.959,0,0q�$	��Top=�$ d� 	IsChecked$
$ TrueY�% 	� 	GroupName$% posters��$
��Left=�	� -;   $# posterGoogleRadio��$��Google��$��548.999,45.919,0,0q�$	��Top=�$% posters��$
��Left=�	� -<   $# posterMoviePosterDBRadio��$��Movie Poster DB��$��548.999,6.999,0,0q�$	��Top=�$% posters��$
��Left=��� -=   $ searchPosterBTN��$��55��$��676,6.999,0,0q�$	��Top=�$
��Left=�$��55��$
��Bold"�
���   �$��0q�$��-4q�$
 Trazi poster����Z� ��Z� .���� $��45��$��45��$:��4/VideoKatalog.View;component/Resources/search64p.png��$��UniformToFill=�	� ->   $# trailerYoutubeRadio��$��Youtube��$
��Left=�$��397.391,456,0,0q�$	��Top=�$
$ TrueY�$% trailer��$��8(�	� -?   $# trailerGoogleRadio��$��Google��$��0,456,369.159,0q�$	��Top=�$% trailer��$��8(�$��Right=��� -@   $ searchTrailer��$��28��$��498.841,451.725,0,0q�$	��Top=�$��9.333(�$��Verdana)�$
��Left=�$��29.431��$
 Traži trailer����Z� ��Z� .���� $��23��$��23��$:��4/VideoKatalog.View;component/Resources/search30p.png���� -A   $ openTrailerLink��$��24��$��474.136,502.1,0,0q�$	��Top=�$��9.333(�$��Verdana)�$
��Left=�$��25��$
 Otvori link����Z� ��Z� .���� $��20��$��20��$8��2/VideoKatalog.View;component/Resources/link20p.png��$	��Top=�$
��Left=��� -B   $ openInternetLink��$��159.109,502.1,0,0q�$��9.333(�$��Verdana)�$
��Left=�$��25��$��24��$	��Top=�$
 Otvori link����Z� ��Z� .���� $��18��$��18��$8��2/VideoKatalog.View;component/Resources/link20p.png��$
  ���� -C   $" useOriginalNameCB��$"��Koristi samo orginalni naziv��$
��Left=�$��176.272,8.959,0,0q�$	��Top=�$ 0.484,0.747&�$��9.333(��� -D   $ copyAndOpenEditorBTN��$��55��$��794,6.999,0,0q�$	��Top=�$��10.667(�$,
 &Kopiraj sliku i otvori slikovni editor��$
��Left=�$��55����Z� ��Z� .���� $��47��$��50��$=��7/VideoKatalog.View;component/Resources/ptohostop48p.png���� -E   & ��Name$& imageQualityLevelSlider��$��560,512,0,0q�  SimpleSlider#���  $	��Top=�' � Maximum$	' 100Q�( � Minimum$( 1Q�) � Value$) 70Q�* � UseLayoutRounding* . $
 	Kvaliteta��$��18�����	#FF661B1B$
��Left=�$	��281����Z� ��Z� %  Xceed.Wpf.Toolkit.IntegerUpDown  -F   +  Name$+ 	castLimit��$��136.109,204,0,0q�$	��Top=�$
��Left=�$��51���� -G   $ cancelNewMovieButton��$��Right=�$��85��$��
0,0,300,15q�$��Bottom=�$	��110��$
��Bold"�$��14.667(�$
 Odbaci i zatvori������ $	 0,1&�$	 0,0&�.��� $ 	#FFF3F3F3��$ 0Q�� $ 	#FFF5B6AF��$ 1Q�� $ 	#FFF1E6E5��$ 0.435Q�.���� $��70��$��70��$<��6/VideoKatalog.View;component/Resources/dismiss128p.png���� -H   $ acceptNewMovieButton��$��85��$
��Bold"�$��14.667(�$ 0.5,0.5&�$
 Prihvati��$
��Left=�$��Bottom=�$	��110��$��
300,0,0,15q�z�V� .~��� �� �� , �� Angle$, -0.642Q�T� ���� $	 0,1&�$	 0,0&�.��� $ White��$
 0.01Q�� $ 	#FFE4FDE4��$ 0.474Q�� $ 	#FFB5FFB2��$ 0.974Q�.���� $��70��$��70��$;��5/VideoKatalog.View;component/Resources/accept128p.png���� -I   $" viewedCheckBox��$��	Pogledano��$��0,0,56,97.17q�$��Bottom=�$��18.667(�$��Right=�$ �� *
IsViewed+� -J   $ releaseDateGrid��$
��Left=�$��62��$��	10,0,0,38q�$��Bottom=�$	��128�� 	  $ 11Q����� - �� Color#- � ��.+��� $��
8,8.04,0,0q�$
��Wrap=�$��Datum izlaska:��$	��Top=�$��13.333(�$
��Left=�$��88��(  "System.Windows.Controls.DatePicker  -K   .  Name$. releaseDatePicker��$��8,30,8,0q�$	��Top=�  -L   $  busyIndicatorScanningVideo��$��926,8,0,126q�$
��Left=�$	��168��.��� -M   $ movieTechInfoGrid��
 �   ����	#FFBAD6DA$	��Top=�$	��544��$
��Left=�$	��168�� 	  $ 26Q�.+��� -N   $ widthTextBox��$��10.759,316.894,0,0q�$
 Wrap=�$
��Left=�$��64.983��{�.  $0��*/Zavrsni Rad;component/Fonts/#Cambria Math)�$��16(�
���   �$	��Top=����� *Width+�� -O   $ heightTextBox��$��92.713,316.894,0,0q�$
 Wrap=�{�.  $
��Left=�$��67.287��$0��*/Zavrsni Rad;component/Fonts/#Cambria Math)�$��16(�
���   �$	��Top=����� *Height+�� -P   $ bitrateTextBox��$��10.759,270.901,0,0q�$
 Wrap=�{�.  $��16(�$��149.241��$
��Left=�$��24��$	��Top=�$0��*/Zavrsni Rad;component/Fonts/#Cambria Math)�
���   ����� *	Bitrate+# [ �� -Q   $ runtimeTextBox��$��92.713,367.041,0,0q�$
 Wrap=�$ 0.444,0.736&�{�.  $
��Left=�$��67.287��$��24��$	��Top=�$0��*/Zavrsni Rad;component/Fonts/#Cambria Math)�$��16(�
���   ����� *	Runtime+# [ �� -R   $ aspectRatioTextBox��$��24��$��10.996,367.041,0,0q�$
 Wrap=�$	��Top=�$
��Left=�$��64.746��{�.  $0��*/Zavrsni Rad;component/Fonts/#Cambria Math)�$��16(�
���   ����� *AspectRatio+�� -S   $ sizeTextBox��$��26.08��$��10.996,222.861,0,0q�$
 Wrap=�$	��Top=�{�.  $��16(�$
��Left=�$��149.004��
���   �$0��*/Zavrsni Rad;component/Fonts/#Cambria Math)����� *Size+# [ �� $
��Left=�$��9.976,301.041,0,0q�$
��Wrap=�$��
Rezolucija��$��15.854��$	��Top=����	#BF000000�� $
��Left=�$��11.709,352.04,0,0q�$
��Wrap=�$	��Top=�$��Aspect ratio�����	#BF000000$��10.667(��� $��11.497,254.94,0,0q�$
��Wrap=�$��Bitrate��$
��Left=�$	��Top=����	#B2000000�� $��20��$��	8,152,0,0q�$
��Wrap=�$	��Top=�$��14.667(�$��# HDD��$
��Left=����	#B2000000�� $
��Left=�$��11.497,206.9,0,0q�$
��Wrap=�$	��Top=�$��	Veličina��$ 0.617,0.433&����	#B2000000�� $��92.713,352.04,0,0q�$
��Wrap=�$	��Top=�$��Trajanje��$ 0.496,0.687&�$
��Left=����	#BF000000�� $
��Left=�$��11.901,403.667,0,0q�$
��Wrap=�$��13.333(�$��Audio��$	��Top=�$��73��$��17.74�����	#B2000000�� -T   $ audioLanguagesListBox��$��11.17,421.667,0,0q�$��44��$	��Top=�$
��Left=�$��148.83������ *	AudioList+�� -U   $ addAudioLng��$��140,401.407,0,0q�$
��Bold"�$��14.667(�$��-4,-11,-4,-4q�$��20��$	��Top=�$
��Left=�$��20��$
 Izmijeni jezik audia����Z� ��Z� .���� $��16��$��16��$8��2/VideoKatalog.View;component/Resources/edit20p.png��$��-0.2,6,0,-1.2q�$
 Izmijeni jezik audia���� $
��Left=�$��11.17,466.667,0,0q�$
��Wrap=�$	��Top=�$��13.333(�$��Jezici titlova��$��17.733��$��77.71�����	#B2000000�� -V   $ subtitleLangsListBox��$��11.17,484.667,0,0q�$
��Left=�$��128.83��$��44��$	��Top=����� *SubtitleLanguageList+�� -W   $ addSubtitleLng��$��140,484.667,0,0q�$
��Bold"�$��14.667(�$��-4,-10.8,-4,-4q�$	��Top=�$��20��$
��Left=�$��20��$"
 Dodaj prozvoljni jezik titla����Z� ��Z� .���� $��16��$��16��$8��2/VideoKatalog.View;component/Resources/plus20p.png��$��	0,6.8,0,0q�$
 Dodaj novi jezik titla���� -X   $ removeSubtitleLng��$��20��$
��Left=�$��140,508.667,0,0q�$	��Top=�$��20��$
��Bold"�$��14.667(�$��-4,-11,-4,-4q�$
 Makni odabrani jezik����Z� ��Z� .���� $��16��$��16��$9��3/VideoKatalog.View;component/Resources/minus20p.png��$��	0,6.8,0,0q�$ 
 Makni odabrani jezik titla���� -Y   $ hddComboBox��$��8.5,172,0,0q�$	��Top=�$��24��$��13.333(�$
��Left=�$��127.5������ / �� SelectedItem/ �� *Hdd+�� -Z   $ loadFileButton��$��132.43��$��8,8,0,0q�$	��Top=�$
��Bold"�$��Center=�$��Center=�$��24(�$
��Left=�$	��141�����	#57000000.���� $	��128��$	��128��$@��:/VideoKatalog.View;component/Resources/browseVideo128p.png��$"
 Učitaj video datoteku filma���� $��79.976,320,79.12,0q�$
��Wrap=�$��X��$��13.333(�$	��Top=�$
��Bold"��� -[   $ 	addNewHdd��$��140,172,0,0q�$
��Bold"�$��14.667(�$��-4,-10.8,-4,-4q�$	��Top=�$��24��$
��Left=�$��24��$
 Dodaj novi HDD����Z� ��Z� .���� $��20��$��20��$8��2/VideoKatalog.View;component/Resources/plus20p.png��$��	0,6.8,0,0q�$
 Dodaj novi HDD���� -\   $��
pauseTimer��$��Pause��$��23��$
��Left=�$��1019,643,0,0q�$	��Top=�$��75��$y�Hidden=��� -]   $��timerValueTB��$��16.667��$��Right=�$��0,623.333,18,0q�$	��Top=�$��69��0 �� TextAlignment$0 Center=�$��	0.0 / 1.0��