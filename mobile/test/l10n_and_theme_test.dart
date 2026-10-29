import 'dart:convert';
import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:tan_erp_field/core/theme/app_theme.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';

void main() {
  group('Localization parity test', () {
    test('app_th.arb and app_en.arb have identical non-metadata keys', () {
      final thFile = File('lib/core/l10n/app_th.arb').existsSync()
          ? File('lib/core/l10n/app_th.arb')
          : File('mobile/lib/core/l10n/app_th.arb');
      final enFile = File('lib/core/l10n/app_en.arb').existsSync()
          ? File('lib/core/l10n/app_en.arb')
          : File('mobile/lib/core/l10n/app_en.arb');

      expect(thFile.existsSync(), isTrue);
      expect(enFile.existsSync(), isTrue);

      final thDecoded = jsonDecode(thFile.readAsStringSync());
      final enDecoded = jsonDecode(enFile.readAsStringSync());

      expect(thDecoded, isA<Map<String, Object?>>());
      expect(enDecoded, isA<Map<String, Object?>>());

      final thMap = thDecoded is Map<String, Object?>
          ? thDecoded
          : const <String, Object?>{};
      final enMap = enDecoded is Map<String, Object?>
          ? enDecoded
          : const <String, Object?>{};

      final thKeys = thMap.keys.where((k) => !k.startsWith('@')).toSet();
      final enKeys = enMap.keys.where((k) => !k.startsWith('@')).toSet();

      expect(thKeys, equals(enKeys));
      expect(thKeys, isNotEmpty);

      for (final key in thKeys) {
        final thVal = thMap[key];
        final enVal = enMap[key];

        expect(thVal, isA<String>());
        expect(enVal, isA<String>());

        if (thVal is String) {
          expect(thVal.trim(), isNotEmpty);
        }
        if (enVal is String) {
          expect(enVal.trim(), isNotEmpty);
        }
      }
    });
  });

  group('ERP Theme and Tokens test', () {
    test('buildTheme attaches ErpTokens with zero corner radius', () {
      final lightTheme = buildTheme(mode: Brightness.light);
      final lightTokens = lightTheme.extension<ErpTokens>();

      expect(lightTokens, isNotNull);
      expect(lightTokens?.corner, equals(BorderRadius.zero));
      expect(lightTokens?.radius, equals(0));

      final darkTheme = buildTheme(mode: Brightness.dark);
      final darkTokens = darkTheme.extension<ErpTokens>();

      expect(darkTokens, isNotNull);
      expect(darkTokens?.corner, equals(BorderRadius.zero));
      expect(darkTokens?.radius, equals(0));
    });

    test('ErpTokens direct light and dark factories use sharp 0 radius', () {
      final light = ErpTokens.light();
      expect(light.corner, equals(BorderRadius.zero));
      expect(light.radius, equals(0));

      final dark = ErpTokens.dark();
      expect(dark.corner, equals(BorderRadius.zero));
      expect(dark.radius, equals(0));
    });
  });
}
