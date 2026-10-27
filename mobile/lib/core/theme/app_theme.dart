import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';

/// Thai line height from design.md 8.3.
const double _thaiLineHeight = 1.5;

/// Builds the app theme from tokens. [textScale] multiplies the token font
/// sizes (user setting 0.9-1.3); the OS text scale still applies on top.
ThemeData buildTheme({
  required Brightness mode,
  ErpDensity density = ErpDensity.normal,
  double textScale = 1,
  Color? brandColor,
}) {
  final t = mode == Brightness.dark
      ? ErpTokens.dark(density, brandColor)
      : ErpTokens.light(density, brandColor);
  final shape = RoundedRectangleBorder(borderRadius: t.corner);
  final side = BorderSide(color: t.border, width: t.borderWidth);

  TextStyle style(double size, Color color, {FontWeight? weight}) => TextStyle(
    fontSize: size * textScale,
    height: _thaiLineHeight,
    color: color,
    fontWeight: weight,
    fontFeatures: const [FontFeature.tabularFigures()],
  );

  final scheme = ColorScheme(
    brightness: mode,
    primary: t.action,
    onPrimary: t.onAction,
    secondary: t.actionLight,
    onSecondary: t.textMain,
    error: t.danger,
    onError: t.onAction,
    surface: t.surface,
    onSurface: t.textMain,
    outline: t.border,
    outlineVariant: t.border,
  );

  OutlineInputBorder inputBorder(Color color) => OutlineInputBorder(
    borderRadius: t.corner,
    borderSide: BorderSide(color: color, width: t.borderWidth),
  );

  return ThemeData(
    useMaterial3: true,
    brightness: mode,
    colorScheme: scheme,
    scaffoldBackgroundColor: t.canvas,
    canvasColor: t.surface,
    dividerColor: t.border,
    splashFactory: NoSplash.splashFactory,
    extensions: [t],
    textTheme: TextTheme(
      bodySmall: style(t.textCaption, t.textMuted),
      bodyMedium: style(t.textBodySize, t.textBody),
      bodyLarge: style(t.textSubtitle, t.textMain),
      titleMedium: style(t.textSubtitle, t.textMain, weight: FontWeight.w600),
      titleLarge: style(t.textTitle, t.textMain, weight: FontWeight.w600),
      displayMedium: style(t.textMeasure, t.textMain, weight: FontWeight.w700),
      labelLarge: style(t.textBodySize, t.textMain, weight: FontWeight.w600),
    ),
    appBarTheme: AppBarTheme(
      backgroundColor: t.surface,
      foregroundColor: t.textMain,
      elevation: 0,
      scrolledUnderElevation: 0,
      shape: Border(bottom: side),
    ),
    cardTheme: CardThemeData(
      color: t.surface,
      elevation: 0,
      shape: shape.copyWith(side: side),
    ),
    dialogTheme: DialogThemeData(
      backgroundColor: t.surface,
      shape: shape.copyWith(side: side),
    ),
    bottomSheetTheme: BottomSheetThemeData(
      backgroundColor: t.surface,
      shape: shape,
    ),
    inputDecorationTheme: InputDecorationTheme(
      filled: true,
      fillColor: t.surface,
      isDense: false,
      contentPadding: EdgeInsets.symmetric(
        horizontal: t.space3,
        vertical: t.space3,
      ),
      hintStyle: style(t.textBodySize, t.textSubtle),
      errorStyle: style(t.textCaption, t.danger),
      border: inputBorder(t.border),
      enabledBorder: inputBorder(t.border),
      focusedBorder: inputBorder(t.borderStrong),
      errorBorder: inputBorder(t.danger),
      focusedErrorBorder: inputBorder(t.danger),
      disabledBorder: inputBorder(t.border),
    ),
    navigationBarTheme: NavigationBarThemeData(
      backgroundColor: t.surface,
      indicatorColor: t.actionLight,
      indicatorShape: shape,
      elevation: 0,
      labelTextStyle: WidgetStatePropertyAll(
        style(t.textCaption, t.textMain, weight: FontWeight.w600),
      ),
    ),
    navigationRailTheme: NavigationRailThemeData(
      backgroundColor: t.surface,
      indicatorColor: t.actionLight,
      indicatorShape: shape,
      selectedLabelTextStyle: style(
        t.textCaption,
        t.textMain,
        weight: FontWeight.w600,
      ),
      unselectedLabelTextStyle: style(t.textCaption, t.textMuted),
    ),
  );
}
