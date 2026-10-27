import 'package:tan_erp_field/core/auth/auth_repository.dart';

/// DEV ONLY (dev flavor with MOCK_AUTH=true): the backend accepts the token
/// `dev-uid:<uid>` only in its Development environment. In sign-in the `email`
/// argument carries the uid. The session is in memory, so a restart signs out.
class MockAuthRepository implements AuthRepository {
  String? _uid;

  @override
  bool get hasSession => _uid != null;

  @override
  Future<void> signIn({required String email, required String password}) async {
    _uid = email.trim();
  }

  @override
  Future<String?> idToken({bool forceRefresh = false}) async =>
      _uid == null ? null : 'dev-uid:$_uid';

  @override
  Future<void> signOut() async => _uid = null;
}
