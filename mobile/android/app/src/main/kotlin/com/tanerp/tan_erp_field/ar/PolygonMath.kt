package com.tanerp.tan_erp_field.ar

import kotlin.math.abs
import kotlin.math.hypot

/** 2D point in the reference plane's local frame (metres; x and z of the plane pose). */
data class PlanePoint(val x: Double, val z: Double)

object PolygonMath {
    /** Shoelace area of the closed polygon [points]. */
    fun area(points: List<PlanePoint>): Double {
        if (points.size < 3) return 0.0
        var sum = 0.0
        for (i in points.indices) {
            val a = points[i]
            val b = points[(i + 1) % points.size]
            sum += a.x * b.z - b.x * a.z
        }
        return abs(sum) / 2.0
    }

    /** Edge lengths; [closed] adds the edge from the last point back to the first. */
    fun edgeLengths(points: List<PlanePoint>, closed: Boolean): List<Double> {
        val count = if (closed) points.size else points.size - 1
        return (0 until count.coerceAtLeast(0)).map { i ->
            val a = points[i]
            val b = points[(i + 1) % points.size]
            hypot(b.x - a.x, b.z - a.z)
        }
    }

    /** True when any two non-adjacent edges of the closed polygon cross or touch. */
    fun isSelfIntersecting(points: List<PlanePoint>): Boolean {
        val n = points.size
        if (n < 4) return false
        for (i in 0 until n) {
            for (j in i + 1 until n) {
                // Skip edges sharing a vertex (consecutive, or first and last).
                if (j == i + 1 || (i == 0 && j == n - 1)) continue
                if (segmentsIntersect(points[i], points[(i + 1) % n], points[j], points[(j + 1) % n])) return true
            }
        }
        return false
    }

    private fun segmentsIntersect(p1: PlanePoint, p2: PlanePoint, p3: PlanePoint, p4: PlanePoint): Boolean {
        val d1 = cross(p3, p4, p1)
        val d2 = cross(p3, p4, p2)
        val d3 = cross(p1, p2, p3)
        val d4 = cross(p1, p2, p4)
        if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) && ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0))) return true
        return (d1 == 0.0 && onSegment(p3, p4, p1)) ||
            (d2 == 0.0 && onSegment(p3, p4, p2)) ||
            (d3 == 0.0 && onSegment(p1, p2, p3)) ||
            (d4 == 0.0 && onSegment(p1, p2, p4))
    }

    private fun cross(a: PlanePoint, b: PlanePoint, c: PlanePoint): Double =
        (b.x - a.x) * (c.z - a.z) - (b.z - a.z) * (c.x - a.x)

    private fun onSegment(a: PlanePoint, b: PlanePoint, p: PlanePoint): Boolean =
        minOf(a.x, b.x) <= p.x && p.x <= maxOf(a.x, b.x) && minOf(a.z, b.z) <= p.z && p.z <= maxOf(a.z, b.z)
}
