import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_api/tan_erp_api.dart';
import 'package:tan_erp_field/core/auth/auth_repository.dart';
import 'package:tan_erp_field/core/config/app_config.dart';
import 'package:tan_erp_field/core/l10n/locale_provider.dart';
import 'package:tan_erp_field/core/membership/membership_provider.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:tan_erp_field/core/network/interceptors.dart';

/// Typed API access: the generated `tan_erp_api` client running on the app's
/// own [Dio] (interceptors do auth, headers and Problem Details mapping).
class ApiClient {
  ApiClient(Dio dio) : api = TanErpApi(dio: dio, interceptors: const []);

  final TanErpApi api;

  /// Runs a generated call and returns its body; failures surface as
  /// [ApiFailure] only.
  Future<T> call<T>(Future<Response<T>> Function() request) async {
    try {
      final data = (await request()).data;
      if (data == null) throw const UnknownFailure(code: 'EMPTY_RESPONSE');
      return data;
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
