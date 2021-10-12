# RedDotGraph
基于unity UIElements实现的游戏红点工具及运行时代码。

在游戏客户端开发中，红点功能是必不可少的，难度不大，但红点之间的关系比较复杂，一般也不是那么直观，后续维护起来比较麻烦，有些时候还会多出一些逻辑代码的重复，比如ListView里的每个cell都要有红点。出于以上原因，搞了这个红点编辑器。

## 实现的功能

1. 红点及红点间的关系可视化
2. 红点key和红点逻辑ID只需编辑一次，无需在运行时代码中多次输入字符串或ID。根据模板代码生成所对应常量
3. cell的红点逻辑代码和非cell统一实现
4. 数据刷新采用非必要不刷新机制，数据没变化、没有绑定红点UI或者UI不可见时，都不会额外的触发刷新

红点编辑器相关界面展示

![image-2021101101](images/image-2021101101.png)

![image-2021101102](images/image-2021101102.png)

![image-2021101103](images/image-2021101103.png)

## 怎么使用

使用流程

Demo：在Acenes下的TestRedDot.unity，运行效果

![image-2021101104](images/image-2021101104.png)



## 文档

各红点之间构成的是一个有向无环图，每个红点是一个节点

unity 版本：开发编辑器使用的版本是2020.2.2f1c1。应该支持UIElements的unity版本都能用

## License



