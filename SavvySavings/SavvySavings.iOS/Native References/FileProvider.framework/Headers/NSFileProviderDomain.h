//
//  NSFileProviderDomain.h
//  FileProvider
//
//  Copyright © 2017 Apple Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <FileProvider/NSFileProviderExtension.h>

NS_ASSUME_NONNULL_BEGIN

typedef NSString *NSFileProviderDomainIdentifier NS_EXTENSIBLE_STRING_ENUM;

/*
 File provider domain.

 A file provider domain can be used to represent accounts or different locations
 exposed within a given file provider.

 Domains can be registered to the system using
 `-[NSFileProviderMananger addDomain:completionHandler:]`).

 By default, a file provider extension does not have any domain.

 On the extension side, a separate instance of NSFileProviderExtension will be
 created for each `NSFileProviderDomain` registered.  In that case, the
 `NSFileProviderExtension.domain` properties will indicate which domain the
 NSFileProviderExtension belongs to (or nil if none).

 All the files on disk belonging to the same domain must be grouped inside a
 common directory. That directory path is indicated by the
 `pathRelativeToDocumentStorage` property.
 */
API_AVAILABLE(ios(11.0), macos(10.13))
@interface NSFileProviderDomain : NSObject

/*
 Initialize a new NSFileProviderDomain

 The file provider extension implementation can pick any `identifier` as it sees
 fit to identify the group of items.

 The `displayName` is a user visible string representing the group of items the
 file provider extension is using.

 The `pathRelativeToDocumentStorage` is a path relative to
 `NSFileProviderExtension.documentStorageURL`.

 */
- (instancetype)initWithIdentifier:(NSFileProviderDomainIdentifier)identifier displayName:(NSString *)displayName pathRelativeToDocumentStorage:(NSString *)pathRelativeToDocumentStorage;

/*
 The identifier - as provided by the file provider extension.
 */
@property (readonly, copy) NSFileProviderDomainIdentifier identifier;

/*
 The display name shown by the system to represent this domain.
 */
@property (readonly, copy) NSString *displayName;

/*
 The path relative to the document storage of the file provider extension.
 Files belonging to this domains should be stored under this path.
 */
@property (readonly, copy) NSString *pathRelativeToDocumentStorage;

@end

@interface NSFileProviderExtension (NSFileProviderDomain)
@property(nonatomic, readonly, nullable) NSFileProviderDomain *domain;
@end

NS_ASSUME_NONNULL_END
