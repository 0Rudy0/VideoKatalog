   M S B A M L   `   `   `  ����L  HVideoKatalog.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:Video_katalog.ConvertersVideo_katalog.Converters  5   	       Video_katalog.SerieForm   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   KVideo_katalog_Converters&clr-namespace:Video_katalog.Converters     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     6Q   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6�   X TXceed.Wpf.Toolkit, Version=2.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  6�   -   5          �Name$  
mainWindow��5      � WindowStartupLocation$ CenterOwner=�5   	    � Title$ Serija��6-   $	��734��6<   $
��1192��6I    � 	Resources 5      /  )Video_katalog.Converters.BitrateConverter   bitrateConverter/  )Video_katalog.Converters.RuntimeConverter  runtimeConverter,  &Video_katalog.Converters.SizeConverter  sizeConverter-  'Video_katalog.Converters.MoneyConverter  moneyConverter��%t   &	        &	      &	 &     &	 9       5      6.     5      6.     5	      6+     5
      6,   5      .��5      � -   5         �Name$ mainGrid��5      .+�5   
   %  Xceed.Wpf.Toolkit.BusyIndicator  -   5          Name$ busyIndicator��5      $
��Left=�64   $��8,8,0,0q�6O   $	��Top=�6`   .��5      � -   5        $ imdbAndTechInfoGrid��5   
   $
��Left=�6'   $	��Top=�6B   .+�5      �� $��	Pogledano��6   $��1030.343,0,0,-48q�6#   $��Bottom=�6=   $��18.667(�6X   $
��Left=�6�    d� 	IsChecked 6j   �� *
IsViewed+6�   � 5      -   5        $ imdbInfoGrid��5       J� OpacityMask
 �   ����	#FFE0E0E0$
��Left=�6L   $	��936��6g   $	��Top=�6s   $	��544��6�    J� Effect 5      3  -System.Windows.Media.Effects.DropShadowEffect  5      	   
BlurRadius$	 26Q�6   .+�5      #  Xceed.Wpf.Toolkit.SplitButton  -   5        
  Name$
 refreshSplitButton��5   '   $
��Left=�6C   $��8,8,0,0q�6^   $	��Top=�6o   $��93��6�   $��94.723��6�   ���	#56000000 � ToolTip$" Osvježi informacije o filmu��6�   &   Xceed.Wpf.Toolkit.DropDownButton   IsOpen .     DropDownContent 5      �� 5   	   .+�5   "   �� -   5         ��Name$ refreshAllButPoster��5   )   $��Sve osim postera��6F   $
��Left=�6�   $��90��6�   $��25��6�   $��11(�6�   ��6a   Z� ��6x   Z� 6�   �� 5   "   -   5        $ refreshEpisodes��5   )   $��Sezone i epizode��6B   $
��Left=�6�   $��90��6�   $��25��6�   $��11(�6�   ��6]   Z� ��6t   Z� 6�   �� 5   "   -	   5        $ refreshOnlyPoster��5   )   $��Samo poster��6D   $
��Left=�6�   $��90��6�   $��25��6�   $��11(�6�   ��6Z   Z� ��6q   Z� 6�   �� 5   "   -
   5        $ refreshSummary��5   )   $��Radnja��6A   $
��Left=�6   $��90��6�   $��25��6�   $��11(�6�   ��6R   Z� ��6i   Z� 6�   �� 5   "   -   5        $ refreshCastAndCrew��5   )   $��Glumci i redatelji��6E   $
��Left=�6�   $��90��6�   $��25��6�   $��11(�6�   ��6b   Z� ��6y   Z� 6�   �� 5   "   -   5        $ refreshRating��5   )   $��Ocjena��6@   $
��Left=�6~   $��90��6�   $��25��6�   $��11(�6�   ��6Q   Z� ��6h   Z� 6�   �� 5   "   -   5        $ refreshGenres��5   )   $��Žanrovi��6@   $
��Left=�6   $��90��6�   $��25��6�   $��11(�6�   ��6R   Z� ��6i   Z� 6�   5      .��5       �� $��80��6   $��74��6   $;��5/VideoKatalog.View;component/Resources/refresh70p.png��6%   5!      �� 5"      -   5         ��Name$ customNameTextBox��5"      $
��Left=�6*   $��28.133��6E   $��108.25,22.707,0,0q�6U   $	��Top=�6p   $��255.75��6�   $��14.667(�6�   $ �� HorizontalScrollBarVisibility$ Disabled=�6�   $
��Bold"�6�   $��Calibri)�6�   $��1,2,1,1q�6  
���   �$��2q�65   �� MaxLines$ 1��6I  ��6�   �� *Name+6I  �� 5#      -   5        $ originalNameTextBox��5#      $��28.133��6,   $��108.25,74.59,0,0q�6<   $	��Top=�6V   $��14.667(�6�   $ Disabled=�6�   $
��Bold"�6�   $��Calibri)�6�   $��1,2,1,1q�6�   $��255.75��6�   $
��Left=�6  
���   �$��2q�6b  $ 1��6v  ��6n   �� *
OrigName+6v  �� 5$      -   5        $ summaryTextBox��5$      $��392,140.962,336,0q�6'    �� TextWrapping$
 Wrap=�6B   " �� VerticalScrollBarVisibility$
 Auto=�6V   $	��Top=�6�   $��358.667��6�   $��Stretch=�6�   $��Stretch=�6�   ��6y   �� *	Summary+6�   �� 5%      $
��Left=�6   $��108.25,8,0,0q�6,   $
��Wrap=�6B   $	��Top=�6V    J� RenderTransformOrigin$ 0.673,0.597&�6n   ���	#B2000000�� 6�    � Language$ hr-hr�6�   $�Naziv��6�   6�   �� 5&      $
��Left=�6   $��108.25,58.63,0,0q�6,   $
��Wrap=�6F   $	��Top=�6Z   ���	#B2000000$��81.268��6�   �� 6�   $ hr-hr�6�   $�Orginalni naziv��6�   6�   �� 5'      $
��Left=�6   $��8.545,125.176,0,0q�6,   $
��Wrap=�6G   $	��Top=�6[   $��Redatelj(i)��6s   $ 
1.12,0.334&�6�   ���	#B20000006�   �� 5(      $
��Left=�6   $��194.122,125.176,0,0q�6,   $
��Wrap=�6I   ���	#B2000000$	��Top=�6t   �� 6�   $ hr-hr�6�   $�Glumci��6�   6�   �� 5)      -   5         ��Name$ summaryLabel��5)      $
��Left=�6'   $��392,125.176,0,0q�6B   $
��Wrap=�6[   $	��Top=�6o   ���	#B2000000$ 0.526,0.594&�6�   �� 6�   $ hr-hr�6�   $�Radnja��6�   6  �� 5*      -   5        $ ratingTextBox��5*      $��376,74.59,0,0q�6&   $ 0.702,0.597&�6U   $��18.667(�6y   $��28.133��6�   $	��Top=�6�   $ Disabled=�6�   $��1,-2,1,1q�6  $ 1��6  $
��Left=�6"  $��59.973��6=  ��6=   �� *Rating+6=  �� 5+      $��376,58.63,0,0q�6   $
��Wrap=�6(   $	��Top=�6<   $ 
0.66,1.138&�6T   ���	#B2000000$
��Left=�6�   �� 6�   $ hr-hr�6�   $�Ocjena��6�   6�   � 5,      -   5        $ 
posterGrid��5,      $��618,68.629,0,0q�6    ���	#FFEBEBEB$	��300��6O   $	��431��6[   $
��Left=�6h   $	��Top=�6�    5-        5.   	   $	 14Q�6   .+�50      �� -   5         ��Name$ posterImage��50      $	��300��6#   $	��431��6/   $
��Fill=�6<    51   	   �� 52   
    �� EndPoint$ 0.5,1&�6    �� 
StartPoint$ 0.5,0&�6/   .��53      �  � Color$ Black��6    � Offset$ 0Q�6&   � 54      $ 	#FF624545��6   $ 1Q�6*   55      57   	   58      �� 59      -   5        $ internetLinkTextBox��59      $��8.545,385.805,0,0q�6,   $ 
0.55,-0.25&�6G   $	��Top=�6j   $��22.195��6�   $
��Left=�6�   $��351.455��6�   $ Disabled=�6�   $ 1��6  ��6�   �� *InternetLink+ �� Mode$ TwoWay=�6  �� 5:      -   5        $ trailerLinkTextBox��5:      $��8.778,450.667,0,0q�6:   $	��Top=�6U   $��22.195��6m   $ Disabled=�6}   $��351.222��6�   $
��Left=�6�   $ 1��6�   ��6�   �� *TrailerLink+$ TwoWay=�6�   �� 5;      -   5        $ internetLinkLabel��5;      $��9.541,371.53,0,0q�6,   $
��Wrap=�6F   $
��Left=�6Z   $��14.275��6u   $	��Top=�6�   ���	#B2000000�� 6�   $ hr-hr�6�   $�Internet link��6�   6�   �� 5<      -   5        $ trailerLinkLabel��5<      $��9.541,434.707,0,0q�6+   $
��Wrap=�6F   $	��Top=�6Z   $
��Left=�6r   ���	#B2000000$ 0.446,0.672&�6�   �� 6�   $ hr-hr�6�   $�Trailer link��6�   6�   �� 5=      -   5        $ pasteImageButton��5=      $��804,4.999,0,0q�6(   $��55��6^   $	��Top=�6j   $
��Left=�6�   $��55��6�   $ Zalijepi poster��6�   ��6�   Z� ��6�   Z� .��5>      �� $��50��6   $��50��6   $9��3/VideoKatalog.View;component/Resources/paste50p.png��6%   $ Zalijepi poster��6b   5?      �� 5@      -   5        $ pasteTrailerLink��5@      $��24��6(   $��335,472.861,0,0q�64   $	��Top=�6M   $��9.333(�6e   $��Verdana)�6v   $
��Left=�6�   $��25��6�   $ Zalijepi link��6�   ��6�   Z� ��6�   Z� .��5A      �� $��20��6   $��20��6   $9��3/VideoKatalog.View;component/Resources/paste20p.png��6%   $ Zalijepi link��6b   5B      �� 5C      -   5        $ pasteInternetLink��5C      $��335,408,0,0q�6)   $��9.333(�6>   $��Verdana)�6O   $
��Left=�6d   $��25��6   $��24��6�   $	��Top=�6�   $ Zalijepi link��6�   ��6�   Z� ��6�   Z� .��5D      �� $��20��6   $��20��6   $9��3/VideoKatalog.View;component/Resources/paste20p.png��6%   $ zalijepi link��6b   5E      �� 5F      -   5         ��Name$ castListBox��5F   ,   $��194.122,140.962,0,0q�6   $
��Left=�6A   $��165.878��6\   $��85��6�   $	��Top=�6�   ��6l   �� *Actors+6�   �� 5G      -   5        $ directorListBox��5G   6   $��85��6   $��8.778,140.962,0,0q�6   $	��Top=�6O   $
��Left=�6�   $��151.222��6�   ��6g   �� *	Directors+6�   �� 5H      -   5        $ addDirector��5H   3   $��20��6   $��160,140.962,0,0q�6   $	��Top=�6H   $
��Bold"�6z   $��18.667(�6�   $��-4,-13,-4,-4q�6�   $" Dodaj proizvoljnog redatelja��6�   $ 0.704,0.235&�6�   $
��Left=�6-  $��20��6H  ��6   Z� ��6  Z� .��5I      �� $��16��6   $��16��6   $8��2/VideoKatalog.View;component/Resources/plus20p.png��6%   $��	0,8.9,0,0q�6a   5J      �� 5K      -   5        $ removeDirector��5K   3   $��20��6   $��160,164.962,0,0q�6   $	��Top=�6K   $��18.667(�6�   $��-4,-12.3,-4,-4q�6�   $
��Bold"�6�   $ Makni odabranog redatelja��6�   $
��Left=�6  $��20��6)  ��6�   Z� ��6�   Z� .��5L      �� $��16��6   $��16��6   $9��3/VideoKatalog.View;component/Resources/minus20p.png��6%   $��	0,8.5,0,0q�6b   5M      �� 5N      -    5        $ addActor��5N   B   $
��Left=�6   $��360,140.962,0,0q�6)   $
��Bold"�6k   $��18.667(�6}   $��-4,-12,-4,-4q�6�   $ Dodaj proizvoljnog glumca��6�   $	��Top=�6�   $��20��6  $��20��6  ��6�   Z� ��6�   Z� .��5O      �� $��16��6   $��16��6   $8��2/VideoKatalog.View;component/Resources/plus20p.png��6%   $��	0,7.7,0,0q�6a   5P      �� 5Q      -!   5        $ removeActor��5Q   N   $��20��6   $
��Left=�6   $��360,164.962,0,0q�65   $	��Top=�6c   $��20��6{   $
��Bold"�6�   $��18.667(�6�   $��-4,-12,-4,-4q�6�   $ 0.409,0.552&�6�   $ Makni odabranog glumca��6�   ��6   Z� ��66  Z� .��5R      �� $��16��6   $��16��6   $9��3/VideoKatalog.View;component/Resources/minus20p.png��6%   $��	0,7.9,0,0q�6b   5S      �� 5T      -"   5         ��Name$ label1��5T   9   $
��MAX:��6   $��270.805,115.002,0,0q�6   $
��Left=�6I   $	��Top=�6d   �� 5U      -#   5         ��Name$ genresComboBox��5U      $��376,21.08,336,0q�6(   $	��Top=�6A   $��28.133��6Y     �� MaxDropDownHeight$	  479��6i   ! �� 
IsEditable! . " �� 
IsReadOnly" . # �� Text$#  ��6�   $��13.333(�6�   $��3,4,3,3q�6�   .@�5V      �� -$   5        $ ��Name$$ actionCheckBox��5V      $��Akcija��6)   
���iii�6l   �� 5W      -%   5        $$ adventureCheckBox��5W      $��Avantura��6,   
���iii�6q   �� 5X      -&   5        $$ animationCheckBox��5X      $��	Animacija��6,   
���iii�6r   �� 5Y      -'   5        $$ biographyCheckBox��5Y      $��
Biografija��6,   
���iii�6s   �� 5Z      -(   5        $$ comedyCheckBox��5Z      $��Komedija��6)   
���iii�6n   �� 5[      -)   5        $$ crimeCheckBox��5[      $��Krimi��6(   
���iii�6j   �� 5\      -*   5        $$ documentaryCheckBox��5\      $��Dokumentarni��6.   
���iii�6w   �� 5]      -+   5        $$ dramaCheckBox��5]      $��Drama��6(   
���iii�6j   �� 5^      -,   5        $$ familyCheckBox��5^      $��
Obiteljski��6)   
���iii�6p   �� 5_      --   5        $$ fantasyCheckBox��5_      $��
Fantastika��6*   
���iii�6q   �� 5`      -.   5        $$ filmNoirCheckBox��5`      $��Crno-bijeli��6+   
���iii�6s   �� 5a      -/   5        $$ historyCheckBox��5a      $��	Povijesni��6*   
���iii�6p   �� 5b      -0   5        $$ horrorCheckBox��5b      $��Horor��6)   
���iii�6k   �� 5c      -1   5        $$ musicCheckBox��5c      $��Muzički��6(   
���iii�6l   �� 5d      -2   5        $$ musicalCheckBox��5d      $��Pjevan��6*   
���iii�6m   �� 5e      -3   5        $$ misteryCheckBox��5e      $��	Misterija��6*   
���iii�6p   �� 5f      -4   5        $$ romanceCheckBox��5f      $��	Romantika��6*   
���iii�6p   �� 5g      -5   5        $$ sciFiCheckBox��5g      $��Znanstvena fantastika��6(   
���iii�6z   �� 5h      -6   5        $$ sportCheckBox��5h      $��Sportski��6(   
���iii�6m   �� 5i      -7   5        $$ thrillerCheckBox��5i      $��Triler��6+   
���iii�6n   �� 5j      -8   5        $$ warCheckBox��5j      $��Ratni��6&   
���iii�6h   �� 5k      -9   5        $$ westernCheckBox��5k      $��Vestern��6*   
���iii�6n   5l      �� 5m      $��376,5.12,0,0q�6   $
��Wrap=�6'   $	��Top=�6;   $��Žanrovi��6S   $
��Left=�6b   $ 0.628,0.461&�6}   ���	#B20000006�   	� 5n      -:   5        % 	�Name$% posterImpAwardsRadio��5n      $��
IMP Awards��61   $��617.999,24.959,0,0q�6F   $	��Top=�6b   $
 TrueY�6z   & 	� 	GroupName$& posters��6�   $
��Left=�6�   	� 5o      -;   5        $% posterGoogleRadio��5o      $��Google��6.   $��618,43.919,0,0q�6?   $	��Top=�6W   $& posters��6o   $
��Left=�6�   	� 5p      -<   5        $% posterMoviePosterDBRadio��5p      $��Movie Poster DB��65   $��617.999,4.999,0,0q�6O   $	��Top=�6j   $& posters��6�   $
��Left=�6�   �� 5q      -=   5        $ searchPosterBTN��5q      $��55��6'   $��745,4.999,0,0q�63   $	��Top=�6J   $
��Left=�6b   $��55��6}   $
��Bold"�6�   
���   �$��0q�6�   $��-4q�6	  $ Trazi poster��6  ��6�   Z� ��6�   Z� .��5r      �� $��45��6   $��45��6   $:��4/VideoKatalog.View;component/Resources/search64p.png��6%   $��UniformToFill=�6c   $ Traži poster��6{   5s      	� 5t      ->   5        $% trailerYoutubeRadio��5t      $��Youtube��60   $��9.541,472.861,0,0q�6B   $	��Top=�6]   $
 TrueY�6u   $& trailer��6�   $��10.667(�6�   $
��Left=�6�   	� 5u      -?   5        $% trailerGoogleRadio��5u      $��Google��6/   $
��Left=�6@   $��9.541,487.047,0,0q�6[   $	��Top=�6v   $& trailer��6�   $��10.667(�6�   �� 5v      -@   5        $ searchTrailer��5v      $��29.903��6%   $��69.221,473.667,0,0q�65   $	��Top=�6Q   $��9.333(�6i   $��Verdana)�6z   $
��Left=�6�   $��31.431��6�   $ Traži trailer��6  ��6�   Z� ��6�   Z� .��5w      �� $��23��6   $��23��6   $:��4/VideoKatalog.View;component/Resources/search30p.png��6%   5x      �� 5y      -A   5        $ openTrailerLink��5y      $��24��6'   $��305,472.861,0,0q�63   $	��Top=�6L   $��9.333(�6d   $��Verdana)�6u   $ Otvori link��6�   $
��Left=�6�   $��26��6  ��6�   Z� ��6�   Z� .��5z      �� $��20��6   $��20��6   $8��2/VideoKatalog.View;component/Resources/link20p.png��6%   $	��Top=�6a   $
��Left=�6y   $ Otvori link��6�   5{      �� 5|      -B   5        $ openInternetLink��5|      $��305,408,0,0q�6(   $��9.333(�6=   $��Verdana)�6N   $
��Left=�6c   $��26��6~   $��23��6�   $	��Top=�6�   $ Otvori link��6�   ��6�   Z� ��6�   Z� .��5}      �� $��18��6   $��18��6   $8��2/VideoKatalog.View;component/Resources/link20p.png��6%   $ Otvori link��6a   5~      �� 5      -C   5        $$ useOriginalNameCB��5      $"��Koristi samo orginalni naziv��6+   $
��Left=�6R   $��214.05,8,0,0q�6m   $	��Top=�6�   $ 0.484,0.747&�6�   $��9.333(�6  �� 5�      -D   5        $ copyAndOpenEditorBTN��5�   1   $��55��6   $��863,4.999,0,0q�6   $	��Top=�6O   $��10.667(�6g   $, &Kopiraj sliku i otvori slikovni editor��6�   $
��Left=�6�   $��55��6�   ��6�   Z� ��6	  Z� .��5�      �� $��47��6   $��50��6   $=��7/VideoKatalog.View;component/Resources/ptohostop48p.png��6%   $, &Kopiraj sliku i otvori slikovni editor��6f   5�      �� 5�      -E   5        ' ��Name$' imageQualityLevelSlider��5�   �   $��629,510,0,0q�6     SimpleSlider#���  6#   $	��Top=�6J   ( � Maximum$	( 100Q�6b   ) � Minimum$) 1Q�6p   * � Value$* 70Q�6|   + � UseLayoutRounding+ . $ 	Kvaliteta��6�   $��18��6	  ���	#FF661B1B$
��Left=�6Y  $	��281��6t  ��6,  Z� ��6B  Z� 6t  %	  Xceed.Wpf.Toolkit.IntegerUpDown	  5�      -F   5        , 	 Name$, 	castLimit��5�   )   $��308.999,115.176,0,0q�6<   $	��Top=�6Y   $
��Left=�6�   $( "Maksimalan broj prikazanih glumaca��6�   �� 5�      -G   5        $ 
addEpisode��5�   -   $��364,252.361,0,271.639q�6   $
��Bold"�6A   $��18.667(�6S   $��-4,-13,-4,-4q�6e   $ Dodaj epizodu��6|   $ 0.704,0.235&�6�   $
��Left=�6�   $��20��6   ��6�   Z� ��6�   Z�  5�        5�   	   -   Color$- 	#FFBDEDA8��6   .   ShadowDepth$. 1Q�6,   .��5�      �� $��16��6   $��16��6   $8��2/VideoKatalog.View;component/Resources/plus20p.png��6%   $��	0,8.9,0,0q�6a   $ Dodaj epizodu��6t   5�      �� 5�      -H   5        $ 	addSeason��5�   6   $��20��6   $��90.778,252.361,0,0q�6   $	��Top=�6I   $
��Bold"�6a   $��18.667(�6s   $��-4,-13,-4,-4q�6�   $ Dodaj sezonu��6�   $ 0.704,0.235&�6�   $
��Left=�6  $��20��6  ��6�   Z� ��6�   Z�  5�        5�   	   $- 	#FFBDEDA8��6   $. 1Q�6,   .��5�      �� $��16��6   $��16��6   $8��2/VideoKatalog.View;component/Resources/plus20p.png��6%   $��	0,8.9,0,0q�6a   $ Dodaj sezonu��6t   5�      �� 5�      -I   5        $ removeEpisode��5�   '   $��364,276.361,0,0q�6   $��18.667(�6>   $��-4,-12.3,-4,-4q�6P   $
��Bold"�6i   $ Makni odabranu epizodu��6{   $
��Left=�6�   $��20��6�   $��20��6  $	��Top=�6  ��6�   Z� ��6�   Z�  5�        5�   	   $- 	#FFFFBBBB��6   $. 1Q�6,   .��5�      �� $��16��6   $��16��6   $9��3/VideoKatalog.View;component/Resources/minus20p.png��6%   $��	0,8.5,0,0q�6b   $ Makni odabranu epizodu��6u   5�      �� 5�      -J   5        $ removeSeason��5�   6   $��20��6   $��90.778,276.361,0,0q�6   $	��Top=�6L   $��18.667(�6d   $��-4,-12.3,-4,-4q�6v   $
��Bold"�6�   $ Makni odabranu sezonu��6�   $
��Left=�6�   $��20��6	  ��6�   Z� ��6�   Z�  5�        5�   	   $- 	#FFFFBBBB��6   $. 1Q�6,   .��5�      �� $��16��6   $��16��6   $9��3/VideoKatalog.View;component/Resources/minus20p.png��6%   $��	0,8.5,0,0q�6b   $ Makni odabranu sezonu��6u   5�      �� 5�      $
��Left=�6   $��8.545,233.187,0,0q�6,   $
��Wrap=�6G   $	��Top=�6[   $��SEZONE��6s   $ 
1.12,0.334&�6�   ���	#B2000000$��14.667(�6�   $
��Bold"�6�   �� 5�      -K   5        $ seasonListBox��5�      $��8.778,252.361,0,0q�6&   $/ �� IsSynchronizedWithCurrentItem$
/ TrueY�6Y   $��78��6�   $
��Left=�6�   $��103.334��6   $	��Top=�61  ��6A   ��  5�        5�   	   5�      �� 5�      -L   5        $ episodeListBox��5�      $��120,252.361,0,0q�66   $	��240��6g   $
��Left=�6s   $��10.667(�6�   $��Disabled=�6�   $��103.334��6T  $	��Top=�6e  ��6O   ��  5�        5�   	   5�      �� 5�      $
��Left=�6   $��120,234.96,0,0q�6,   $
��Wrap=�6D   ���	#B2000000$	��Top=�6o   $��EPIZODE��6�   $��13.333(�6�   $
��Bold"�6�   �� 5�      -M   5        $ 
editSeason��5�   6   $��20��6   $��90.778,335.695,0,0q�6   $	��Top=�6J   $��18.667(�6b   $��-4,-12.3,-4,-4q�6t   $
��Bold"�6�   $ Izmijeni odabranu sezonu��6�   $
��Left=�6�   $��20��6
  ��6�   Z� ��6�   Z� .��5�      �� $��16��6   $��16��6   $8��2/VideoKatalog.View;component/Resources/edit20p.png��6%   $��	0,8.5,0,0q�6a   $ Izmjeni odabranu sezonu��6t   5�      �� 5�      -N   5        $ editEpisode��5�   3   $��20��6   $��364,335.695,0,0q�6   $	��Top=�6H   $��18.667(�6`   $��-4,-12.3,-4,-4q�6r   $
��Bold"�6�   $ Izmijeni odabranu epizodu��6�   $
��Left=�6�   $��20��6	  ��6�   Z� ��6�   Z� .��5�      �� $��16��6   $��16��6   $8��2/VideoKatalog.View;component/Resources/edit20p.png��6%   $��	0,8.5,0,0q�6a   $ Izmijeni odabranu epizodu��6t   5�      �� 5�      -O   5        $ setInfoToMain��5�      $��472,58.63,0,0q�6%   $	��Top=�6<   $��9.333(�6T   $��44.093��6e   $
��Left=�6�   $	��128��6�   .��5�      �� $!��Prikaži glavne informacije��6   $��Center=�64   $��Center=�6Q   $
��Wrap=�6l   $��13.333(�6�   0 �� TextAlignment$0 Center=�6�   $C =Postavi internet link, trailer link i radnju od glavne serije��6�   5�      5�      � 5�      -P   5        $ serieTechInfoGrid��5�      $��	976,0,0,0q�6&   
 �   ����	#FFBAD6DA$	��Top=�6d   $	��544��6|   $
��Left=�6�   $	��168��6�    5�        5�      $	 26Q�6   .+�5�      �� -Q   5        $ widthTextBox��5�      $��8,323.665,0,0q�6%   $
 Wrap=�6<   $
��Left=�6g   $��67.287��6�   {�.  $4��./Video%20Katalog;component/Fonts/#Cambria Math)�6�   $��16(�6�   
���   �$	��Top=�6   ��6P   �� *Width+6   �� 5�      -R   5        $ heightTextBox��5�   +   $��92.713,323.665,0,0q�6   $
 Wrap=�6B   {�.  $
��Left=�6h   $��67.287��6�   $4��./Video%20Katalog;component/Fonts/#Cambria Math)�6�   $��16(�6�   
���   �$	��Top=�6  ��6�   �� *Height+6  �� 5�      -S   5        $ bitrateTextBox��5�      $��8.525,273.167,0,0q�6'   $
 Wrap=�6B   {�.  $��16(�6�   $��151.475��6�   $
��Left=�6�   $��24��6�   $	��Top=�6�   $4��./Video%20Katalog;component/Fonts/#Cambria Math)�6  
���   ���6V   �� *	Bitrate+1 �� 	Converter#1 [  6G  �� 5�      -T   5        $ runtimeTextBox��5�      $��92.713,377.167,0,0q�6'   $
 Wrap=�6C   $ 0.444,0.736&�6W   {�.  $
��Left=�6�   $��67.287��6�   $��24��6�   $	��Top=�6	  $4��./Video%20Katalog;component/Fonts/#Cambria Math)�6!  $��16(�6]  
���   ���6{   �� *	Runtime+#1 [ 6k  �� 5�      -U   5        $ aspectRatioTextBox��5�      $��24��6+   $��8,377.167,0,0q�67   $
 Wrap=�6N   $	��Top=�6b   $
��Left=�6z   $��67.287��6�   {�.  $4��./Video%20Katalog;component/Fonts/#Cambria Math)�6�   $��16(�6  
���   ���6�   �� *AspectRatio+6  �� 5�      -V   5        $ sizeTextBox��5�      $��26.08��6$   $��8.762,225.127,0,0q�63   $
 Wrap=�6N   $	��Top=�6b   {�.  $��16(�6�   $
��Left=�6�   $��151.238��6�   
���   �$4��./Video%20Katalog;component/Fonts/#Cambria Math)�6  ��6z   �� *Size+#1 [ 6  �� 5�      $
��Left=�6   $��9.263,307.812,0,0q�6,   $
��Wrap=�6G   $��
Rezolucija��6[   $��15.854��6m   $	��Top=�6}   ���	#BF0000006�   �� 5�      $
��Left=�6   $��8,362.98,0,0q�6,   $
��Wrap=�6B   $	��Top=�6V   $��Aspect ratio��6n   ���	#BF000000$��10.667(�6�   �� 5�      $��9.263,257.206,0,0q�6   $
��Wrap=�6,   $��Bitrate��6@   $
��Left=�6O   $	��Top=�6j   ���	#B20000006�   �� 5�      $��20��6   $��	8,152,0,0q�6   $
��Wrap=�60   $	��Top=�6D   $��14.667(�6\   $��# HDD��6n   $
��Left=�6{   ���	#B20000006�   �� 5�      $
��Left=�6   $��9.263,209.166,0,0q�6,   $
��Wrap=�6G   $	��Top=�6[   $��	Veličina��6s   $ 0.617,0.433&�6�   ���	#B20000006�   �� 5�      $��92.713,361.206,0,0q�6   $
��Wrap=�6-   $	��Top=�6A   $��Trajanje��6Y   $ 0.496,0.687&�6i   $
��Left=�6�   ���	#BF0000006�   �� 5�      $
��Left=�6   $��9.234,410,0,0q�6,   $
��Wrap=�6C   $��13.333(�6W   $��Audio��6i   $	��Top=�6v   $��17.74��6�   ���	#B20000006�   �� 5�      -W   5        $ audioLanguagesListBox��5�   "   $��	8,428,0,0q�6   $��44��6c   $	��Top=�6o   $
��Left=�6�   $	��152��6�   ��6A   �� *	AudioList+6�   �� 5�      -X   5        $ addAudioLng��5�   #   $��140,408,0,0q�6   $
��Bold"�68   $��14.667(�6J   $��-4,-11,-4,-4q�6\   $
��Left=�6�   $��20��6�   $ Izmijeni audio jezik��6�   $��20��6   $	��Top=�6  ��6�   Z� ��6�   Z� .��5�      �� $��16��6   $��16��6   $8��2/VideoKatalog.View;component/Resources/edit20p.png��6%   $��-0.2,6,0,-1.2q�6a   $ Izmijeni audio jezik��6x   5�      �� 5�      $
��Left=�6   $��8.503,473,0,0q�6,   $
��Wrap=�6C   $	��Top=�6W   $��13.333(�6o   $��Jezici titlova��6�   $��17.733��6�   $��77.71��6�   ���	#B20000006�   �� 5�      -Y   5        $ subtitleLangsListBox��5�   "   $��	8,491,0,0q�6   $
��Left=�6m   $	��132��6�   $��44��6�   $	��Top=�6�   ��6@   �� *SubtitleLanguageList+6�   �� 5�      -Z   5        $ addSubtitleLng��5�   #   $��140,491,0,0q�6   $
��Bold"�6;   $��14.667(�6M   $��-4,-10.8,-4,-4q�6_   $	��Top=�6�   $��20��6�   $
��Left=�6�   $��20��6�   $" Dodaj prozvoljni jezik titla��6  ��6�   Z� ��6�   Z� .��5�      �� $��16��6   $��16��6   $8��2/VideoKatalog.View;component/Resources/plus20p.png��6%   $��	0,6.8,0,0q�6a   5�      �� 5�      -[   5        $ removeSubtitleLng��5�   J   $��20��6   $
��Left=�6   $��140,515,0,0q�65   $	��Top=�6e   $��20��6}   $
��Bold"�6�   $��14.667(�6�   $��-4,-11,-4,-4q�6�   $ Makni odabrani jezik��6  ��6�   Z� ��6�   Z� .��5�      �� $��16��6   $��16��6   $9��3/VideoKatalog.View;component/Resources/minus20p.png��6%   $��	0,6.8,0,0q�6b   5�      �� 5�      -\   5        $ hddComboBox��5�      $��8.5,172,0,0q�6%   $	��Top=�6:   $��24��6R   $��13.333(�6^   $
��Left=�6�   $��123.5��6�   �� 5�      -]   5        $ loadFileButton��5�      $��124.43��6&   $��8,19.57,0,0q�66   $	��Top=�6K   $
��Bold"�6c   $��Center=�6u   $��Center=�6�   $��24(�6�   $
��Left=�6�   $	��152��6  ���	#57000000$ Učitaj��6$  .��5�      �� $��116.43��6   $	��128��6   $@��:/VideoKatalog.View;component/Resources/browseVideo128p.png��6*   $ Učitaj��6n   5�      �� 5�      $��79.596,326.932,79.667,0q�6   $
��Wrap=�62   $��X��6F   $��13.333(�6O   $	��Top=�6a   $
��Bold"�6y   �� 5�      -^   5        $ 	addNewHdd��5�   #   $��136,172,0,0q�6   $
��Bold"�66   $��14.667(�6H   $��-4,-10.8,-4,-4q�6Z   $	��Top=�6s   $��24��6�   $
��Left=�6�   $��24��6�   $ Dodaj novi HDD��6�   ��6�   Z� ��6�   Z� .��5�      �� $��20��6   $��20��6   $8��2/VideoKatalog.View;component/Resources/plus20p.png��6%   $��	0,6.8,0,0q�6a   $ Dodaj novi HDD��6t   5�      �� 5�      -_   5        $ 	loadLabel��5�      $��8.25,1.61,5.25,0q�6$   $
��Wrap=�6>   $��Učitaj folder��6R   $	��Top=�6g   $
��Bold"�6   5�      5�      5�      �� 5�      -`   5        $ cancelNewMovieButton��5�      $��Right=�6)   $��85��6E   $��
0,0,300,15q�6Q   $��Bottom=�6e   $	��110��6�   $
��Bold"�6�   $��14.667(�6�   $# Odustani od unosa nove serije��6�   ��5�      �� 5�      $	 0,1&�6   $	 0,0&�6)   .��5�      � $ 	#FFF3F3F3��6   $ 0Q�6&   � 5�      $ 	#FFF5B6AF��6   $ 1Q�6&   � 5�      $ 	#FFF1E6E5��6   $ 0.435Q�6&   5�      .��5�      �� $��70��6   $��70��6   $<��6/VideoKatalog.View;component/Resources/dismiss128p.png��6"   $# Odustani od unosa nove serije��6b   5�      �� 5�      -a   5        $ acceptNewMovieButton��5�      $��85��6)   $
��Bold"�65   $��14.667(�6G   $ 0.5,0.5&�6|   $ Prihvati unos nove serije��6�   $
��Left=�6�   $��Bottom=�6�   $	��110��6�   $��
300,0,0,15q�6  z�5�      V� 5�      .~�5�      �� �� 5�      �� 5�      2 �� Angle$2 -0.642Q�6   T� 5�      5�      ��5�      �� 5�      $	 0,1&�6   $	 0,0&�6)   .��5�      � $ White��6   $
 0.01Q�6"   � 5�      $ 	#FFE4FDE4��6   $ 0.474Q�6&   � 5�      $ 	#FFB5FFB2��6   $ 0.974Q�6&   5�      .��5�      �� $��70��6   $��70��6   $;��5/VideoKatalog.View;component/Resources/accept128p.png��6"   $ Prihvati unos nove serije��6a   5�      � 5�      -b   5        $ releaseDateGrid��5�   	   $
��Left=�6"   $��62��6=   $��	10,0,0,56q�6I   $��Bottom=�6\   $	��128��6w    5�        5�      $	 11Q�6   ��5�      �� 5�      3 �� Color#3 � ��6   .+�5�      �� $��
8,8.04,0,0q�6   $
��Wrap=�6#   $��Datum izlaska:��67   $	��Top=�6M   $��13.333(�6e   $
��Left=�6w   $��88��6�   (
  "System.Windows.Controls.DatePicker
  5      -c   5        4 
 Name$4 releaseDatePicker��5      $��8,30,8,0q�6+   $	��Top=�6=   6}   5     5     5     