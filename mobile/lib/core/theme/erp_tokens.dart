import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/tokens.g.dart';

enum ErpDensity { normal, compact }

/// Semantic design tokens as a [ThemeExtension]. Widgets read this and never
/// hard-code colours, radii or spacing (design.md 7.2).
@immutable
class ErpTokens extends ThemeExtension<ErpTokens> {
  const ErpTokens({
    required this.canvas,
    required this.surface,
    required this.surfaceMuted,
    required this.surfaceSubtle,
    required this.border,
    required this.borderStrong,
    required this.textMain,
    required this.textBody,
    required this.textMuted,
    required this.textSubtle,
    required this.action,
    required this.actionHover,
    required this.actionLight,
    required this.onAction,
    required this.success,
    required this.successBg,
    required this.successBorder,
    required this.warning,
    required this.warningBg,
    required this.warningBorder,
    required this.danger,
    required this.dangerBg,
    required this.dangerBorder,
    required this.info,
    required this.infoBg,
    required this.accentMeasure,
    required this.space1,
    required this.space2,
    required this.space3,
    required this.space4,
    required this.space6,
    required this.space8,
    required this.radius,
    required this.borderWidth,
    required this.tapTarget,
    required this.primaryActionHeight,
    required this.textCaption,
    required this.textBodySize,
    required this.textSubtitle,
    required this.textTitle,
    required this.textMeasure,
  });

  /// Light palette + scale tokens. [density] only shrinks spacing; tap
  /// targets never go below `appTapTarget`.
  factory ErpTokens.light([ErpDensity density = ErpDensity.normal]) => _build(
    density,
    canvas: ErpTokensLight.canvas,
    surface: ErpTokensLight.surface,
    surfaceMuted: ErpTokensLight.surfaceMuted,
    surfaceSubtle: ErpTokensLight.surfaceSubtle,
    border: ErpTokensLight.border,
    borderStrong: ErpTokensLight.borderStrong,
    textMain: ErpTokensLight.textMain,
    textBody: ErpTokensLight.textBody,
    textMuted: ErpTokensLight.textMuted,
    textSubtle: ErpTokensLight.textSubtle,
    action: ErpTokensLight.navy,
    actionHover: ErpTokensLight.navyHover,
    actionLight: ErpTokensLight.navyLight,
    onAction: ErpTokensLight.textOnNavy,
    success: ErpTokensLight.success,
    successBg: ErpTokensLight.successBg,
    successBorder: ErpTokensLight.successBorder,
    warning: ErpTokensLight.warning,
    warningBg: ErpTokensLight.warningBg,
    warningBorder: ErpTokensLight.warningBorder,
    danger: ErpTokensLight.danger,
    dangerBg: ErpTokensLight.dangerBg,
    dangerBorder: ErpTokensLight.dangerBorder,
    info: ErpTokensLight.info,
    infoBg: ErpTokensLight.infoBg,
    accentMeasure: ErpTokensLight.accentMeasure,
  );

  factory ErpTokens.dark([ErpDensity density = ErpDensity.normal]) => _build(
    density,
    canvas: ErpTokensDark.canvas,
    surface: ErpTokensDark.surface,
    surfaceMuted: ErpTokensDark.surfaceMuted,
    surfaceSubtle: ErpTokensDark.surfaceSubtle,
    border: ErpTokensDark.border,
    borderStrong: ErpTokensDark.borderStrong,
    textMain: ErpTokensDark.textMain,
    textBody: ErpTokensDark.textBody,
    textMuted: ErpTokensDark.textMuted,
    textSubtle: ErpTokensDark.textSubtle,
    action: ErpTokensDark.navy,
    actionHover: ErpTokensDark.navyHover,
    actionLight: ErpTokensDark.navyLight,
    onAction: ErpTokensDark.textOnNavy,
    success: ErpTokensDark.success,
    successBg: ErpTokensDark.successBg,
    successBorder: ErpTokensDark.successBorder,
    warning: ErpTokensDark.warning,
    warningBg: ErpTokensDark.warningBg,
    warningBorder: ErpTokensDark.warningBorder,
    danger: ErpTokensDark.danger,
    dangerBg: ErpTokensDark.dangerBg,
    dangerBorder: ErpTokensDark.dangerBorder,
    info: ErpTokensDark.info,
    infoBg: ErpTokensDark.infoBg,
    accentMeasure: ErpTokensDark.accentMeasure,
  );

  // Shared by the light/dark factories; a constructor would repeat 26 args.
  // ignore: prefer_constructors_over_static_methods
  static ErpTokens _build(
    ErpDensity density, {
    required Color canvas,
    required Color surface,
    required Color surfaceMuted,
    required Color surfaceSubtle,
    required Color border,
    required Color borderStrong,
    required Color textMain,
    required Color textBody,
    required Color textMuted,
    required Color textSubtle,
    required Color action,
    required Color actionHover,
    required Color actionLight,
    required Color onAction,
    required Color success,
    required Color successBg,
    required Color successBorder,
    required Color warning,
    required Color warningBg,
    required Color warningBorder,
    required Color danger,
    required Color dangerBg,
    required Color dangerBorder,
    required Color info,
    required Color infoBg,
    required Color accentMeasure,
  }) {
    // shortcut: one factor for compact; add per-step values if design asks.
    final k = density == ErpDensity.compact ? 0.75 : 1.0;
    return ErpTokens(
      canvas: canvas,
      surface: surface,
      surfaceMuted: surfaceMuted,
      surfaceSubtle: surfaceSubtle,
      border: border,
      borderStrong: borderStrong,
      textMain: textMain,
      textBody: textBody,
      textMuted: textMuted,
      textSubtle: textSubtle,
      action: action,
      actionHover: actionHover,
      actionLight: actionLight,
      onAction: onAction,
      success: success,
      successBg: successBg,
      successBorder: successBorder,
      warning: warning,
      warningBg: warningBg,
      warningBorder: warningBorder,
      danger: danger,
      dangerBg: dangerBg,
      dangerBorder: dangerBorder,
      info: info,
      infoBg: infoBg,
      accentMeasure: accentMeasure,
      space1: ErpTokensScale.space1 * k,
      space2: ErpTokensScale.space2 * k,
      space3: ErpTokensScale.space3 * k,
      space4: ErpTokensScale.space4 * k,
      space6: ErpTokensScale.space6 * k,
      space8: ErpTokensScale.space8 * k,
      radius: ErpTokensScale.radius,
      borderWidth: ErpTokensScale.borderWidthHairline,
      tapTarget: ErpTokensScale.appTapTarget,
      primaryActionHeight: ErpTokensScale.appActionPrimary,
      textCaption: ErpTokensScale.appTextCaption,
      textBodySize: ErpTokensScale.appTextBody,
      textSubtitle: ErpTokensScale.appTextSubtitle,
      textTitle: ErpTokensScale.appTextTitle,
      textMeasure: ErpTokensScale.appTextMeasure,
    );
  }

  final Color canvas;
  final Color surface;
  final Color surfaceMuted;
  final Color surfaceSubtle;
  final Color border;
  final Color borderStrong;
  final Color textMain;
  final Color textBody;
  final Color textMuted;
  final Color textSubtle;
  final Color action;
  final Color actionHover;
  final Color actionLight;
  final Color onAction;
  final Color success;
  final Color successBg;
  final Color successBorder;
  final Color warning;
  final Color warningBg;
  final Color warningBorder;
  final Color danger;
  final Color dangerBg;
  final Color dangerBorder;
  final Color info;
  final Color infoBg;

  /// Oak accent: only for measurement dimension lines (design.md 8.1).
  final Color accentMeasure;
  final double space1;
  final double space2;
  final double space3;
  final double space4;
  final double space6;
  final double space8;
  final double radius;
  final double borderWidth;
  final double tapTarget;
  final double primaryActionHeight;
  final double textCaption;
  final double textBodySize;
  final double textSubtitle;
  final double textTitle;
  final double textMeasure;

  /// Corner shape shared by every control, card and sheet.
  BorderRadius get corner => BorderRadius.circular(radius);

  @override
  ErpTokens copyWith() => this;

  @override
  ErpTokens lerp(ThemeExtension<ErpTokens>? other, double t) =>
      // shortcut: tokens switch instantly instead of interpolating; fine
      // because themes change only from the settings screen.
      other is ErpTokens && t >= 0.5 ? other : this;
}

extension ErpTokensContext on BuildContext {
  ErpTokens get tokens => Theme.of(this).extension<ErpTokens>()!;
}
