# Quick Guide

OSK Messaging aims to provide easy to use and flexible ways to manage various message delivery systems (couriers) and providing a seamless API to access and call these systems and the related process handling once messages are received. The underlying approach uses a concept of communication similar to a real world application of messaging through paper mail systems:

Sender -> Dispatch Center -> Courier -> Message Center (Receive) -> Message Box -> Recipients

The goal is to allow the sender a simple to use mechanism to initiate this process, which is done via a simple API call utilizing `IMessageDispatcher`, which will take any message of type `IMessage`. Using the dispatch options, callers can target specific courier systems to use during the dispatch process as well as how dispatching should process the message (i.e. to all couriers, first success, etc.)

Once a message has been dispatched, the caller can move on to process other requirements and leave the message processing component to the courier and related final delivery mechanism. Each courier is an integration with a custom or third party message system, which can include RabbitMQ, Kafka, or others. Couriers are only meant to handle the processing of the transmission of the message and not the immediate final delivery. 

After a message has been received by an `IMessageCenter`, it will then be sent to a specific message box that has been configured for the message of type <TMessage>. This message will then be passed through the message recipient's delegate that contains a pipeline of middleware configured by the application.

By using various configuration options, a message can be sent on a one to one delivery for a given message type or an inheritance pattern can be utilized to deliver a message to more than one box that supports that message. For example, if given a set of messages structure like below:
```
class SpecialEvent: IMessage
{

}

class SpecialEventPlus: SpecialEvent
{

}
```
a one to one delivery will mean that only messages of type `SpecialEvent` or `SpecialEventPlus` will process the events respectively. But, in the inheritance pattern approach, `SpecialEventPlus` could be processed by Message boxes that handle `SpecialEvent` and `SpecialEventPlus`. In this way, in some cases redundant message objects do not need to be created.

## Notes
* The current library does not handle support for tracking state of acknowledgement for messages that are processed. If a message fails any one of a given set of message recipients or message boxes, the message is considered `Failed` and thus could be reprocessed. For this reason, code designs should account for the possibility of more than one message being processed for any given event (i.e. duplicate processing). As the need arises, there may be work done to better integrate selecting specific message delivery strategies.

# Couriers

Each courier is an integration with either a custom or third party implementation of some message delivery system (message bus). The current library supports integration with `Pigeons` and `EasyNetQ` out of the box. 

## Pigeons

There is a need to support sending messages via a local on-device process in some cases, like game engines or other similar needs. For this, the idea of a courier pigeon is used. Known for their ability to deliver messages back to their home nests, this project aims to provide local messaging for on-device applications either via a direct in-process or multi-threaded mechanism. 

## EasyNetQ

[EasyNetQ](https://github.com/EasyNetQ/EasyNetQ) is a popular library for supporting RabbitMQ style message buses and queues. The EasyNetQ integration allows setting up queues via a discovery pattern so having to hard define configuration per message type is unnecessary, though it is configurable as needed on a per message type basis if an application needs it.

# Usage

There are a couple of ways of using the SDK. One is for the API users and the other is for the courier integrations.

## Integrations

There are a few key interfaces that must be considered for any given integration:
* `ICourierService` - The bread and butter of an integration, this is the mechanism that is used to actually start transmitting a message to a message center
* `ICourierDescriptor` - The main data structure used to find and send messages to couriers. This provides the name, which is used to filter to specific couriers if desired, and the courier service type, which should be a typed of `ICourierService` that is registered on the DI to pull. This is used by the dispatcher to transmit the message.
* `IMessageDeliveryPipeline` - An optional integration point, this is meant to be a long-running thread, subsriber, or similar data structure. The purpose is to handle messages being received by a courier and to initiate the delivery process once received. This pipeline should transmit all messages received to the message center. 
  * **Note:** If an integration doesn't need long-running subscribers then this integration point can be skipped
* `IMessageDeliveryPipelineFactory` - An optional integration point, _which should be implemented if a deliver pipeline is created_, that is used to construct the delivery pipelines using the reciipient details.
* `IMessageCenter` - The main initiator of delivering messages from a delivery pipeline. 

Depending on the integration needs, only the use of a courier descriptor, courier service, and messge center might be required. You can check out the implementation for [Pigeons](https://github.com/OpenSourceKingdom/OSK.Messages.Couriers.Pigeons) as an example of an integration that doesn't require long running subscribers. In other cases where long running subscribers are needed, integrations will need to implement the message deliver pipeline and related factory interfaces for the integration to function as expected. Youc an check out the implementation for the [EasyNetQ Courier](https://github.com/OpenSourceKingdom/OSK.Messages.Couriers.EasyNetQ) as an example of an integration that uses these.

Finally, it is important that integrations add the needed descriptor, courier service, etc. to the DI chain for the SDK to function properly.

**Note:** in order for the delivery pipelines to run, it is important that a hosted or similar service is started that runs the pipelines in a background thread or similar mechanism. Simply adding them to DI is not sufficient. There is a project that does this `OSK.Messages.Runtime` will add a hosted service that will pull in all integrated factories and start the pipelines by creating each pipeline needed for all message boxes to processs

## Consumers

Depending on the usage for a project, the requirements to utilize the SDK will vary slightly. For projects only needing access to the interfaces to call the dispatcher to send messages, only the `OSK.Messages.Abstractions` project should be needed. With that stated, it should be noted that unless an application is using the core `OSK.Message.Messenging` library with the dependencies applies to the service contain that the system will not work. There is a need for the main application to add the messaging system to the DI chain.

The main focal points for consumers will be the following:
`IMessageDispatcher` - the main port used to initiate sending messages in an application
`IMessageCenterBuilder` - the main builder that constructs a message center used to process final delivery of messages received from a courier. This builder is needed to be used via the related `AddMessaging` extensions to the DI chain and will help to configure the messaging system for an application's needs.
`IMessageBoxConfigurator` - the box configurator is used to help register message boxes to a message center and also connect them to various message recipients

When using the SDK for consumption, only adding the needed extensions for the messaging system and any needed couriers should be required.