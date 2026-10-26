import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:package_info_plus/package_info_plus.dart';
import 'package:tan_erp_field/core/network/api_client.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:tan_erp_field/core/network/interceptors.dart';
import 'package:tan_erp_field/features/app_update/domain/version_policy.dart';

final versionPolicyProvider = FutureProvider<VersionPolicy?>((ref) async {
  final api = ref.watch(apiClientProvider);
  final platform = defaultTargetPlatform == TargetPlatform.iOS
      ? 'ios'
      : 'android';
  try {
    final response = await api.call(
      () => api.api.getAppVersionApi().apiV1AppVersionPolicyGet(
        platform: platform,
        extra: {skipAuthKey: true},
      ),
    );
    final minimum = response.minimumVersion;
    final latest = response.latestVersion;
    if (minimum == null || latest == null) return null;
    return VersionPolicy(minimumVersion: minimum, latestVersion: latest);
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
