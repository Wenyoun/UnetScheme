# UnetScheme
1. 模块划分思路
● Base 
Client和Server的公用代码
● Client
客户端代码，可以使用Base库下的公用代码，不能使用Server下的代码
● Server
服务器代码，可以使用Base库下的公用代码，不能使用Client下的代码
● Host
当使用Host模式时用来桥借Client和Server，以支持Host运行模式，可以使用Base，Client，Server的下的代码
2. Attribute修饰
● BroadcastAttribute属性只能用在Server下，它与RecvAttribute配合使用，它用来修饰一个方法签名，调用此方法会将消息同步到所有客户端
● ProtocolAttribute属性可以在Client和Server下使用(仅仅只能存在一个)，它用来修饰实现IProtocolHandler接口的脚本，它的主要作用是用来观察BroadcastAttribute和SendAttribute修饰的方法，然后会具体实现注入到这个脚本中
● RecvAttribute属性可以在Client和Server下使用，它与BroadcastAttribute和SendAttribute配合使用，即当我用SendAttribute或BroadcastAttribute属性修饰一个方法后，那么在对应的远端需要用RecvAttribute来接收这个方法
● SendAttribute属性可以在Client和Server下使用,它与RecvAttribute配合使用
● SyncClassAttribute属性用来修饰实现了ISyncAttribute的脚本，可在Server和Client使用
● SyncFieldAttribute属性用来修饰字段，需要配合SyncClassAttribute使用，在Server只要当属性改变时会自动同步到Client