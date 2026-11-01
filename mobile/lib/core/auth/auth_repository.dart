import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_api/tan_erp_api.dart';
import 'package:tan_erp_field/core/auth/mock_auth_repository.dart';
import 'package:tan_erp_field/core/auth/rest_auth_repository.dart';
import 'package:tan_erp_field/core/auth/session_storage.dart';
import 'package:tan_erp_field/core/config/app_config.dart';
import 'package:tan_erp_field/core/network/interceptors.dart';

sealed class SignInOutcome {
  const SignInOutcome();
}

class SignInCompleted extends SignInOutcome {
  const SignInCompleted();
}

class SignInTwoFactorRequired extends SignInOutcome {
  const SignInTwoFactorRequired({required this.challengeToken});

  final String challengeToken;
}

class SignInTwoFactorSetupRequired extends SignInOutcome {
  const SignInTwoFactorSetupRequired({
    required this.challengeToken,
    required this.secretKey,
    required this.otpAuthUri,
  });

  final String challengeToken;
  final String secretKey;
  final String otpAuthUri;
}

/// Identity provider boundary for TAN ERP field app.
abstract interface class AuthRepository {
  /// True when a valid memory session or persisted refresh token exists.
  bool get hasSession;

  Future<SignInOutcome> signIn({
    required String email,
    required String password,
  });

  Future<List<String>> verifyTwoFactor({
    required String challengeToken,
    required String code,
  });

  /// Latest access token, or null when signed out.
  Future<String?> idToken({bool forceRefresh = false});

  Future<void> signOut();
}

final authRepositoryProvider = Provider<AuthRepository>((ref) {
  final config = ref.watch(appConfigProvider);
  if (config.isMockAuth) return MockAuthRepository();

  final dio = Dio(
    BaseOptions(
      baseUrl: config.apiBaseUrl,
      connectTimeout: const Duration(seconds: 15),
      receiveTimeout: const Duration(seconds: 30),
    ),
  );
  dio.interceptors.add(ProblemDetailsInterceptor());
  final authApi = AuthApi(dio, standardSerializers);
  final sessionStorage = ref.watch(sessionStorageProvider);

  return RestAuthRepository(
    authApi: authApi,
    sessionStorage: sessionStorage,
  );
});
