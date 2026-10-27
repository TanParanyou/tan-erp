import 'dart:math' as math;

// 1:1 port of frontend/src/lib/geometry/plane-measurement.ts. Pure Dart, no
// Flutter imports, so tool/check_plane_measurement.dart can run it directly.

class Point2D {
  const Point2D(this.x, this.y);

  final double x;
  final double y;
}

/// Row-major 3x3 homography (9 values) with h33 fixed to 1.
typedef Homography = List<double>;

class ReferenceSizeMm {
  const ReferenceSizeMm({required this.widthMm, required this.heightMm});

  final double widthMm;
  final double heightMm;
}

class PlaneMeasurementResult {
  const PlaneMeasurementResult({
    required this.areaSqm,
    required this.widthMm,
    required this.lengthMm,
    required this.vertexCount,
  });

  final double areaSqm;

  /// Shorter plane dimension in mm.
  final int widthMm;

  /// Longer plane dimension in mm.
  final int lengthMm;
  final int vertexCount;
}

const double _pivotEpsilon = 1e-12;
const double _areaEpsilon = 1e-9;

/// Solves the 8x8 DLT system (h33 = 1) with Gaussian elimination and partial
/// pivoting. Returns null when the system is singular.
Homography? computeHomography(List<Point2D> src, List<Point2D> dst) {
  if (src.length != 4 || dst.length != 4) return null;

  final m = <List<double>>[];
  for (var i = 0; i < 4; i++) {
    final x = src[i].x;
    final y = src[i].y;
    final u = dst[i].x;
    final v = dst[i].y;
    m
      ..add([x, y, 1, 0, 0, 0, -u * x, -u * y, u])
      ..add([0, 0, 0, x, y, 1, -v * x, -v * y, v]);
  }

  const n = 8;
  for (var col = 0; col < n; col++) {
    var pivotRow = col;
    for (var r = col + 1; r < n; r++) {
      if (m[r][col].abs() > m[pivotRow][col].abs()) pivotRow = r;
    }
    if (m[pivotRow][col].abs() < _pivotEpsilon) return null;
    final swap = m[col];
    m[col] = m[pivotRow];
    m[pivotRow] = swap;

    for (var r = col + 1; r < n; r++) {
      final factor = m[r][col] / m[col][col];
      for (var c = col; c <= n; c++) {
        m[r][c] -= factor * m[col][c];
      }
    }
  }

  final h = List<double>.filled(n, 0);
  for (var r = n - 1; r >= 0; r--) {
    var sum = m[r][n];
    for (var c = r + 1; c < n; c++) {
      sum -= m[r][c] * h[c];
    }
    h[r] = sum / m[r][r];
  }
  if (h.any((v) => !v.isFinite)) return null;

  return [...h, 1];
}

Point2D applyHomography(Homography h, Point2D p) {
  final w = h[6] * p.x + h[7] * p.y + h[8];
  return Point2D(
    (h[0] * p.x + h[1] * p.y + h[2]) / w,
    (h[3] * p.x + h[4] * p.y + h[5]) / w,
  );
}

/// Sorts points clockwise (screen coordinates, y down) starting at the
/// top-left-most point.
List<Point2D> orderQuadClockwise(List<Point2D> points) {
  final cx = points.fold<double>(0, (s, p) => s + p.x) / points.length;
  final cy = points.fold<double>(0, (s, p) => s + p.y) / points.length;
  final sorted = [...points]
    ..sort(
      (a, b) => math
          .atan2(a.y - cy, a.x - cx)
          .compareTo(math.atan2(b.y - cy, b.x - cx)),
    );
  var start = 0;
  for (var i = 1; i < sorted.length; i++) {
    if (sorted[i].x + sorted[i].y < sorted[start].x + sorted[start].y) {
      start = i;
    }
  }
  return [...sorted.sublist(start), ...sorted.sublist(0, start)];
}

double _signedArea(List<Point2D> points) {
  var sum = 0.0;
  for (var i = 0; i < points.length; i++) {
    final a = points[i];
    final b = points[(i + 1) % points.length];
    sum += a.x * b.y - b.x * a.y;
  }
  return sum / 2;
}

/// Shoelace area (absolute value) in the units of the input coordinates.
double polygonAreaAbs(List<Point2D> points) => _signedArea(points).abs();

double _cross(Point2D o, Point2D a, Point2D b) =>
    (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);

bool _segmentsIntersect(Point2D a, Point2D b, Point2D c, Point2D d) {
  final d1 = _cross(c, d, a);
  final d2 = _cross(c, d, b);
  final d3 = _cross(a, b, c);
  final d4 = _cross(a, b, d);
  return d1 * d2 < 0 && d3 * d4 < 0;
}

/// True when no two non-adjacent edges properly cross.
bool isSimplePolygon(List<Point2D> points) {
  final n = points.length;
  if (n < 3) return false;
  for (var i = 0; i < n; i++) {
    for (var j = i + 1; j < n; j++) {
      final adjacent = j == i + 1 || (i == 0 && j == n - 1);
      if (adjacent) continue;
      if (_segmentsIntersect(
        points[i],
        points[(i + 1) % n],
        points[j],
        points[(j + 1) % n],
      )) {
        return false;
      }
    }
  }
  return true;
}

bool _isConvexNonDegenerate(List<Point2D> quad) {
  var sign = 0;
  for (var i = 0; i < quad.length; i++) {
    final c = _cross(
      quad[i],
      quad[(i + 1) % quad.length],
      quad[(i + 2) % quad.length],
    );
    if (c.abs() < _areaEpsilon) return false;
    final s = c > 0 ? 1 : -1;
    if (sign == 0) {
      sign = s;
    } else if (s != sign) {
      return false;
    }
  }
  return true;
}

double _dist(Point2D a, Point2D b) =>
    math.sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));

/// Measures a polygon lying on the same plane as a reference rectangle.
/// Returns null when inputs are invalid or the reference quad is
/// non-convex/degenerate.
PlaneMeasurementResult? measurePlanePolygon({
  required List<Point2D> referenceQuadPx,
  required ReferenceSizeMm referenceSizeMm,
  required List<Point2D> polygonPx,
}) {
  final refW = referenceSizeMm.widthMm;
  final refH = referenceSizeMm.heightMm;
  if (referenceQuadPx.length != 4 || polygonPx.length < 3) return null;
  if (!(refW > 0) || !(refH > 0)) return null;

  final quad = orderQuadClockwise(referenceQuadPx);
  if (!_isConvexNonDegenerate(quad)) return null;

  final longMm = math.max(refW, refH);
  final shortMm = math.min(refW, refH);
  // Pair A = top/bottom edges (p0p1, p3p2); pair B = right/left (p1p2, p0p3).
  final pairA = (_dist(quad[0], quad[1]) + _dist(quad[3], quad[2])) / 2;
  final pairB = (_dist(quad[1], quad[2]) + _dist(quad[0], quad[3])) / 2;
  final dx = pairA >= pairB ? longMm : shortMm;
  final dy = pairA >= pairB ? shortMm : longMm;
  final dst = [
    const Point2D(0, 0),
    Point2D(dx, 0),
    Point2D(dx, dy),
    Point2D(0, dy),
  ];

  final h = computeHomography(quad, dst);
  if (h == null) return null;

  final world = polygonPx.map((p) => applyHomography(h, p)).toList();
  if (world.any((p) => !p.x.isFinite || !p.y.isFinite)) return null;

  final areaMm2 = polygonAreaAbs(world);
  double side1;
  double side2;
  if (world.length == 4) {
    side1 = (_dist(world[0], world[1]) + _dist(world[3], world[2])) / 2;
    side2 = (_dist(world[1], world[2]) + _dist(world[0], world[3])) / 2;
  } else {
    final xs = world.map((p) => p.x);
    final ys = world.map((p) => p.y);
    side1 = xs.reduce(math.max) - xs.reduce(math.min);
    side2 = ys.reduce(math.max) - ys.reduce(math.min);
  }

  return PlaneMeasurementResult(
    areaSqm: areaMm2 / 1000000,
    widthMm: math.min(side1, side2).round(),
    lengthMm: math.max(side1, side2).round(),
    vertexCount: world.length,
  );
}
