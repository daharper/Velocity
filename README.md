This is a C# XMPP framework I've been working on.

The network plumbing is in place, I'll build out the protocol/parser types next.

The objectives are performance and simplicity. 

XMMP servers like ejabberd can scale to 2 million connections on a single node. They are real-time messaging powerhouses. 

The goal of this project is modest: to bring the simplicity found in many REST APIs to XMPP Components, abstracting away XML and XMPP messaging concerns from API developers. HTTP/XMPP brokering services will manage stateless HTTP connections.

I have a personal need for such a framework, but it may be of interest to others.

The project is using C# 15/.NET 11 preview. It's being developed under an MIT license so feel free to fork or use as you see fit.

C# is a pleasure to work with, it keeps getting better.
