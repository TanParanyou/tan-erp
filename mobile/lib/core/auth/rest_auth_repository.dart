import 'package:dio/dio.dart';
import 'package:tan_erp_api/tan_erp_api.dart';
import 'package:tan_erp_field/core/auth/auth_failure.dart';
import 'package:tan_erp_field/core/auth/auth_repository.dart';
import 'package:tan_erp_field/core/auth/session_storage.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';

/// Self-hosted REST authentication repository backed by TAN ERP OpenAPI.
/// Persists refresh tokens in [SessionStorage] securely while maintaining
/// access tokens strictly in memory.
class RestAuthRepository implements AuthRepository {
  RestAuthRepository({
    required this.authApi,
    required this.sessionStorage,
  });

  final AuthApi authApi;
  final SessionStorage sessionStorage;

  String? _accessToken;
  DateTime? _expiresAtUtc;
  Future<String?>? _refreshFlight;

  @override
  bool get hasSession {
    final now = DateTime.now().toUtc();
    final expiresAt = _expiresAtUtc?.toUtc();
    final hasValidMemoryToken =
        _accessToken != null && expiresAt != null && expiresAt.isAfter(now);
    return hasValidMemoryToken || sessionStorage.hasPersistedSession;
  }

  @override
  Future<SignInOutcome> signIn({
    required String email,
    required String password,
  }) async {
    try {
      final response = await authApi.apiV1AuthLoginPost(
        loginRequest: LoginRequest(
          (b) => b
            ..email = email.trim()
            ..password = password
            ..clientPlatform = 'mobile',
        ),
      );
      final data = response.data;
      if (data == null) throw const AuthUnknownFailure();

      final status = data.status;
      switch (status) {
        case 'authenticated':
          _accessToken = data.accessToken;
          _expiresAtUtc = data.expiresAtUtc;
          if (data.refreshToken != null && data.refreshToken!.isNotEmpty) {
            await sessionStorage.writeRefreshToken(data.refreshToken!);
          }
          if (data.sessionId != null && data.sessionId!.isNotEmpty) {
            await sessionStorage.writeSessionId(data.sessionId!);
          }
          return const SignInCompleted();

        case 'requires_two_factor':
          final challengeToken = data.challengeToken;
          if (challengeToken == null || challengeToken.isEmpty) {
            throw const TwoFactorChallengeInvalidFailure();
          }
          return SignInTwoFactorRequired(challengeToken: challengeToken);

        case 'requires_two_factor_setup':
          final challengeToken = data.challengeToken;
          final totp = data.totpSetup;
          if (challengeToken == null ||
              challengeToken.isEmpty ||
              totp == null ||
              totp.secretKey == null) {
            throw const TwoFactorChallengeInvalidFailure();
          }
          return SignInTwoFactorSetupRequired(
            challengeToken: challengeToken,
            secretKey: totp.secretKey!,
            otpAuthUri: totp.otpAuthUri ?? '',
          );

        default:
          throw const AuthUnknownFailure();
      }
    } on DioException catch (e) {
      throw _mapDioException(e);
    }
  }

  @override
  Future<List<String>> verifyTwoFactor({
    required String challengeToken,
    required String code,
  }) async {
    try {
      final response = await authApi.apiV1Auth2faVerifyLoginPost(
        verifyTwoFactorLoginRequest: VerifyTwoFactorLoginRequest(
          (b) => b
            ..challengeToken = challengeToken.trim()
            ..code = code.trim()
            ..clientPlatform = 'mobile',
        ),
      );
      final data = response.data;
      if (data == null) throw const AuthUnknownFailure();

      if (data.status != 'authenticated') {
        throw const TwoFactorCodeInvalidFailure();
      }

      _accessToken = data.accessToken;
      _expiresAtUtc = data.expiresAtUtc;
      if (data.refreshToken != null && data.refreshToken!.isNotEmpty) {
        await sessionStorage.writeRefreshToken(data.refreshToken!);
      }
      if (data.sessionId != null && data.sessionId!.isNotEmpty) {
        await sessionStorage.writeSessionId(data.sessionId!);
      }

      final backupCodes = data.backupCodes?.toList() ?? const <String>[];
      return backupCodes;
    } on DioException catch (e) {
      throw _mapDioException(e);
    }
  }

  @override
  Future<String?> idToken({bool forceRefresh = false}) async {
    final now = DateTime.now().toUtc();
    final expiresAt = _expiresAtUtc?.toUtc();
    final isValid = _accessToken != null &&
        expiresAt != null &&
        expiresAt.difference(now) > const Duration(seconds: 30);

    if (isValid && !forceRefresh) {
      return _accessToken;
    }

    if (_refreshFlight != null) {
      return await _refreshFlight;
    }

    final flight = _performRefresh();
    _refreshFlight = flight;
    try {
      return await flight;
    } finally {
      _refreshFlight = null;
    }
  }

  Future<String?> _performRefresh() async {
    final refreshToken = await sessionStorage.readRefreshToken();
    if (refreshToken == null || refreshToken.isEmpty) {
      _accessToken = null;
      _expiresAtUtc = null;
      return null;
    }

    try {
      final response = await authApi.apiV1AuthRefreshPost(
        refreshSessionRequest: RefreshSessionRequest(
          (b) => b
            ..refreshToken = refreshToken
            ..clientPlatform = 'mobile',
        ),
      );
      final data = response.data;
      if (data == null || data.accessToken == null) {
        _accessToken = null;
        _expiresAtUtc = null;
        await sessionStorage.clearAuth();
        return null;
      }

      _accessToken = data.accessToken;
      _expiresAtUtc = data.expiresAtUtc;
      if (data.refreshToken != null && data.refreshToken!.isNotEmpty) {
        await sessionStorage.writeRefreshToken(data.refreshToken!);
      }
      if (data.sessionId != null && data.sessionId!.isNotEmpty) {
        await sessionStorage.writeSessionId(data.sessionId!);
      }
      return _accessToken;
    } on DioException catch (e) {
      final statusCode = e.response?.statusCode;
      final error = e.error;
      final apiFailure = error is ApiFailure ? error : null;
      if (statusCode == 401 || apiFailure?.status == 401) {
        _accessToken = null;
        _expiresAtUtc = null;
        await sessionStorage.clearAuth();
        return null;
      }
      return null;
    } on Object catch (_) {
      return null;
    }
  }

  @override
  Future<void> signOut() async {
    final storedRefreshToken = await sessionStorage.readRefreshToken();
    _accessToken = null;
    _expiresAtUtc = null;
    await sessionStorage.clear();

    if (storedRefreshToken != null && storedRefreshToken.isNotEmpty) {
      try {
        await authApi.apiV1AuthLogoutPost(
          logoutRequest: LogoutRequest(
            (b) => b..refreshToken = storedRefreshToken,
          ),
        );
      } on Object catch (_) {
        // Ignored to guarantee offline logout always succeeds.
      }
    }
  }

  AuthFailure _mapDioException(DioException e) {
    final failure = e.error;
    if (failure is ApiFailure) {
      if (failure is NetworkFailure) {
        return const AuthNetworkFailure();
      }
      if (failure.code != null) {
        return AuthFailure.fromCode(failure.code);
      }
    }

    final response = e.response;
    if (response != null) {
      final data = response.data;
      if (data is ApiProblemDetails && data.code != null) {
        return AuthFailure.fromCode(data.code);
      }
      if (data is Map<String, Object?>) {
        final code = data['code'];
        if (code is String) {
          return AuthFailure.fromCode(code);
        }
      }
      if (response.statusCode == 401) {
        return const InvalidCredentialsFailure();
      }
      if (response.statusCode == 403) {
        return const AccountDisabledFailure();
      }
    }

    if (e.type == DioExceptionType.connectionTimeout ||
        e.type == DioExceptionType.sendTimeout ||
        e.type == DioExceptionType.receiveTimeout ||
        e.type == DioExceptionType.connectionError) {
      return const AuthNetworkFailure();
    }

    return const AuthUnknownFailure();
  }
}
