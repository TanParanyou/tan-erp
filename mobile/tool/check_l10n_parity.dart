// Fails when app_th.arb and app_en.arb do not define the same message keys.
// Run: dart run tool/check_l10n_parity.dart
import 'dart:convert';
import 'dart:io';

Set<String> _keys(String path) {
  final decoded = jsonDecode(File(path).readAsStringSync());
  if (decoded is! Map<String, Object?>) {
    throw FormatException('$path is not a JSON object');
  }
  return decoded.keys.where((k) => !k.startsWith('@')).toSet();
}

void main() {
  const dir = 'lib/core/l10n';
  final th = _keys('$dir/app_th.arb');
  final en = _keys('$dir/app_en.arb');
  final missingInEn = th.difference(en);
  final missingInTh = en.difference(th);
  if (missingInEn.isEmpty && missingInTh.isEmpty) {
    stdout.writeln('l10n parity OK (${th.length} keys)');
    return;
  }
  for (final k in missingInEn) {
    stderr.writeln('missing in app_en.arb: $k');
  }
  for (final k in missingInTh) {
    stderr.writeln('missing in app_th.arb: $k');
  }
  exit(1);
}
