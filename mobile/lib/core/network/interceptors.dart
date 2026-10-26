import 'package:dio/dio.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:uuid/uuid.dart';

/// Header names shared with the web client (frontend/src/lib/api/api-client.ts).
abstract final class ApiHeaders {
  static const membershipId = 'X-Membership-Id';
  static const idempotencyKey = 'Idempotency-Key';
  static const ifMatch = 'If-Match';
  static const acceptLanguage = 'Accept-Language';
}

/// Per-request switch for public endpoints (e.g. version policy).
const skipAuthKey = 'skipAuth';
const _retriedKey = 'authRetried';

/// Attaches the Firebase ID token; one forced refresh + retry on 401.
class AuthInterceptor extends Interceptor {
  AuthInterceptor({required this.dio, required this.idToken});

  final Dio dio;
  final Future<String?> Function({bool forceRefresh}) idToken;

  @override
  Future<void> onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    if (options.extra[skipAuthKey] != true) {
      final token = await idToken();
      if (token != null) options.headers['Authorization'] = 'Bearer $token';
    }
    handler.next(options);
  }

  @override
  Future<void> onError(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    final req = err.requestOptions;
    if (err.response?.statusCode != 401 ||
        req.extra[skipAuthKey] == true ||
        req.extra[_retriedKey] == true) {
      return handler.next(err);
    }
    final token = await idToken(forceRefresh: true);
    if (token == null) return handler.next(err);
    req
      ..extra[_retriedKey] = true
      ..headers['Authorization'] = 'Bearer $token';
    try {
      handler.resolve(await dio.fetch<Object?>(req));
    } on DioException catch (e) {
      handler.next(e);
    }
  }
}

/// Membership, language, idempotency key (writes) and If-Match quoting.
class ContextInterceptor extends Interceptor {
  ContextInterceptor({required this.membershipId, required this.languageCode});

  static const _uuid = Uuid();
  static const _writeMethods = {'POST', 'PUT', 'PATCH', 'DELETE'};

  final String? Function() membershipId;
  final String Function() languageCode;

  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    final h = options.headers;
    h[ApiHeaders.acceptLanguage] = languageCode();
    h['Accept'] = 'application/json';
    final membership = membershipId();
    if (membership != null) h[ApiHeaders.membershipId] = membership;
    // Callers (outbox) may pass their own key so retries stay idempotent.
    if (_writeMethods.contains(options.method.toUpperCase())) {
      h.putIfAbsent(ApiHeaders.idempotencyKey, _uuid.v4);
    }
    final ifMatch = h[ApiHeaders.ifMatch];
    if (ifMatch is String &&
        !(ifMatch.startsWith('"') && ifMatch.endsWith('"'))) {
      h[ApiHeaders.ifMatch] = '"$ifMatch"';
    }
    handler.next(options);
  }
}

/// Converts any failed request into an [ApiFailure] carried in
/// `DioException.error`.
class ProblemDetailsInterceptor extends Interceptor {
  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    handler.next(err.copyWith(error: _toFailure(err)));
  }

  ApiFailure _toFailure(DioException err) {
    final response = err.response;
    if (response == null) return const NetworkFailure();
    final status = response.statusCode ?? 0;
    final data = response.data;
    if (data is! Map<String, Object?>) {
      return UnknownFailure(status: status);
    }
    final code = data['code'];
    final traceId = data['traceId'];
    final errors = data['errors'];
    return ApiFailure.fromProblem(
      status: status,
      code: code is String ? code : null,
      traceId: traceId is String ? traceId : null,
      fieldErrors: errors is Map<String, Object?>
          ? {
              for (final e in errors.entries)
                if (e.value is List<Object?>)
                  e.key: (e.value! as List<Object?>)
                      .whereType<String>()
                      .toList(),
            }
          : const {},
    );
  }
}
