import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

/// Keychain/Keystore-backed storage for session facts (never tokens or PII
/// in logs). Firebase itself persists the refresh token in the same stores.
class SessionStorage {
  SessionStorage([FlutterSecureStorage? storage])
    : _storage = storage ?? const FlutterSecureStorage();

  static const _membershipKey = 'last_membership_id';
  final FlutterSecureStorage _storage;

  Future<String?> readMembershipId() => _storage.read(key: _membershipKey);

  Future<void> writeMembershipId(String id) =>
      _storage.write(key: _membershipKey, value: id);

  Future<void> clear() => _storage.deleteAll();
}

final sessionStorageProvider = Provider<SessionStorage>(
  (ref) => SessionStorage(),
);
