import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';

@immutable
class ThemeSettings {
  const ThemeSettings({
    this.mode = ThemeMode.system,
    this.density = ErpDensity.normal,
    this.textScale = 1,
  });

  final ThemeMode mode;
  final ErpDensity density;

  /// 0.9 - 1.3 (design.md 7.3).
  final double textScale;

  ThemeSettings copyWith({
    ThemeMode? mode,
    ErpDensity? density,
    double? textScale,
  }) => ThemeSettings(
    mode: mode ?? this.mode,
    density: density ?? this.density,
    textScale: textScale ?? this.textScale,
  );
}

/// In-memory settings: filled from `/me` on login, saved through
/// `PreferencesSaveController`.
class ThemeSettingsController extends Notifier<ThemeSettings> {
  @override
  ThemeSettings build() => const ThemeSettings();

  ThemeSettings get settings => state;
  set settings(ThemeSettings value) => state = value;
}

final themeSettingsProvider =
    NotifierProvider<ThemeSettingsController, ThemeSettings>(
      ThemeSettingsController.new,
    );
