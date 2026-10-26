import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:package_info_plus/package_info_plus.dart';
import 'package:tan_erp_field/core/network/api_client.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:tan_erp_field/features/app_update/domain/version_policy.dart';

/// Maps the raw version-policy JSON to [VersionPolicy].
///
/// PENDING CODEGEN: override with a mapper from the generated
/// `AppVersionPolicyResponse` after `tool/generate_api.sh`; no hand-written
/// DTO parsing.
typedef VersionPolicyParser = VersionPolicy Function(Object? json);

final versionPolicyParserProvider = Provider<VersionPolicyParser>(
  (ref) =>
      (_) => throw const UnknownFailure(code: 'PENDING_CODEGEN'),
);

final versionPolicyProvider = FutureProvider<VersionPolicy?>((ref) async {
  final api = ref.watch(apiClientProvider);
  final parse = ref.watch(versionPolicyParserProvider);
  final platform = defaultTargetPlatform == TargetPlatform.iOS
      ? 'ios'
      : 'android';
  try {
    return parse(
      await api.getJson(
        '/api/v1/app/version-policy',
        query: {'platform': platform},
        authenticated: false,
      ),
    );
  } on ApiFailure {
    return null; // fail open: a failed check must never lock users out
  }
});

final appVersionProvider = FutureProvider<String>(
  (ref) async => (await PackageInfo.fromPlatform()).version,
);

@immutable
class UpdateRequirement {
  const UpdateRequirement({required this.minimum, required this.current});

  final String minimum;
  final String current;
}

/// Non-null when the installed version is below the server minimum.
final updateRequirementProvider = Provider<UpdateRequirement?>((ref) {
  final policy = ref.watch(versionPolicyProvider).value;
  final current = ref.watch(appVersionProvider).value;
  if (policy == null || current == null) return null;
  return compareVersions(current, policy.minimumVersion) < 0
      ? UpdateRequirement(minimum: policy.minimumVersion, current: current)
      : null;
});
