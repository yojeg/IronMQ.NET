IronMQ .NET Client
----------------

Getting Started
===============

*  [Download the IronMQ Project](https://github.com/Eskat0n/IronMQ.NET/archive/master.zip). 
*  Open IronMQ.NET.sln file in Visual Studio 2012 or Visual C# 2012
*  Ensure you have an internet access since there is NuGet package restore enabled
*  Build solution
*  Go to _<project_root>_/sources/IronMQ.NET/bin/Debug where IronMQ.NET.dll is located

The Basics
==========
**Initialize** a client and get a queue object:

```c#
var client = new Client("my project", "my token");	// default Host and Port
var queue = client.Queue("my_queue");
```

**Push** a message on the queue:

```c#
queue.Push("Hello, world!");
```

**Pop** a message off the queue:

```c#
var msg = queue.Get();
```

When you pop/get a message from the queue, it will *not* be deleted. It will
eventually go back onto the queue after a timeout if you don't delete it. (The
default timeout is 60 seconds)

**Delete** a message from the queue:

```c#
queue.deleteMessage(msg);
```

Choosing Cloud
==============
**Initialize** a client and get a queue object (Amazon):

```c#
var client = new Client("my project", "my token", "mq-aws-us-east-1.iron.io");	// Amazon (default)
```

**Initialize** a client and get a queue object (Rackspace):

```c#
var client = new Client("my project", "my token", "mq-rackspace-dfw.iron.io");	// Rackspace
```


