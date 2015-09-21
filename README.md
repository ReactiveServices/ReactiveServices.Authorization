# Reactive Services

Reactive Services is a framework that support the development of distributed applications over the [.NET/Mono](http://www.mono-project.com/) frameworks, following the principles of the [Reactive Manifesto](http://www.reactivemanifesto.org) and ready for cloud distribution.

See [this link](http://reactiveservices.github.io) for an overview about the framework.

[![Build status](https://ci.appveyor.com/api/projects/status/ovwbbiiuqfrqmhc3?svg=true)](https://ci.appveyor.com/project/rafaelromao/reactiveservices-messagebus)

## ReactiveServices.MessageBus

ReactiveServices.MessageBus is a support package for the Reactive Services framework that provide, among other shared features, the ability to talk to a Message Bus in an abstract way.

By default the framework uses RabbitMQ as the messaging middleware, but other implementations can be easily implemented.

Although designed to be used by the Reactive Services framework and Reactive Services applications, this package can be used as a client for RabbitMQ and other messaging middleware for any .NET/Mono application.

### Modules and Components

This package provides the following modules:

- **ReactiveServices.Extensions**: This module provides some extension methods used by the framework.
- **ReactiveServices.Configuration**: This module provides classes to handle the config files of the framework.
- **ReactiveServices.Authorization**: This module provides classes to handle authorization rules on Reactive Services applications.
- **ReactiveServices.MessageBus**: This is the main module of this package and provides the following components:
	- **ReactiveServices.MessageBus.IPublishingBus**: Interface used to publish a message.
	- **ReactiveServices.MessageBus.ISubscriptionBus**: Interface used to subscribe to a message type.
	- **ReactiveServices.MessageBus.IRequestBus**: Interface used to publish a request.
	- **ReactiveServices.MessageBus.IResponseBus**: Interface used to listen for a request and publish a response.
	- **ReactiveServices.MessageBus.ISendingBus**: Interface used to send a direct message.
	- **ReactiveServices.MessageBus.IReveivingBus**: Interface used listen for a direct message.

## License

MIT License

## Versioning

SemVer 2.0