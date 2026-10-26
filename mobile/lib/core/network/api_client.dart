import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/auth/auth_repository.dart';
import 'package:tan_erp_field/core/config/app_config.dart';
import 'package:tan_erp_field/core/l10n/locale_provider.dart';
import 'package:tan_erp_field/core/membership/membership_provider.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:tan_erp_field/core/network/interceptors.dart';

/// Thin dio wrapper. PENDING CODEGEN: calls are generic (raw JSON) until
/// `tool/generate_api.sh` produces typed clients in lib/generated/api; callers
/// must not hand-write DTOs on top of this.
class ApiClient {
  ApiClient(this._dio);

  final Dio _dio;

  Future<Object?> getJson(
    String path, {
    Map<String, Object?>? query,
    bool authenticated = true,
  }) async {
    try {
      final response = await _dio.get<Object?>(
        path,
        queryParameters: query,
        options: Options(extra: {skipAuthKey: !authenticated}),
      );
      return response.data;
    } on DioException catch (e) {
      final failure = e.error;
      throw failure is ApiFailure ? failure : const UnknownFailure();
    }
  }
}

final apiClientProvider = Provider<ApiClient>((ref) {
  final config = ref.watch(appConfigProvider);
  final dio = Dio(
    BaseOptions(
      baseUrl: config.apiBaseUrl,
      connectTimeout: const Duration(seconds: 15),
      receiveTimeout: const Duration(seconds: 30),
    ),
  );
  // Reads happen lazily per request, so no provider cycle with the session.
  dio.interceptors.addAll([
    AuthInterceptor(
      dio: dio,
      idToken: ({forceRefresh = false}) =>
          ref.read(authRepositoryProvider).idToken(forceRefresh: forceRefresh),
    ),
    ContextInterceptor(
      membershipId: () => ref.read(activeMembershipIdProvider),
      languageCode: () => ref.read(localeProvider).languageCode,
    ),
    ProblemDetailsInterceptor(),
  ]);
  return ApiClient(dio);
});
