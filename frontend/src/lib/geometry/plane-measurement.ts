/**
 * Pure planar-measurement geometry: maps image pixels onto a real-world plane
 * (millimetres) via a homography derived from a reference rectangle of known size.
 * No React / DOM dependencies.
 */

export interface Point2D {
  x: number;
  y: number;
}

/** Row-major 3x3 homography matrix with h33 fixed to 1. */
export type Homography = readonly [number, number, number, number, number, number, number, number, number];

export interface ReferenceSizeMm {
  widthMm: number;
  heightMm: number;
}

export interface MeasurePlanePolygonInput {
  /** Four reference corners in image pixels (any order). */
  referenceQuadPx: readonly Point2D[];
  referenceSizeMm: ReferenceSizeMm;
  /** Area polygon vertices in image pixels (>= 3). */
  polygonPx: readonly Point2D[];
}

export interface PlaneMeasurementResult {
  areaSqm: number;
  /** Shorter plane dimension in mm. */
  widthMm: number;
  /** Longer plane dimension in mm. */
  lengthMm: number;
  vertexCount: number;
}

const PIVOT_EPSILON = 1e-12;
const AREA_EPSILON = 1e-9;

/** Solves the 8x8 DLT system (h33 = 1) with Gaussian elimination and partial pivoting. */
export function computeHomography(
  src: readonly Point2D[],
  dst: readonly Point2D[]
): Homography | null {
  if (src.length !== 4 || dst.length !== 4) return null;

  const m: number[][] = [];
  for (let i = 0; i < 4; i++) {
    const { x, y } = src[i];
    const { x: u, y: v } = dst[i];
    m.push([x, y, 1, 0, 0, 0, -u * x, -u * y, u]);
    m.push([0, 0, 0, x, y, 1, -v * x, -v * y, v]);
  }

  const n = 8;
  for (let col = 0; col < n; col++) {
    let pivotRow = col;
    for (let r = col + 1; r < n; r++) {
      if (Math.abs(m[r][col]) > Math.abs(m[pivotRow][col])) pivotRow = r;
    }
    if (Math.abs(m[pivotRow][col]) < PIVOT_EPSILON) return null;
    [m[col], m[pivotRow]] = [m[pivotRow], m[col]];

    for (let r = col + 1; r < n; r++) {
      const factor = m[r][col] / m[col][col];
      for (let c = col; c <= n; c++) m[r][c] -= factor * m[col][c];
    }
  }

  const h: number[] = new Array<number>(n).fill(0);
  for (let r = n - 1; r >= 0; r--) {
    let sum = m[r][n];
    for (let c = r + 1; c < n; c++) sum -= m[r][c] * h[c];
    h[r] = sum / m[r][r];
  }
  if (h.some((value) => !Number.isFinite(value))) return null;

  return [h[0], h[1], h[2], h[3], h[4], h[5], h[6], h[7], 1];
}

export function applyHomography(h: Homography, p: Point2D): Point2D {
  const w = h[6] * p.x + h[7] * p.y + h[8];
  return {
    x: (h[0] * p.x + h[1] * p.y + h[2]) / w,
    y: (h[3] * p.x + h[4] * p.y + h[5]) / w,
  };
}

/** Sorts points clockwise (screen coordinates, y down) starting at the top-left-most point. */
export function orderQuadClockwise(points: readonly Point2D[]): Point2D[] {
  const cx = points.reduce((s, p) => s + p.x, 0) / points.length;
  const cy = points.reduce((s, p) => s + p.y, 0) / points.length;
  const sorted = [...points].sort(
    (a, b) => Math.atan2(a.y - cy, a.x - cx) - Math.atan2(b.y - cy, b.x - cx)
  );
  let start = 0;
  for (let i = 1; i < sorted.length; i++) {
    if (sorted[i].x + sorted[i].y < sorted[start].x + sorted[start].y) start = i;
  }
  return [...sorted.slice(start), ...sorted.slice(0, start)];
}

function signedArea(points: readonly Point2D[]): number {
  let sum = 0;
  for (let i = 0; i < points.length; i++) {
    const a = points[i];
    const b = points[(i + 1) % points.length];
    sum += a.x * b.y - b.x * a.y;
  }
  return sum / 2;
}

/** Shoelace area (absolute value) in the units of the input coordinates. */
export function polygonAreaAbs(points: readonly Point2D[]): number {
  return Math.abs(signedArea(points));
}

function cross(o: Point2D, a: Point2D, b: Point2D): number {
  return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
}

function segmentsIntersect(a: Point2D, b: Point2D, c: Point2D, d: Point2D): boolean {
  const d1 = cross(c, d, a);
  const d2 = cross(c, d, b);
  const d3 = cross(a, b, c);
  const d4 = cross(a, b, d);
  return d1 * d2 < 0 && d3 * d4 < 0;
}

/** True when no two non-adjacent edges properly cross. */
export function isSimplePolygon(points: readonly Point2D[]): boolean {
  const n = points.length;
  if (n < 3) return false;
  for (let i = 0; i < n; i++) {
    for (let j = i + 1; j < n; j++) {
      const adjacent = j === i + 1 || (i === 0 && j === n - 1);
      if (adjacent) continue;
      if (segmentsIntersect(points[i], points[(i + 1) % n], points[j], points[(j + 1) % n])) {
        return false;
      }
    }
  }
  return true;
}

function isConvexNonDegenerate(quad: readonly Point2D[]): boolean {
  let sign = 0;
  for (let i = 0; i < quad.length; i++) {
    const c = cross(quad[i], quad[(i + 1) % quad.length], quad[(i + 2) % quad.length]);
    if (Math.abs(c) < AREA_EPSILON) return false;
    const s = c > 0 ? 1 : -1;
    if (sign === 0) sign = s;
    else if (s !== sign) return false;
  }
  return true;
}

function dist(a: Point2D, b: Point2D): number {
  return Math.hypot(a.x - b.x, a.y - b.y);
}

/**
 * Measures a polygon lying on the same plane as a reference rectangle.
 * Returns null when inputs are invalid or the reference quad is non-convex/degenerate.
 */
export function measurePlanePolygon({
  referenceQuadPx,
  referenceSizeMm,
  polygonPx,
}: MeasurePlanePolygonInput): PlaneMeasurementResult | null {
  const { widthMm: refW, heightMm: refH } = referenceSizeMm;
  if (referenceQuadPx.length !== 4 || polygonPx.length < 3) return null;
  if (!(refW > 0) || !(refH > 0)) return null;

  const quad = orderQuadClockwise(referenceQuadPx);
  if (!isConvexNonDegenerate(quad)) return null;

  const longMm = Math.max(refW, refH);
  const shortMm = Math.min(refW, refH);
  // Pair A = top/bottom edges (p0p1, p3p2); pair B = right/left edges (p1p2, p0p3).
  const pairA = (dist(quad[0], quad[1]) + dist(quad[3], quad[2])) / 2;
  const pairB = (dist(quad[1], quad[2]) + dist(quad[0], quad[3])) / 2;
  const dx = pairA >= pairB ? longMm : shortMm;
  const dy = pairA >= pairB ? shortMm : longMm;
  const dst: Point2D[] = [
    { x: 0, y: 0 },
    { x: dx, y: 0 },
    { x: dx, y: dy },
    { x: 0, y: dy },
  ];

  const h = computeHomography(quad, dst);
  if (!h) return null;

  const world = polygonPx.map((p) => applyHomography(h, p));
  if (world.some((p) => !Number.isFinite(p.x) || !Number.isFinite(p.y))) return null;

  const areaMm2 = polygonAreaAbs(world);
  let side1: number;
  let side2: number;
  if (world.length === 4) {
    side1 = (dist(world[0], world[1]) + dist(world[3], world[2])) / 2;
    side2 = (dist(world[1], world[2]) + dist(world[0], world[3])) / 2;
  } else {
    const xs = world.map((p) => p.x);
    const ys = world.map((p) => p.y);
    side1 = Math.max(...xs) - Math.min(...xs);
    side2 = Math.max(...ys) - Math.min(...ys);
  }

  return {
    areaSqm: areaMm2 / 1_000_000,
    widthMm: Math.round(Math.min(side1, side2)),
    lengthMm: Math.round(Math.max(side1, side2)),
    vertexCount: world.length,
  };
}
