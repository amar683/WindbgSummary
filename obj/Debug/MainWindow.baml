   M S B A M L   `   `   `  ����H  DWindbgSummary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:WindbgSummaryWindbgSummary  5   	       WindbgSummary.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -localclr-namespace:WindbgSummary     5   	     � Title$  Windbg Summary Analyzer��5   	   $	��450��6)   $	��800��66   ���#F0F0F0 � 	Resources 5          BooleanToVisibilityConverter  ButtonStyle �� 
TargetType �� Property �� Value  TextBoxStyle��%�  &	        &	      &	 �     �� 5   
   6'   �� 5	   
   " ��6$   .�5
      ��  � ��$
 10,5q�6(   �� 5       � ��$ 5q�6'   �� 5       � �� �#007ACC6+   �� 5       � ��
 �����6+   �� 5       � ��$ 0q�60   5      �� 5   
   " ��6%   .�5      ��  � ��$ 5q�6(   �� 5       � ��$ 5q�6'   �� 5       � ��$ 1q�60   �� 5       � �� �#CCCCCC6,   5      5      .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   .+�5       �� $	��500��6    �� 
IsReadOnly . #��[ 5#      ��5!      �� *DumpFilePath+ �� Mode$ OneWay=�	 �� ValidatesOnExceptions	 . 
 �� NotifyOnValidationError
 . 5#      �� 5$      $��Browse��6   #��[ 5&      ��5%      �� *BrowseCommand+	 . 
 . 5&      5'      �� 5)   
   $��1��6   $
��0,10q�6   ���#CCCCCC$��1q�5*      
�������._�5+      �� .��5,      ��  �� Padding$ 10q�5-      $
��Wrap=�5.      ��5,      �� *AnalysisResult+	 . 
 . 5.      5/      50      � 52   
   $��2��6    � ColumnDefinitions 53      �� 54      $��* �6#   �� 55      $
��Auto �6#   .+�58      � $��0��6   $��20��6*    � IsIndeterminate . y�59      �� *IsAnalyzing+ �� 	Converter# [  	 . 
 . �� 5:      $��1��6   $��Analyze��6%   #��[ 5=      ��5;      �� *AnalyzeCommand+	 . 
 . {�5<      �� *
CanAnalyze+	 . 
 . 5=      5>      5?      5@      