import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_api/tan_erp_api.dart';
import 'package:tan_erp_field/core/network/api_client.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/theme/theme_settings.dart';

/// UI-facing view of the signed-in user (not a transport DTO).
@immutable
class CurrentUser {
  const CurrentUser({
    required this.displayName,
    required this.email,
    required this.memberships,
    this.preferences,
    this.preferredLocale,
  });

  final String displayName;
  final String email;
  final List<Membership> memberships;

  /// Null when the server value is missing or not recognised; the app then
  /// keeps its current settings instead of guessing.
  final ThemeSettings? preferences;
  final Locale? preferredLocale;
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

/// Text shown when the server sent no value (AGENTS.md: `-`).
const _empty = '-';

/// Maps the generated `/me` response to [CurrentUser].
CurrentUser mapCurrentUser(CurrentUserResponse response) {
  final user = response.user;
  if (user == null) throw const UnknownFailure(code: 'ME_RESPONSE_INVALID');
  return CurrentUser(
    displayName: user.displayName ?? _empty,
    email: user.email ?? _empty,
    memberships: [
      for (final m in response.memberships ?? const <MembershipDto>[])
        if (m.id case final id?)
          Membership(
            id: id,
            organizationName: m.organization?.name ?? _empty,
            branchName: m.branch?.name,
          ),
    ],
    preferences: mapPreferences(user.preferences),
    preferredLocale: switch (user.preferredLocale) {
      'th' => const Locale('th'),
      'en' => const Locale('en'),
      _ => null,
    },
  );
}

/// Wire values: themeMode light|dark|system, density comfortable|compact,
/// textScale 90-130 (percent).
ThemeSettings? mapPreferences(UserPreferencesDto? dto) {
  final mode = switch (dto?.themeMode) {
    'light' => ThemeMode.light,
    'dark' => ThemeMode.dark,
    'system' => ThemeMode.system,
    _ => null,
  };
  final density = switch (dto?.density) {
    'comfortable' => ErpDensity.normal,
    'compact' => ErpDensity.compact,
    _ => null,
  };
  final percent = dto?.textScale;
  if (mode == null || density == null || percent == null) return null;
  return ThemeSettings(mode: mode, density: density, textScale: percent / 100);
}

UpdateMyPreferencesRequest preferencesRequest(ThemeSettings s) =>
    UpdateMyPreferencesRequest(
      (b) => b
        ..themeMode = switch (s.mode) {
          ThemeMode.light => 'light',
          ThemeMode.dark => 'dark',
          ThemeMode.system => 'system',
        }
        ..density = switch (s.density) {
          ErpDensity.normal => 'comfortable',
          ErpDensity.compact => 'compact',
        }
        ..textScale = (s.textScale * 100).round(),
    );

abstract interface class CurrentUserRepository {
  Future<CurrentUser> load();

  /// `PUT /api/v1/me/preferences`; the endpoint needs all three values.
  Future<void> savePreferences(ThemeSettings settings);
}

class ApiCurrentUserRepository implements CurrentUserRepository {
  const ApiCurrentUserRepository(this._api);

  final ApiClient _api;

  @override
  Future<CurrentUser> load() async => mapCurrentUser(
    await _api.call(() => _api.api.getCurrentUserApi().apiV1MeGet()),
  );

  @override
  Future<void> savePreferences(ThemeSettings settings) async {
    await _api.call(
      () => _api.api.getCurrentUserApi().apiV1MePreferencesPut(
        updateMyPreferencesRequest: preferencesRequest(settings),
      ),
    );
  }
}

final currentUserRepositoryProvider = Provider<CurrentUserRepository>(
  (ref) => ApiCurrentUserRepository(ref.watch(apiClientProvider)),
);
