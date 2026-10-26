import 'package:flutter/foundation.dart';

@immutable
class VersionPolicy {
  const VersionPolicy({
    required this.minimumVersion,
    required this.latestVersion,
  });

  final String minimumVersion;
  final String latestVersion;
}

/// Compares dotted numeric versions (`1.2.3`); build suffixes are ignored.
int compareVersions(String a, String b) {
  List<int> parts(String v) =>
      v.split('+').first.split('.').map((p) => int.tryParse(p) ?? 0).toList();
  final pa = parts(a);
  final pb = parts(b);
  for (var i = 0; i < pa.length || i < pb.length; i++) {
    final x = i < pa.length ? pa[i] : 0;
    final y = i < pb.length ? pb[i] : 0;
    if (x != y) return x.compareTo(y);
  }
  return 0;
}
