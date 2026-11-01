import 'package:tan_erp_field/core/auth/auth_repository.dart';

/// DEV ONLY (dev flavor with MOCK_AUTH=true): the backend accepts the token
/// `dev-uid:<uid>` only in its Development environment. In sign-in the `email`
/// argument carries the uid. The session is in memory, so a restart signs out.
class MockAuthRepository implements AuthRepository {
  String? _uid;

  @override
  bool get hasSession => _uid != null;

  @override
  Future<SignInOutcome> signIn({
    required String email,
    required String password,
  }) async {
    _uid = email.trim();
    return const SignInCompleted();
  }

  @override
  Future<List<String>> verifyTwoFactor({
    required String challengeToken,
    required String code,
  }) async =>
      const <String>[];

  @override
  Future<String?> idToken({bool forceRefresh = false}) async =>
      _uid == null ? null : 'dev-uid:$_uid';

  @override
  Future<void> signOut() async => _uid = null;
}
