//
//  NSFileProviderMessaging.h
//  FileProvider
//
//  Copyright © 2017 Apple Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <FileProvider/NSFileProviderExtension.h>
#import <FileProvider/NSFileProviderItem.h>

NS_ASSUME_NONNULL_BEGIN

// A file provider can override the methods in this category to provide a custom communication
// channel to client applications.

// The application and the file provider must have agreed on an identifier
// (NSFileProviderMessageInterfaceName) and a protocol. The app can request a list of available
// identifiers to verify availability, and request a connection to the exported object.

@interface NSFileProviderExtension (NSFileProviderMessaging)

- (NSArray <NSFileProviderMessageInterfaceName> *)supportedMessageInterfaceNamesForItemWithIdentifier:(NSFileProviderItemIdentifier)itemIdentifier API_AVAILABLE(ios(11.0)) API_UNAVAILABLE(macos, watchos, tvos);

- (Protocol *)protocolForMessageInterface:(NSFileProviderMessageInterface *)messageInterface API_AVAILABLE(ios(11.0)) API_UNAVAILABLE(macos, watchos, tvos);

- (nullable id)exportedObjectForMessageInterface:(NSFileProviderMessageInterface *)messageInterface itemIdentifier:(NSFileProviderItemIdentifier)itemIdentifier error:(NSError **)error API_AVAILABLE(ios(11.0)) API_UNAVAILABLE(macos, watchos, tvos);

@end


NS_ASSUME_NONNULL_END
