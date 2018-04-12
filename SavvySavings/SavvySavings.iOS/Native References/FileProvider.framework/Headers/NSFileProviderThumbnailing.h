//
//  NSFileProviderThumbnailing.h
//  FileProvider
//
//  Copyright © 2017 Apple Inc. All rights reserved.
//

#import <FileProvider/NSFileProviderDefines.h>
#import <CoreGraphics/CoreGraphics.h>
#import <FileProvider/NSFileProviderExtension.h>
#import <FileProvider/NSFileProviderItem.h>

NS_ASSUME_NONNULL_BEGIN

@interface NSFileProviderExtension (NSFileProviderThumbnailing)

/*
 The system calls this methods to get thumbnails for the previously enumerated
 items.

 If there is a thumbnail request in flight and the system decides it's not
 needed anymore, the system will call the `NSProgress.cancel` method of the
 returned `NSProgress` object.
 The implementation should cleanup resources in the -cancel method.

 */
- (NSProgress *)fetchThumbnailsForItemIdentifiers:(NSArray<NSFileProviderItemIdentifier> *)itemIdentifiers
                                    requestedSize:(CGSize)size
                    perThumbnailCompletionHandler:(void (^)(NSFileProviderItemIdentifier identifier, NSData * _Nullable imageData, NSError * _Nullable error))perThumbnailCompletionHandler
                                completionHandler:(void (^)(NSError * _Nullable error))completionHandler API_AVAILABLE(ios(11.0)) API_UNAVAILABLE(macos, watchos, tvos);

@end

NS_ASSUME_NONNULL_END
