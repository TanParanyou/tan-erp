import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/brand_rules.g.dart';
import 'package:tan_erp_field/core/theme/tokens.g.dart';

/// Action colours derived from the organization brand colour. Mirrors
/// frontend/src/lib/color/wcag.ts (design.md 7.4, ADR 0031).
@immutable
class BrandActionColors {
  const BrandActionColors({
    required this.action,
    required this.hover,
    required this.light,
  });

  final Color action;
  final Color hover;
  final Color light;
}

/// Parses `#RRGGBB`; null for null or malformed input (default Atelier Navy).
Color? parseBrandColor(String? hex) {
  if (hex == null || !RegExp(r'^#[0-9a-fA-F]{6}$').hasMatch(hex)) return null;
  return Color(0xFF000000 | int.parse(hex.substring(1), radix: 16));
}

/// WCAG contrast ratio between two opaque colours.
double contrastRatio(Color a, Color b) {
  final la = a.computeLuminance();
  final lb = b.computeLuminance();
  return (la > lb ? la + 0.05 : lb + 0.05) / (la > lb ? lb + 0.05 : la + 0.05);
}

/// Dark-mode action colour: lightens toward white until the fill stands out
/// from [canvas] (>= 3:1) while white text on it still passes (>= 4.5:1);
/// keeps [color] if no step satisfies both.
Color darkVariant(Color color, Color canvas) {
  const white = ErpTokensDark.textOnNavy;
  for (var step = 0; step <= BrandRules.darkVariantSteps; step++) {
    final candidate = Color.lerp(
      color,
      white,
      step * BrandRules.darkVariantStep,
    )!;
    if (contrastRatio(white, candidate) < BrandRules.textContrastMin) break;
    if (contrastRatio(candidate, canvas) >= BrandRules.canvasContrastMin) {
      return candidate;
    }
  }
  return color;
}

BrandActionColors brandActionColors(Color brand, {required bool dark}) {
  final action = dark ? darkVariant(brand, ErpTokensDark.canvas) : brand;
  return BrandActionColors(
    action: action,
    hover: Color.lerp(
      action,
      BrandRules.hoverTarget,
      BrandRules.hoverMixRatio,
    )!,
    light: dark
        ? Color.lerp(
            ErpTokensDark.canvas,
            action,
            BrandRules.lightMixRatioDarkTheme,
          )!
        : Color.lerp(
            action,
            ErpTokensLight.textOnNavy,
            BrandRules.lightMixRatioLightTheme,
          )!,
  );
}
