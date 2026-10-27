// Self-check for lib/core/geometry/plane_measurement.dart (not a test file).
// Run: dart run tool/check_plane_measurement.dart
import 'dart:io';

import 'package:tan_erp_field/core/geometry/plane_measurement.dart';

const _h = <double>[0.9, 0.15, 200, -0.05, 0.7, 150, 0.00012, 0.00025, 1];

Point2D _px(double x, double y) => applyHomography(_h, Point2D(x, y));

List<Point2D> _toPx(List<List<double>> mm) => [
  for (final p in mm) _px(p[0], p[1]),
];

void _check(String name, bool ok, String detail) {
  stdout.writeln('${ok ? 'PASS' : 'FAIL'} $name: $detail');
  if (!ok) exitCode = 1;
}

void main() {
  const a4 = ReferenceSizeMm(widthMm: 210, heightMm: 297);
  final ref = _toPx([
    [500, 500],
    [710, 500],
    [710, 797],
    [500, 797],
  ]);

  final rect = measurePlanePolygon(
    referenceQuadPx: ref,
    referenceSizeMm: a4,
    polygonPx: _toPx([
      [1000, 1000],
      [4000, 1000],
      [4000, 3000],
      [1000, 3000],
    ]),
  );
  _check(
    'rectangle 3000x2000 mm',
    rect != null &&
        (rect.areaSqm - 6).abs() < 1e-6 &&
        rect.widthMm == 2000 &&
        rect.lengthMm == 3000,
    rect == null
        ? 'null'
        : '${rect.areaSqm.toStringAsFixed(4)} m2, '
              '${rect.widthMm} x ${rect.lengthMm} mm, ${rect.vertexCount} pts',
  );

  final lShape = measurePlanePolygon(
    referenceQuadPx: ref,
    referenceSizeMm: a4,
    polygonPx: _toPx([
      [1000, 1000],
      [4000, 1000],
      [4000, 2000],
      [3000, 2000],
      [3000, 3000],
      [1000, 3000],
    ]),
  );
  _check(
    'L-shape',
    lShape != null && (lShape.areaSqm - 5).abs() < 1e-6,
    lShape == null ? 'null' : '${lShape.areaSqm.toStringAsFixed(4)} m2',
  );

  final degenerate = measurePlanePolygon(
    referenceQuadPx: const [
      Point2D(0, 0),
      Point2D(100, 0),
      Point2D(200, 0),
      Point2D(300, 0),
    ],
    referenceSizeMm: a4,
    polygonPx: _toPx([
      [1000, 1000],
      [4000, 1000],
      [4000, 3000],
    ]),
  );
  _check('degenerate reference', degenerate == null, '$degenerate');

  _check(
    'bowtie is not simple',
    !isSimplePolygon(const [
      Point2D(0, 0),
      Point2D(10, 10),
      Point2D(10, 0),
      Point2D(0, 10),
    ]),
    'isSimplePolygon false',
  );
}
