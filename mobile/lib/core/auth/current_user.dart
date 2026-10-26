import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/network/api_client.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';

/// UI-facing view of the signed-in user (not a transport DTO).
@immutable
class CurrentUser {
  const CurrentUser({
    required this.displayName,
    required this.email,
    required this.memberships,
  });

  final String displayName;
  final String email;
  final List<Membership> memberships;
}

@immutable
class Membership {
  const Membership({
    required this.id,
    required this.organizationName,
    this.branchName,
  });

  final String id;
  final String organizationName;

  /// Null = organisation-wide membership (all branches).
  final String? branchName;
}

/// Maps the raw `GET /api/v1/me` JSON to [CurrentUser].
///
/// PENDING CODEGEN: hand-written DTO parsing is not allowed. Once
/// `tool/generate_api.sh` has produced `CurrentUserResponse`, override
/// [currentUserParserProvider] with a mapper from that generated type.
typedef CurrentUserParser = CurrentUser Function(Object? json);

final currentUserParserProvider = Provider<CurrentUserParser>(
  (ref) =>
      (_) => throw const UnknownFailure(code: 'PENDING_CODEGEN'),
);

abstract interface class CurrentUserRepository {
  Future<CurrentUser> load();
}

class ApiCurrentUserRepository implements CurrentUserRepository {
  const ApiCurrentUserRepository(this._api, this._parse);

  final ApiClient _api;
  final CurrentUserParser _parse;

  @override
  Future<CurrentUser> load() async => _parse(await _api.getJson('/api/v1/me'));
}

final currentUserRepositoryProvider = Provider<CurrentUserRepository>(
  (ref) => ApiCurrentUserRepository(
    ref.watch(apiClientProvider),
    ref.watch(currentUserParserProvider),
  ),
);
