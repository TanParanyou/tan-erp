import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

/// Keychain/Keystore-backed storage for session tokens and facts (never tokens
/// or PII in logs).
class SessionStorage {
  SessionStorage({
    FlutterSecureStorage? storage,
    String? initialRefreshToken,
    String? initialSessionId,
  })  : _storage = storage ?? const FlutterSecureStorage(),
        _cachedRefreshToken = initialRefreshToken,
        _cachedSessionId = initialSessionId,
        _isLoaded = initialRefreshToken != null || initialSessionId != null;

  static const _membershipKey = 'last_membership_id';
  static const _refreshTokenKey = 'auth_refresh_token';
  static const _sessionIdKey = 'auth_session_id';

  final FlutterSecureStorage _storage;
  String? _cachedRefreshToken;
  String? _cachedSessionId;
  bool _isLoaded;

  /// True when a refresh token exists in secure storage or memory cache.
  bool get hasPersistedSession =>
      _cachedRefreshToken != null && _cachedRefreshToken!.isNotEmpty;

  /// Cached refresh token in memory.
  String? get cachedRefreshToken => _cachedRefreshToken;

  /// Cached session ID in memory.
  String? get cachedSessionId => _cachedSessionId;

  /// Preloads tokens from secure storage into memory cache.
  Future<void> init() async {
    _cachedRefreshToken = await _storage.read(key: _refreshTokenKey);
    _cachedSessionId = await _storage.read(key: _sessionIdKey);
    _isLoaded = true;
  }

  Future<String?> readRefreshToken() async {
    if (!_isLoaded) {
      _cachedRefreshToken = await _storage.read(key: _refreshTokenKey);
    }
    return _cachedRefreshToken;
  }

  Future<void> writeRefreshToken(String token) async {
    _cachedRefreshToken = token;
    _isLoaded = true;
    await _storage.write(key: _refreshTokenKey, value: token);
  }

  Future<String?> readSessionId() async {
    if (!_isLoaded) {
      _cachedSessionId = await _storage.read(key: _sessionIdKey);
    }
    return _cachedSessionId;
  }

  Future<void> writeSessionId(String sessionId) async {
    _cachedSessionId = sessionId;
    _isLoaded = true;
    await _storage.write(key: _sessionIdKey, value: sessionId);
  }

  Future<String?> readMembershipId() => _storage.read(key: _membershipKey);

  Future<void> writeMembershipId(String id) =>
      _storage.write(key: _membershipKey, value: id);

  /// Clears only authentication tokens (refresh token and session ID).
  Future<void> clearAuth() async {
    _cachedRefreshToken = null;
    _cachedSessionId = null;
    await _storage.delete(key: _refreshTokenKey);
    await _storage.delete(key: _sessionIdKey);
  }

  /// Clears all stored data.
  Future<void> clear() async {
    _cachedRefreshToken = null;
    _cachedSessionId = null;
    await _storage.deleteAll();
  }
}

final sessionStorageProvider = Provider<SessionStorage>(
  (ref) => SessionStorage(),
);
