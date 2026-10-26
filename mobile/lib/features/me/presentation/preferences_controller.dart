import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart' show Locale;
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/auth/current_user.dart';
import 'package:tan_erp_field/core/l10n/locale_provider.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:tan_erp_field/core/theme/theme_settings.dart';

@immutable
class PreferencesSaveState {
  const PreferencesSaveState({this.isSaving = false, this.failure});

  final bool isSaving;
  final ApiFailure? failure;
}

/// Optimistic save of theme preferences: applied at once, reverted when the
/// API call fails.
class PreferencesSaveController extends Notifier<PreferencesSaveState> {
  @override
  PreferencesSaveState build() => const PreferencesSaveState();

  Future<void> save(ThemeSettings next) async {
    if (state.isSaving) return;
    final theme = ref.read(themeSettingsProvider.notifier);
    final previous = theme.settings;
    theme.settings = next;
    state = const PreferencesSaveState(isSaving: true);
    try {
      await ref.read(currentUserRepositoryProvider).savePreferences(next);
      state = const PreferencesSaveState();
    } on ApiFailure catch (e) {
      theme.settings = previous;
      state = PreferencesSaveState(failure: e);
    }
  }

  /// Same optimistic pattern for the language; theme values are resent as is.
  Future<void> saveLocale(Locale next) async {
    if (state.isSaving) return;
    final localeNotifier = ref.read(localeProvider.notifier);
    final previous = localeNotifier.locale;
    localeNotifier.locale = next;
    state = const PreferencesSaveState(isSaving: true);
    try {
      await ref
          .read(currentUserRepositoryProvider)
          .savePreferences(
            ref.read(themeSettingsProvider),
            preferredLocale: next.languageCode,
          );
      state = const PreferencesSaveState();
    } on ApiFailure catch (e) {
      localeNotifier.locale = previous;
      state = PreferencesSaveState(failure: e);
    }
  }
}

final preferencesSaveProvider =
    NotifierProvider<PreferencesSaveController, PreferencesSaveState>(
      PreferencesSaveController.new,
    );
