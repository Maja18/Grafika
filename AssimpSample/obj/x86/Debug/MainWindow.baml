   M S B A M L   `   `   `  ����R  NSharpGL.WPF, Version=2.3.0.1, Culture=neutral, PublicKeyToken=27fc851303210b27>.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPFSharpGL.WPF  5   	    AssimpSample   AssimpSample.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   BsharpGL.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPF     5   	   -   5          � Title$  SharpGL WPF Application��5   	   $	��350��6)   $	��525��66   .��5      �  � RowDefinitions 5   
   �� 5      $	��30* �6   $��30��6)   5
      �� 5      $
��289* �6   5      .+�5   
      SharpGL.WPF.OpenGLControl  -   5        $��openGLControl��5      $��1��6H      DrawFPS .    RenderContextType$	 FBO=�5         	FrameRate$ 41Q�64   c� 5   
   -   5         c�Name$ toolBar��5      $��Stretch=�6#   $��0��6B   $	��Top=�6O   $��30��6h   $��30��6t   .@�5      �� $��Stubovi��6   $��25��6&   $	��Top=�62   �� 5      -   5         ��Name$ scaleSlider��5      $��25��6*   $	��Top=�66   $	��100��6N    � Minimum$	 0.8Q�6�    � Maximum$	 2.5Q�6�   	 � Value$	 1Q�6�   
 � LargeChange$
 1Q�6�   �� 5      $��Vrata��6   $��25��6$   $	��Top=�60   �� 5      -   5        $ 
doorSlider��5      $��25��6)   $	��Top=�65   $��80��6M   $	 -15Q�6Y   $ 90Q�6g   $		 -15Q�6t   $
 1Q�6�   6�   �� 5      $��
Svetlo (R)��6   $��25��6*   $	��Top=�66   �� 5      -   5        $ r��5      $��25��6    $	��Top=�6,   $��80��6D   $ 0Q�6m   $ 1Q�6y   $	 1Q�6�   $

 0.01Q�6�   �� 5       $��
Svetlo (G)��6   $��25��6*   $	��Top=�66   �� 5!      -   5        $ g��5!      $��25��6    $	��Top=�6,   $��80��6D   $ 0Q�6m   $ 1Q�6y   $	 0Q�6�   $

 0.01Q�6�   �� 5#      $��
Svetlo (B)��6   $��25��6*   $	��Top=�66   �� 5$      -   5        $ b��5$      $��25��6    $	��Top=�6,   $	��100��6D   $ 0Q�6n   $ 1Q�6z   $	 0Q�6�   $

 0.01Q�6�   �� 5%      $��15��6    �� Orientation$ 
Horizontal=�6%   .+�5&      �� -	   5         ��Name$ cb��5&       d� IsThreeState .   d� 	IsChecked$ FalseY�6W   .��6�   GL_MODULATE6�   �� 5'      -
   5        $	 cb2��5'       .  $
 TrueY�6Y   .��6�   Stacionarno svetlo6�   5(      5)      5+      5,      