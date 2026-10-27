import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/auth/auth_failure.dart';
import 'package:tan_erp_field/core/auth/firebase_email_auth_repository.dart';
import 'package:tan_erp_field/core/auth/mock_auth_repository.dart';
import 'package:tan_erp_field/core/config/app_config.dart';

/// Identity provider boundary. Email/password via Firebase first; other
/// providers (Google/Apple) add an implementation without touching callers.
abstract interface class AuthRepository {
  /// True when a previously signed-in user is persisted on the device.
  bool get hasSession;

  Future<void> signIn({required String email, required String password});

  /// Latest ID token, or null when signed out.
  Future<String?> idToken({bool forceRefresh = false});

  Future<void> signOut();
}

/// Used when Firebase options are absent so the app still starts.
class NotConfiguredAuthRepository implements AuthRepository {
  const NotConfiguredAuthRepository();

  @override
  bool get hasSession => false;

  @override
  Future<void> signIn({required String email, required String password}) =>
      Future.error(const AuthNotConfiguredFailure());

  @override
  Future<String?> idToken({bool forceRefresh = false}) async => null;

  @override
  Future<void> signOut() async {}
}

final authRepositoryProvider = Provider<AuthRepository>((ref) {
  final config = ref.watch(appConfigProvider);
  if (config.isMockAuth) return MockAuthRepository();
  return config.firebase.isConfigured
      ? FirebaseEmailAuthRepository()
      : const NotConfiguredAuthRepository();
});
