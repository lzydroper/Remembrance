# 类逃离塔克夫的网格库存系统的实现

## 使用方法
场景中添加Image，图片修改为Tile，作为格子背景，挂载脚本GridInteract。添加另一个白底的Image，作为高光显示。  
挂载InventoryController 和 InventoryHighLight脚本,这两个脚本挂在同一物体上。  
prefab添加InventoryItem脚本，并需要设定ItemData。挂载在InventoryController上。  
   
使用Q建创建随机物体，W插入随机物体，R旋转物体。  

## 问题
测试的时候遇到了数组越界的问题，修改ItemGrid参数后又没有触发了，暂时不知道原因。

## 参考教程
[油管Greg Dev Stuff](https://www.youtube.com/watch?v=2ajD1GDbEzA)

## 改进方向
当不同的网格连在一起时如何计算，以便能够得到任意形状的背包？  
更加炫酷的视觉效果和更加流畅的操作手感。  