   M S B A M L   `   `   `  ����J  FDesktopStreamer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DesktopStreamerDesktopStreamer  5   	   G(clr-namespace:DesktopStreamer.PropertiesDesktopStreamer.Properties  5   	        DesktopStreamer.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   /localclr-namespace:DesktopStreamer     5   	   @appSettings(clr-namespace:DesktopStreamer.Properties     5   	   -   5          � Title$  
MainWindow��5   	   $	��350��5   	   $	��240��5   	   $	��300��5	   	   $	��500��5
   	    � AllowsTransparency .  � WindowStyle$
 None=�5   	    � 
ResizeMode$ CanResizeWithGrip=�5   	    � Icon$B </DesktopStreamer;component/Resources/DesktopStreamerIcon.ico��5   	    � WindowStartupLocation$ CenterScreen=�6M   ��5   	   Z� .��5      �� -   5        $��
mainBorder��5      
�������$��2q�5       �� CornerRadius$ 6,6,6,6��5      ���	#FF405055._�5   
   ]� .+�5      ]� $��22��6   $	��Top=�5      ���	#FF2D383B$��1q�5      .+�5      �� -   5        $��btnClose��5      $��Close��5      $��Right=�5      $��2q�5      $��0q�5!      $��Center=�5"      $��0q�5$      ��5       Z� ��5#      Z� 5%      �� 5&      -   5        $��btnMinimize��5&      $��Minimize��5'      $��Right=�5(      $��2q�5)      $��0q�5+      $��Center=�5,      $��0q�5.      ��5*      Z� ��5-      Z� 5.      �� 5/      -   5        $��btnTray��5/      $
��Tray��50      $��Right=�51      $��2q�52      $��0q�54      $��Center=�55      $��0q�57      ��53      Z� ��56      Z� 57      �� 58      
��� ���$��DesktopStreamer  ��59      $��14(�5:      $
��Bold"�5;      $
��Left=�5<      $��2q�5=      $��Center=�5>      �� 5?      -   5        $��
tbxVersion��5@      
��� ���$��14(�5A      $
��Bold"�5B      $
��Left=�5C      $��2q�5D      $��Center=�5F      ��5E      �� )   #DesktopStreamer.Properties.Settings   Default �� Source# Z 	 �� Path$	 Version�5F      5G      �� 5H      $	��Top=�6   
�������$��5��5J      � 5K      $��Bottom=�6   
 � ColumnDefinitions
 5L      �� 5M      $��80 �6'   �� 5N      $��1* �6'   �� 5O      $��9* �6'    � RowDefinitions 5Q      �� 5R      $��40 �5S      $��40��5T      �� 5U      -   5         �Name$ 
ExpandArea��5W      $��5* �5V      5W      .+�5Y      �� -   5         ��Name$ 	btnExpand��5Y      $��0��5Z      $��Stretch=�5\      $��Stretch=�5]      $��14(�5^      $
��Bold"�5_      $��6q�5`      $��0q�5b      ��5a      Z� ��5c      Z� .��5d      �� -	   5        $��	imgExpand��5d      $ ��../Resources/arrow4832.png��5e      5f      "   DesktopStreamer.FavoriteList  5g      -
   5          Name$ favList��5g   %   "   DesktopStreamer.FavoriteList   
onAddClick$ favList_AddClick��5l   %   $��1��5h   %   $��1��5i   %   $��2��5j   %   $��3q�5k   %   5l   %   !   DesktopStreamer.MainElement  5m      -   5          Name$ mainEle��5m   $   $��1��5n   $   $��2��5o   $   $��3q�5p   $   ]� 5q      $��0��6   $��1��5r      $��6,4,0,4q�5s      ��.  .+�5{      �� -   5        $��btnSettings��5{      $��Bottom=�5|      $��
2, 5, 2, 5q�5}      $��35��5~      
������ $��0q�5�      .��5�      �� -   5         ��Name$ imgSettings��5�       $��../Resources/repair17.png��65   5�      �� 5�      -   5        $��btnAbout��5�      $��Bottom=�5�      $��
2, 5, 2, 5q�5�      $��35��5�      
������ $��0q�5�      .��5�      �� -   5        $ imgAbout��5�       $!��../Resources/questions3.png��62   5�      5�      5�      5�      5�      5�      