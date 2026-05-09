This is a C# XMPP framework I've just started.

The network plumbing is in place, I'll build out the protocol/parser types next.

The objectives are performance and simplicity. 

XMMP servers like ejabberd can scale to two million connections on a single node. 

They are real-time messaging powerhouses. 

The goal of this project is modest: to bring the simplicity found in many REST APIs to XMPP Components, abstracting away messaging concerns from API developers, to make XMPP services an attractive alternative to REST services.

HTTP/XMPP brokering services will manage stateless HTTP connections, whereas XMPP clients just function as normal via a persistent XMPP connection to the server. A planned code generator will create client-proxies from the service APIs.

I have a personal need for such a framework, but it may be of interest to others.

The project is using C# 15/.NET 11 preview. It's being developed under an MIT license so feel free to fork or use as you see fit.

C# is a pleasure to work with, it keeps getting better.
