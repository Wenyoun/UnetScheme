# UnetScheme
设计Unet客户端和服务器分离方案

通过参考Unet与Mirror实现一套基于Attribute的网络同步方案

[Send(MsgId)]  Send修饰的方法可以将消息发送到远端(注意Send方法的签名必须与Recv方法的签名一致)
[Recv(MsgId)]  Recv修饰的方法可以接收远端的消息(注意Recv方法的签名必须与Send方法的签名一致)
[Broadcast] 用来在Server下修饰方法，可以将服务器上消息广播到所有已连接的远端(注意Broadcast方法的签名必须与Recv方法的签名一致)
[Protocol] 修饰Class使用， 将Recv修饰的方法都注册到此Class中
[SyncClass] 需要自动同步Class使用与SyncField配合使用
[SyncField] 修饰字段使用与SyncClass配合使用
