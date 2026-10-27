// Self-check for lib/core/geometry/plane_measurement.dart (not a test file).
// Cases live in shared-fixtures/plane-measurement.json (repo root).
// Run from mobile/: dart run tool/check_plane_measurement.dart
import 'dart:convert';
import 'dart:io';

import 'package:tan_erp_field/core/geometry/plane_measurement.dart';

const _fixturePath = '../shared-fixtures/plane-measurement.json';

void _check(String name, bool ok, String detail) {
  stdout.writeln('${ok ? 'PASS' : 'FAIL'} $name: $detail');
  if (!ok) exitCode = 1;
}

double _num(Object? v) => (v! as num).toDouble();

Point2D _point(Object? raw) {
  final xy = raw! as List<Object?>;
  return Point2D(_num(xy[0]), _num(xy[1]));
}

List<Point2D> _points(Object? raw) => [
  for (final p in raw! as List<Object?>) _point(p),
];

bool _matches(
  PlaneMeasurementResult r,
  Map<String, Object?> want,
  double tolerance,
) {
  final area = want['areaSqm'];
  final width = want['widthMm'];
  final length = want['lengthMm'];
  return (area == null || (r.areaSqm - _num(area)).abs() < tolerance) &&
      (width == null || r.widthMm == _num(width)) &&
      (length == null || r.lengthMm == _num(length));
}

void main() {
  final root =
      jsonDecode(File(_fixturePath).readAsStringSync()) as Map<String, Object?>;
  final h = [for (final v in root['homography']! as List<Object?>) _num(v)];
  final size = root['referenceSizeMm']! as Map<String, Object?>;
  final refSize = ReferenceSizeMm(
    widthMm: _num(size['widthMm']),
    heightMm: _num(size['heightMm']),
  );
  final tolerance = _num(root['tolerance']);

  Point2D px(Point2D mm) => applyHomography(h, mm);

  for (final raw in root['cases']! as List<Object?>) {
    final c = raw! as Map<String, Object?>;
    final name = c['name']! as String;
    final expect = c['expect'];

    if (c.containsKey('polygonPx')) {
      // Raw image-space polygon: isSimplePolygon directly, no homography.
      final simple = isSimplePolygon(_points(c['polygonPx']));
      final want = (expect! as Map<String, Object?>)['simple'];
      _check(name, simple == want, 'isSimplePolygon $simple');
      continue;
    }

    final result = measurePlanePolygon(
      referenceQuadPx: _points(c['referenceMm']).map(px).toList(),
      referenceSizeMm: refSize,
      polygonPx: _points(c['polygonMm']).map(px).toList(),
    );

    if (expect == null) {
      _check(name, result == null, '$result');
      continue;
    }
    if (result == null) {
      _check(name, false, 'null');
      continue;
    }
    _check(
      name,
      _matches(result, expect as Map<String, Object?>, tolerance),
      '${result.areaSqm.toStringAsFixed(4)} m2, '
      '${result.widthMm} x ${result.lengthMm} mm, '
      '${result.vertexCount} pts',
    );
  }
}
