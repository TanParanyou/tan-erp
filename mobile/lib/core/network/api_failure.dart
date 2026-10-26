import 'package:flutter/foundation.dart';

/// Structured failure for every API call. Built from RFC 7807 Problem Details:
/// the HTTP status picks the category and the machine `code` is carried
/// verbatim for l10n lookup. Messages are never compared.
@immutable
sealed class ApiFailure implements Exception {
  const ApiFailure({this.code, this.status, this.traceId});

  /// Maps a Problem Details payload (already narrowed by the caller).
  factory ApiFailure.fromProblem({
    required int status,
    String? code,
    String? traceId,
    Map<String, List<String>> fieldErrors = const {},
  }) {
    return switch (status) {
      400 || 422 => ValidationFailure(
        code: code,
        status: status,
        traceId: traceId,
        fieldErrors: fieldErrors,
      ),
      401 => UnauthorizedFailure(code: code, status: status, traceId: traceId),
      403 => ForbiddenFailure(code: code, status: status, traceId: traceId),
      404 => NotFoundFailure(code: code, status: status, traceId: traceId),
      409 ||
      412 => ConflictFailure(code: code, status: status, traceId: traceId),
      _ => UnknownFailure(code: code, status: status, traceId: traceId),
    };
  }

  /// Backend error code such as `USER_ACCESS_DISABLED`.
  final String? code;
  final int? status;
  final String? traceId;
}

class ValidationFailure extends ApiFailure {
  const ValidationFailure({
    super.code,
    super.status,
    super.traceId,
    this.fieldErrors = const {},
  });

  final Map<String, List<String>> fieldErrors;
}

class ConflictFailure extends ApiFailure {
  const ConflictFailure({super.code, super.status, super.traceId});
}

class UnauthorizedFailure extends ApiFailure {
  const UnauthorizedFailure({super.code, super.status, super.traceId});
}

class ForbiddenFailure extends ApiFailure {
  const ForbiddenFailure({super.code, super.status, super.traceId});
}

class NotFoundFailure extends ApiFailure {
  const NotFoundFailure({super.code, super.status, super.traceId});
}

class NetworkFailure extends ApiFailure {
  const NetworkFailure();
}

class UnknownFailure extends ApiFailure {
  const UnknownFailure({super.code, super.status, super.traceId});
}
