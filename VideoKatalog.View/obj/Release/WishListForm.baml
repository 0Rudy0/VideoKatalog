   M S B A M L   `   `   `  ���� L  HVideoKatalog.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:Video_katalog.ConvertersVideo_katalog.Converters       Video_katalog.WishListForm   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     BlocalConverters&clr-namespace:Video_katalog.Converters     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     X TXceed.Wpf.Toolkit, Version=2.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  -     �Name$  wishListWindow�� � WindowStartupLocation$ CenterOwner=� � Title$ Lista želja��$	��598��$	��816�� � 	Resources ,  &Video_katalog.Converters.DateConverter   dateConverter7  1Video_katalog.Converters.PersonsToStringConverter  personsToStringConverter6  0Video_katalog.Converters.GenresToStringConverter  genresToStringConverter7  1Video_katalog.Converters.NameToImagePathConverter  nameToImagePathConverter;  5Video_katalog.Converters.ReleaseDateToStampVisibility   releaseDateToStampVisibility0  *Video_katalog.Converters.DateSpanConverter  dateSpanConverter  wishSummaryBackground �� Color  wishMovieLBTemplate J� RenderTransformOrigin �� AngleX T� X ��Name	 �� ConverterParameter
 �� 	Converter ��Name&   System.Windows.Media.TextOptions   TextFormattingMode �� Padding   TextHintingMode J� Effect3  -System.Windows.Media.Effects.DropShadowEffect   ShadowDepth J� Opacity �� Angle � ToolTip ��Name  ListBoxControlTemplate1 �� 
TargetType 	 wishHomeVideoTemplate �� TextAlignment 
 serieTemplate ��Name ��Name �Name ��Name$ �� IsSynchronizedWithCurrentItem��%�"  &	        &	      &	 
     &	      &	      &	      &	      &	 3     7  7   7  7  7  &	 �    &		 �    7  7   7  &	
 �    7  7  7  7  7  7  7               �� $ 	#FFDEDEDE���� .R�� $	��222��$	��735�����	#74E5E5E5$ 0.5,0.5&�$	��0,5q�z�V� .~��� �� $ -0.337Q��� T� $ -0.494Q�.+��� $ 
movieImage��$
��Left=�$��8,8,0,28q�$	��130��$��186.76��$��UniformToFill=����� *Name+$	 movie��8
   �� $ movieNameTB��$��Stretch=�$��149,7.74,166.954,0q�$��NoWrap=�$	��Top=�$��26.667(�$��Buxton Sketch)�$ Display=����	#FF071C74$��WordEllipsis=����� *
OrigName+�� $ movieYearTB��$��Right=�$��
0,7.74,8,0q�$
��Wrap=�$	��Top=�$��66.141��$��18.667(�$��Papyrus)�$ Display=����� *Year+�� $ movieSummaryTB��$
��Left=�$��
149,89,0,0q�$��WrapWithOverflow=�$	��Top=�$ Display=�$��WordEllipsis=�#���  $ 5q�$ Animated=�$��96��$	��578������ *	Summary+   $ 1Q��� $ movieReleaseDateTB��$
��Left=�$��45.104,0,0,7.28q�$
��Wrap=�$��Bottom=�$��88.777��$ Display=����� *ReleaseDate+8
  �� $
��Left=�$��12.537,0,0,7.28q�$
��Wrap=�$��Izlazi:��$��Bottom=�$ 0.002,0.516&�$ Display=��� $
��Left=�$��149,43.08,0,0q�$
��Wrap=�$��	Redatelj:��$	��Top=�$
��Bold"��� $
��Left=�$��149,63.04,0,0q�$
��Wrap=�$��Uloge:��$	��Top=�$
��Bold"��� $��Stretch=�$��208.094,43.08,8,0q�$��NoWrap=�$	��Top=�$��WordEllipsis=����� *	Directors+8
  �� $��Stretch=�$��208.094,63.04,8,0q�$��NoWrap=�$	��Top=�$��WordEllipsis=����� *Actors+8
  �� $	��160��$	��400��$��216.941,27,0,0q�$
��Left=�$	��Top=�$��Resources/outStamp.png��$
 0.71Q�$ 0.5,0.5&�y��� *ReleaseDate+8
  z�V� .~��� �� �� $ -4.861Q�T� �� $��wishMovieToRegular��-   $��Right=�$��0,0,8.007,3.25q�$��Bottom=�$��41.316��$��28.917��.���� $��22.75��$��37.707��$"��Resources/transferToList.png��$ Dodaj u glavnu listu���� $ 
moveRemove��-   $��Right=�$��0,0,97.878,5q�$��Bottom=�$��26.325��$��26��.���� $��20��$��21��$��Resources/remove22p.png��$ Izbriši iz liste želja��$
��Fill=��� $ 	movieEdit��-   $��Right=�$��0,0,67.878,5q�$��Bottom=�$��26��$ Izmijeni��.���� $��20��$��20��$��Resources/edit22p.png���� " ��.R�� #��z��.+��� $
��0,-5q��� .R�� $	��160��$	��735�����	#74E5E5E5$ 0.5,0.5&�$	��0,5q�z�V� .~��� �� $ -0.337Q��� T� $ -0.494Q�.+��� $ homeVideoImage��$
��Left=�$��8,8,0,0q�$	��100��$��143.667��$��UniformToFill=�$	��Top=����� *Name+$	 	homeVideo��8
   �� $ homeVideoNameTB��$��Stretch=�$��128.724,8,226.492,0q�$��NoWrap=�$	��Top=�$��26.667(�$��Buxton Sketch)�$ Display=����	#FF071C74$��CharacterEllipsis=����� *Name+�� $ homeVideoComment��$��Stretch=�$��128.724,56,8,0q�$��NoWrap=�$	��Top=�$ Display=�$��WordEllipsis=�$
��58.1��#���  $ 5q����� *	Comment+   $ 1Q��� $ homeVideoDueDateTB��$��Right=�$��0,16.666,8,0q�$
��Wrap=�$	��Top=�$��81.015��$ Display=�$ Right=����� *
FilmDate+8
  �� $��Right=�$��0,16.666,98.881,0q�$
��Wrap=�$��Zakazan datum:��$	��Top=�$ 0.002,0.516&�$ Display=�$
��Auto���� $	��160��$	��400��$��121.206,232,0,0q�$
��Left=�$	��Top=�$��Resources/outStamp.png��$
 0.71Q�$ 0.5,0.5&�y��� *ReleaseDate+8
  z�V� .~��� �� �� $ -4.861Q�T� �� $ wishHomeVideoToRegular��-   $��Right=�$��0,123.083,8,0q�$	��Top=�$��41.316��$��28.917��.���� $��22.75��$��37.707��$"��Resources/transferToList.png��$ Dodaj u glavnu listu���� $
��Left=�$��132.882,0,0,14q�$
��Wrap=�$��Kategorija:��$��Bottom=�$ 0.002,0.516&�$ Display=�$
��Auto���� $ homeVideoCategoryTB��$��Stretch=�$��192.882,0,362.536,14q�$��NoWrap=�$��Bottom=�$
��Auto��$ Display=�$��WordEllipsis=����� *VideoCategory+�� $ homeVideoRemove��-   $��Right=�$��0,0,93.015,8q�$��Bottom=�$��28.917��$��28.917��.���� $��22��$
��23.2��$��Resources/remove22p.png��$ Izbriši iz liste želja��$
��Fill=��� $ homeVideoEdit��-   $��Right=�$��0,0,60.098,8q�$��Bottom=�$��28.917��$ Izmijeni��$��28.917��.���� $��22��$��22��$��Resources/edit22p.png���� .R�� $	��256��$	��735�����	#74E5E5E5$ 0.5,0.5&�$	��0,8q�z�V� .~��� �� $ -0.337Q��� T� $ -0.494Q�.+��� $ 
serieImage��$
��Left=�$��8,8,0,0q�$	��140��$	��201��$��UniformToFill=�$	��Top=����� *Name+$	 serie��8
   �� $ serieNameTB��$��Stretch=�$��158.644,7.74,255.824,0q�$
��Wrap=�$	��Top=�$��26.667(�$��Buxton Sketch)�$ Display=����	#FF071C74���� *
OrigName+�� $ serieSummaryTB��$��Right=�$��0,24,8.007,0q�$��WrapWithOverflow=�$	��Top=�$ Display=�$��WordEllipsis=�$��221.912��#���  $ 5q�$��188.5������ *	Summary+   $ 1Q��� $ serieReleaseDateTB��-   $
��Left=�$��	4,227,0,0q�$
��Wrap=�$��Center=�$	��149��$ Display=�$ Center=�$
��Bold"����� *WishSeasons+8
  $	 serie���� $
��Left=�$��39.949,0,0,27.5q�$
��Wrap=�$��Datum izlaska��$��Bottom=�$ 0.002,0.516&�$ Display=��� $
��Left=�$��158.644,43.08,0,0q�$
��Wrap=�$��	Redatelj:��$	��Top=�$
��Bold"�$ Display=��� $
��Left=�$��158.644,63.04,0,0q�$
��Wrap=�$��Uloge:��$	��Top=�$
��Bold"�$ Display=��� $ serieDirectors��$��Stretch=�$��212.094,43.08,239.07,0q�$��NoWrap=�$	��Top=�$��WordEllipsis=�$ Display=����� *	Directors+8
  �� $ 	serieCast��$��Stretch=�$��212.094,63.04,239.07,0q�$��NoWrap=�$	��Top=�$��WordEllipsis=�$ Display=����� *Actors+8
  �� $	��160��$	��400��$��498.282,280,0,0q�$
��Left=�$	��Top=�$��Resources/outStamp.png��$
 0.71Q�$ 0.5,0.5&�y��� *ReleaseDate+8
  z�V� .~��� �� �� $ -4.861Q�T� �� $ wishSerieToRegular��-	   $��Right=�$��0,0,8.007,3.25q�$��Bottom=�$��41.316��$��28.917��.���� $��22.75��$��37.707��$"��Resources/transferToList.png��$ Dodaj u glavnu listu���� $ serieRemove��-
   $��Right=�$��0,0,91.823,5q�$��Bottom=�$��26.325��$��26��.���� $��20��$��21��$��Resources/remove22p.png��$ Izbriši iz liste želja��$
��Fill=��� $ 	serieEdit��-   $��Right=�$��0,0,61.823,5q�$��Bottom=�$��26��$ Izmijeni��.���� $��20��$��20��$��Resources/edit22p.png���� $ serieEpisodeTabControl��$	��125��$��158.644,88.5,0,0q�$	��Top=�$
��Left=�$��337.286��.@��� $ 
seasonsTab��$��Sezone��$ Display=�.��� $	 bla�����	#FFE5E5E5.+��� $ seasonListBox��-   $��8,8,107.836,7q�$��Stretch=�$
��Auto��$
 TrueY�$ Display=�$��13.333(����� *WishSeasons+�� $��Right=�$��0,7.6,15.8,0q�$
��Wrap=�$��Datum izlaska��$	��Top=��� $ wishSeasonToRegular��-   $��Right=�$��0,0,8,8q�$��Bottom=�$��38.566��$��23.75��$  ��.���� $��17.904��$��29.675��$"��Resources/transferToList.png��$! Dodaj sezonu u glavnu listu���� $ seasonRemove��-   $��Right=�$��0,0,81.836,8q�$��Bottom=�$��22��$��22��$  ��.���� $
��14.5��$��15.985��$��Resources/remove22p.png��$% Izbriši sezonu iz liste želja��$
��Fill=��� $ 
seasonEdit��-   $��Right=�$��0,0,55.836,8q�$��Bottom=�$��22��$  ��$��22��.���� $��17��$��15.482��$��Resources/edit22p.png��$ Izmijeni sezonu���� $ seasonReleaseDate��$��Right=�$��0,-10.1,0,0q�$
��Wrap=�$��Center=�$��107.5��$ Center=�$��13.333(����	#FF116800$
��Bold"����� *WishEpisodes+8
  $	 serie���� $ episodesTab��$��Epizode��$ Display=�.��� ���	#FFE5E5E5.+��� $��Right=�$��0,7.6,15.8,0q�$
��Wrap=�$��Datum izlaska��$	��Top=�$ Display=��� $ episodesListBox��$��8,8,107.836,0q�$
 TrueY�$��Stretch=�$
��Auto��$ Display=�$��Disabled=�$	��Top=�$��80������ �� $ episodeRemove��-   $��Right=�$��0,0,81.836,8q�$��Bottom=�$��22��$��22��.���� $
��14.5��$��15.985��$��Resources/remove22p.png��$ Izbriši iz liste želja��$
��Fill=��� $ wishEpisodeToRegular��-   $��Right=�$��0,0,8,8q�$��Bottom=�$��38.566��$��23.75��.���� $��17.904��$��29.675��$"��Resources/transferToList.png��$ Dodaj u glavnu listu���� $ episodeEdit��-   $��Right=�$��0,0,55.836,8q�$��Bottom=�$��22��$ Izmijeni��$��22��.���� $��17��$��15.482��$��Resources/edit22p.png���� $ episodeReleaseDate��$��Right=�$��0,-10.1,0,0q�$
��Wrap=�$��Center=�$��107.5��$ Center=�$��14.667(����	#FF116800$
��Bold"�$ Display=����� *	AirDate+8
  .��� .+��� $��8,8,0,0q�$
��Left=�$	��Top=�$	��544��$	��784��.@��� $��Filmovi��.��� ���	#FFE5E5E5.+��� -   $ wishMovieListBox��$��6,53.04,8,6q���.  $��Visible=�$
 TrueY�#���  $	��760������ �� -   $��addWishMovie��$��Right=�$��
0,4.79,8,0q�$	��Top=�$��45.75��$��44.25����Z� .���� $��40��$��40��$��Resources/wishListAdd.png��$$ Dodaj novi film u listu želja��(	  "Xceed.Wpf.Toolkit.WatermarkTextBox	  -   $��searchWishMovieTB��$
��Left=�$��12,13.04,0,0q� 	  	Watermark$ Upiši naziv za pretragu��$	��Top=�$	��215��$��24.96��$��16(�$��	Tw Cen MT)�$��3,3,21,3q� �� MaxLines$ 1���� $��35.333��$��-125,9.333,0,0q�$	��Top=�$��Resources/sort.png��$��36��$��Center=��� -   $��movieSortCategory��$��65,16.04,0,0q�$	��Top=�$��Center=�$	��143��$ Opcije sortiranja��.@��� $��Po vremenu izlaska���� $��	Po nazivu���� $��	Po godini���� -   $ movieClearSearchName��$
��Left=�$��208.542,18.207,0,0q�$	��Top=�$��15.333��$
��15.5��$��-3q���Z� ��Z� .���� $��15��$��15��$��Resources/delete.png���� $��Serije��.��� ���	#FFE5E5E5$��0q�.+��� -   $ wishSerieListBox��$��6,53.04,8,6q���.  $
 TrueY�$��Visible=�$	��760��#��� 
 ���� �� -   $ addWishSerie��$��Right=�$��0,16.04,156,0q�$	��Top=�$��33��$��33��$��0q���Z� .���� $��30��$��30��$��Resources/wishListAdd.png��$$ Dodaj novi film u listu želja��	  -    	 Name$ searchWishSerie��$
��Left=�$��12,13.04,0,0q�$ Upiši naziv za pretragu��$	��Top=�$	��215��$��24.96��$��16(�$��	Tw Cen MT)�$��3,3,21,3q�$ 1���� $��35.333��$��-125,9.333,0,0q�$	��Top=�$��Resources/sort.png��$��36��$��Center=��� -    ��Name$ serieSortCategory��$��65,16.04,0,0q�$	��Top=�$��Center=�$	��143��$ Opcije sortiranja��.@��� $��Po vremenu izlaska���� $��	Po nazivu���� -   $ serieClearSearch��$
��Left=�$��208.542,18.207,0,0q�$	��Top=�$��15.333��$
��15.5��$��-3q���Z� ��Z� .���� $��15��$��15��$��Resources/delete.png���� $��Right=�$��0,0.08,157.726,0q�$
��Wrap=�$��Serija��$	��Top=�$
��Bold"��� -   $ addWishSerieSeason��$��Right=�$��0,16.04,85,0q�$	��Top=�$��33��$��33��$��0q���Z� .���� $��30��$��30��$��Resources/wishListAdd.png��$$ Dodaj novi film u listu želja���� $��Right=�$��0,0,80.666,0q�$
��Wrap=�$��Sezona��$	��Top=�$
��Bold"��� -   $ addWishSerieEpisode��$��Right=�$��0,16.04,18,0q�$	��Top=�$��33��$��33��$��0q���Z� .���� $��30��$��30��$��Resources/wishListAdd.png��$$ Dodaj novi film u listu želja���� $��Right=�$��0,0.08,11,0q�$
��Wrap=�$��Epizoda��$	��Top=�$
��Bold"��� $��Kućni video��.��� ���	#FFE5E5E5$��0q�.+��� -   $ wishHomeVideoListBox��$��6,53.04,8,6q���.  $
 TrueY�$��Visible=�$	��760��#��� 	 ���� �� -    $ addWishHomeVideo��$��Right=�$��
0,4.79,8,0q�$	��Top=�$��45.75��$��44.25��$' !Dodaj kućni video u listu želja����Z� .���� $��40��$��40��$��Resources/wishListAdd.png��$$ Dodaj novi film u listu želja��	  -!   $ searchWishHomeVideo��$
��Left=�$��12,13.04,0,0q�$ Upiši naziv za pretragu��$	��Top=�$	��215��$��24.96��$��16(�$��	Tw Cen MT)�$��3,3,21,3q�$ 1���� $��35.333��$��-125,9.333,0,0q�$	��Top=�$��Resources/sort.png��$��36��$��Center=��� -"   $ homeVideoSortCategory��$��79,16.04,0,0q�$	��Top=�$��Center=�$	��159��$ Opcije sortiranja��.@��� $��Po zakazanom datumu���� $��	Po nazivu���� -#   $ homeVideoClearSearch��$
��Left=�$��208.542,18.207,0,0q�$	��Top=�$��15.333��$
��15.5��$��-3q���Z� ��Z� .���� $��15��$��15��$��Resources/delete.png��